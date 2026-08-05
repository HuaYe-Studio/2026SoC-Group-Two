using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Linq;

public class ItemMeshDetection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("画布")]
    public Canvas canvas;
    [Header("父级物体")]
    public GameObject parentItem;
    private RectTransform parentRect;
    private ItemMeshCreator itemMeshCreator;
    [Header("物体图片")]
    public Image itemImage;
    [Header("检测范围")]
    public float detectDistance;
    [Header("背包网格检索")]
    public GameObject[] backpackMeshes;
    [Header("物体网格列表")]
    public ItemMesh[] itemMeshes;
    [Header("物体放置的背包网格")]
    public GameObject[] usingBackpackMeshes;
    [Header("物体网格旋转控制按钮")]
    public KeyCode rotate_KeyCode;

    #region 私有成员
    GameObject[] targetMeshes;
    BackpackMesh backpackMesh_S;
    GameObject pivotBackpackMesh;
    List<GameObject> readyMeshes = new List<GameObject>();  // 记录准备放入的网格
    BackpackCreator backpackCreator; // 背包网格父级
    bool isDragging = false;
    #endregion

    void Start()
    {
        // 获取所有背包网格
        backpackMeshes = GameObject.FindGameObjectsWithTag("backpackmesh");
        // 获取本身物体网格脚本
        itemMeshes = parentItem.GetComponentsInChildren<ItemMesh>();
        itemMeshCreator = parentItem.GetComponent<ItemMeshCreator>();

        parentRect = parentItem.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isDragging)
        {
            ItemMeshRotate();
        }
    }

    #region 检测是否靠近背包网格
    void DetectBackpackMesh()
    {
        // 恢复颜色
        if (readyMeshes.Count > 0) ChangeBackpackMeshColor(backpackCreator.originMeshColor , readyMeshes.ToArray());
        readyMeshes.Clear();
        targetMeshes = null;
        backpackMesh_S = null;
        pivotBackpackMesh = null;

        foreach (GameObject backpackMesh in backpackMeshes)
        {
            if (Vector3.Distance(parentItem.transform.position , backpackMesh.transform.position) < detectDistance)
            {
                BackpackMesh backpackMeshScript = backpackMesh.GetComponent<BackpackMesh>();

                if (!backpackMeshScript.isMeshUsed)
                {
                    // 获取此背包中的网格
                    backpackCreator = backpackMesh.transform.parent.gameObject.GetComponent<BackpackCreator>();
                    GameObject[] thisPackMeshes = backpackCreator.backpackMeshes.ToArray();
                    if (isSpaceEnough(backpackMeshScript , thisPackMeshes , out List<GameObject> matchedMeshes))
                    {
                        targetMeshes = matchedMeshes.ToArray();
                        readyMeshes = matchedMeshes;
                        backpackMesh_S = backpackMeshScript;
                        pivotBackpackMesh = backpackMesh;

                        ChangeBackpackMeshColor(backpackCreator.hightlightMeshColor , readyMeshes.ToArray());
                        break;
                    }
                }
            }
        }
    }
    #endregion

    #region 检测是否能放下整个物体
    bool isSpaceEnough(BackpackMesh backpackMeshScript , GameObject[] thisPackMeshes , out List<GameObject> matchedMeshes)
    {
        matchedMeshes = new List<GameObject>();
        Vector2 offset = new Vector2(backpackMeshScript.meshPos.x - Vector2.zero.x , backpackMeshScript.meshPos.y - Vector2.zero.y);

        foreach (ItemMesh itemMesh in itemMeshes)
        {
            bool find_Pos_FitMesh = false;
            foreach (GameObject packMesh in thisPackMeshes)
            {
                if (itemMesh.itemMeshPos + offset == packMesh.GetComponent<BackpackMesh>().meshPos)
                {
                    find_Pos_FitMesh = true;
                    if (packMesh.GetComponent<BackpackMesh>().isMeshUsed)
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

    #region 物体放入背包网格
    void PutInBackpack(GameObject backpackMesh , GameObject[] selectedMeshes)
    {
        parentItem.transform.position = backpackMesh.transform.position;
        foreach (GameObject mesh in selectedMeshes)
        {
            mesh.GetComponent<BackpackMesh>().isMeshUsed = true;
        }

        usingBackpackMeshes = selectedMeshes;
    }
    #endregion


    #region 物体拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (usingBackpackMeshes != null)
        {
            foreach (GameObject backpackMesh in usingBackpackMeshes)
            {
                backpackMesh.GetComponent<BackpackMesh>().isMeshUsed = false;
            }
            if (backpackCreator != null) ChangeBackpackMeshColor(backpackCreator.originMeshColor , usingBackpackMeshes);
            usingBackpackMeshes = null;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        // 将鼠标移动量转换为 Canvas 下的本地坐标移动
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect.parent as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 currentPos
        );
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect.parent as RectTransform,
            eventData.position - eventData.delta,
            canvas.worldCamera,
            out Vector2 lastPos
        );
        
        delta = currentPos - lastPos;
        parentRect.anchoredPosition += delta;

        DetectBackpackMesh();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (backpackMesh_S != null && targetMeshes != null && pivotBackpackMesh != null)
        {
            // backpackCreator = pivotBackpackMesh.transform.parent.GetComponent<BackpackCreator>();
            if (isSpaceEnough(backpackMesh_S , backpackCreator.backpackMeshes.ToArray() , out List<GameObject> matchedMeshes))
            {
                PutInBackpack(pivotBackpackMesh , matchedMeshes.ToArray());
            }
        }

        // 重置值：
        backpackMesh_S = null;
        targetMeshes = null;
        pivotBackpackMesh = null;

        // 恢复颜色
        if (readyMeshes.Count > 0) ChangeBackpackMeshColor(backpackCreator.originMeshColor , readyMeshes.ToArray());
        readyMeshes.Clear();
    }
    #endregion

    #region 改变背包网格样式
    void ChangeBackpackMeshColor(Color targetColor , GameObject[] meshes)
    {
        foreach (GameObject backpackMesh in meshes)
        {
            backpackMesh.GetComponent<Image>().color = targetColor;
        }
    }
    #endregion

    #region 物体网格旋转
    void ItemMeshRotate()
    {
        if (Input.GetKeyDown(rotate_KeyCode))
        {
            // 保持拖拽位置
            parentItem.GetComponent<RectTransform>().position = Input.mousePosition;

            if (itemMeshes == null || itemMeshes.Length == 0) return;

            // 直接旋转父对象
            RectTransform parentRectTransform = parentItem.GetComponent<RectTransform>();
            parentRectTransform.localEulerAngles += new Vector3(0f, 0f, -90f);

            // 重新计算每个网格的逻辑坐标
            foreach (ItemMesh itemMesh in itemMeshes)
            {
                Vector2 oldPos = itemMesh.itemMeshPos;
                itemMesh.itemMeshPos = new Vector2(oldPos.y, -oldPos.x); // 每按下一次 R ，顺时针转 90 度
            }
        }
    }
    #endregion
}
