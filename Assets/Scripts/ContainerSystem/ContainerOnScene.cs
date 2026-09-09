using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


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
    [Tooltip("要添加的物品")]
    public GameObject itemPrefab;
    [Tooltip("物品锚点所在容器网格")]
    public GameObject containerMesh;
    [Header("容器画布")]
    public GameObject contaienrCanvas;

    [Header("随机生成物品列表")] 
    public List<GameObject> randomItemsList = new List<GameObject>();
    [Header("随机生成物品数量")]
    public int randomItemNumber = 0;

    private bool isContainerOpened = false;
    private bool enterTrigger = false;
    private readonly List<GameObject> randomlyGeneratedItems = new List<GameObject>();

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
        // 随机生成物品
        RandomlyCreateItems();

        foreach (GameObject item in itemInContainerOnScene)
        {
            item.SetActive(false);
        }
        RegisterSceneItems();
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
    [ContextMenu("自定义生成物品")] 
    public void CreateItemInEditor()
    {
    #if UNITY_EDITOR
        if (containerItemManager == null)
        {
            containerItemManager = containerPanel != null
            ? containerPanel.GetComponent<Container_ItemManager>()
            : null;
        }
    #endif

        if (itemPrefab == null || containerMesh == null || containerItemManager == null)
        {
            Debug.LogError("生成物品失败：请确认物品预制体、容器网格和 Container_ItemManager 均已绑定。", gameObject);
            return;
        }

        GameObject newItem = Instantiate(itemPrefab , containerMesh.transform.position ,
        itemPrefab.transform.rotation , GameObject.Find("ContainerCanvas").transform);

    #if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(newItem, "生成容器物品");
        Undo.RecordObject(this, "记录场景容器物品");
        Undo.RecordObject(containerItemManager, "记录容器物品列表");
    #endif
        
        newItem.GetComponent<ItemPivot>().pivotPositionInContainer = containerMesh.GetComponent<ContainerMesh>().meshPos;
        containerItemManager.AddItem(newItem.GetComponent<ItemPivot>() , containerMesh.GetComponent<ContainerMesh>().meshPos);
        itemInContainerOnScene.Add(newItem);

    #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(containerItemManager);
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
    #endif
    }
    #endregion

    private void RegisterSceneItems()
    {
        if (containerItemManager == null || itemInContainerOnScene == null)
        {
            return;
        }

        foreach (GameObject item in itemInContainerOnScene)
        {
            if (item == null)
            {
                continue;
            }

            ItemPivot itemPivot = item.GetComponent<ItemPivot>();
            if (itemPivot != null)
            {
                containerItemManager.AddItem(itemPivot, itemPivot.pivotPositionInContainer);
            }
        }
    }

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

    #region 随机生成物品
    void RandomlyCreateItems()
    {
        if (randomItemNumber <= 0 || randomItemsList == null || randomItemsList.Count == 0)
        {
            Debug.LogWarning("随机生成物品失败：请设置随机物品列表和生成数量。", gameObject);
            return;
        }

        if (containerItemManager == null && containerPanel != null)
        {
            containerItemManager = containerPanel.GetComponent<Container_ItemManager>();
        }

        ContainerCreator containerCreator = containerItemManager != null
            ? containerItemManager.GetComponent<ContainerCreator>()
            : null;
        if (containerItemManager == null || containerCreator == null || containerCreator.containerMeshes == null ||
            containerCreator.containerMeshes.Count == 0)
        {
            Debug.LogError("随机生成物品失败：未找到有效的容器物品管理器或容器网格。", gameObject);
            return;
        }

        RemoveRandomlyGeneratedItems();
        containerItemManager.LoadItemInContainer();

        Transform itemParent = contaienrCanvas != null
            ? contaienrCanvas.transform
            : containerPanel != null && containerPanel.transform.parent != null
                ? containerPanel.transform.parent
                : transform;
        int generatedCount = 0;

        for (int i = 0; i < randomItemNumber; i++)
        {
            List<GameObject> availablePrefabs = randomItemsList
                .Where(prefab => prefab != null)
                .OrderBy(_ => UnityEngine.Random.value)
                .ToList();
            bool generated = false;

            foreach (GameObject itemPrefabToCreate in availablePrefabs)
            {
                ItemPivot prefabPivot = itemPrefabToCreate.GetComponent<ItemPivot>();
                List<Vector2> itemMeshPositions = GetItemMeshPositions(itemPrefabToCreate, prefabPivot);
                if (itemMeshPositions.Count == 0)
                {
                    continue;
                }

                List<GameObject> candidateMeshes = containerCreator.containerMeshes
                    .Where(mesh => mesh != null)
                    .OrderBy(_ => UnityEngine.Random.value)
                    .ToList();
                foreach (GameObject candidateMesh in candidateMeshes)
                {
                    ContainerMesh mesh = candidateMesh.GetComponent<ContainerMesh>();
                    if (mesh == null || !CanPlaceItem(mesh.meshPos, itemMeshPositions, containerCreator.containerMeshes))
                    {
                        continue;
                    }

                    GameObject newItem = Instantiate(itemPrefabToCreate, candidateMesh.transform.position,
                        itemPrefabToCreate.transform.rotation, itemParent);
                    ItemPivot newItemPivot = newItem.GetComponent<ItemPivot>();
                    if (newItemPivot == null)
                    {
                        DestroyGeneratedObject(newItem);
                        continue;
                    }

                    newItemPivot.itemMeshPositions.Clear();
                    newItemPivot.itemMeshPositions.AddRange(itemMeshPositions);
                    containerItemManager.AddItem(newItemPivot, mesh.meshPos);
                    MarkItemMeshesUsed(mesh.meshPos, itemMeshPositions, containerCreator.containerMeshes);
                    newItem.SetActive(false);
                    itemInContainerOnScene.Add(newItem);
                    randomlyGeneratedItems.Add(newItem);
                    generatedCount++;
                    generated = true;
                    break;
                }

                if (generated)
                {
                    break;
                }
            }

            if (!generated)
            {
                Debug.LogWarning($"随机生成物品：第 {i + 1} 个物品没有找到可用空间。", gameObject);
                break;
            }
        }

        if (!isContainerOpened)
        {
            containerItemManager.HideItemInContainer();
        }

        Debug.Log($"随机生成物品完成：生成 {generatedCount}/{randomItemNumber} 个物品。", gameObject);
    }

    private List<Vector2> GetItemMeshPositions(GameObject itemObject, ItemPivot itemPivot)
    {
        if (itemPivot != null && itemPivot.itemMeshPositions != null && itemPivot.itemMeshPositions.Count > 0)
        {
            return new List<Vector2>(itemPivot.itemMeshPositions);
        }

        return itemObject.GetComponentsInChildren<ItemMesh>(true)
            .Select(itemMesh => itemMesh.itemMeshPos)
            .Distinct()
            .ToList();
    }

    private bool CanPlaceItem(Vector2 pivotPosition, List<Vector2> itemMeshPositions,
        List<GameObject> containerMeshes)
    {
        foreach (Vector2 itemMeshPosition in itemMeshPositions)
        {
            Vector2 targetPosition = pivotPosition + itemMeshPosition;
            GameObject targetMesh = containerMeshes.FirstOrDefault(mesh =>
                mesh != null && mesh.GetComponent<ContainerMesh>() != null &&
                mesh.GetComponent<ContainerMesh>().meshPos == targetPosition);
            if (targetMesh == null || targetMesh.GetComponent<ContainerMesh>().isMeshUsed)
            {
                return false;
            }
        }

        return true;
    }

    private void MarkItemMeshesUsed(Vector2 pivotPosition, List<Vector2> itemMeshPositions,
        List<GameObject> containerMeshes)
    {
        foreach (Vector2 itemMeshPosition in itemMeshPositions)
        {
            Vector2 targetPosition = pivotPosition + itemMeshPosition;
            GameObject targetMesh = containerMeshes.FirstOrDefault(mesh =>
                mesh != null && mesh.GetComponent<ContainerMesh>() != null &&
                mesh.GetComponent<ContainerMesh>().meshPos == targetPosition);
            if (targetMesh != null)
            {
                targetMesh.GetComponent<ContainerMesh>().isMeshUsed = true;
            }
        }
    }

    private void RemoveRandomlyGeneratedItems()
    {
        foreach (GameObject item in randomlyGeneratedItems)
        {
            if (item != null)
            {
                itemInContainerOnScene.Remove(item);
                containerItemManager?.RemoveItem(item.GetComponent<ItemPivot>());
                DestroyGeneratedObject(item);
            }
        }

        randomlyGeneratedItems.Clear();
    }

    private void DestroyGeneratedObject(GameObject item)
    {
        if (Application.isPlaying)
        {
            Destroy(item);
        }
        else
        {
            DestroyImmediate(item);
        }
    }
    #endregion
}

