using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Status;

// 此脚本用于方便配置itemSO，相当于在另一个场景中的statusManager的搬运工，主要方便在编辑器中直接拖拽配置SO
public class PlayerStatus : MonoBehaviour
{
    private StatusManager statusManager;

    void Awake()
    {
        statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();
    }

    #region 公共方法
    public void ChangeStatus(StatusType statusType , float delta_value)
    {
        statusManager.ChangeStatusValue(statusType , delta_value);
    }
    #endregion
}
