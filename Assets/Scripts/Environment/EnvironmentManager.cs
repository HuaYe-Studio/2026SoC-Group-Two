using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance{ get; private set; }
        
        [SerializeField]private Light sunLight;
        [SerializeField]private List<SkySettingsSO>  skySettings = new List<SkySettingsSO>();
        private Dictionary<WeatherType, SkySettingsSO> skySettingsMap = new Dictionary<WeatherType, SkySettingsSO>();
        private SkySettingsSO currentSetting;

        public float CurrentSunIntensityMultiplier =>
            currentSetting != null ? currentSetting.lightIntensityMultiplier : 1.0f;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void InitializeWeather()
        {
            skySettingsMap.Clear();
            
            foreach (SkySettingsSO settings in skySettings)
            {
                skySettingsMap.TryAdd(settings.weatherType, settings);
            }
            
            ChangeWeather(WeatherType.Day_Sunny);
        }

        public void ChangeWeather(WeatherType weatherType)
        {
            if (!skySettingsMap.TryGetValue(weatherType, out SkySettingsSO newSettings))
                return;
            currentSetting = newSettings;
            
            ChangeSkySettings(currentSetting);
        }

        private void ChangeSkySettings(SkySettingsSO settings)
        {
            if (settings.skyMaterial == null)
                return;
            
            RenderSettings.skybox = settings.skyMaterial;

            if (settings.skyMode == SkyMode.Procedural)
            {
                RenderSettings.skybox.SetColor("_GroundColor",settings.proceduralGroundColor);
                RenderSettings.skybox.SetColor("_SkyTint",settings.proceduralSkyTint);
            }
            
            if(sunLight!=null)
                sunLight.color = settings.sunLightColor;
            
            RenderSettings.ambientLight = settings.ambientLightColor;
            RenderSettings.fog = settings.enableFog;
            RenderSettings.fogColor = settings.fogColor;
            RenderSettings.fogDensity = settings.fogDensity;
            
            DynamicGI.UpdateEnvironment();
        }
    }
}