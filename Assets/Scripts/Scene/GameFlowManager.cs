using UnityEngine;
using System;
using WorldTime;
using Scene;
using Status;
using Environment;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }
    
    private DateTime gameEndTime;
    private int gameTotalDay = 7;
    
    public bool isPlaying { get; private set; }

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
        //待inputsystem替换，先用着
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged -= CheckGameEnd;
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
        if (TimeManager.Instance == null|| StatusManager.Instance == null || EnvironmentManager.Instance == null)
        {
            this.enabled = false;
            return;
        }
        
        TimeManager.Instance.InitializeTime();
        StatusManager.Instance.InitializeStatusFromConfig();
        EnvironmentManager.Instance.InitializeWeather();
        
        gameEndTime = TimeManager.Instance.GameStartTime.AddDays(gameTotalDay);

        if (TimeManager.Instance.CurrentTime >= gameEndTime)
            return;
        
        isPlaying = true;
        
        TimeManager.Instance.OnHourChanged -= CheckGameEnd;
        TimeManager.Instance.OnHourChanged += CheckGameEnd;
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
        
        isPlaying = false;
    }

    private void CheckGameEnd(DateTime checkTime)
    {
        if (checkTime >= gameEndTime)
        {
            TimeManager.Instance.OnHourChanged -= CheckGameEnd;
            OnGameEnd();
        }
    }

    private void OnGameEnd()
    {
        QuitGame();
    }
    
    #endregion
}

