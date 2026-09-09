using UnityEngine;
using System;

namespace WorldTime
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Time Flow Settings")] 
        [Tooltip("时间流速")] [SerializeField] private float timeMultiplier = 60f;

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

        public static TimeManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void InitializeTime()
        {
            GameStartTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, startSecond);
            CurrentTime = GameStartTime;

            lastDay = CurrentTime.Day;
            lastHour = CurrentTime.Hour;
            lastMinute = CurrentTime.Minute;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (!GameFlowManager.Instance.isGameActive)
                return;
            
            float passedTime = Time.deltaTime * timeMultiplier * timeScale;
            TimeSpan passedTimeSpan = TimeSpan.FromSeconds(passedTime);
            CurrentTime = CurrentTime.Add(passedTimeSpan);

            CheckTime();
        }
        
        //此方法处理正常时间流逝产生的变化
        //为了可靠性还是改成了平级而非嵌套
        private void CheckTime()
        {
            if (CurrentTime.Minute != lastMinute)
            {
                lastMinute = CurrentTime.Minute;
                OnMinuteChanged?.Invoke(CurrentTime);
            }
            
            if (CurrentTime.Hour != lastHour)
            {
                lastHour = CurrentTime.Hour;
                OnHourChanged?.Invoke(CurrentTime);
            }
            
            if (CurrentTime.Day != lastDay)
            {
                lastDay = CurrentTime.Day;
                OnDayChanged?.Invoke(CurrentTime);
            }
        }
        
        private void HandleTimeJump(DateTime oldTime, DateTime targetTime)
        {
            TimeSpan diff = targetTime - oldTime;

            lastDay = targetTime.Day;
            lastHour = targetTime.Hour;
            lastMinute = targetTime.Minute;
            
            bool isMinutePassed = oldTime.Minute != targetTime.Minute || diff.TotalMinutes >= 1;
            if (isMinutePassed) OnMinuteChanged?.Invoke(targetTime);
            
            bool isHourPassed = oldTime.Hour != targetTime.Hour || diff.TotalHours >= 1;
            if (isHourPassed) OnHourChanged?.Invoke(targetTime);
            
            bool isDayPassed = oldTime.Day != targetTime.Day || diff.TotalDays >= 1;
            if (isDayPassed) OnDayChanged?.Invoke(targetTime);
        }

        #region 强制设置时间

        //这里应该只用于测试的时候便捷跳转时间
        //仅仅时间跳转而不含有属性处理，
        //因为考虑到美观问题（一长串），暂时没提供设置年份月份，如果需要可以改
        [Header("Time Jump Settings")] 
        [SerializeField] private int targetDay;
        [SerializeField] private int targetHour;
        [SerializeField] private int targetMinute;

        [ContextMenu("跳转时间")]
        private void SetNewTimeForTest()
        {
            SetNewTime(targetDay, targetHour, targetMinute);
        }

        #endregion

        #region 时间流逝

        //这些方法是对外接口
        private void SkipTime(TimeSpan duration)
        {
            DateTime oldTime = CurrentTime;
            DateTime targetTime = CurrentTime.Add(duration);
            CurrentTime = targetTime;
            HandleTimeJump(oldTime, targetTime);
        }

        public void SkipDays(int skipDay)
        {
            SkipTime(TimeSpan.FromDays(skipDay));
        }

        public void SkipHours(int skipHour)
        {
            SkipTime(TimeSpan.FromHours(skipHour));
        }

        public void SkipMinutes(int skipMinute)
        {
            SkipTime(TimeSpan.FromMinutes(skipMinute));
        }
        
        public void SetNewTime(int setDay, int setHour, int setMinute)
        {
            DateTime setTime = new DateTime(CurrentTime.Year, CurrentTime.Month, setDay, setHour, setMinute,
                CurrentTime.Second);
            if (setTime <= CurrentTime)
                return;
            TimeSpan setTimeSpan = setTime - CurrentTime;
            SkipTime(setTimeSpan);
        }

        public void SetNewTime(int setHour, int setMinute)
        {
            const int oneDayForError = 1;
            
            DateTime setTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, setHour, setMinute,
                CurrentTime.Second);
            
            if (setTime <= CurrentTime)
            {
                setTime = setTime.AddDays(oneDayForError);
                Debug.Log($"传入时间小于当前时间，已默认为跳转到第二天设置时间{setTime}，若非第二天请检查调用方法或增添跳转天数参数");
            }
            
            TimeSpan setTimeSpan = setTime - CurrentTime;
            SkipTime(setTimeSpan);
        }
        #endregion
        
        //这里应有留给存读档的接口，但因为未明确存读档方式所以不写了
        //如果之后开局时间需要动态生成需要save start和current两个时间
    }
}

