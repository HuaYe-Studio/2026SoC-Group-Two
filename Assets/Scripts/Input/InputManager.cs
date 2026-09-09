using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // InputSystem的单例管理器，方便其他脚本获取输入信息
    // 其他脚本可以通过InputManager.Instance.Input来获取输入信息
    public static InputManager Instance;

    private PlayerInputControl input;

    public PlayerInputControl Input => input;


    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        input = new PlayerInputControl();


        input.GamePlay.Enable();

    }


    private void OnDestroy()
    {
        input.GamePlay.Disable();
    }
}
