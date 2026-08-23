using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UI;

public class PauseView : MonoBehaviour
{
    [Header("‘›Õ£√Ê∞Â")]
    public GameObject pausePanel;
    [Header("∞¥≈•")]
    public Button continueButton;
    public Button settingButton;
    public Button quitButton;
    [Header(" ¬º˛")]
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
    //œ‘ æ‘›Õ£≤Àµ•
    public void Show()
    {
        pausePanel.SetActive(true);
        UIManager.Instance.OpenUI(pausePanel.gameObject);
    }
    //“˛≤ÿ‘›Õ£≤Àµ•
    public void Hide()
    {
        pausePanel.SetActive(false);
        UIManager.Instance.CloseUI(pausePanel.gameObject);
    }

}
