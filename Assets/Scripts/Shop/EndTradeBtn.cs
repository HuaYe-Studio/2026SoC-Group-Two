using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 此脚本用于控制“结束交易”按钮的可交互性（在出售区还有物品时无法进行结束交易）
public class EndTradeBtn : MonoBehaviour
{
    private Container_ItemManager sellZoneContainerIManager;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        sellZoneContainerIManager = GameObject.Find("SellContainer").GetComponent<Container_ItemManager>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (sellZoneContainerIManager.itemPivots != null)
        {
            GetComponent<Button>().interactable = false;
            canvasGroup.alpha = 0.5f;
        }
        else
        {
            GetComponent<Button>().interactable = true;
            canvasGroup.alpha = 1f;
        }
    }
}
