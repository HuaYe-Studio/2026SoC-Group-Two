using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Status;
using UnityEngine;

public class Health_UI : MonoBehaviour
{
    [Header("健康UI图标")]
    public Image health_icon_normal;  // 根据玩家健康值进行颜色渐变（由鲜红变暗代表健康值下降）
    public Image health_icon_dead;  // 如果玩家死亡，直接切换为这个图标

    private float currentHealthValue;
    private float maxHealthValue;
    private float minHealthValue;

    void Start()
    {
        // 获取健康状态最值
        maxHealthValue = StatusManager.Instance.GetStatusModule(StatusType.Healthy).MaxValue;
        minHealthValue = StatusManager.Instance.GetStatusModule(StatusType.Healthy).MinValue;
    }

    void Update()
    {
        // 获取玩家健康状态
        currentHealthValue = StatusManager.Instance.GetStatusModule(StatusType.Healthy).CurrentValue;

        if (currentHealthValue > minHealthValue)
        {
            ChangeIconColor(currentHealthValue);
            health_icon_normal.gameObject.SetActive(true);
            health_icon_dead.gameObject.SetActive(false);
        }
        else
        {
            health_icon_normal.gameObject.SetActive(false);
            health_icon_dead.gameObject.SetActive(true);
        }
    }

    #region 设置图标颜色渐变
    void ChangeIconColor(float currentHealthValue)
    {
        health_icon_normal.color = new Color(currentHealthValue / maxHealthValue * 255f , 0f , 0f);
    }
    #endregion
}
