using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class PauseView : MonoBehaviour
{
    [Header("暂停面板")]
    public GameObject pausePanel;
    [Header("按钮")]
    public Button continueButton;
    public Button settingButton;
    public Button quitButton;
    [Header("事件")]
    public UnityEvent OnContinue;
    public UnityEvent OnSetting;
    public UnityEvent OnQuit;

    private void Awake()
    {

        continueButton.onClick.AddListener(
            () => OnContinue?.Invoke()
        );


        settingButton.onClick.AddListener(
            () => OnSetting?.Invoke()
        );


        quitButton.onClick.AddListener(
            () => OnQuit?.Invoke()
        );

    }
    //显示暂停菜单
    public void Show()
    {
        pausePanel.SetActive(true);
    }
    //隐藏暂停菜单
    public void Hide()
    {
        pausePanel.SetActive(false);
    }

}
