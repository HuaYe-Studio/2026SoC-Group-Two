using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Settings;
using TMPro;
using UnityEngine.Events;

public class SettingUIPanels : MonoBehaviour
    {
        [Header("音频组件")]
        [SerializeField]private Slider masterVolumeSlider;
        [SerializeField]private Slider musicVolumeSlider;
        [SerializeField]private Slider sfxVolumeSlider;
        
        [Header("数值文本")]
        [SerializeField]private TMP_Text masterVolumeText;
        [SerializeField]private TMP_Text musicVolumeText;
        [SerializeField]private TMP_Text sfxVolumeText;
        
        [Header("图像组件")]
        [SerializeField]private Toggle fullscreenToggle;
        [SerializeField]private TMP_Dropdown resolutionDropdown; 
        [SerializeField]private TMP_Dropdown frameRateDropdown;
        
        [Header("其他组件")]
        [SerializeField]private Button applyButton;
        [SerializeField]private Button closeButton;

        [Header("事件")]
        public UnityEvent OnCloseButtonClicked;

        //图像设置不支持预览，点击应用才会应用
        private bool tempIsFullscreen;
        private int tempResolutionIndex;
        private int tempFrameRateIndex;
        
        //音频设置预览效果的备份
        private float originalMasterVolume;
        private float originalMusicVolume;
        private float originalSfxVolume;
        
        private void Awake()
        {
            masterVolumeSlider.onValueChanged.AddListener(value =>
            {
                SettingManager.Instance?.SetMasterVolume(value);
                masterVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            });
            musicVolumeSlider.onValueChanged.AddListener(value=>
            {
                SettingManager.Instance?.SetMusicVolume(value);
                musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            });
            sfxVolumeSlider.onValueChanged.AddListener(value=>
            {
                SettingManager.Instance?.SetSfxVolume(value);
                sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            });

            fullscreenToggle.onValueChanged.AddListener(isOn => tempIsFullscreen = isOn);
            resolutionDropdown.onValueChanged.AddListener(index => tempResolutionIndex = index);
            frameRateDropdown.onValueChanged.AddListener(index => tempFrameRateIndex = index);
            
            applyButton.onClick.AddListener(OnApplyClicked);
            closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        private void OnApplyClicked()
        {
            if (SettingManager.Instance != null)
            {
                SettingManager.Instance.SetIsFullscreen(tempIsFullscreen);
                SettingManager.Instance.SetResolution(tempResolutionIndex);
                SettingManager.Instance.SetFrameRate(tempFrameRateIndex);

                originalMasterVolume = masterVolumeSlider.value;
                originalMusicVolume = musicVolumeSlider.value;
                originalSfxVolume = sfxVolumeSlider.value;
                
                SettingManager.Instance.SaveSettings();
            }
        }

        private void OnCloseClicked()
        {
            SettingManager.Instance.SetMasterVolume(originalMasterVolume);
            SettingManager.Instance.SetMusicVolume(originalMusicVolume);
            SettingManager.Instance.SetSfxVolume(originalSfxVolume);
            
            gameObject.SetActive(false);
            OnCloseButtonClicked?.Invoke();
    }

        //之后UIManager需要做一个防止重复打开的拦截
        private void OnEnable()
        {
            originalMasterVolume = SettingManager.Instance.MasterVolume;
            originalMusicVolume = SettingManager.Instance.MusicVolume;
            originalSfxVolume = SettingManager.Instance.SfxVolume;
            
            RefreshUI();
        }

        private void OnDisable()
        {
            SettingManager.Instance.SetMasterVolume(originalMasterVolume);
            SettingManager.Instance.SetMusicVolume(originalMusicVolume);
            SettingManager.Instance.SetSfxVolume(originalSfxVolume);
        }

        public void OpenPanel()
        {
            if (gameObject.activeSelf) 
                return;
            GameFlowManager.Instance?.PauseGameplay("Settings");
            RefreshUI();
            gameObject.SetActive(true);
        }
        public void Close()
        {
            if (!gameObject.activeSelf) return;
            GameFlowManager.Instance?.ResumeGameplay("Settings");
            
            SettingManager.Instance.SetMasterVolume(originalMasterVolume);
            SettingManager.Instance.SetMusicVolume(originalMusicVolume);
            SettingManager.Instance.SetSfxVolume(originalSfxVolume);
            
            gameObject.SetActive(false);
        }
        
        private void RefreshUI()
        {
            if (SettingManager.Instance == null)
            {
                Debug.Log("Setting Manager Instance Null");
                return;
            }
            
            //刷新音量设置
            masterVolumeSlider.SetValueWithoutNotify(SettingManager.Instance.MasterVolume);
            float masterVolume = masterVolumeSlider.value;
            masterVolumeText.text = $"{Mathf.RoundToInt(masterVolume * 100)}%";
            
            musicVolumeSlider.SetValueWithoutNotify(SettingManager.Instance.MusicVolume);
            float musicVolume = musicVolumeSlider.value;
            musicVolumeText.text = $"{Mathf.RoundToInt(musicVolume * 100)}%";
            
            sfxVolumeSlider.SetValueWithoutNotify(SettingManager.Instance.SfxVolume);
            float sfxVolume = sfxVolumeSlider.value;
            sfxVolumeText.text = $"{Mathf.RoundToInt(sfxVolume * 100)}%";
            
            //填充临时变量
            tempIsFullscreen = SettingManager.Instance.IsFullscreen;
            tempResolutionIndex = SettingManager.Instance.ResolutionIndex;
            tempFrameRateIndex = SettingManager.Instance.FrameRateIndex;
            
            //刷新图形设置
            fullscreenToggle.SetIsOnWithoutNotify(tempIsFullscreen);
            
            resolutionDropdown.ClearOptions();
            List<string> resolutionOptions = new List<string>();
            foreach (var resolutions in SettingManager.Instance.AvailableResolutions)
            {
                resolutionOptions.Add($"{resolutions.width}x{resolutions.height} @ {resolutions.refreshRateRatio.value:0.##}Hz");
            }
            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.SetValueWithoutNotify(tempResolutionIndex);

            frameRateDropdown.ClearOptions();
            List<string> frameRateOptions = new List<string>();
            foreach (var frameRates in SettingManager.Instance.AvailableFrameRates)
            {
                frameRateOptions.Add(frameRates == -1? "不限制" : $"{frameRates} FPS");
            }
            frameRateDropdown.AddOptions(frameRateOptions);
            frameRateDropdown.SetValueWithoutNotify(tempFrameRateIndex);
        }
    }