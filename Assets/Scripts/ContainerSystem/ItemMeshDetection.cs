using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ItemMeshDetection : MonoBehaviour
{
    [Header("父级物体")]
    public GameObject parentItem;
    private RectTransform parentRect;
    private ItemMesh anchorItemMesh;
    [Header("画布")]
    public Canvas canvas;
    [Header("检测范围")]
    public float detectDistance;
    [Header("容器网格检索")]
    public GameObject[] containerMeshes;
    [Header("物体网格列表")]
    public ItemMesh[] itemMeshes;
    [Header("物体放置的容器网格")]
    public GameObject[] usingContainerMeshes;
    [Header("物体网格旋转控制按钮")]
    public Key itemRotationKey;

    #region 私有成员
    GameObject[] targetMeshes;
    ContainerMesh containerMesh_S;
    GameObject pivotContainerMesh;
    List<GameObject> readyMeshes = new List<GameObject>();  // 记录准备放入的网格
    ContainerCreator containerCreator; // 容器网格父级
    bool isDragging = false;
    #endregion

    public Transform AnchorTransform => anchorItemMesh != null ? anchorItemMesh.transform : null;

    void Start()
    {
        if (parentItem == null)
        {
            parentItem = gameObject;
            Debug.LogWarning("ItemMeshDetection: 未指定 parentItem，已自动设置为当前 GameObject。", gameObject);
        }

        itemRotationKey = KeyManager.Instance.item_Rotation_key;

        // 获取所有容器网格
        containerMeshes = GameObject.FindGameObjectsWithTag("containermesh");
        if (containerMeshes.Length == 0)
        {
            containerMeshes = GameObject.FindGameObjectsWithTag("backpackmesh");
        }
        if (containerMeshes.Length == 0)
        {
            Debug.LogWarning("ItemMeshDetection: 未找到任何 containermesh 或 backpackmesh 标签的容器网格。", gameObject);
        }

        RefreshItemMeshData();

        parentRect = parentItem.GetComponent<RectTransform>();
        if (parentRect == null)
        {
            Debug.LogWarning("ItemMeshDetection: parentItem 未包含 RectTransform。", parentItem);
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("ItemMeshDetection: 未指定 Canvas，坐标检测可能不准确。", gameObject);
            }
        }
    }

    void RefreshItemMeshData()
    {
        itemMeshes = parentItem.GetComponentsInChildren<ItemMesh>(true);
        anchorItemMesh = itemMeshes.FirstOrDefault(mesh => mesh.itemMeshPos == Vector2.zero);
        if (anchorItemMesh == null && itemMeshes.Length > 0)
        {
            anchorItemMesh = itemMeshes[0];
            Debug.LogWarning("ItemMeshDetection: 未找到逻辑坐标为 (0,0) 的锚点网格，已使用第一个网格作为锚点。", gameObject);
        }
        if (anchorItemMesh == null)
        {
            Debug.LogError("ItemMeshDetection: 未找到任何 ItemMesh。", gameObject);
        }

        ItemPivot itemPivot = GetComponent<ItemPivot>();
        if (itemPivot != null)
        {
            itemPivot.itemMeshPositions.Clear();
            foreach (ItemMesh itemMesh in itemMeshes)
            {
                itemPivot.itemMeshPositions.Add(itemMesh.itemMeshPos);
            }
        }
    }

    void Update()
    {
        if (isDragging)
        {
            ItemMeshRotate();
            DetectContainerMesh();
        }
    }

    #region 拖拽状态管理
    public void OnBeginDrag()
    {
        isDragging = true;
        RefreshItemMeshData();
        ItemPivot itemPivot = GetComponentInParent<ItemPivot>();
        if (itemPivot != null && itemPivot.containerOfItem != null)
        {
            Container_ItemManager itemManager = itemPivot.containerOfItem.GetComponent<Container_ItemManager>();
            itemManager?.RemoveItem(itemPivot);
        }
        if (usingContainerMeshes != null)
        {
            foreach (GameObject containerMesh in usingContainerMeshes)
            {
                containerMesh.GetComponent<ContainerMesh>().isMeshUsed = false;
            }
            if (containerCreator != null) ChangeContainerMeshColor(containerCreator.originMeshColor, usingContainerMeshes);
            usingContainerMeshes = null;
        }
    }

    public void OnDrag()
    {
        DetectContainerMesh();
    }

    public bool OnEndDrag()
    {
        isDragging = false;
        bool placed = false;

        if (containerMesh_S != null && targetMeshes != null && pivotContainerMesh != null && containerCreator != null)
        {
            if (isSpaceEnough(containerMesh_S, containerCreator.containerMeshes.ToArray(), out List<GameObject> matchedMeshes))
            {
                PutInContainer(pivotContainerMesh, matchedMeshes.ToArray());
                placed = true;
            }
        }

        containerMesh_S = null;
        targetMeshes = null;
        pivotContainerMesh = null;

        if (readyMeshes.Count > 0 && containerCreator != null) ChangeContainerMeshColor(containerCreator.originMeshColor, readyMeshes.ToArray());
        readyMeshes.Clear();

        return placed;
    }
    #endregion

    #region 检测是否靠近容器网格
    public void DetectContainerMesh()
    {
        if (readyMeshes.Count > 0 && containerCreator != null)
        {
            ChangeContainerMeshColor(containerCreator.originMeshColor, readyMeshes.ToArray());
        }

        readyMeshes.Clear();
        targetMeshes = null;
        containerMesh_S = null;
        pivotContainerMesh = null;

        if (anchorItemMesh == null)
        {
            return;
        }

        foreach (GameObject containerMesh in containerMeshes)
        {
            bool within = IsWithinDetectDistance(anchorItemMesh.transform, containerMesh.transform);
            if (within)
            {
                ContainerMesh containerMeshScript = containerMesh.GetComponent<ContainerMesh>();

                if (!containerMeshScript.isMeshUsed)
                {
                    containerCreator = containerMesh.transform.parent.gameObject.GetComponent<ContainerCreator>();
                    GameObject[] thisPackMeshes = containerCreator.containerMeshes.ToArray();
                    if (isSpaceEnough(containerMeshScript, thisPackMeshes, out List<GameObject> matchedMeshes))
                    {
                        targetMeshes = matchedMeshes.ToArray();
                        readyMeshes = matchedMeshes;
                        containerMesh_S = containerMeshScript;
                        pivotContainerMesh = containerMesh;

                        ChangeContainerMeshColor(containerCreator.hightlightMeshColor, readyMeshes.ToArray());
                        break;
                    }
                }
            }
        }
    }

    public bool IsWithinDetectDistance(Transform anchor, Transform target)
    {
        float distance;
        if (canvas != null)
        {
            Vector2 anchorPos;
            Vector2 targetPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, anchor.position),
                canvas.worldCamera,
                out anchorPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, target.position),
                canvas.worldCamera,
                out targetPos);

            distance = Vector2.Distance(anchorPos, targetPos);
        }
        else
        {
            distance = Vector3.Distance(anchor.position, target.position);
        }

        return distance < detectDistance;
    }
    #endregion

    #region 检测是否能放下整个物体
    bool isSpaceEnough(ContainerMesh containerMeshScript, GameObject[] thisPackMeshes, out List<GameObject> matchedMeshes)
    {
        matchedMeshes = new List<GameObject>();
        Vector2 offset = containerMeshScript.meshPos - anchorItemMesh.itemMeshPos;

        foreach (ItemMesh itemMesh in itemMeshes)
        {
            bool find_Pos_FitMesh = false;
            foreach (GameObject packMesh in thisPackMeshes)
            {
                ContainerMesh packMeshScript = packMesh.GetComponent<ContainerMesh>();
                if (itemMesh.itemMeshPos + offset == packMeshScript.meshPos)
                {
                    find_Pos_FitMesh = true;
                    if (packMeshScript.isMeshUsed)
                    {
                        matchedMeshes.Clear();
                        return false;
                    }

                    matchedMeshes.Add(packMesh);
                    break;
                }
            }
            if (!find_Pos_FitMesh)
            {
                matchedMeshes.Clear();
                return false;
            }
        }

        return true;
    }
    #endregion

    #region 物体放入容器网格
    void PutInContainer(GameObject containerMesh, GameObject[] selectedMeshes)
    {
        parentItem.transform.position = containerMesh.transform.position;
        foreach (GameObject mesh in selectedMeshes)
        {
            mesh.GetComponent<ContainerMesh>().isMeshUsed = true;
        }

        usingContainerMeshes = selectedMeshes;

        ContainerMesh selectedContainerMesh = containerMesh.GetComponent<ContainerMesh>();
        Container_ItemManager itemManager = selectedContainerMesh != null && selectedContainerMesh.containerCreator != null
            ? selectedContainerMesh.containerCreator.GetComponent<Container_ItemManager>()
            : null;
        ItemPivot itemPivot = GetComponentInParent<ItemPivot>();
        if (itemManager == null || itemPivot == null)
        {
            Debug.LogWarning("PutInContainer: 无法找到容器物品管理器或 ItemPivot，已取消登记。", gameObject);
            return;
        }

        itemManager.AddItem(itemPivot, selectedContainerMesh.meshPos);

        Debug.Log($"PutInContainer: 放入容器，锚点由 {anchorItemMesh.name} 对齐到 {containerMesh.name}");
    }
    #endregion

    #region 改变容器网格样式
    void ChangeContainerMeshColor(Color targetColor, GameObject[] meshes)
    {
        foreach (GameObject containerMesh in meshes)
        {
            Image image = containerMesh.GetComponent<Image>();
            if (image == null)
            {
                continue;
            }
            image.color = targetColor;
        }
    }
    #endregion

    #region 物体网格旋转
    void ItemMeshRotate()
    {
        if (Keyboard.current?[itemRotationKey].wasPressedThisFrame ?? false)
        {
            if (itemMeshes == null || itemMeshes.Length == 0) return;

            RectTransform parentRectTransform = parentItem.GetComponent<RectTransform>();
            parentRectTransform.localEulerAngles += new Vector3(0f, 0f, -90f);

            foreach (ItemMesh itemMesh in itemMeshes)
            {
                Vector2 oldPos = itemMesh.itemMeshPos;
                itemMesh.itemMeshPos = new Vector2(oldPos.y, -oldPos.x);
            }

            RefreshItemMeshData();
        }
    }
    #endregion
}
