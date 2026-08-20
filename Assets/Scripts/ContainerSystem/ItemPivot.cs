using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// 物体锚点：用于管理物体网格本地坐标数据
public class ItemPivot : MonoBehaviour
{
    [Header("物品属性")]
    public ItemData itemData;
    [Header("本地坐标系物体网格分布坐标列表")]
    public List<Vector2> itemMeshPositions = new List<Vector2>();
    [Header("物体图片")]
    public Image itemImage;
    [Header("物体所处容器")]
    public GameObject containerOfItem;
    [Header("物体锚点所在容器本地坐标")]
    public Vector2 pivotPositionInContainer;
}
