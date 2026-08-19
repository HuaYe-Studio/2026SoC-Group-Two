using System;
using System.Collections.Generic;
using UnityEngine;
using WorldTime;
using System.Threading.Tasks;

namespace Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance{ get; private set; }
        
        [Header("场景光源")]
        [SerializeField]private Light sunLight;
        
        [Header("SO配置")]
        [SerializeField]private List<SkySettingsSO>  skySettings = new List<SkySettingsSO>();
        private Dictionary<(TimePeriod,WeatherType), SkySettingsSO> skySettingsMap = new Dictionary<(TimePeriod,WeatherType), SkySettingsSO>();
        private SkySettingsSO currentSetting;
        private TimePeriod currentTimePeriod = TimePeriod.Day;
        private WeatherType currentWeatherType = WeatherType.Sunny;
        
        [Header("时间切换配置")]
        [SerializeField]private List<SkySchedule> dailySchedules = new List<SkySchedule>();
        [SerializeField]private float transitionDuration = 10f;
        private bool isTransitioning = false;

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

        private void Start()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnMinuteChanged -= CheckSchedule;
                TimeManager.Instance.OnMinuteChanged += CheckSchedule;
            }
            
        }

        private void OnDestroy()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnMinuteChanged -= CheckSchedule;
            }
        }
        public void InitializeWeather()
        {
            skySettingsMap.Clear();
            
            foreach (SkySettingsSO settings in skySettings)
            {
                var weatherState = (settings.timePeriod, settings.weatherType);
                skySettingsMap.TryAdd(weatherState, settings);
            }
            currentTimePeriod=GetTimePeriod(TimeManager.Instance.CurrentTime);
            RefreshEnvironment(isInstant:true);
        }

        private TimePeriod GetTimePeriod(DateTime time)
        {
            int currentMinutes= time.Hour * 60 + time.Minute;
            
            if (dailySchedules.Count == 0)                                                                                                                                               
                return TimePeriod.Day;
            
            TimePeriod resultPeriod = dailySchedules[dailySchedules.Count - 1].timePeriod;
            int maxpastMinutes = -1;

            foreach (var schedule in dailySchedules)
            {
                int periodMinutes =schedule.triggerHour * 60 + schedule.triggerMinute;

                if (periodMinutes <= currentMinutes && periodMinutes > maxpastMinutes)
                {
                    maxpastMinutes = periodMinutes;
                    resultPeriod = schedule.timePeriod;
                }
            }
            
            return resultPeriod;
        }
        private void CheckSchedule(DateTime currentTime)
        {
            if(isTransitioning)
                return;

            TimePeriod targetTimePeriod = GetTimePeriod(currentTime);
            if (targetTimePeriod != currentTimePeriod)
            {
                bool shouldInstant = false;
                
                int oldIndex = dailySchedules.FindIndex(x => x.timePeriod == currentTimePeriod);
                int newIndex = dailySchedules.FindIndex(x => x.timePeriod == targetTimePeriod);

                if (oldIndex != -1 && newIndex != -1)
                {
                    int count = dailySchedules.Count;
                    int distance = (newIndex - oldIndex + count) % count;
                    if (distance > 1)
                        shouldInstant = true;
                }
                currentTimePeriod = targetTimePeriod;
                RefreshEnvironment(isInstant : shouldInstant);
            }
        }
        
        public void ChangeWeather(WeatherType newWeatherType)
        {
            if(currentWeatherType == newWeatherType)
                return;
            
            currentWeatherType = newWeatherType;
            RefreshEnvironment(isInstant:false);
        }

        private void RefreshEnvironment(bool isInstant)
        {
            var targetWeatherState = (currentTimePeriod, currentWeatherType);
            if (!skySettingsMap.TryGetValue(targetWeatherState, out SkySettingsSO targetSetting))
            {
                Debug.LogWarning($"({currentTimePeriod}, {currentWeatherType}) 没有配置 SkySettingsSO，请检查 skySettings 列表");
                return;
            }

            if (targetSetting == currentSetting)
                return;
            
            if (isInstant)
            {
                currentSetting = targetSetting;
                ChangeSkySettingsInstant(currentSetting);
            }
            else
            {
                _ = ChangeSkySettingsAsync(targetSetting);
            }
        }

        private async Task ChangeSkySettingsAsync(SkySettingsSO targetSetting)
        {
            isTransitioning = true;

            Color startSunLightColor = sunLight != null ? sunLight.color : Color.white;
            Color startAmbientColor = RenderSettings.ambientLight;
            
            //静态转程序化
            Color startSkyTint = Color.gray;
            Color startGroundColor = Color.gray;

            if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_SkyTint"))
            {
                startSkyTint = RenderSettings.skybox.GetColor("_SkyTint");
                startGroundColor = RenderSettings.skybox.GetColor("_GroundColor");
            }
            
            float elapsedTime = 0f;
            bool isSkyboxChanged = false;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;
                
                if(sunLight!=null)
                    sunLight.color = Color.Lerp(startSunLightColor, targetSetting.sunLightColor, t);
                RenderSettings.ambientLight = Color.Lerp(startAmbientColor, targetSetting.ambientLightColor, t);

                if (targetSetting.skyMode == SkyMode.Procedural)
                {
                    if (!isSkyboxChanged && targetSetting.skyMaterial != null)
                    {
                        RenderSettings.skybox=targetSetting.skyMaterial;
                        isSkyboxChanged = true;
                    }

                    if (RenderSettings.skybox != null)
                    {
                        RenderSettings.skybox.SetColor("_SkyTint",
                            Color.Lerp(startSkyTint, targetSetting.proceduralSkyTint, t));
                        RenderSettings.skybox.SetColor("_GroundColor",Color.Lerp(startGroundColor, targetSetting.proceduralGroundColor, t));
                    }
                }
                else
                {
                    if (t >= 0.5f && !isSkyboxChanged && targetSetting.skyMaterial != null)
                    {
                        RenderSettings.skybox =  targetSetting.skyMaterial;
                        DynamicGI.UpdateEnvironment();
                        isSkyboxChanged = true;
                    }
                }

                await Task.Yield();
            }

            RenderSettings.fog = targetSetting.enableFog;
            RenderSettings.fogColor = targetSetting.fogColor;
            RenderSettings.fogDensity = targetSetting.fogDensity;
            
            currentSetting = targetSetting;
            isTransitioning = false;
        }
        private void ChangeSkySettingsInstant(SkySettingsSO settings)
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