using UnityEngine;
using System;
using WorldTime;
public class GameFlowManager : MonoBehaviour
{
    private DateTime gameEndTime;
    private int gameTotalDay = 7;

    private void Start()
    {
        if (TimeManager.instance == null)
        {
            this.enabled = false;
            return;
        }
        
        gameEndTime = TimeManager.instance.gameStartTime.AddDays(gameTotalDay);

        if (TimeManager.instance.currentTime >= gameEndTime)
            return;
        
        TimeManager.instance.OnHourChanged += CheckGameEnd;
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
        if (TimeManager.instance != null)
        {
            TimeManager.instance.OnHourChanged -= CheckGameEnd;
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
            TimeManager.instance.OnHourChanged -= CheckGameEnd;
            OnGameEnd();
        }
    }

    private void OnGameEnd()
    {
        QuitGame();
    }
}

