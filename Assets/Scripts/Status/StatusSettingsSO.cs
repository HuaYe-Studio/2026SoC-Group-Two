using System.Collections.Generic;
using UnityEngine;

namespace Status
{
    public enum StatusType
    {
        Healthy,     
        Hungry,
        Mental,
        Stamina    //体力
    }
    
    [CreateAssetMenu(fileName = "NewStatusSettings", menuName = "Settings/Status Settings")]
    public class StatusSettingsSO: ScriptableObject
    {
        [System.Serializable]
        public struct StatusConfig
        {
            public StatusType statusType;
            public float defaultMax;
            public float defaultMin;
            public float defaultValue;
            public float intervalMinute;
            public float changePerInterval;
        }
        
         public List<StatusConfig> InitialStatusConfigs = new List<StatusConfig>();
    }
}