using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMeshImageDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDragging { get; private set; }
    private RectTransform parent_rectTransform;
    private Transform targetTransform;
    private Canvas this_Canvas;
    private ItemMeshDetection itemMeshDetection;
    public Vector3 lastPosition;

    void Start()
    {
        itemMeshDetection = GetComponentInParent<ItemMeshDetection>();
        if (itemMeshDetection == null) Debug.LogError("物体网格无法获取 ItemMeshDetection 组件！");

        if (itemMeshDetection != null && itemMeshDetection.parentItem != null)
        {
            targetTransform = itemMeshDetection.parentItem.transform;
            parent_rectTransform = itemMeshDetection.parentItem.GetComponent<RectTransform>();
        }

        if (parent_rectTransform == null)
        {
            parent_rectTransform = GetComponentInParent<RectTransform>();
        }
        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        this_Canvas = GetComponentInParent<Canvas>();
        if (this_Canvas == null) Debug.LogError("物品图片无法获取所在画布 Canvas 组件！");
    }

    #region 物体拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        itemMeshDetection?.OnBeginDrag();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (parent_rectTransform != null && this_Canvas != null)
        {
            Vector2 delta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent_rectTransform.parent as RectTransform,
                eventData.position,
                this_Canvas.worldCamera,
                out Vector2 currentPos
            );
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent_rectTransform.parent as RectTransform,
                eventData.position - eventData.delta,
                this_Canvas.worldCamera,
                out Vector2 lastPos
            );
            
            delta = currentPos - lastPos;
            parent_rectTransform.anchoredPosition += delta;
        }
        else if (targetTransform != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, cam.WorldToScreenPoint(targetTransform.position).z);
                Vector3 worldPos = cam.ScreenToWorldPoint(screenPoint);
                targetTransform.position = worldPos;
            }
        }

        itemMeshDetection?.OnDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        itemMeshDetection?.OnEndDrag();
    }
    #endregion
}
