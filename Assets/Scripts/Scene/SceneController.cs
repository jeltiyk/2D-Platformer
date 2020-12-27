using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public enum Scenes
{
    MainMenu,
    First,
    Second,
    Third,
    MegaEnemy
}

public enum Prefs
{
    LastPlayedLevel = 0,
    PlayedLevels = 1
}

public enum MusicMode
{
    Appearance,
    Fading
}

[RequireComponent(typeof(AudioSource))]
public class SceneController : MonoBehaviour
{
    [Header("Cut-Scenes")] 
    [SerializeField] private GameObject sceneCamera;
    [SerializeField] private GameObject levelBorders;
    [SerializeField] private bool startCutscene;
    [SerializeField] private Transform toStartPosition;
    [SerializeField] private bool endEndCutscene;
    [SerializeField] private float triggerDistance;
    [SerializeField] private Transform toEndPosition;
    public bool ExistsEndCutscene => endEndCutscene;
    
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private float movementSpeed;

    [Header("Song")]
    [SerializeField] private AudioSource backgroundSound;
    private const float ChangeTime = 0.05f;

    public static SceneController Instance;

    private Rigidbody2D _playerRb;
    private Animator _playerAnimator;

    private CameraController _cameraController;
    private PlayerController _playerController;
    private Movement _movement;
    public Canvas canvas;

    private bool _faceRight;
    private bool _faceDirection;

    private bool _start;
    private Transform _helperSpawnArea;
    private GameObject[] _helperGameObject;
    private MegaEnemyController _megaEnemyController;

    private void Awake()
    {
        // PlayerPrefs.DeleteAll();
        
        if (Instance == null)
        {
            Instance = this;
            
            if(backgroundSound.clip != null)
                backgroundSound.Play();
        }
        else
        {
            Destroy(gameObject);
        }
        
        Time.timeScale = 1;

        backgroundSound.volume = 0f;
        backgroundSound.Play();

        // if (SceneManager.GetActiveScene().buildIndex == (int)Scenes.MainMenu)
        //     StartCoroutine(MusicController(MusicMode.Appearance, ChangeTime / 2));

        // if (SceneManager.GetActiveScene().buildIndex == 0) return;

        if (SceneManager.GetActiveScene().buildIndex != (int) Scenes.MainMenu)
        {
            PlayerPrefs.SetInt(Prefs.LastPlayedLevel.ToString(), SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetInt(Prefs.PlayedLevels.ToString() + SceneManager.GetActiveScene().buildIndex, 1);
        }
        else
        {
            StartCoroutine(MusicController(MusicMode.Appearance, ChangeTime / 2));
        }

        if (SceneManager.GetActiveScene().buildIndex == (int) Scenes.MegaEnemy)
        {
            _megaEnemyController = FindObjectOfType<MegaEnemyController>();
            _helperSpawnArea = transform.GetChild(0);
            _helperGameObject = new GameObject[3];
            _helperGameObject[0] = _helperSpawnArea.GetComponentInChildren<AcornController>(true).gameObject;
            _helperGameObject[1] = _helperSpawnArea.GetComponentInChildren<SuperAcornController>(true).gameObject;
            _helperGameObject[2] = _helperSpawnArea.GetComponentInChildren<ShieldController>(true).gameObject;
            // _helperGameObject[0] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Items/Super Acorn.prefab");
            // _helperGameObject[1] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Items/Acorn.prefab");
            // _helperGameObject[2] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Items/Shield.prefab");
        }

        if (!startCutscene && !endEndCutscene) return;
        
        if(startCutscene)
            if(levelBorders != null)
                levelBorders.SetActive(!levelBorders.activeSelf);
        
        // if (sceneCamera == null)
        //     sceneCamera = FindObjectOfType<CameraController>().gameObject;
        //
        // if (player == null)
        //     player = FindObjectOfType<PlayerController>().gameObject;
        
        _playerRb = player.GetComponent<Rigidbody2D>();
        _playerAnimator = player.GetComponent<Animator>();

        _cameraController = sceneCamera.GetComponent<CameraController>();
        _playerController = player.GetComponent<PlayerController>();
        _movement = player.GetComponent<Movement>();

        // if(levelBorders == null)
        //     // finds 'Level Borders' gameObject
        //     levelBorders = FindObjectOfType<Parallax>().transform.root.GetChild(0).gameObject;
    }

    private void Start()
    {
        #region Settings
        
        SettingsMenuController settingsMenuController = GameObject.Find("In Game UI").GetComponentInChildren<SettingsMenuController>(true);

        #region Sounds
        
        settingsMenuController.Master.value = PlayerPrefs.GetFloat("Master Volume", 1f);
        settingsMenuController.AudioMixer.audioMixer.SetFloat("Master",
            Mathf.Lerp(-80, 0, settingsMenuController.Master.value));

        settingsMenuController.Effects.value = PlayerPrefs.GetFloat("Effects Volume", 0.75f);
        settingsMenuController.AudioMixer.audioMixer.SetFloat("Effects",
            Mathf.Lerp(-80, 0, settingsMenuController.Effects.value));
        
        settingsMenuController.Music.value = PlayerPrefs.GetFloat("Music Volume", 0.85f);
        settingsMenuController.AudioMixer.audioMixer.SetFloat("Music",
            Mathf.Lerp(-80, 0, settingsMenuController.Music.value));

        #endregion

        #region Full Screen
        
        settingsMenuController.FullScreen.isOn = PlayerPrefs.GetInt("Full Screen", 1) == 1;

        #endregion

        #region Resolution
        
        Resolution [] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width < 800) continue;

            if (resolutions[i].width != Screen.currentResolution.width ||
                resolutions[i].height != Screen.currentResolution.height) continue;

            settingsMenuController.Resolution.value = PlayerPrefs.GetInt("Resolution", i);
            settingsMenuController.Resolution.RefreshShownValue();
            break;
        }
        
