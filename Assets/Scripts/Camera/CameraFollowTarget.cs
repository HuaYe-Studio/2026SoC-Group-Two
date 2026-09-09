using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform player;

    private Vector3 fixedPosition;

    void Start()
    {
        //对摄像机的初始位置进行固定，避免摄像机在Z轴上发生变化
        fixedPosition = transform.position;
    }


    void LateUpdate()
    {
        transform.position = new Vector3( player.position.x,player.position.y, fixedPosition.z );
    }
}
