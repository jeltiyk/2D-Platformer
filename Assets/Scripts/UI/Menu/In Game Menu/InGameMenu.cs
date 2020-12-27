using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MenuController
{
    [Header("In Game Menu")]
    [SerializeField] private Button resume;
    [SerializeField] private Button restart;
    [SerializeField] private Button mainMenu;
    [SerializeField] private Button info;
    [SerializeField] private GameObject infoMenu;
    [SerializeField] private Button closeInfoMenu;

    protected override void Start()
    {
        base.Start();
        
        info.onClick.AddListener(OnInfoClicked);
        closeInfoMenu.onClick.AddListener(OnInfoClicked);
        resume.onClick.AddListener(OnResumeClicked);
        restart.onClick.AddListener(OnRestartClicked);
        mainMenu.onClick.AddListener(OnMainMenuClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        info.onClick.RemoveListener(OnInfoClicked);
        closeInfoMenu.onClick.RemoveListener(OnInfoClicked);
        resume.onClick.RemoveListener(OnResumeClicked);
        restart.onClick.RemoveListener(OnRestartClicked);
        mainMenu.onClick.RemoveListener(OnMainMenuClicked);
    }

    protected override void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu.activeInHierarchy)
            {
                OnSettingsClicked();
                return;
            }

            if (infoMenu.activeInHierarchy)
            {
                OnInfoClicked();
                return;
            
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
            OnResumeClicked();
    }

    protected override void OnMenuClicked()
    {
        base.OnMenuClicked();
 
        if(menu.activeInHierarchy)
            inMenu.TransitionTo(1f);
        else
            normal.TransitionTo(1f);

        Time.timeScale = menu.activeInHierarchy ? 0 : 1;
    }

    private void OnResumeClicked()
    {
        OnMenuClicked();
    }
    
    private void OnRestartClicked()
    {
        normal.TransitionTo(1f);
        SceneController.ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMainMenuClicked()
    {
        normal.TransitionTo(1f);
        SceneController.Instance.ChangeScene((int) Scenes.MainMenu);
    }

    private void OnInfoClicked()
    {
        menu.SetActive(!menu.activeInHierarchy);
        infoMenu.SetActive(!infoMenu.activeInHierarchy);
    }
}
