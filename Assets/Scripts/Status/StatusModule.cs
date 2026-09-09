using UnityEngine;
using System;

namespace Status
{
    [System.Serializable]
    public class StatusModule
    {
        public StatusType StatusType { get; private set; }
        public float CurrentValue { get; private set; }
        public float MaxValue { get; private set; }
        public float MinValue { get; private set; }
        
        public event Action<float,float> OnStatusChanged;    
        public event Action OnStatusMax;
        public event Action OnStatusMin;
        
        public StatusModule(StatusType statusType,float initialValue,float maxValue,float minValue)
        {
            StatusType = statusType;
            MaxValue = maxValue;
            MinValue = minValue;
            CurrentValue = Mathf.Clamp(initialValue,minValue,maxValue);
        }

        public bool HasEnough(float costValue)
        {
            return CurrentValue >= costValue;
        }

        public bool TryConsume(float costValue)
        {
            if (costValue <= 0)
            {
                Debug.Log("消耗值应为正数，已按照相反数调用方法");
                costValue = Mathf.Abs(costValue);
            }

            if (HasEnough(costValue))
            {
                ChangeValue(-costValue);
                return true;
            }
            
            return false;
        }
        
        public void ChangeValue(float amount)
        {
            float oldValue = CurrentValue;
            float newValue = Mathf.Clamp(CurrentValue + amount, MinValue, MaxValue);

            if (Mathf.Approximately(oldValue, newValue))
                return;
            CurrentValue = newValue;
            
            OnStatusChanged?.Invoke(CurrentValue, MaxValue);

            if (CurrentValue >= MaxValue)
                OnStatusMax?.Invoke();
            if (CurrentValue <= MinValue)
                OnStatusMin?.Invoke();
        }
    }
}