using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCarriageCamera : MonoBehaviour
{
    [SerializeField]
    Collider2D[] cameraBounding;
    public CinemachineConfiner currentConfiner;

    [SerializeField] int startCarriageNum = 7; 

    private void Start()
    {
        //ToDo:这里应该后续添加一个车厢的manager来全局管理车厢
        SetCameraConfiner(startCarriageNum);
    }



    //通过事件调用，切换当前车厢的摄像机限制区域
    public void SetCameraConfiner(int carriageNum)
    {
        if (carriageNum < 0 || carriageNum >= cameraBounding.Length)
        {
            Debug.LogError("Invalid carriage number: " + carriageNum);
            return;
        }
        
        currentConfiner.m_BoundingShape2D
            = cameraBounding[carriageNum];

    }

    

}
