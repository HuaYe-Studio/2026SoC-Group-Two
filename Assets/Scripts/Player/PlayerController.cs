using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private CharacterController controller;
    [Header("Basic Paraments")]
    public float moveSpeed;

    private void Awake()
    {
        controller=GetComponent<CharacterController>();
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
        Move();
    }

    private void Move()
    {
        //翻转
        int faceDir = (int)transform.localScale.x;
        if(inputDirection.x>0.1)
            faceDir=1;
        if(inputDirection.x<-0.1)
            faceDir=-1;
        transform.localScale = new Vector3(faceDir, 1, 1);
        //移动
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y);
        controller.Move(movement * (moveSpeed * Time.deltaTime));
    }
}
