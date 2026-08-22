using UnityEngine;
using System;
using System.Collections.Generic;
using WorldTime;
using Scene;
using Status;
using Environment;
using WorldSeed;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }
    
    private DateTime gameEndTime;
    private int gameTotalDay = 7;
    
    public bool isPlaying { get; private set; }
    private HashSet<string> pauseLocks = new HashSet<string>();
    public bool isGameActive => isPlaying && pauseLocks.Count > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged -= CheckGameEndForTime;
        }

        if (StatusManager.Instance != null)
        {
            StatusManager.Instance.OnAnyStatusEmpty -= CheckGameEndForStatus;
        }
        
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region 游戏开始相关逻辑

    public async void OnGameStart()
    {
        isPlaying = false;
        
        await GameSceneManager.Instance.ChangeWorldSceneAsync(SceneType.Gameplay);
        await GameSceneManager.Instance.ChangeUISceneAddictiveAsync(SceneType.UI_Gameplay);
        
        InitializeGame();
    }

    //之后挂在mainmenu的按钮上即可
    private void InitializeGame()
    {
        if (TimeManager.Instance == null|| StatusManager.Instance == null || EnvironmentManager.Instance == null || WorldSeedManager.Instance == null)
        {
            this.enabled = false;
            return;
        }
        
        WorldSeedManager.Instance.InitializeWorldSeed();
        TimeManager.Instance.InitializeTime();
        StatusManager.Instance.InitializeStatusFromConfig();
        EnvironmentManager.Instance.InitializeWeather();
        
        gameEndTime = TimeManager.Instance.GameStartTime.AddDays(gameTotalDay);

        if (TimeManager.Instance.CurrentTime >= gameEndTime)
            return;
        
        isPlaying = true;
        
        TimeManager.Instance.OnHourChanged -= CheckGameEndForTime;
        TimeManager.Instance.OnHourChanged += CheckGameEndForTime;
        
        StatusManager.Instance.OnAnyStatusEmpty -= CheckGameEndForStatus;
        StatusManager.Instance.OnAnyStatusEmpty += CheckGameEndForStatus;
    }

    #endregion
    #region 退出相关逻辑
    private void QuitGame()
    {
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void CheckGameEndForTime(DateTime checkTime)
    {
        if (checkTime >= gameEndTime)
        {
            TimeManager.Instance.OnHourChanged -= CheckGameEndForTime;
            OnGameEnd();
        }
    }

    private void CheckGameEndForStatus(StatusType emptyStatusType)
    {
        if (emptyStatusType == StatusType.Healthy || emptyStatusType == StatusType.Hungry)
        {
            Debug.Log($"因为{emptyStatusType}归零，确认游戏失败");
            OnGameEnd();
        }
    }

    private void OnGameEnd()
    {
        isPlaying = false;
        QuitGame();
    }
    
    #endregion
    
    public void PauseGameplay(string pauseReason)
    {
        if (pauseLocks.Add(pauseReason))
        {
            Debug.Log($"游戏因为{pauseReason}申请已暂停");
        }
    }
    
    public void ResumeGameplay(string pauseReason)
    {
        if (pauseLocks.Remove(pauseReason))
        {
            Debug.Log($"游戏已经解锁因为{pauseReason}申请的暂停");
        }
    }
}

