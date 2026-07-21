using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private Rigidbody rb;
    [Header("基本参数")]
    public float moveSpeed;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
        inputControl = new PlayerInputControl();
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        //翻转
        int faceDir = (int)transform.localScale.x;
        if(inputDirection.x>0)
            faceDir=1;
        if(inputDirection.x<0)
            faceDir=-1;
        transform.localScale = new Vector3(faceDir, 1, 1);
        //移动
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y);
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime*moveSpeed);
    }
}
