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
        if (itemInteractingWith.GetComponent<ItemPivot>().itemData.isUsable == false) return;
        itemInteractingWith.GetComponent<ItemPivot>().itemData.itemUsedEvent.Invoke();
        gameObject.SetActive(false);
    }
    #endregion

    #region 丢弃物品按钮事件
    public void OnDropBtnClick()
    {
        itemInteractingWith.GetComponent<ItemFunction>().DropItem();
        gameObject.SetActive(false);
    }
    #endregion

    #region 取消操作按钮事件
    public void OnCancelBtnClick()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
