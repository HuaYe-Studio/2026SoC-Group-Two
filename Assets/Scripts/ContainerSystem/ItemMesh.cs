using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMesh : MonoBehaviour
{
    [Header("物体网格坐标")]
    public Vector2 itemMeshPos = new Vector2();
    [Header("物体网格生成器")]
    public ItemMeshCreator itemMeshCreator;
    [Header("添加网格预制体 ")]
    public GameObject itemMeshPrefab;

    #region 手动添加网格
    [ContextMenu("向上添加单个网格")]
    public void AddItemMesh_Up()
    {
        Vector2 direction = new Vector2(0 , 1);
        Vector2 newMeshPos = itemMeshPos + direction;
        AddSingleItemMesh(newMeshPos , direction);
    }
    [ContextMenu("向下添加单个网格")]
    public void AddItemMesh_Down()
    {
        Vector2 direction = new Vector2(0 , -1);
        Vector2 newMeshPos = itemMeshPos + direction;
        AddSingleItemMesh(newMeshPos , direction);
    }
    [ContextMenu("向左添加单个网格")]
    public void AddItemMesh_Left()
    {
        Vector2 direction = new Vector2(-1 , 0);
        Vector2 newMeshPos = itemMeshPos + direction;
        AddSingleItemMesh(newMeshPos , direction);
    }
    [ContextMenu("向右添加单个网格")]
    public void AddItemMesh_Right()
    {
        Vector2 direction = new Vector2(1 , 0);
        Vector2 newMeshPos = itemMeshPos + direction;
        AddSingleItemMesh(newMeshPos , direction);
    }

    private void AddSingleItemMesh(Vector2 newMeshPos , Vector2 direction)
    {
        itemMeshPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(itemMeshCreator.itemMeshWidth , itemMeshCreator.itemMeshHeight);

        // 检查是否能添加网格
        foreach (GameObject itemMesh in itemMeshCreator.itemMeshes)
        {
            if (itemMesh.GetComponent<ItemMesh>().itemMeshPos == newMeshPos)
            {
                Debug.LogWarning("无法添加物体网格：网格位置已被占用！");
                return;
            }
        }

        GameObject newItemMesh = Instantiate(itemMeshPrefab , transform.position + new Vector3 (itemMeshCreator.itemMeshWidth * direction.x ,
        direction.y * itemMeshCreator.itemMeshHeight , 0f) , itemMeshPrefab.transform.rotation , itemMeshCreator.gameObject.transform);

        ItemMesh itemMeshScript = newItemMesh.GetComponent<ItemMesh>() ?? newItemMesh.AddComponent<ItemMesh>();

        itemMeshScript.itemMeshPos = newMeshPos;

        itemMeshCreator.itemMeshes.Add(newItemMesh);
    }
    #endregion
}