        #endregion

        #region Quality

        settingsMenuController.Quality.value = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        settingsMenuController.Quality.RefreshShownValue();

        #endregion

        #endregion
        
        if (SceneManager.GetActiveScene().buildIndex == (int) Scenes.MegaEnemy)
            StartCoroutine(HelperController());
    }

    private void FixedUpdate()
    {
        if(startCutscene)
            StartCutscene();
        
        if(endEndCutscene)
            EndCutscene();
    }

    private void StartCutscene()
    {
        if (player.transform.position.x + 1f > toStartPosition.transform.position.x &&
            player.transform.position.x - 1f < toStartPosition.transform.position.x)
        {
            startCutscene = false;
            _faceDirection = false;
            
            if(levelBorders != null)
                levelBorders.SetActive(!levelBorders.activeSelf);

            canvas.gameObject.SetActive(true);
            return;
        }

        if (!_faceDirection)
        {
            _faceDirection = true;
            _faceRight = _movement.FaceRight;

            StartCoroutine(MusicController(MusicMode.Appearance, ChangeTime));

            canvas.gameObject.SetActive(false);
        }
        
        _playerAnimator.SetBool("Ground", true);

        if (player.transform.position.x < toStartPosition.transform.position.x)
        {
            if(!_faceRight)
                Flip();

            _playerRb.velocity = new Vector2(1 * movementSpeed, _playerRb.velocity.y);
        }
        else if (player.transform.position.x > toStartPosition.transform.position.x)
        {
            if(_faceRight)
                Flip();
            
            _playerRb.velocity = new Vector2(-1 * movementSpeed, _playerRb.velocity.y);
        }

        _playerAnimator.SetFloat("Speed", Mathf.Abs(_playerRb.velocity.x));
    }
    
    private void EndCutscene()
    {
        if(player.transform.position.x + triggerDistance < toEndPosition.transform.position.x
        || player.transform.position.x - triggerDistance > toEndPosition.transform.position.x
        || !_playerController.ToNextLevel)
            return;
        
        if (!_faceDirection)
        {
            _faceDirection = true;
            _faceRight = _movement.FaceRight;

            StartCoroutine(MusicController(MusicMode.Fading, ChangeTime));

            _cameraController.Smoothing = 0.5f;
            _cameraController.FollowObject = toEndPosition.gameObject;
            _cameraController.transform.position = new Vector3(_cameraController.transform.position.x, -5.2f, _cameraController.transform.position.z);
            
            canvas.gameObject.SetActive(false);
            
            PlayerPrefs.SetInt("Player Health", _playerController.CurrentHealth);
            PlayerPrefs.SetInt("PineCones Count", _playerController.CurrentObjects);
            PlayerPrefs.SetInt("Coins Count", _playerController.CurrentCoins);
            PlayerPrefs.SetInt("Shield Boost Count", _playerController.CurrentShieldCount);

            canvas.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        }
        
        if(levelBorders != null)
            levelBorders.SetActive(false);

        if (player.transform.position.x + 1f > toEndPosition.transform.position.x &&
            player.transform.position.x - 1f < toEndPosition.transform.position.x)
        {
            endEndCutscene = false;
            _faceDirection = false;
            
            if(levelBorders != null)
                levelBorders.SetActive(!levelBorders.activeSelf);

            _cameraController.Smoothing = 3.5f;
            _cameraController.FollowObject = toEndPosition.gameObject;
            
            canvas.gameObject.SetActive(true);

            EndScene();
        }

        _playerAnimator.SetBool("Ground", true);
        
        if (player.transform.position.x < toEndPosition.transform.position.x)
        {
            if(!_faceRight)
                Flip();
            
            _playerRb.velocity = new Vector2(1 * movementSpeed, _playerRb.velocity.y);
        }
        else if (player.transform.position.x > toEndPosition.transform.position.x)
        {
            if(_faceRight)
                Flip();
            
            _playerRb.velocity = new Vector2(-1 * movementSpeed, _playerRb.velocity.y);
        }

        _playerAnimator.SetFloat("Speed", Mathf.Abs(_playerRb.velocity.x));
    }

    private IEnumerator MusicController(MusicMode mode, float time)
    {
        if (time == 0) yield break;

        switch (mode)
        {
            case MusicMode.Appearance:
            {
                while (backgroundSound.volume < 1)
                {
                    backgroundSound.volume += 0.05f;
                    yield return new WaitForSeconds(time);
                }

                break;
            }
            case MusicMode.Fading:
            {
                while (backgroundSound.volume >= 0)
                {
                    backgroundSound.volume -= 0.05f;
                    yield return new WaitForSeconds(time);
                }

                break;
            }
        }
    }

    private IEnumerator HelperController()
    {
        while (_megaEnemyController.CurrentHealth > 0)
        {
            GameObject helperObject = Instantiate(_helperGameObject[UnityEngine.Random.Range(0, _helperGameObject.Length)],
                new Vector2(_helperSpawnArea.position.x + UnityEngine.Random.Range(-17, 17), _helperSpawnArea.position.y),
                Quaternion.identity);
            
            if(!helperObject.activeSelf) helperObject.SetActive(true);
            
            helperObject.AddComponent<Rigidbody2D>().gravityScale = 0.5f * Time.deltaTime;

            Destroy(helperObject, 40f);

            yield return new WaitForSeconds(UnityEngine.Random.Range(20f, 30f));
        }
    }
    
    public void RestartScene()
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
        startCutscene = true;
    }
    
    public void ChangeScene(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void EndScene()
    {
        if ((Scenes) SceneManager.GetActiveScene().buildIndex == Scenes.MegaEnemy)
        {
            ChangeScene((int) Scenes.MainMenu);

            return;
        }        
        
        ChangeScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Flip()
    {
        _faceRight = !_faceRight;
        player.gameObject.transform.Rotate(0f, 180f, 0f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y + 13f), new Vector2(35f, 0.5f));
    }
}
