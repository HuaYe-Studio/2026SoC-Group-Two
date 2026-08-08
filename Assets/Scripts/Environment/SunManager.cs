using UnityEngine;
using System;
using WorldTime;

namespace Environment
{
    public class SunManager: MonoBehaviour
    {
        [SerializeField]private Light sunLight;
        
        [Header("Light Settings")]
        [SerializeField]private float maxLightIntensity = 1.2f;
        [SerializeField]private float minLightIntensity = 0.1f;
        [SerializeField]private float sunYRotation = 30f;
        
        private const float MinuteOfHour = 60f;
        private const float SecondOfHour = 60f;
        private const float MinuteOfDay = 1440f;
        
        private const float AngleXInMidNight = -90f;
        private const float AngleXInNewMidNight = 270f;

        private void Start()
        {
            if(sunLight == null)
                sunLight = GetComponent<Light>();
            
            RenderSettings.sun = sunLight;
        }

        //非常简陋的圆形轨迹，可优化
        private void Update()
        {
            if (GameFlowManager.Instance == null || GameFlowManager.Instance.isPlaying == false ||
                TimeManager.Instance == null || sunLight == null)
                return;

            float dayProgress = CalculateDayProgress();

            float sunXAngle = CalculateSunXAngle(dayProgress);
            sunLight.transform.rotation = Quaternion.Euler(sunXAngle,sunYRotation, 0f);

            float intensityFactor = CalculateSunLightIntensity(dayProgress);
            float baseIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, intensityFactor);

            float weatherLightMultiplier = (EnvironmentManager.Instance != null)
                ? EnvironmentManager.Instance.CurrentSunIntensityMultiplier
                : 1.0f;
            sunLight.intensity = weatherLightMultiplier * baseIntensity;
        }

        private float CalculateDayProgress()
        {
            DateTime currentTime = TimeManager.Instance.CurrentTime;
            float currentTotalMinutes =
                currentTime.Hour * MinuteOfHour + currentTime.Minute + currentTime.Second / SecondOfHour;
            float dayProgress = currentTotalMinutes / MinuteOfDay;
            
            return dayProgress;
        }
        private float CalculateSunXAngle(float dayProgress)
        {
            float sunXAngle = Mathf.Lerp(AngleXInMidNight, AngleXInNewMidNight, dayProgress);
            return sunXAngle;
        }
        private float CalculateSunLightIntensity(float dayProgress)
        {
            //早上六点强度为0，十八点为0.以此变换正弦波
            //之后如果要有季节乘以季节系数可能吧
            float sineValue = Mathf.Sin((dayProgress - 0.25f) * Mathf.PI * 2f);
            
            return Mathf.Clamp01(sineValue);
        }
    }
}