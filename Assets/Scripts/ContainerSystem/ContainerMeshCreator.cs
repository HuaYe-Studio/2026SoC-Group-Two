using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ContainerCreator : MonoBehaviour
{
    [Header("容器网格单元预制体")]
    public GameObject singleMeshPrefab;
    [Header("容器网格初始生成单元数")]
    [Tooltip("初始生成仅生成矩阵网格")]
    public int rollNumber;
    public int columnNumber;
    [Header("容器网格尺寸")]
    public float meshWidth = 100f;
    public float meshHeight = 100f;
    [Header("容器网格生成锚点（左上角）")]
    [Tooltip("此锚点确定网格生成原点之坐标并为生成的所有网格之父级")]
    public GameObject containerMeshPivotObj;

    [Header("网格列表")]
    public List<GameObject> containerMeshes = new List<GameObject>();

    [Header("容器网格样式")]
    public Color originMeshColor;
    public Color hightlightMeshColor;


    [ContextMenu("生成容器网格")]
    public void CreateContainerMesh()
    {
        // 根据预设值设置网格预制体尺寸
        singleMeshPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(meshWidth, meshHeight);

        // 生成网格（矩阵）
        for (int i = 0; i < rollNumber; i++)
        {
            for (int j = 0; j < columnNumber; j++)
            {
                GameObject newContainerMesh = Instantiate(singleMeshPrefab, containerMeshPivotObj.transform);
                RectTransform newMeshRect = newContainerMesh.GetComponent<RectTransform>();
                newMeshRect.anchoredPosition = new Vector2(j * meshWidth, -i * meshHeight);
                newMeshRect.localScale = Vector3.one;

                ContainerMesh containerMeshScript = newContainerMesh.GetComponent<ContainerMesh>() ?? newContainerMesh.AddComponent<ContainerMesh>();
                containerMeshScript.meshPos = new Vector2(j, -i);
                containerMeshScript.isMeshUsed = false;
                containerMeshScript.containerCreator = this;

                containerMeshes.Add(newContainerMesh);
            }
        }

        Debug.Log($"已生成初始容器网格（{rollNumber} x {columnNumber}）。");
    }

    
    [ContextMenu("删除容器网格(不删除锚点)")]
    public void DestroyAllContainerMesh()
    {
        if (containerMeshPivotObj.GetComponentsInChildren<RectTransform>() == null) return;

        Transform[] containerMesh = containerMeshPivotObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in containerMesh)
        {
            if (child != containerMeshPivotObj.transform) DestroyImmediate(child.gameObject);
        }

        containerMeshes.Clear();

        Debug.Log("容器网格已被删除！");
    }

    #region 拓展网格
    public void ExpandContainerMesh(Vector2 newMeshPos, Vector2 expandDirection, GameObject pivotMesh)
    {
        GameObject newContainerMesh = Instantiate(singleMeshPrefab, containerMeshPivotObj.transform);
        RectTransform newMeshRect = newContainerMesh.GetComponent<RectTransform>();
        RectTransform pivotRect = pivotMesh.GetComponent<RectTransform>();
        newMeshRect.anchoredPosition = pivotRect.anchoredPosition + new Vector2(expandDirection.x * meshWidth, expandDirection.y * meshHeight);
        newMeshRect.localScale = Vector3.one;

        ContainerMesh containerMeshScript = newContainerMesh.GetComponent<ContainerMesh>() ?? newContainerMesh.AddComponent<ContainerMesh>();

        containerMeshScript.meshPos = newMeshPos;
        containerMeshScript.isMeshUsed = false;
        containerMeshScript.containerCreator = this;

        containerMeshes.Add(newContainerMesh);

        Debug.Log($"成功拓展网格：({newMeshPos.x} , {newMeshPos.y})");
    }
    #endregion

    void Awake()
    {
        if (GetComponent<Container_ItemManager>() == null)
        {
            this.AddComponent<Container_ItemManager>();
        }
    }
}
