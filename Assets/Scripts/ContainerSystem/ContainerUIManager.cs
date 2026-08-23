using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UI;

// 此管理器用于管理不同容器的UI面板
public class ContainerUIManager : MonoBehaviour
{
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
        UIManager.Instance.CloseUI(backpackContainerPanel);
        foreach (GameObject trashContainerPanel in trashContainerPanels)
        {
            UIManager.Instance.CloseUI(trashContainerPanel);
        }
        UIManager.Instance.CloseUI(shopContainerPanel);
    }

    void Update()
    {
        isBackpackContainerOpened = backpackContainerPanel.activeSelf;
        if (Keyboard.current?[callBackpackContainerPanelKey].wasPressedThisFrame ?? false)
        {
            isBackpackContainerOpened = !isBackpackContainerOpened;
            if (isBackpackContainerOpened)
            {
                UIManager.Instance.OpenUI(backpackContainerPanel);
            }
            else
            {
                UIManager.Instance.CloseUI(backpackContainerPanel);
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
    }

    public void OnCloseBackpackContainerPanelClick()
    {
        isBackpackContainerOpened = false;
        UIManager.Instance.CloseUI(backpackContainerPanel);
        foreach (Container_ItemManager containerItemManager in backpackContainerPanel.GetComponentsInChildren<Container_ItemManager>())
        {
            containerItemManager.HideItemInContainer();
        }
    }

    private void OnDialogueEvent(string id, object data)
    {
        if (id == "container.open")
        {
            UIManager.Instance.OpenUI(backpackContainerPanel);
        }
    }
}
