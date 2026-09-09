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
        private float currentMultiplier = 1f;                                                                                                   
        private float targetMultiplier = 1f;                                                                                                       
        [SerializeField] private float multiplierLerpSpeed = 0.05f; 
        
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
        private DateTime lastCheckedTime;
        private float timeBigJumpThreshold = 120f;
        private float timeJumpThreshold = 10f;

        public float CurrentSunIntensityMultiplier => currentMultiplier;
        
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
            
            lastCheckedTime = TimeManager.Instance.CurrentTime;
        }

        void Update()
        {
            currentMultiplier = Mathf.Lerp(currentMultiplier, targetMultiplier, multiplierLerpSpeed);                                                                                    
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
            currentTimePeriod = GetTimePeriod(TimeManager.Instance.CurrentTime);
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

            TimeSpan diff = currentTime - lastCheckedTime;
            lastCheckedTime = currentTime;
            
            TimePeriod targetTimePeriod = GetTimePeriod(currentTime);
            int periodTotalMinutes = 0;
            
            foreach (var schedule in dailySchedules)
            {
                if (schedule.timePeriod == targetTimePeriod)
                {
                    periodTotalMinutes = schedule.triggerHour * 60 + schedule.triggerMinute;
                    break;
                }
            }

            int currentTotalMinutes = currentTime.Hour * 60 + currentTime.Minute;
            int minutesToPeriod = currentTotalMinutes - periodTotalMinutes;
            if (minutesToPeriod < 0)
                minutesToPeriod = 1440;

            bool isTimeBigJump = diff.TotalMinutes >= timeBigJumpThreshold;
            bool isJustStart = minutesToPeriod <= timeJumpThreshold;
            bool shouldInstant = isTimeBigJump && !isJustStart;

            if (targetTimePeriod != currentTimePeriod)
            {
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
            
            targetMultiplier = targetSetting.lightIntensityMultiplier;
            
            if (isInstant)
            {
                currentSetting = targetSetting;
                targetMultiplier = targetSetting.lightIntensityMultiplier;
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

            Material transitionMat = Material.Instantiate(RenderSettings.skybox);                                                                                                        
            RenderSettings.skybox = transitionMat;                                                                                                                                       
                                                                                                                                                                                   
            Material targetMat = targetSetting.skyMaterial;
            
            Color startTint = transitionMat.HasProperty("_Tint") ? transitionMat.GetColor("_Tint") : Color.white;                                                                        
            Color targetTint = targetMat.HasProperty("_Tint") ? targetMat.GetColor("_Tint") : Color.white;                                                                               
            float startExposure = transitionMat.HasProperty("_Exposure") ? transitionMat.GetFloat("_Exposure") : 1f;                                                                     
            float targetExposure = targetMat.HasProperty("_Exposure") ? targetMat.GetFloat("_Exposure") : 1f;                                                                            
                                                                                                                                                                                   
            Color startSunLightColor = sunLight != null ? sunLight.color : Color.white;                                                                                                  
            Color startAmbientColor = RenderSettings.ambientLight;                                                                                                                       
                                                                                                                                                                                   
            float elapsedTime = 0f;                                                                                                                                                      
            bool isTextureSwapped = false;                                                                                                                                               
                                                                                                                                                                                   
            while (elapsedTime < transitionDuration)                                                                                                                                     
            {                                                                                                                                                                            
                elapsedTime += Time.deltaTime;                                                                                                                                           
                float t = elapsedTime / transitionDuration;                                                                                                                              
                t = t * t * (3f - 2f * t); 
                if (sunLight != null)                                                                                                                                                    
                    sunLight.color = Color.Lerp(startSunLightColor, targetSetting.sunLightColor, t);                                                                                     
                RenderSettings.ambientLight = Color.Lerp(startAmbientColor, targetSetting.ambientLightColor, t);                                                                         
                
                transitionMat.SetColor("_Tint", Color.Lerp(startTint, targetTint, t));                                                                                                   
                transitionMat.SetFloat("_Exposure", Mathf.Lerp(startExposure, targetExposure, t));                                                                                       
                
                if (t >= 0.5f && !isTextureSwapped)                                                                                                                                      
                {                                                                                                                                                                        
                    Texture targetTex = targetMat.GetTexture("_Tex");                                                                                                                    
                    if (targetTex != null)                                                                                                                                               
                        transitionMat.SetTexture("_Tex", targetTex);                                                                                                                     
                    isTextureSwapped = true;                                                                                                                                             
                }                                                                                                                                                                        
                                                                                                                                                                                   
                await Task.Yield();                                                                                                                                                      
            }                                                                                                                                                                            
                                                                                                                                                                                   
            RenderSettings.skybox = targetMat;                                                                                                      
            RenderSettings.fog = targetSetting.enableFog;                                                                                                                                
            RenderSettings.fogColor = targetSetting.fogColor;                                                                                                                            
            RenderSettings.fogDensity = targetSetting.fogDensity;                                                                                                                        
                                                                                                                                                                                   
            currentSetting = targetSetting; 
            targetMultiplier = targetSetting.lightIntensityMultiplier;
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