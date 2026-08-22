using System.Collections;
using System.Collections.Generic;
using Status;
using UnityEngine;

// 仅为测试用（初始化状态 & 启动游戏时间流动）
public class InitializeStatus_TestScript : MonoBehaviour
{
    public void StartGame()
    {
        GameFlowManager.Instance.OnGameStart();
    }
}
