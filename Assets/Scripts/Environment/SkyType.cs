using UnityEngine;

namespace Environment
{
    public enum SkyMode
    {
        HDR,
        Procedural
    }
    
    public enum WeatherType
    {
        Sunny,
    }
    
    public enum TimePeriod
    {
        Day,
        Night,
        SunSet,
    }

    [System.Serializable]
    public struct SkySchedule
    {
        public string stageName;
        [Range(0,23)]public int triggerHour;
        [Range(0,59)]public int triggerMinute;
        
        public TimePeriod timePeriod;
    }
}