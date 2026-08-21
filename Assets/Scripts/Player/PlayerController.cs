using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //private PlayerInputControl inputControl;
    //修改输入方案
    public Vector2 inputDirection;
    [SerializeField]private CharacterController controller;
    [Header("Basic Paraments")]
    public float moveSpeed;
    public float gravity=-9.8f;
    private Vector3 velocity;

    private void Awake()
    {
        
        if(controller==null)
        {
            Debug.Log("Find none CharacterController");
            controller=gameObject.AddComponent<CharacterController>();
        }
        
    }


    private void Update()
    {
        //inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
        GetKeyboardInput();
        Move();
    }

    private void GetKeyboardInput()
    {
        inputDirection = Vector2.zero;

        if (Keyboard.current[KeyManager.Instance.player_MoveUp_key].isPressed)
        {
            inputDirection.y += 1;
        }
        if (Keyboard.current[KeyManager.Instance.player_MoveDown_key].isPressed)
        {
            inputDirection.y -= 1;
        }
        if (Keyboard.current[KeyManager.Instance.player_MoveLeft_key].isPressed)
        {
            inputDirection.x -= 1;
        }
        if (Keyboard.current[KeyManager.Instance.player_MoveRight_key].isPressed)
        {
            inputDirection.x += 1;
        }
        inputDirection.Normalize();
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
