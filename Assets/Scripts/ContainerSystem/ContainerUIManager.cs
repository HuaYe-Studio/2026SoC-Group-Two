using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UI;
using NPC;

// 此管理器用于管理不同容器的UI面板
public class ContainerUIManager : MonoBehaviour
{
    [Header("总容器面板画布")]
    public GameObject containerCanvas;
    [Header("背包容器面板")]
    public GameObject backpackContainerPanel;
    [Header("垃圾桶容器面板")]
    public GameObject[] trashContainerPanels;
    [Header("商店容器面板")]
    public GameObject shopContainerPanel;

    [Header("呼唤背包容器按键绑定")]
    public Key callBackpackContainerPanelKey;
    private bool isBackpackContainerOpened;

    void OnEnable()
    {
        NPC.DialogueEvents.OnRaised += OnDialogueEvent;
    }

    void OnDisable()
    {
        NPC.DialogueEvents.OnRaised -= OnDialogueEvent;
    }

    void Awake()
    {
        // 获取相关键位
        callBackpackContainerPanelKey = KeyManager.Instance.container_Call_key;
        isBackpackContainerOpened = false;

        // 获取垃圾桶面板
        trashContainerPanels = GameObject.FindGameObjectsWithTag("trashcanpanel");
    }

    void Start()
    {
        // 各个面板初始默认不激活
        backpackContainerPanel.SetActive(false);
        foreach (GameObject trashContainerPanel in trashContainerPanels)
        {
            trashContainerPanel.SetActive(false);
        }
        shopContainerPanel.SetActive(false);
    }

    void Update()
    {
        #region 背包容器呼唤逻辑
        if (DialogueManager.Instance?.IsDialogueActive ?? false) 
            return;
        isBackpackContainerOpened = backpackContainerPanel.activeInHierarchy;
        if (Keyboard.current?[callBackpackContainerPanelKey].wasPressedThisFrame ?? false)
        {
            isBackpackContainerOpened = !isBackpackContainerOpened;
            if (isBackpackContainerOpened)
            {
                UIManager.Instance.OpenUI(containerCanvas);
                backpackContainerPanel.SetActive(true);
            }
            else
            {
                backpackContainerPanel.SetActive(false);
            }

            if (isBackpackContainerOpened)
            {
                foreach (Container_ItemManager containerItemManager in backpackContainerPanel.GetComponentsInChildren<Container_ItemManager>())
                {
                    containerItemManager.LoadItemInContainer();
                }
            }
            else
            {
                foreach (Container_ItemManager containerItemManager in backpackContainerPanel.GetComponentsInChildren<Container_ItemManager>())
                {
                    containerItemManager.HideItemInContainer();
                }
            }
        }
        #endregion

        #region 容器面板控制逻辑
        if (!ExistOpeningContainer())
        {
            UIManager.Instance.CloseUI(containerCanvas);
        } 
        #endregion
    }

    #region 关闭背包按钮被点击事件
    public void OnCloseBackpackContainerPanelClick()
    {
        isBackpackContainerOpened = false;
        backpackContainerPanel.SetActive(false);
        foreach (Container_ItemManager containerItemManager in backpackContainerPanel.GetComponentsInChildren<Container_ItemManager>())
        {
            containerItemManager.HideItemInContainer();
        }
    }
    #endregion

    #region 对话事件：打开商店面板
    private void OnDialogueEvent(string id, object data)
    {
        if (id == "container.open")
        {
            UIManager.Instance.OpenUI(containerCanvas);
            shopContainerPanel.SetActive(true);
        }
    }
    #endregion

    #region 判断是否有打开的容器面板
    bool ExistOpeningContainer()
    {
        if (backpackContainerPanel.activeInHierarchy||
        shopContainerPanel.activeInHierarchy) return true;
        
        foreach (GameObject trashPanel in trashContainerPanels)
        {
            if (trashPanel.activeInHierarchy) return true;
        }

        return false;
    }
    #endregion
}
