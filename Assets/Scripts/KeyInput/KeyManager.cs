using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// 注：此管理器目前仅充当一个键位的临时存储站，事件的绑定可能需要其他系统脚本中更改
public class KeyManager : MonoBehaviour
{
    #region 单例实现
    private static KeyManager _instance;
    public static KeyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<KeyManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("KeyManager");
                    _instance = go.AddComponent<KeyManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    // 修改键位示例：KeyManager.Instance.player_MoveUp_key = newKey;
    // 注：键位的修改仅应该在游戏设置选项中进行
    #region 键位
    [Header("玩家上移")]
    public Key player_MoveUp_key = Key.W;
    [Header("玩家下移")]
    public Key player_MoveDown_key = Key.S;
    [Header("玩家左移")]
    public Key player_MoveLeft_key = Key.A;
    [Header("玩家右移")]
    public Key player_MoveRight_key = Key.D;
    [Header("玩家多功能交互键")]
    public Key player_Interact_key = Key.E;
    [Header("游戏暂停/继续")]
    public Key game_PauseOrContinue_key = Key.Escape;
    [Header("背包容器界面呼唤键")]
    public Key backpack_Call_key = Key.B;
    [Header("容器界面物体旋转")]
    public Key item_Rotation_key = Key.R;
    #endregion

    // 键位列表 - 主要用于冲突检测
    private List<Key> keyList;

    void Awake() 
    {
        keyList = new List<Key>
        {
            player_MoveUp_key, player_MoveDown_key, player_MoveLeft_key, player_MoveRight_key,
            player_Interact_key, game_PauseOrContinue_key, backpack_Call_key, item_Rotation_key
        };
    }

    #region 改键冲突检测 - 主要供负责设置选项的组员使用
    public bool CanKeyBeChanged(Key key_ToChange , Key newKey)
    {
        foreach (Key key in keyList)
        {
            if (newKey == key && key != key_ToChange) return false;
        }
        return true;
    }
    #endregion
}
