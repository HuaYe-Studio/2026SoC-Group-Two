using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using WorldTime;

// <summary>
// 睡眠系统
// 1.靠近床可以选择睡眠，跳转到第二天早上6：00，体力条回满
// 2.晚上0：00到2：00，每晚30分钟恢复的体力减少10（0点睡回90，2点昏迷回50）
// 3.晚上2：00若玩家未上床睡觉会原地昏迷跳转到第二天
// </summary>

// 时间轴：0：00 -> 2：00 -------  2：00 -> 6:00 -------- 6：00 -> 23：59：59
//      不睡觉有体力恢复惩罚      必定处于睡觉/昏迷中         玩家自由支配时间

public class SleepSystem : MonoBehaviour
{
    [Header("睡眠交互按键")]
    public KeyCode sleep_KeyCode;
    [Header("玩家睡眠/昏迷监测")]
    public bool isSleeping = false;
    public bool isFainting = false;
    [Header("事件")]
    public UnityEvent sleepEvent; // 入睡事件
    public UnityEvent faintEvent; // 昏迷事件
    public UnityEvent getUpEvent; // 起床事件
    [Header("时间切换间隔")]
    [Tooltip("玩家入睡或昏迷后等待多少秒发动体力恢复、时间切换等事件")]
    public float transitionDuration = 0f;

    // 玩家体力恢复值
    private int playerStrength_recover = 0;

    #region 靠近床选择睡眠
    public void ListenKeyInput()
    {
        if (Input.GetKeyDown(sleep_KeyCode) && !isFainting)
        {
            isSleeping = !isSleeping;
            if (isSleeping) sleepEvent.Invoke();
        }
    }
    #endregion

    #region 玩家体力恢复值设置 - 通过睡觉
    public void SetRecoverStrength_BySleeping()
    {
        // 如果在0点前入睡，直接回满体力并到第二天早上六点：
        if (TimeManager.Instance.CurrentTime.Hour >= 6)
        {
            playerStrength_recover = 100;
        }

        // 如果在0点到2点之间才入睡，有体力恢复惩罚
        else if (TimeManager.Instance.CurrentTime.Hour >= 0 && TimeManager.Instance.CurrentTime.Hour <= 2)
        {
            TimeSpan pastTime = TimeManager.Instance.CurrentTime - new DateTime(0 , 0 , 0);
            int pastTime_len = (int)pastTime.TotalMinutes;
            playerStrength_recover = 90 - pastTime_len / 30 * 10;  // 晚上0：00到2：00，每晚30分钟恢复的体力减少10（0点睡回90，2点昏迷回50）
        }
    }
    #endregion

    #region 玩家昏迷 及 昏迷体力恢复值设置
    public void PlayerFaint()
    {
        // 同玩家状态系统联动...
        isFainting = true;
    }

    public void SetRecoverStrength_ByFainting()
    {
        playerStrength_recover = 50; // 玩家直接昏迷则恢复50点体力
    }
    #endregion

    #region 玩家起床/从昏迷中苏醒
    public void PlayerGetUp()
    {
        // 恢复玩家体力条


        // 重置玩家睡觉/昏迷状态
        isSleeping = false;
        isFainting = false;
    }
    #endregion
}
