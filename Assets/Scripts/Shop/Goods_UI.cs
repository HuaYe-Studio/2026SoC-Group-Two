using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 此脚本挂载于商品货架槽按钮上
public class Goods_UI : MonoBehaviour
{
    [Header("存放的商品（物品）")]
    public GameObject goods;

    private Canvas containerCanvas; // 容器画布
    private GameObject pivot_1; // 定位用空物体
    private GameObject pivot_2; // 定位用空物体
    private CurrencySystem player_CurrencySystem; // 玩家货币系统

    void Awake()
    {
        containerCanvas = GameObject.Find("ContainerCanvas").GetComponent<Canvas>();
        pivot_1 = GameObject.Find("pivot_1");
        pivot_2 = GameObject.Find("pivot_2");
        player_CurrencySystem = GameObject.FindGameObjectWithTag("Player").GetComponent<CurrencySystem>();
    }

    #region 物品生成
    public void InstantiateItem()
    {
        // 如果钱不够，滚蛋
        if (player_CurrencySystem.GetCurrencyInTotal() < goods.GetComponent<ItemPivot>().itemData.itemPrice) return;

        Instantiate(goods , new Vector3(pivot_1.transform.position.x , Random.Range(pivot_1.transform.position.y , pivot_2.transform.position.y) , 0f) , goods.transform.rotation , containerCanvas.transform);
        // 货币扣钱结算
        player_CurrencySystem.CostCurrency(goods.GetComponent<ItemPivot>().itemData.itemPrice);
    }
    #endregion
}
