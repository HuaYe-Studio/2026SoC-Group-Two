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
        [SerializeField] private int startYear = 1;
        [SerializeField] private int startMonth = 1;
        [SerializeField] private int startDay = 1;
        [SerializeField] private int startHour = 8;
        [SerializeField] private int startMinute;
        public DateTime GameStartTime { get; private set; }
        private readonly int startSecond = 0;
        
        public DateTime CurrentTime { get; private set; }
 
        private int lastDay;
        private int lastHour;
        private int lastMinute;

        public event Action<DateTime> OnDayChanged;
        public event Action<DateTime> OnHourChanged;
        public event Action<DateTime> OnMinuteChanged;
        
        public static TimeManager Instance{ get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeTime();
        }

        private void InitializeTime()
        {
            GameStartTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, startSecond);
            CurrentTime = GameStartTime;

            lastDay = CurrentTime.Day;
            lastHour = CurrentTime.Hour;
            lastMinute = CurrentTime.Minute;
        }

        private void Update()
        {
            float passedTime = Time.deltaTime * timeMultiplier * timeScale;
            TimeSpan passedTimeSpan = TimeSpan.FromSeconds(passedTime);
            CurrentTime = CurrentTime.Add(passedTimeSpan);

            CheckTime();
        }

        private void CheckTime()
        {
            if (CurrentTime.Minute != lastMinute)
            {
                lastMinute = CurrentTime.Minute;
                OnMinuteChanged?.Invoke(CurrentTime);
                if (CurrentTime.Hour != lastHour)
                {
                    lastHour = CurrentTime.Hour;
                    OnHourChanged?.Invoke(CurrentTime);
                    if (CurrentTime.Day != lastDay)
                    {
                        lastDay = CurrentTime.Day;
                        OnDayChanged?.Invoke(CurrentTime);
                    }
                }
            }
        }
        //这里应有留给存读档的接口，但因为未明确存读档方式所以不写了
        //如果之后开局时间需要动态生成需要save start和current两个时间
    }
}

