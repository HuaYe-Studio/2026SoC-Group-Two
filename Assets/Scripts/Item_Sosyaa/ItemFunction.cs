using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

// 此脚本处理物品使用逻辑（鼠标右键唤出操作选择框），应当挂载于物品图片上！
// 注：一个场景中应仅保留一个操作选择框

public class ItemFunction : MonoBehaviour , IPointerClickHandler
{
    private GameObject interactionUI; // 物品互动UI框 - 游戏开始时应默认先处于隐藏/未激活状态
    private ItemPivot itemPivot;

    // 鼠标双击时间判定参数
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    void Awake()
    {
        itemPivot = GetComponentInParent<ItemPivot>();
        interactionUI = GameObject.Find("InteractionUI");

        if (interactionUI == null)
        {
            InteractionUI[] interactionUIs = Resources.FindObjectsOfTypeAll<InteractionUI>();
            foreach (InteractionUI candidate in interactionUIs)
            {
                if (candidate.gameObject.scene.IsValid())
                {
                    interactionUI = candidate.gameObject;
                    break;
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnMouseRightClick();
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickThreshold)
            {
                OnDoubleClick();
                lastClickTime = 0f;
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }

    #region 鼠标双击事件 - 一般用于快速使用物品
    public void OnDoubleClick()
    {
        if (!itemPivot.itemData.isUsable) return;
        // 目前先默认物品都是一次性物品
        // 触发物品使用效果
        itemPivot.itemData.itemUsedEvent.Invoke();
        // 使用后移除物品
        DropItem();
    }
    #endregion

    #region 鼠标右键点击事件 - 召唤出物品互动UI框
    void OnMouseRightClick()
    {
        if (interactionUI == null)
        {
            return;
        }

        InteractionUI interaction = interactionUI.GetComponent<InteractionUI>();
        if (interaction == null) return;

        interactionUI.GetComponent<RectTransform>().position = Input.mousePosition;
        interaction.itemInteractingWith = gameObject;
        interactionUI.SetActive(true);
    }
    #endregion

    #region 移除物品
    public void DropItem()
    {
        if (itemPivot == null)
        {
            return;
        }

        if (itemPivot.containerOfItem != null)
        {
            itemPivot.containerOfItem.GetComponent<Container_ItemManager>()?.RemoveItem(itemPivot);

        }
        Destroy(itemPivot.gameObject);
    }
    #endregion
}
