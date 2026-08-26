using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UI;

public class PauseView : MonoBehaviour
{
    [Header("��ͣ���")]
    public GameObject pausePanel;
    [Header("��ť")]
    public Button continueButton;
    public Button settingButton;
    public Button quitButton;
    [Header("�¼�")]
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
    //��ʾ��ͣ�˵�
    public void Show()
    {
        pausePanel.SetActive(true);
        UIManager.Instance.OpenUI(pausePanel.gameObject);
    }
    //������ͣ�˵�
    public void Hide()
    {
        pausePanel.SetActive(false);
        UIManager.Instance.CloseUI(pausePanel.gameObject);
    }

}
