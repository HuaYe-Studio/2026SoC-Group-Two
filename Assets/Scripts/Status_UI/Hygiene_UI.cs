using System.Collections;
using System.Collections.Generic;
using Status;
using UnityEngine;
using UnityEngine.UI;

public class Hygiene_UI : MonoBehaviour
{
    [Header("卫生状态图标")]
    public Image hygiene_icon_good; // 卫生 - 妙
    public Image hygiene_icon_worse; // 卫生 - 不妙 
    public Image hygiene_icon_bad; // 卫生 - 我chovy

    private float currentHygieneValue;
    private float maxHygieneValue;
    private float minHygieneValue;

    void Start()
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.isPlaying)
            return;
        maxHygieneValue = StatusManager.Instance.GetStatusModule(StatusType.Hygiene).MaxValue;
        minHygieneValue = StatusManager.Instance.GetStatusModule(StatusType.Hygiene).MinValue;
    }

    void Update()
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.isPlaying)
            return;
        currentHygieneValue = StatusManager.Instance.GetStatusModule(StatusType.Hygiene).CurrentValue;
        ChangeIconStyle(currentHygieneValue);
    }

    #region 改变卫生图标样式
    void ChangeIconStyle(float currentHygieneValue)
    {
        if (currentHygieneValue > 2 / 3 * maxHygieneValue)
        {
            hygiene_icon_good.gameObject.SetActive(true);
            hygiene_icon_bad.gameObject.SetActive(false);
            hygiene_icon_worse.gameObject.SetActive(false);
        }
        else if (currentHygieneValue >= 1 / 3 * maxHygieneValue && currentHygieneValue <= 2 / 3 * maxHygieneValue)
        {
            hygiene_icon_good.gameObject.SetActive(false);
            hygiene_icon_bad.gameObject.SetActive(false);
            hygiene_icon_worse.gameObject.SetActive(true);
        }
        else if (currentHygieneValue < 1 / 3 * maxHygieneValue)
        {
            hygiene_icon_good.gameObject.SetActive(false);
            hygiene_icon_bad.gameObject.SetActive(true);
            hygiene_icon_worse.gameObject.SetActive(false);
        }
    }
    #endregion
}
