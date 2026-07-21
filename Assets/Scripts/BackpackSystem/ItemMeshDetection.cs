using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemMeshDetection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("画布")]
    public Canvas canvas;
    [Header("父级物体")]
    public GameObject parentItem;
    private RectTransform parentRect;
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
    [Header("背包网格颜色切换")]
    public Color originalColor;
    public Color selectedColor;

    #region 私有成员
    GameObject[] targetMeshes;
    BackpackMesh backpackMesh_S;
    GameObject pivotBackpackMesh;
    List<GameObject> readyMeshes = new List<GameObject>();  // 记录准备放入的网格
    #endregion

    void Start()
    {
        // 获取所有背包网格
        backpackMeshes = GameObject.FindGameObjectsWithTag("backpackmesh");
        // 获取本身物体网格脚本
        itemMeshes = parentItem.GetComponentsInChildren<ItemMesh>();

        parentRect = parentItem.GetComponent<RectTransform>();
    }

    #region 检测是否靠近背包网格
    void DetectBackpackMesh()
    {
        // 恢复颜色
        if (readyMeshes.Count > 0) ChangeBackpackMeshColor(originalColor , readyMeshes.ToArray());
        readyMeshes.Clear();

        foreach (GameObject backpackMesh in backpackMeshes)
        {
            if (Vector3.Distance(parentItem.transform.position , backpackMesh.transform.position) < detectDistance)
            {
                BackpackMesh backpackMeshScript = backpackMesh.GetComponent<BackpackMesh>();

                if (!backpackMeshScript.isMeshUsed)
                {
                    // 获取此背包中的网格
                    GameObject[] thisPackMeshes = backpackMesh.transform.parent.gameObject.GetComponent<BackpackCreator>().backpackMeshes.ToArray();
                    if (isSpaceEnough(backpackMeshScript , thisPackMeshes))  // 此时readyMeshes被赋值
                    {
                        targetMeshes = thisPackMeshes;
                        backpackMesh_S = backpackMeshScript;
                        pivotBackpackMesh = backpackMesh;

                        ChangeBackpackMeshColor(selectedColor , readyMeshes.ToArray());
                    }
                }
                else
                {
                    // 恢复颜色
                    if (readyMeshes.Count > 0) ChangeBackpackMeshColor(originalColor , readyMeshes.ToArray());
                    readyMeshes.Clear();
                }
            }
        }
    }
    #endregion

    #region 检测是否能放下整个物体
    bool isSpaceEnough(BackpackMesh backpackMeshScript , GameObject[] thisPackMeshes)
    {
        // 将锚点物体网格坐标“平移”到范围内背包网格坐标 - (注意：锚点处物体网格坐标为（0，0）)
        Vector2 offset = new Vector2(backpackMeshScript.meshPos.x - Vector2.zero.x , backpackMeshScript.meshPos.y - Vector2.zero.y);

        foreach (ItemMesh itemMesh in itemMeshes)
        {
            bool find_Pos_FitMesh = false;
            foreach (GameObject packMesh in thisPackMeshes)
            {
                // 能找到对应位置网格
                if (itemMesh.itemMeshPos + offset == packMesh.GetComponent<BackpackMesh>().meshPos)
                {
                    find_Pos_FitMesh = true;
                    if (packMesh.GetComponent<BackpackMesh>().isMeshUsed) return false;

                    // 将网格添加入readyMeshes中
                    readyMeshes.Add(packMesh);
                }
            }
            // 找不到对应位置网格
            if (!find_Pos_FitMesh) return false;
        }

        return true;
    }
    #endregion

    #region 物体放入背包网格
    void PutInBackpack(GameObject backpackMesh , GameObject[] backpackMeshes)
    {
        parentItem.transform.position = backpackMesh.transform.position;
        foreach (GameObject mesh in backpackMeshes)
        {
            mesh.GetComponent<BackpackMesh>().isMeshUsed = true;
        }

        usingBackpackMeshes = backpackMeshes;
    }
    #endregion


    #region 物体拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (usingBackpackMeshes != null)
        {
            foreach (GameObject backpackMesh in usingBackpackMeshes)
            {
                backpackMesh.GetComponent<BackpackMesh>().isMeshUsed = false;
            }
            ChangeBackpackMeshColor(originalColor , usingBackpackMeshes);
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
        if (backpackMesh_S != null && targetMeshes != null && pivotBackpackMesh != null)
        {
            if (isSpaceEnough(backpackMesh_S , targetMeshes))
            {
                PutInBackpack(pivotBackpackMesh , targetMeshes);
            }
        }

        // 重置值：
        backpackMesh_S = null;
        targetMeshes = null;
        pivotBackpackMesh = null;

        // 恢复颜色
        if (readyMeshes.Count > 0) ChangeBackpackMeshColor(originalColor , readyMeshes.ToArray());
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
}
