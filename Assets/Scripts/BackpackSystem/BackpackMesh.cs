using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackMesh : MonoBehaviour
{
    [Header("网格坐标")]
    [Tooltip("如果为锚点处网格，则坐标为(0,0)")]
    public Vector2 meshPos = new Vector2();
    [Header("网格占用情况")]
    public bool isMeshUsed = false;

    [Header("背包BackpackCreator脚本")]
    public BackpackCreator backpackCreator;


    #region 单个拓展网格
    [ContextMenu("向上拓展网格")]
    public void CreateMesh_Up()
    {
        Vector2 newMeshPos = meshPos + new Vector2(0 , 1);

        if (!isMeshExpandable(newMeshPos)) return;

        Vector2 expandDirection = new Vector2(0 , 1);
        backpackCreator.ExpandBackpackMesh(newMeshPos , expandDirection , gameObject);
    }

    [ContextMenu("向下拓展网格")]
    public void CreateMesh_Down()
    {
        Vector2 newMeshPos = meshPos + new Vector2(0 , -1);

        if (!isMeshExpandable(newMeshPos)) return;

        Vector2 expandDirection = new Vector2(0 , -1);
        backpackCreator.ExpandBackpackMesh(newMeshPos , expandDirection , gameObject);
    }

    [ContextMenu("向左拓展网格")]
    public void CreateMesh_Left()
    {
        Vector2 newMeshPos = meshPos + new Vector2(-1 , 0);

        if (!isMeshExpandable(newMeshPos)) return;

        Vector2 expandDirection = new Vector2(-1 , 0);
        backpackCreator.ExpandBackpackMesh(newMeshPos , expandDirection , gameObject);
    }

    [ContextMenu("向右拓展网格")]
    public void CreateMesh_Right()
    {
        Vector2 newMeshPos = meshPos + new Vector2(1 , 0);

        if (!isMeshExpandable(newMeshPos)) return;

        Vector2 expandDirection = new Vector2(1 , 0);
        backpackCreator.ExpandBackpackMesh(newMeshPos , expandDirection , gameObject);
    }
    #endregion

    #region 检测是否可以在对应方向上拓展网格
    private bool isMeshExpandable(Vector2 newMeshPos)
    {
        for (int i = 0 ; i < backpackCreator.backpackMeshes.Count ; i++)
        {
            if (backpackCreator.backpackMeshes[i].GetComponent<BackpackMesh>().meshPos == newMeshPos)
            {
                Debug.LogWarning("新网格位置已被占用！无法拓展网格！");
                return false;
            }
        }

        return true;
    }
    #endregion

    #region 网格样式切换
    public void SetBackpackMeshStyle()
    {
        
    }
    #endregion
}
