using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UI;


// 此脚本用于 场景容器同玩家交互判定 缓存场景容器中的物品 并 负责在场景编辑中生成缓存的物品
public class ContainerOnScene : MonoBehaviour
{
    [Header("容器中的物品")]
    public List<GameObject> itemInContainerOnScene = new List<GameObject>();
    [Header("对应的容器网格面板")]
    public GameObject containerPanel;
    [Header("此场景容器专属物品管理器")]
    public Container_ItemManager containerItemManager;
    [Header("单个添加物品")]
    public GameObject itemPrefab;
    public GameObject containerMesh;
    [Header("容器画布")]
    public GameObject contaienrCanvas;

    private bool isContainerOpened = false;
    private bool enterTrigger = false;

    void Awake()
    {
        containerItemManager = containerPanel.GetComponent<Container_ItemManager>();
        if (containerItemManager == null)
        {
            Debug.LogError("未为场景容器绑定专属 Container_ItemManager。", gameObject);
        }
    }

    void Start()
    {
        foreach (GameObject item in itemInContainerOnScene)
        {
            item.SetActive(false);
        }
    }

    void Update()
    {
        if (!enterTrigger) return;

        // 容器激活控制
        if (Keyboard.current?[KeyManager.Instance.player_Interact_key].wasPressedThisFrame ?? false && enterTrigger)
        {
            isContainerOpened = !isContainerOpened;
            
            if (isContainerOpened)
            {
                if (containerItemManager == null || containerPanel == null)
                {
                    isContainerOpened = false;
                    return;
                }

                Debug.Log("容器开启");
                UIManager.Instance.OpenUI(contaienrCanvas);
                containerPanel.transform.parent.gameObject.SetActive(true);

                containerItemManager.LoadItemInContainer();
            }
            else
            {
                if (containerItemManager == null || containerPanel == null)
                {
                    isContainerOpened = true;
                    return;
                }

                Debug.Log("容器关闭");
                
                containerItemManager.HideItemInContainer();
                
                containerPanel.transform.parent.gameObject.SetActive(false);
            }
        }

        
    }

    #region 添加物品
    [ContextMenu("生成物品")]
    public void CreateItemInEditor()
    {
        GameObject newItem = Instantiate(itemPrefab , containerMesh.transform.position ,
        itemPrefab.transform.rotation , GameObject.Find("ContainerCanvas").transform);
        
        newItem.GetComponent<ItemPivot>().pivotPositionInContainer = containerMesh.GetComponent<ContainerMesh>().meshPos;
        newItem.GetComponent<ItemPivot>().containerOfItem = gameObject;
        containerItemManager.AddItem(newItem.GetComponent<ItemPivot>() , containerMesh.GetComponent<ContainerMesh>().meshPos);
        itemInContainerOnScene.Add(newItem);
    }
    #endregion

    #region 交互逻辑
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            enterTrigger = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            enterTrigger = false;
            if (isContainerOpened)
            {
                Debug.Log("容器关闭");
            
                if (containerItemManager != null)
                {
                    containerItemManager.HideItemInContainer();
                }
                
                containerPanel.transform.parent.gameObject.SetActive(false);
                isContainerOpened = false;
            }
        }
    }
    #endregion
}

