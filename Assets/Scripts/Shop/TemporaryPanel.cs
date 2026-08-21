using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemporaryPanel : MonoBehaviour
{
    private CurrencySystem player_CurrencySystem;

    void Awake()
    {
        player_CurrencySystem = GameObject.FindGameObjectWithTag("Player").GetComponent<CurrencySystem>();
    }

    #region 结束交易时清空并回退处于暂存区的物品
    public void GiveBackItem()
    {
        ItemPivot[] itemPivots = GameObject.FindGameObjectsWithTag("item").
        Where(item => item.activeInHierarchy)
        .Select(item => item.GetComponent<ItemPivot>())
        .Where(comp => comp != null)
        .ToArray();

        foreach (ItemPivot itemPivot in itemPivots)
        {
            if (itemPivot.containerOfItem != null)
            {
                player_CurrencySystem.AddCurrency(itemPivot.itemData.itemPrice);
                Destroy(itemPivot.gameObject);
            }
        }
    }
    #endregion
}
