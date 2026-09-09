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
                                                                                                                                                                                         
        [Header("一天光强曲线")]                                                                                                                                 
        [SerializeField] private AnimationCurve dayLightCurve = new AnimationCurve(
            new Keyframe(0.00f, 0.05f), 
            new Keyframe(0.21f, 0.05f),  
            new Keyframe(0.25f, 0.25f), 
            new Keyframe(0.50f, 1.00f),  
            new Keyframe(0.67f, 0.55f),  
            new Keyframe(0.71f, 0.30f),  
            new Keyframe(0.75f, 0.12f),  
            new Keyframe(0.79f, 0.06f),  
            new Keyframe(1.00f, 0.05f)  
        );
        
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

            float lightFactor = dayLightCurve.Evaluate(dayProgress);
            float baseIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, lightFactor);

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
    }
}