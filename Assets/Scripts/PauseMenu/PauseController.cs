using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseController : MonoBehaviour
{
    public bool IsPaused { get; private set; } = false;
    public static PauseController Instance { get; private set; }
    
    public void SetPause(bool value)
    {
        IsPaused = value;
    }
    //ToDo: ��UIManager���úú�Դ˴������޸�
    [SerializeField] private GameObject settingPanel;
    private bool isSettingPanelOpen = false;
    public bool isPausePanelOpen = false;

    public PauseView view;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        //view�¼�����
        view.OnContinue.AddListener(
            ResumeGame
        );
        view.OnSetting.AddListener(
            OpenSetting
        );
        view.OnQuit.AddListener(
            QuitGame
        );
        //������ͣ����ʼ״̬Ϊ����
        view.Hide();
        //���ڹر���������¼��ļ���
        
    }

    private void Start()
    {
        settingPanel = FindAnyObjectByType<SettingUIPanels>(FindObjectsInactive.Include)?.gameObject; 
        settingPanel
            ?.GetComponent<SettingUIPanels>()
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
        isPausePanelOpen = true;
        //��ͣ��Ϸʱ��
        GameFlowManager.Instance.PauseGameplay("PauseMenu");
    }

    public void ResumeGame()
    {
        SetPause(false);
        view.Hide();
        isPausePanelOpen = false;
        //�ָ���Ϸʱ��
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
        //������ͣ�˵�����ʾ�������
        view.Hide();
        isSettingPanelOpen = false;
        settingPanel.GetComponent<SettingUIPanels>().OpenSettingPanel();
        isSettingPanelOpen = true;
    }

    private void ReturnPauseMenu()
    {
        //�������������ر��¼�����ʾ��ͣ�˵�
        if (IsPaused)
            view.Show();
        isPausePanelOpen = true;
        isSettingPanelOpen = false;
    }
}
