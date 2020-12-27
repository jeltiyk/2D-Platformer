using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioMixerGroup audioMixer;

    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider effectsVolume;
    [SerializeField] private Slider musicVolume;
    
    [Header("Screen")]
    [SerializeField] private Toggle fullScreen;
    [SerializeField] private TMP_Dropdown resolution;
    private Resolution[] resolutions;

    [Header("Quality")]
    [SerializeField] private TMP_Dropdown quality;
    private string[] _qualityLevels;
    
    
    public AudioMixerGroup AudioMixer => audioMixer;
    public Slider Master => masterVolume;
    public Slider Effects => effectsVolume;
    public Slider Music => musicVolume;
    
    public Toggle FullScreen => fullScreen;
    public TMP_Dropdown Resolution => resolution;
    public TMP_Dropdown Quality => quality;
    
    private void Awake()
    {
        masterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
        effectsVolume.onValueChanged.AddListener(OnEffectsVolumeChanged);
        musicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);

        fullScreen.onValueChanged.AddListener(OnFullScreenChanged);
        resolution.onValueChanged.AddListener(OnResolutionChanged);
        quality.onValueChanged.AddListener(OnQualityChanged);
        
        #region Resolution
        
        int resolutionIndex = 0;
        
        List<string> options = new List<string>();

        resolutions = Screen.resolutions;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width < 800) continue;
            
            options.Add(resolutions[i].width + "x" + resolutions[i].height);
            
            if (resolutions[i].width != Screen.currentResolution.width ||
                    resolutions[i].height != Screen.currentResolution.height) continue;
            
                resolutionIndex = i;
        }

        resolution.ClearOptions();
        resolution.AddOptions(options);
        resolution.value = PlayerPrefs.GetInt("Resolution", resolutionIndex);
        resolution.RefreshShownValue();

        #endregion
        
        #region Quality
        
        _qualityLevels = QualitySettings.names;
        
        quality.ClearOptions();
        quality.AddOptions(QualitySettings.names.ToList());
        quality.value = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        quality.RefreshShownValue();

        #endregion
    }

    private void OnDestroy()
    {
        masterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        effectsVolume.onValueChanged.RemoveListener(OnEffectsVolumeChanged);
        musicVolume.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        
        fullScreen.onValueChanged.RemoveListener(OnFullScreenChanged);
        resolution.onValueChanged.RemoveListener(OnResolutionChanged);
        quality.onValueChanged.RemoveListener(OnQualityChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("Master Volume", value);
        
        audioMixer.audioMixer.SetFloat("Master", Mathf.Lerp(-80, 0, value));
    }

    private void OnEffectsVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("Effects Volume", value);
        
        audioMixer.audioMixer.SetFloat("Effects", Mathf.Lerp(-80, 0, value));
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("Music Volume", value);
        
        audioMixer.audioMixer.SetFloat("Music", Mathf.Lerp(-80, 0, value));
    }

    private void OnFullScreenChanged(bool fullScreenValue)
    {
        PlayerPrefs.SetInt("Full Screen", fullScreenValue ? 1 : 0);
        
        Screen.fullScreen = fullScreenValue;
    }

    private void OnResolutionChanged(int resolutionIndexValue)
    {
        PlayerPrefs.SetInt("Resolution", resolutionIndexValue);

        Resolution newResolution = resolutions[resolutionIndexValue];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }

    private void OnQualityChanged(int qualityLevelValue)
    {
        PlayerPrefs.SetInt("Quality", qualityLevelValue);

        QualitySettings.SetQualityLevel(qualityLevelValue, true);
    }
}
