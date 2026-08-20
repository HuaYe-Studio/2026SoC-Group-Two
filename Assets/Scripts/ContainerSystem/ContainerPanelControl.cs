using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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
        container_Key = KeyManager.Instance.container_Call_key;
    }

    void Update()
    {
        Open_Close_Container();
    }

    #region 容器按键输入控制
    void Open_Close_Container()
    {
        if (Keyboard.current?[container_Key].wasPressedThisFrame ?? false)
        {
            isContainerOpened = !isContainerOpened;

            if (isContainerOpened)
            {
                containerPanel.gameObject.SetActive(true);
                GetComponent<Container_ItemManager>().LoadItemInContainer();
            }
            else
            {
                containerPanel.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region 关闭按钮事件
    public void onCloseBtnClick()
    {
        isContainerOpened = false;
        containerPanel.gameObject.SetActive(false);
        GetComponent<Container_ItemManager>().HideItemInContainer();
    }
    #endregion
}
