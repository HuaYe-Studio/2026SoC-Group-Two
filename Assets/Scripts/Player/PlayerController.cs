using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;
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
            Debug.LogError("Find none CharacterController");
            controller=gameObject.AddComponent<CharacterController>();
        }
        try
        {
            inputControl = InputManager.Instance.Input;
        }
        catch
        {
            Debug.Log("Find none InputManager.");
            inputControl = new PlayerInputControl();
        }

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
