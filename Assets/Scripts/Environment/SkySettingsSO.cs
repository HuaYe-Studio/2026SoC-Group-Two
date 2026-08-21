using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    [CreateAssetMenu(menuName = "Settings/Sky Settings", fileName = "NewSkySettings")]
    public class SkySettingsSO: ScriptableObject
    {
        [Header("Basic Settings")]
        public WeatherType weatherType;
        public TimePeriod timePeriod;
        public SkyMode skyMode;
        
        [Header("Sky Material")]
        public Material skyMaterial;
        
        //基础设置，待添加其他参数
        [Header("Sky Light Settings")]
        [Range(0f,1f)]public float lightIntensityMultiplier = 1f;
        public Color sunLightColor = Color.white;
        public Color ambientLightColor = Color.white;

        [Header("Fog Settings")]
        public bool enableFog = false;
        public Color fogColor = Color.gray;
        [Range(0f,0.05f)]public float fogDensity = 0f;
        
        [Header("Procedural Sky Settings")]
        public Color proceduralGroundColor = new Color(0.3f, 0.3f, 0.3f);
        public Color proceduralSkyTint = new Color(0.5f, 0.5f, 0.5f);

    }
}