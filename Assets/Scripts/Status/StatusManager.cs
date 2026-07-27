using System;
using System.Collections.Generic;
using UnityEngine;
using WorldTime;

namespace Status
{
    public class StatusManager : MonoBehaviour
    {
        public static StatusManager Instance { get; private set; }
        
        [SerializeField]private StatusSettingsSO statusSettings;

        private Dictionary<StatusType,StatusModule> statusMap = new Dictionary<StatusType, StatusModule>();

        //这两个到时候也应该被存档
        private Dictionary<StatusType,DateTime> statusLastUpdateMap = new Dictionary<StatusType, DateTime>();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (statusSettings == null)
            {
                Debug.LogWarning("status settings is missing ");
                this.enabled = false;
                return;
            }
        }
        private void Start()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnMinuteChanged += OnTimeChanged;
            }
        }

        private void OnDestroy()
        {
            TimeManager.Instance.OnMinuteChanged -= OnTimeChanged;
            
            if(Instance == this)
            {
                Instance = null;
            }
        }
        
        public void InitializeStatusFromConfig()
        {
            if (statusSettings == null)
                return;

            statusMap.Clear();
            statusLastUpdateMap.Clear();
            
            foreach (var config in statusSettings.InitialStatusConfigs)
            {
                statusLastUpdateMap[config.statusType] = TimeManager.Instance.CurrentTime;
                
                var newModule =new StatusModule(config.statusType,config.defaultValue,config.defaultMax,config.defaultMin);
                statusMap[config.statusType] = newModule;
            }
        }

        private void OnTimeChanged(DateTime currentTime)
        {
            if (!GameFlowManager.Instance.isPlaying)
                return;
            
            foreach (var config in statusSettings.InitialStatusConfigs)
            {
                TimeSpan interval = TimeSpan.FromMinutes(config.intervalMinute);
                StatusType statusType = config.statusType;

                if (!statusLastUpdateMap.TryGetValue(statusType, out DateTime lastUpdate))
                {
                    lastUpdate = currentTime;
                    statusLastUpdateMap[statusType] = lastUpdate;
                }

                TimeSpan passedMinutes = currentTime - lastUpdate;
                if (passedMinutes >= interval)
                {
                    int steps = (int)(passedMinutes.Ticks/interval.Ticks);
                    ChangeStatusValue(statusType, config.changePerInterval * steps);
                    statusLastUpdateMap[statusType] = lastUpdate.AddTicks(steps * interval.Ticks);
                }
            }
        }

        public bool HasEnoughStatus(StatusType statusType, float amount)
        {
            if (statusMap.TryGetValue(statusType, out StatusModule module))
            {
                return module.HasEnough(amount);
            }
            return false;
        }

        public bool TryConsumeStatus(StatusType statusType, float amount)
        {
            if (statusMap.TryGetValue(statusType, out StatusModule module))
            {
                return module.TryConsume(amount);
            }
            return false;
        }
        
        public void ChangeStatusValue(StatusType statusType, float amount)
        {
            if (statusMap.TryGetValue(statusType, out StatusModule module))
            {
                 module.ChangeValue(amount);
            }
        }

        public StatusModule GetStatusModule(StatusType statusType)
        {
            statusMap.TryGetValue(statusType, out StatusModule module);
            return module;
        }
    }
}