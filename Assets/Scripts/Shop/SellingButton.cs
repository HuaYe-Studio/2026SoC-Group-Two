using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

// 此脚本挂载到出售面板下方的“确认出售按钮”上，用于检测出售区物品并控制按钮样式及是否可被按下
public class SellingButton : MonoBehaviour
{
    [Header("出售区容器网格阵物品管理器")]
    public Container_ItemManager sell_ContainerItemManager;
    
    // 玩家货币系统
    private CurrencySystem player_CurrencySystem;
    // 按钮CanvasGroup组件
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // 获取玩家货币系统
        player_CurrencySystem = GameObject.FindGameObjectWithTag("Player").GetComponent<CurrencySystem>();
        if (player_CurrencySystem == null)
        {
            Debug.LogError("出售区容器无法获取玩家货币系统");
        }

        // 获取按钮CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!GotItemInSellZone())
        {
            canvasGroup.alpha = 0.5f;
            GetComponent<Button>().interactable = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            GetComponent<Button>().interactable = true;
        }
    }

    #region 检测出售区是否有物品
    public bool GotItemInSellZone()
    {
        return sell_ContainerItemManager.itemPivots.Count > 0;
    }
    #endregion

    #region 确认出售
    public void ConfirmSelling()
    {
        // 交易货币结算
        foreach (ItemPivot itemPivot in sell_ContainerItemManager.itemPivots)
        {
            player_CurrencySystem.AddCurrency(itemPivot.itemData.itemPrice);
            sell_ContainerItemManager.RemoveItem(itemPivot);
            Destroy(itemPivot.gameObject);
        }
    }
    #endregion
}
