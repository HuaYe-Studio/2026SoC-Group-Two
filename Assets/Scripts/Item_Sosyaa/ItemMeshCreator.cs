using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemMeshCreator : MonoBehaviour
{
    [Header("容器网格生成脚本")]
    [Tooltip("用于确保容器网格与物体网格之统一")]
    public ContainerCreator containerCreator;
    [Header("物体网格预制体")]
    [Tooltip("强烈建议同容器网格选择同一预制体")]
    public GameObject itemMeshPrefab;

    [Header("物体图片")]
    [Tooltip("建议在UI画布中调整好物体图片尺寸再进行物体网格生成!")]
    public Image itemImage;
    [Header("物体网格排布")]
    public int meshNumber_Hor = 0;
    public int meshNumber_Ver = 0;
    [Header("物体网格尺寸")]
    public float itemMeshWidth;
    public float itemMeshHeight;
    [Header("物体网格列表")]
    public List<GameObject> itemMeshes = new List<GameObject>();

    #region 自动生成物体网格
    [ContextMenu("自动生成物体网格")]
    public void CreateItemMesh_Auto()
    {
        ItemPivot itemPivot = GetComponent<ItemPivot>();

        // 获取物体图片尺寸
        float imageWidth = itemImage.GetComponent<RectTransform>().sizeDelta.x;
        float imageHeight = itemImage.GetComponent<RectTransform>().sizeDelta.y;

        // 强制统一使用容器网格尺寸来进行比对
        itemMeshPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(containerCreator.meshWidth, containerCreator.meshHeight);
        itemMeshHeight = itemMeshPrefab.GetComponent<RectTransform>().sizeDelta.y;
        itemMeshWidth = itemMeshPrefab.GetComponent<RectTransform>().sizeDelta.x;

        if (itemMeshHeight != 0 && itemMeshWidth != 0)
        {
            if (imageWidth / itemMeshWidth > (int)imageWidth / (int)itemMeshWidth)
            {
                meshNumber_Hor = (int)imageWidth / (int)itemMeshWidth + 1;
            }
            else
            {
                meshNumber_Hor = (int)imageWidth / (int)itemMeshWidth;
            }

            if (imageHeight / itemMeshHeight > (int)imageHeight / (int)itemMeshHeight)
            {
                meshNumber_Ver = (int)imageHeight / (int)itemMeshHeight + 1;
            }
            else
            {
                meshNumber_Ver = (int)imageHeight / (int)itemMeshHeight;
            }
        }

        for (int i = 0; i < meshNumber_Hor; i++)
        {
            for (int j = 0; j < meshNumber_Ver; j++)
            {
                GameObject newItemMesh = Instantiate(itemMeshPrefab, transform);
                RectTransform newMeshRect = newItemMesh.GetComponent<RectTransform>();
                newMeshRect.anchoredPosition = new Vector2(i * itemMeshWidth, -j * itemMeshHeight);
                newMeshRect.localScale = Vector3.one;

                ItemMesh itemMeshScript = newItemMesh.GetComponent<ItemMesh>() ?? newItemMesh.AddComponent<ItemMesh>();

                itemMeshScript.itemMeshPos = new Vector2(i, -j);

                itemMeshes.Add(newItemMesh);
                if (itemPivot != null)
                {
                    itemPivot.itemMeshPositions.Add(newItemMesh.GetComponent<ItemMesh>().itemMeshPos); // 添加网格本地坐标到列表中
                }
            }
        }

        Debug.Log($"成功创建物体网格：({meshNumber_Hor} x {meshNumber_Ver})");
    }
    #endregion

    #region 清除物体网格
    [ContextMenu("清除物体网格")]
    public void DestroyItemMesh()
    {
        Transform[] itemMeshesTransform = GetComponentsInChildren<Transform>();
        ItemPivot itemPivot = GetComponent<ItemPivot>();

        foreach (Transform child in itemMeshesTransform)
        {
            if (child != gameObject.transform && child != itemImage.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        itemMeshes.Clear();
        if (itemPivot != null)
        {
            itemPivot.itemMeshPositions.Clear(); // 清空坐标列表
        }

        Debug.Log("已清除所有物体网格！");
    }
    #endregion

    #region 隐藏物体网格
    [ContextMenu("隐藏物体网格")]
    public void HideItemMesh()
    {
        Transform[] itemMeshes = GetComponentsInChildren<Transform>();

        foreach (Transform child in itemMeshes)
        {
            if (child != gameObject.transform && child != itemImage.transform)
            {
                if (child.gameObject.GetComponent<CanvasGroup>() == null) child.gameObject.AddComponent<CanvasGroup>().alpha = 0f;
                child.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }
    }
    #endregion

    #region 显示物体网格
    [ContextMenu("显示物体网格")]
    public void ShowItemMesh()
    {
        Transform[] itemMeshes = GetComponentsInChildren<Transform>();

        foreach (Transform child in itemMeshes)
        {
            if (child != gameObject.transform && child != itemImage.transform)
            {
                if (child.gameObject.GetComponent<CanvasGroup>() == null) child.gameObject.AddComponent<CanvasGroup>().alpha = 1f;
                child.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
    #endregion


    #region 手动添加锚点网格
    [ContextMenu("手动添加锚点网格")]
    public void AddPivotMesh()
    {
        // 如果锚点网格已存在
        foreach (GameObject itemMesh in itemMeshes)
        {
            if (itemMesh.GetComponent<ItemMesh>().itemMeshPos == Vector2.zero)
            {
                Debug.LogWarning("无法添加锚点网格：已存在锚点网格!");
                return;
            }
        }

        GameObject newItemMesh = Instantiate(itemMeshPrefab, transform);
        RectTransform newMeshRect = newItemMesh.GetComponent<RectTransform>();
        newMeshRect.anchoredPosition = Vector2.zero;
        newMeshRect.localScale = Vector3.one;

        ItemMesh itemMeshScript = newItemMesh.GetComponent<ItemMesh>() ?? newItemMesh.AddComponent<ItemMesh>();

        itemMeshScript.itemMeshPos = new Vector2(0, 0);

        itemMeshes.Add(newItemMesh);

        ItemPivot itemPivot = GetComponent<ItemPivot>();
        if (itemPivot != null)
        {
            itemPivot.itemMeshPositions.Add(newItemMesh.GetComponent<ItemMesh>().itemMeshPos);
        }

        Debug.Log("成功添加锚点网格!");
    }
    #endregion
}
