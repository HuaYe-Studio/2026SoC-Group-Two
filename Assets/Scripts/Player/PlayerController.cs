using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private CharacterController controller;
    [Header("Basic Paraments")]
    public float moveSpeed;
    public float gravity=-9.8f;
    public Vector3 velocity;

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
        //移动
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;
        controller.Move(movement * (moveSpeed * Time.deltaTime));
        //重力
        if (controller.isGrounded&&velocity.y<0)
        {
            velocity.y = -0.2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
       
    }
}
