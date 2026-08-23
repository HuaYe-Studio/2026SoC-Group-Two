using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseController : MonoBehaviour
{
    public bool IsPaused { get; private set; } = false;
    public void SetPause(bool value)
    {
        IsPaused = value;
    }
    //ToDo: 在UIManager设置好后对此处进行修改
    [SerializeField] private GameObject settingPanel;
    private bool isSettingPanelOpen = false;

    public PauseView view;


    private void Awake()
    {
        //view事件监听
        view.OnContinue.AddListener(
            ResumeGame
        );
        view.OnSetting.AddListener(
            OpenSetting
        );
        view.OnQuit.AddListener(
            QuitGame
        );
        //设置暂停面板初始状态为隐藏
        view.Hide();
        //对于关闭设置面板事件的监听
        settingPanel
        .GetComponent<SettingUIPanels>()
        .OnCloseButtonClicked
        .AddListener(ReturnPauseMenu);
    }


    private void Update()
    {
        if (Keyboard.current[KeyManager.Instance.game_PauseOrContinue_key].wasPressedThisFrame)
        {
            if (isSettingPanelOpen)
                return;
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

    }



    public void PauseGame()
    {
        SetPause(true);
        view.Show();
        //暂停游戏时间
        GameFlowManager.Instance.PauseGameplay("PauseMenu");
    }

    public void ResumeGame()
    {
        SetPause(false);
        view.Hide();
        //恢复游戏时间
        GameFlowManager.Instance.ResumeGameplay("PauseMenu");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OpenSetting()
    {
        //隐藏暂停菜单，显示设置面板
        view.Hide();
        settingPanel.SetActive(true);
        isSettingPanelOpen = true;
    }

    private void ReturnPauseMenu()
    {
        //监听到设置面板关闭事件后，显示暂停菜单
        if (IsPaused)
            view.Show();
        isSettingPanelOpen = false;
    }
}
