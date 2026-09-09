using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 考虑到未来拓展需要，此货币系统可挂载于不同对象上（如玩家手机、其他NPC的手机、提款机等）
// 目前仅实现非常基础的功能
public class CurrencySystem : MonoBehaviour
{
    [Header("总货币持有数")]
    public float currency_total;

    #region 公共方法
    // 获取货币
    public void AddCurrency(float getValue)
    {
        currency_total += getValue;
    }
    // 支付货币
    public void CostCurrency(float costValue)
    {
        currency_total -= costValue;
        // 这里暂不添加判断总数是否小于0的逻辑，相关支付逻辑建议在交互过程中编写（如果买不起就无法支付等）
    }
    // 获取现有的货币总量
    public float GetCurrencyInTotal() => currency_total;
    #endregion
}
