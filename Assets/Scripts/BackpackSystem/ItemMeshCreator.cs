using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemMeshCreator : MonoBehaviour
{
    [Header("背包网格生成脚本")]
    [Tooltip("用于确保背包网格与物体网格之统一")]
    public BackpackCreator backpackCreator;
    [Header("物体网格预制体")]
    [Tooltip("强烈建议同背包网格选择同一预制体")]
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
        // 获取物体图片尺寸
        float imageWidth = itemImage.GetComponent<RectTransform>().sizeDelta.x;
        float imageHeight = itemImage.GetComponent<RectTransform>().sizeDelta.y;

        // 强制统一使用背包网格尺寸来进行比对
        itemMeshPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(backpackCreator.meshWidth , backpackCreator.meshHeight);
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

        for (int i = 0 ; i < meshNumber_Hor ; i++)
        {
            for (int j = 0 ; j < meshNumber_Ver ; j++)
            {
                GameObject newItemMesh = Instantiate(itemMeshPrefab , transform.position + new Vector3 (i * itemMeshWidth , -j * itemMeshHeight , 0f) , 
                itemMeshPrefab.transform.rotation , transform);

                ItemMesh itemMeshScript = newItemMesh.GetComponent<ItemMesh>() ?? newItemMesh.AddComponent<ItemMesh>();

                itemMeshScript.itemMeshPos = new Vector2(i , -j);

                itemMeshes.Add(newItemMesh);
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

        foreach (Transform child in itemMeshesTransform)
        {
            if (child != gameObject.transform && child != itemImage.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        itemMeshes.Clear();

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

        GameObject newItemMesh = Instantiate(itemMeshPrefab , transform.position , 
        itemMeshPrefab.transform.rotation , transform);

        ItemMesh itemMeshScript = newItemMesh.GetComponent<ItemMesh>() ?? newItemMesh.AddComponent<ItemMesh>();

        itemMeshScript.itemMeshPos = new Vector2(0 , 0);

        itemMeshes.Add(newItemMesh);
        Debug.Log("成功添加锚点网格!");
    }
    #endregion
}
