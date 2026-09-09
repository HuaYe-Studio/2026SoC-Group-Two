using System.Collections;
using System.Collections.Generic;
using Status;
using UnityEngine;
using UnityEngine.UI;

public class Mental_UI : MonoBehaviour
{
    [Header("卫生状态图标")]
    public Image mental_icon_good; // 卫生 - 妙
    public Image mental_icon_worse; // 卫生 - 不妙 
    public Image mental_icon_bad; // 卫生 - 我chovy

    private float currentMentalValue;
    private float maxMentalValue;
    private float minMentalValue;

    void Start()
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.isPlaying)
            return;
        maxMentalValue = StatusManager.Instance.GetStatusModule(StatusType.Mental).MaxValue;
        minMentalValue = StatusManager.Instance.GetStatusModule(StatusType.Mental).MinValue;
    }

    void Update()
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.isPlaying)
            return;
        
        if (maxMentalValue <= 0)                                                                                                                    
            maxMentalValue = StatusManager.Instance.GetStatusModule(StatusType.Mental).MaxValue;
        
        currentMentalValue = StatusManager.Instance.GetStatusModule(StatusType.Mental).CurrentValue;
        ChangeIconStyle(currentMentalValue);
    }

    #region 改变卫生图标样式
    void ChangeIconStyle(float currentMentalValue)
    {
        if (currentMentalValue > 2f / 3f * maxMentalValue)
        {
            mental_icon_good.gameObject.SetActive(true);
            mental_icon_bad.gameObject.SetActive(false);
            mental_icon_worse.gameObject.SetActive(false);
        }
        else if (currentMentalValue >= 1f / 3f * maxMentalValue && currentMentalValue <= 2f / 3f * maxMentalValue)
        {
            mental_icon_good.gameObject.SetActive(false);
            mental_icon_bad.gameObject.SetActive(false);
            mental_icon_worse.gameObject.SetActive(true);
        }
        else if (currentMentalValue < 1f / 3f * maxMentalValue)
        {
            mental_icon_good.gameObject.SetActive(false);
            mental_icon_bad.gameObject.SetActive(true);
            mental_icon_worse.gameObject.SetActive(false);
        }
    }
    #endregion
}
