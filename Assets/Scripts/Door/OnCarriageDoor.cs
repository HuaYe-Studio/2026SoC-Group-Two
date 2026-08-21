using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class OnCarriageDoor : MonoBehaviour
{

    public int carriageNum; //当前车厢编号

    private Transform playerTrans;
    [SerializeField] PlayerInputControl playerInputControl;
    private BoxCollider doorCollider;
    [SerializeField] bool isRightDoor;
    
    [SerializeField]private float teleportDisteance = 2f; //两车厢之间的距离+墙壁的厚度+一定的缓冲距离
    public bool canPress;

    [Header("事件")]
    public UnityEvent OnPlayerCloseDoor;//UI显示、对话等
    public UnityEvent<int> OnCarriageChange; //传递要去的车厢编号
    public UnityEvent OnPlayerAwayDoor; //UI显示、对话等

    private void Awake()
    {
        try
        {
            playerInputControl = InputManager.Instance.Input;
        }
        catch
        {
             Debug.Log("Find none InputManager.");
             playerInputControl = new PlayerInputControl();
        } 
    }

    private void Start()
    {
        playerTrans= GameObject.FindGameObjectWithTag("Player").transform;
        doorCollider = GetComponent<BoxCollider>();
    }

    #region 按E键触发传送
    private void OnEnable()
    {
        playerInputControl.Enable();
        playerInputControl.GamePlay.Interact.started += OnInteract;
    }

    private void OnDisable()
    {
        playerInputControl.Disable();
        playerInputControl.GamePlay.Interact.started -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (canPress)
        {
            var nextCarriageNum = isRightDoor ? carriageNum - 1 : carriageNum + 1;
            OnCarriageChange?.Invoke(nextCarriageNum);
            Teleport();
            canPress = false;
        }

    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerCloseDoor?.Invoke();
            canPress = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canPress = false;
        if (other.CompareTag("Player"))
        {
            OnPlayerAwayDoor?.Invoke();
        }
    }

   
    private void Teleport()
    {
        // 传送时先禁用玩家的GameObject，
        // 避免character controller的逻辑干扰传送位置的设置，
        // 然后再启用玩家的GameObject
        playerTrans.gameObject.SetActive(false);
        if (isRightDoor)
        {
            playerTrans.position += new Vector3(teleportDisteance, 0, 0);
        }
        else
        {
            playerTrans.position -= new Vector3(teleportDisteance, 0, 0);
        }
        playerTrans.gameObject.SetActive(true);
    }
}
