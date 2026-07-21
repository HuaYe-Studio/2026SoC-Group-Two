using UnityEngine;
using System;
using WorldTime;
public class GameFlowManager : MonoBehaviour
{
    private DateTime gameEndTime;
    private int gameTotalDay = 7;

    private void Start()
    {
        if (TimeManager.Instance == null)
        {
            this.enabled = false;
            return;
        }
        
        gameEndTime = TimeManager.Instance.GameStartTime.AddDays(gameTotalDay);

        if (TimeManager.Instance.CurrentTime >= gameEndTime)
            return;
        
        TimeManager.Instance.OnHourChanged += CheckGameEnd;
    }
    private void Update()
    {
        //待inputsystem替换，不确定是否为空格先用着
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
    }

    private void QuitGame()
    {
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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
}

