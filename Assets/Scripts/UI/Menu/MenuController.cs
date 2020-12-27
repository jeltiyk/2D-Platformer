using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(UIButtonsController))]
public class MenuController : MonoBehaviour
{
    [SerializeField] protected GameObject menu;

    [SerializeField] protected AudioMixerSnapshot normal;
    [SerializeField] protected AudioMixerSnapshot inMenu;
    
    [Header("Main buttons")]
    [SerializeField] protected Button exit;
    [SerializeField] protected Button settings;
    [SerializeField] protected Button closeSettings;

    [Header("Settings")]
    [SerializeField] protected GameObject settingsMenu;

    protected SceneController SceneController;

    private void Awake()
    {
        SceneController = SceneController.Instance;
        
        // PlayerPrefs.DeleteAll();
    }

    protected virtual void Start()
    {
        settings.onClick.AddListener(OnSettingsClicked);
        closeSettings.onClick.AddListener(OnSettingsClicked);
        
        exit.onClick.AddListener(OnExitClicked);
    }

    protected virtual void OnDestroy()
    {
        settings.onClick.RemoveListener(OnSettingsClicked);
        closeSettings.onClick.RemoveListener(OnSettingsClicked);
        
        exit.onClick.RemoveListener(OnExitClicked);
    }

    protected virtual void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && settingsMenu.activeInHierarchy)
            OnSettingsClicked();
    }

    protected virtual void OnMenuClicked()
    {
        menu.SetActive(!menu.activeInHierarchy);
    }

    protected virtual void OnSettingsClicked()
    {
        menu.SetActive(!menu.activeInHierarchy);
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
    }
    protected virtual void OnExitClicked()
    {
        Application.Quit();
        Debug.Log("Quited");
    }
}
