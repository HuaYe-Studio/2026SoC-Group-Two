using System.Collections;
using System.Collections.Generic;
using Status;
using UnityEngine;
using UnityEngine.UI;

public class Hungry_UI : MonoBehaviour
{
    [Header("饥饿值UI图标")]
    public Image hungry_icon;

    private float currentHungryValue;
    private float maxHungryValue;

    void Start()
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.isPlaying)
            return;
        maxHungryValue = StatusManager.Instance.GetStatusModule(StatusType.Hungry).MaxValue;
    }

    void Update()
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.isPlaying)
            return;
        // 获取当前饥饿值
        currentHungryValue = StatusManager.Instance.GetStatusModule(StatusType.Hungry).CurrentValue;

        ChangeIconStyle(currentHungryValue);
    }

    #region 改变饥饿值图标样式
    void ChangeIconStyle(float currentHungryValue)
    {
        hungry_icon.fillAmount = currentHungryValue / maxHungryValue;
    }
    #endregion
}
