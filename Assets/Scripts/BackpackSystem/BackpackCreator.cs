using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class BackpackCreator : MonoBehaviour
{
    [Header("背包网格单元预制体")]
    public GameObject singleMeshPrefab;
    [Header("背包网格初始生成单元数")]
    [Tooltip("初始生成仅生成矩阵网格")]
    public int rollNumber;
    public int columnNumber;
    [Header("背包网格尺寸")]
    public float meshWidth = 100f;
    public float meshHeight = 100f;
    [Header("背包网格生成锚点（左上角）")]
    [Tooltip("此锚点确定网格生成原点之坐标并为生成的所有网格之父级")]
    public GameObject backpackMeshPivotObj;

    [Header("网格列表")]
    public List<GameObject> backpackMeshes = new List<GameObject>();


    [ContextMenu("生成背包网格")]
    public void CreateBackpackMesh()
    {
        // 根据预设值设置网格预制体尺寸
        singleMeshPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(meshWidth , meshHeight);

        // 生成网格（矩阵）
        for (int i = 0 ; i < rollNumber ; i++)
        {
            for (int j = 0 ; j < columnNumber ; j++)
            {
                GameObject newBackpackMesh = Instantiate(singleMeshPrefab , backpackMeshPivotObj.transform.position + new Vector3(j * meshWidth , -(i * meshHeight) , 0f) , 
                singleMeshPrefab.transform.rotation , backpackMeshPivotObj.transform);

                BackpackMesh backpackMeshScript = newBackpackMesh.GetComponent<BackpackMesh>() ?? newBackpackMesh.AddComponent<BackpackMesh>();
                backpackMeshScript.meshPos.x = j;
                backpackMeshScript.meshPos.y = -i;
                backpackMeshScript.isMeshUsed = false;
                backpackMeshScript.backpackCreator = this;

                backpackMeshes.Add(newBackpackMesh);
            }
        }

        Debug.Log($"已生成初始背包网格（{rollNumber} x {columnNumber}）。");
    }

    
    [ContextMenu("删除背包网格(不删除锚点)")]
    public void DestroyAllBackpackMesh()
    {
        if (backpackMeshPivotObj.GetComponentsInChildren<RectTransform>() == null) return;

        Transform[] backpackMesh = backpackMeshPivotObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in backpackMesh)
        {
            if (child != backpackMeshPivotObj.transform) DestroyImmediate(child.gameObject);
        }

        backpackMeshes.Clear();

        Debug.Log("背包网格已被删除！");
    }

    #region 拓展网格
    public void ExpandBackpackMesh(Vector2 newMeshPos , Vector2 expandDirection , GameObject pivotMesh)
    {
        GameObject newBackpackMesh = Instantiate(singleMeshPrefab , pivotMesh.transform.position + new Vector3(expandDirection.x * meshWidth , expandDirection.y * meshHeight , 0f) , 
        singleMeshPrefab.transform.rotation , backpackMeshPivotObj.transform);

        BackpackMesh backpackMeshScript = newBackpackMesh.GetComponent<BackpackMesh>() ?? newBackpackMesh.AddComponent<BackpackMesh>();

        backpackMeshScript.meshPos = newMeshPos;
        backpackMeshScript.isMeshUsed = false;
        backpackMeshScript.backpackCreator = this;

        backpackMeshes.Add(newBackpackMesh);

        Debug.Log($"成功拓展网格：({newMeshPos.x} , {newMeshPos.y})");
    }
    #endregion
}
