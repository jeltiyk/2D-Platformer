using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MenuController
{
    [Header("Choose level")]
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private Button chooseLevel;
    [SerializeField] private Button closeLevelMenu;
    
    [Header("Main menu")]
    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    
    protected override void Start()
    {
        base.Start();
        
        newGame.onClick.AddListener(OnNewGameClicked);
        
        if (PlayerPrefs.HasKey(Prefs.LastPlayedLevel.ToString()))
        {
            continueGame.interactable = true;
            continueGame.onClick.AddListener(OnContinueClicked);
        }
        else
        {
            continueGame.interactable = false;
        }
        
        chooseLevel.onClick.AddListener(OnChooseLevelClicked);
        closeLevelMenu.onClick.AddListener(OnChooseLevelClicked);
        
        #region Settings
        
        // PlayerPrefs.DeleteAll();
        // return;

        SettingsMenuController settingsMenuController = GetComponentInChildren<SettingsMenuController>(true);

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
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        newGame.onClick.RemoveListener(OnNewGameClicked);

        continueGame.onClick.RemoveAllListeners();
        
        chooseLevel.onClick.RemoveListener(OnChooseLevelClicked);
        closeLevelMenu.onClick.RemoveListener(OnChooseLevelClicked);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        
        if(Input.GetKeyDown(KeyCode.Escape) && levelMenu.activeInHierarchy)
            OnChooseLevelClicked();
    }

    private void OnNewGameClicked()
    {
        PlayerPrefs.DeleteKey(Prefs.PlayedLevels.ToString());
        PlayerPrefs.DeleteKey(Prefs.LastPlayedLevel.ToString());
        PlayerPrefs.DeleteKey("Player Health");
        PlayerPrefs.DeleteKey("PineCones Count");
        PlayerPrefs.DeleteKey("Coins Count");
        
        SceneController.Instance.ChangeScene((int) Scenes.First);
    }

    private void OnContinueClicked()
    {
        SceneController.Instance.ChangeScene(PlayerPrefs.GetInt(Prefs.LastPlayedLevel.ToString()));
    }

    private void OnChooseLevelClicked()
    {
        menu.SetActive(!menu.activeInHierarchy);
        levelMenu.SetActive(!levelMenu.activeInHierarchy);
    }
}
