using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// 物体锚点：用于管理物体网格本地坐标数据
public class ItemPivot : MonoBehaviour
{
    [Header("本地坐标系物体网格分布坐标列表")]
    public List<Vector2> itemMeshPositions = new List<Vector2>();
    [Header("物体图片")]
    public Image itemImage;
}
