using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using System;

[Serializable]
public struct CanvasProperty
{
    public GameObject UIobj; // UI组件
    public int sortOrder; // 画布图层层级
    public bool isActiveOnStart; // 是否在游戏开始时显示画布
}

// 此脚本用于控制不同的UI画布的显示和隐藏、层级
public class UICanvasInitializer : MonoBehaviour
{
    [Header("画布及其图层层级")]
    public List<CanvasProperty> canvasProperties = new List<CanvasProperty>();

    void Start()
    {
        InitializeCanvasProperty();
    }

    #region 初始化画布
    void InitializeCanvasProperty()
    {
        foreach (CanvasProperty canvasProperty in canvasProperties)
        {
            Canvas canvas = canvasProperty.UIobj.GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"无法找到 {canvasProperty.UIobj.name} 的 Canvas 组件！");
                continue;
            }
            canvas.sortingOrder = canvasProperty.sortOrder;
            canvasProperty.UIobj.SetActive(canvasProperty.isActiveOnStart);
        }
    }
    #endregion
}
