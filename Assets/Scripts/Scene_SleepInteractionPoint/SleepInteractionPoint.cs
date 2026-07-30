using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 核心检测逻辑：通过具有trigger属性的碰撞器来检测玩家是否进入交互范围内

public class SleepInteractionPoint : MonoBehaviour
{
    private GameObject player;
    [Header("玩家靠近床后事件")]
    public UnityEvent onPlayerCloseToBed; // 玩家靠近床的那一刹那（UI显示、对话等）
    public UnityEvent duringPlayerCloseToBed; // 玩家持续靠近床时（等待玩家按键输入等其他操作、其他场景事件等）
    public UnityEvent onPlayerGetAwayFromBed; // 玩家远离床的一瞬间（UI隐藏等）

    void Awake()
    {
        // 获取玩家对象
        player = GameObject.FindGameObjectWithTag("player");
    }

    #region 当玩家靠近可睡眠场景单位
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            onPlayerCloseToBed.Invoke();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            duringPlayerCloseToBed.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            onPlayerGetAwayFromBed.Invoke();
        }
    }
    #endregion
}
