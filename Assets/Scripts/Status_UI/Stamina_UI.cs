using System.Collections;
using System.Collections.Generic;
using Status;
using UnityEngine;
using UnityEngine.UI;

public class Stamina_UI : MonoBehaviour
{
    [Header("卫生状态图标")]
    public Image stamina_icon_good; // 卫生 - 妙
    public Image stamina_icon_worse; // 卫生 - 不妙 
    public Image stamina_icon_bad; // 卫生 - 我chovy

    private float currentStaminaValue;
    private float maxStaminaValue;
    private float minStaminaValue;

    void Start()
    {
        maxStaminaValue = StatusManager.Instance.GetStatusModule(StatusType.Stamina).MaxValue;
        minStaminaValue = StatusManager.Instance.GetStatusModule(StatusType.Stamina).MinValue;
    }

    void Update()
    {
        currentStaminaValue = StatusManager.Instance.GetStatusModule(StatusType.Stamina).CurrentValue;
        ChangeIconStyle(currentStaminaValue);
    }

    #region 改变体力图标样式
    void ChangeIconStyle(float currentStaminaValue)
    {
        if (currentStaminaValue > 2 / 3 * maxStaminaValue)
        {
            stamina_icon_good.gameObject.SetActive(true);
            stamina_icon_bad.gameObject.SetActive(false);
            stamina_icon_worse.gameObject.SetActive(false);
        }
        else if (currentStaminaValue >= 1 / 3 * maxStaminaValue && currentStaminaValue <= 2 / 3 * maxStaminaValue)
        {
            stamina_icon_good.gameObject.SetActive(false);
            stamina_icon_bad.gameObject.SetActive(false);
            stamina_icon_worse.gameObject.SetActive(true);
        }
        else if (currentStaminaValue < 1 / 3 * maxStaminaValue)
        {
            stamina_icon_good.gameObject.SetActive(false);
            stamina_icon_bad.gameObject.SetActive(true);
            stamina_icon_worse.gameObject.SetActive(false);
        }
    }
    #endregion
}
