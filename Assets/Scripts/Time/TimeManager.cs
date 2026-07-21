using UnityEngine;
using System;

namespace WorldTime
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Time Flow Settings")]
        [Tooltip("时间流速")][SerializeField] private float timeMultiplier = 60f;
        [SerializeField] private float timeScale = 1f;

        [Header("Initial Time Settings")] 
        public DateTime gameStartTime { get; private set; }
        [SerializeField] private int startYear = 1;
        [SerializeField] private int startMonth = 1;
        [SerializeField] private int startDay = 1;
        [SerializeField] private int startHour = 8;
        [SerializeField] private int startMinute = 0;
        private int startSecond = 0;
        
        public DateTime currentTime { get; private set; }
 
        private int lastDay;
        private int lastHour;
        private int lastMinute;

        public event Action<DateTime> OnDayChanged;
        public event Action<DateTime> OnHourChanged;
        public event Action<DateTime> OnMinuteChanged;
        
        public static TimeManager instance;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeTime();
        }

        private void InitializeTime()
        {
            gameStartTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, startSecond);
            currentTime = gameStartTime;

            lastDay = currentTime.Day;
            lastHour = currentTime.Hour;
            lastMinute = currentTime.Minute;
        }

        private void Update()
        {
            float passedTime = Time.deltaTime * timeMultiplier * timeScale;
            TimeSpan passedTimeSpan = TimeSpan.FromSeconds(passedTime);
            currentTime = currentTime.Add(passedTimeSpan);

            CheckTime();
        }

        private void CheckTime()
        {
            if (currentTime.Minute != lastMinute)
            {
                lastMinute = currentTime.Minute;
                OnMinuteChanged?.Invoke(currentTime);
                if (currentTime.Hour != lastHour)
                {
                    lastHour = currentTime.Hour;
                    OnHourChanged?.Invoke(currentTime);
                    if (currentTime.Day != lastDay)
                    {
                        lastDay = currentTime.Day;
                        OnDayChanged?.Invoke(currentTime);
                    }
                }
            }
        }
        //这里应有留给存读档的接口，但因为未明确存读档方式所以不写了
        //如果之后开局时间需要动态生成需要save start和current两个时间
    }
}

