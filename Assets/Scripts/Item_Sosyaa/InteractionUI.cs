using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [Header("当前正处于交互中的物品")]
    public GameObject itemInteractingWith;

    #region 使用物品按钮事件
    public void OnUseBtnClick()
    {
        ItemPivot itemPivot = itemInteractingWith != null ? itemInteractingWith.GetComponentInParent<ItemPivot>() : null;
        if (itemPivot == null || itemPivot.itemData == null || !itemPivot.itemData.isUsable) return;
        
        itemPivot.itemData.itemUsedEvent.Invoke();
        
        itemPivot.GetComponentInChildren<ItemFunction>().DropItem();
        gameObject.SetActive(false);
    }
    #endregion

    #region 丢弃物品按钮事件
    public void OnDropBtnClick()
    {
        itemInteractingWith?.GetComponentInParent<ItemFunction>()?.DropItem();
        gameObject.SetActive(false);
    }
    #endregion

    #region 取消操作按钮事件
    public void OnCancelBtnClick()
    {
        gameObject.SetActive(false);
    }
    #endregion

    // 默认不激活
    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.SetAsLastSibling();
    }
}
