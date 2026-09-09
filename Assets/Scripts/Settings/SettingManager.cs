using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace Settings
{
    public class SettingManager : MonoBehaviour
    {
        public static SettingManager Instance { get; private set; }
        
        [SerializeField]private AudioMixer audioMixer;

        public float MasterVolume { get; private set; } = 1f;
        public float MusicVolume { get; private set; } = 1f;
        public float SfxVolume { get; private set; } = 1f;

        public bool IsFullscreen { get; private set; } = true;
        public int ResolutionIndex { get; private set; } = 0;
        public int FrameRateIndex { get; private set; } = 1;
        public int LanguageIndex { get; private set; } = 0;
        
        public Resolution[] AvailableResolutions {get; private set;}
        public readonly int[] AvailableFrameRates = { 30, 60, 120, -1 };
        public readonly string[] AvailableLanguages = { "zh-CN", "en-US" };
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            InitializeResolutions();
            LoadSettings();
        }

        private void Start()
        {
            ApplySettings();
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void InitializeResolutions()
        {
            Resolution[] allResolutions = Screen.resolutions;
            
            AvailableResolutions = allResolutions
                .Where(resolution => resolution.width >= 800)
                .GroupBy(resolution => new { resolution.width, resolution.height })
                .Select(group => group.OrderByDescending(resolution => resolution.refreshRateRatio.value).First())
                .OrderBy(resolution => resolution.width).ThenBy(resolution => resolution.height)
                .ToArray();

            if (AvailableResolutions.Length == 0)
                AvailableResolutions = new Resolution[] { Screen.currentResolution };
        }

        public void LoadSettings()
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
            
            IsFullscreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
            FrameRateIndex = PlayerPrefs.GetInt("FrameRateIndex", 1);
            LanguageIndex = PlayerPrefs.GetInt("LanguageIndex", 0);
            
            int defaultResolutionIndex = AvailableResolutions.Length>0 ? AvailableResolutions.Length - 1 : 0;
            int saveResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", defaultResolutionIndex);
            ResolutionIndex = Mathf.Clamp(saveResolutionIndex, 0, Mathf.Max(0, AvailableResolutions.Length - 1));
        }
        
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
            PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
            
            PlayerPrefs.SetInt("IsFullscreen", IsFullscreen ? 1 : 0);
            PlayerPrefs.SetInt("FrameRateIndex", FrameRateIndex);
            PlayerPrefs.SetInt("LanguageIndex", LanguageIndex);
            PlayerPrefs.SetInt("ResolutionIndex", ResolutionIndex);
            
            PlayerPrefs.Save();
            Debug.Log("settings saved !!!");
        }

        private void ApplySettings()
        {
            SetMasterVolume(MasterVolume);
            SetMusicVolume(MusicVolume);
            SetSfxVolume(SfxVolume);
            
            SetIsFullscreen(IsFullscreen);
            SetFrameRate(FrameRateIndex);
            SetResolution(ResolutionIndex);
            SetLanguage(LanguageIndex);
        }

        private void OnApplicationQuit()
        {
            SaveSettings();
        }

        #region 具体设置

        public void SetMasterVolume(float value)
        {
            MasterVolume = Mathf.Clamp(value, 0.0001f, 1f);
            if (audioMixer != null)
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp(value, 0.0001f, 1f);
            if (audioMixer != null)
                audioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolume) * 20);
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp(value, 0.0001f, 1f);
            if (audioMixer != null)
                audioMixer.SetFloat("SfxVolume", Mathf.Log10(SfxVolume) * 20);
        }

        public void SetIsFullscreen(bool isFull)
        {
            IsFullscreen = isFull;
            Screen.fullScreen = isFull;
        }

        public void SetResolution(int index)
        {
            if (AvailableResolutions.Length <= index || index < 0 )
                return;
            
            ResolutionIndex = index;
            Resolution resolution = AvailableResolutions[ResolutionIndex];

            Screen.SetResolution(resolution.width, resolution.height,
                IsFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }

        public void SetFrameRate(int index)
        {
            if (AvailableFrameRates.Length <= index || index < 0 )
                return;
            
            int targetFrameRate = AvailableFrameRates[index];
            
            if (targetFrameRate == -1)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = targetFrameRate;
            }
        }
        
        public void SetLanguage(int index)
        {
            if (AvailableLanguages.Length <= index || index < 0)
                return;
            LanguageIndex = index;
            
            //待实现
        }

        public bool RebindKey(string action, Key newKey)
        {
            if(KeyManager.Instance==null)
                return false;
            return KeyManager.Instance.SetKey(action, newKey);
        }
        
        #endregion
    }
}