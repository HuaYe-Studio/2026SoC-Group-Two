using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using NPC;
using UI;
// 注：此脚本处于弃用状态，总的容器面板管理器是 ContainerUIManager.cs
public class ContainerKeyboradInput : MonoBehaviour
{
    [Header("容器面板画布")]
    public Canvas containerPanel;
    [Header("容器面板召唤/隐藏按键")]
    public Key container_Key;
    private bool isContainerOpened = false;

    void Start()
    {
        isContainerOpened = false;
        
        DialogueEvents.OnRaised -= OnDialogueEvent;
        DialogueEvents.OnRaised += OnDialogueEvent;
    }

    void Update()
    {
        Open_Close_Container();
    }

    #region 容器按键输入控制
    void Open_Close_Container()
    {
        if (Keyboard.current?[KeyManager.Instance.container_Call_key].wasPressedThisFrame ?? false)
        {
            isContainerOpened = !isContainerOpened;

            if (isContainerOpened)
            {
                containerPanel.gameObject.SetActive(true);
                UIManager.Instance.OpenUI(containerPanel.gameObject);
                GetComponent<Container_ItemManager>().LoadItemInContainer();
            }
            else
            {
                containerPanel.gameObject.SetActive(false);
                UIManager.Instance.CloseUI(containerPanel.gameObject); 
            }
        }
    }
    #endregion

    #region 关闭按钮事件
    public void onCloseBtnClick()
    {
        isContainerOpened = false;
        containerPanel.gameObject.SetActive(false);
        UIManager.Instance.CloseUI(containerPanel.gameObject); 
        GetComponent<Container_ItemManager>().HideItemInContainer();
    }
    #endregion

    #region 对话系统响应事件

    private void OnDialogueEvent(string id, object data)
    {
        if (id != "container.open") return;

        isContainerOpened = true;
        containerPanel.gameObject.SetActive(true);
        GetComponent<Container_ItemManager>().LoadItemInContainer();
    }

    #endregion
}
