using System.Collections;
using System.Collections.Generic;
using System;
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

    #region 启动自动读档键位

    private void Awake()
    {
        foreach (string actionName in AllActionNames)
        {
            string save = PlayerPrefs.GetString(PlayerPrefsKey(actionName));
            if (string.IsNullOrEmpty(save))
                continue;
            if (Enum.TryParse(save, out Key key))
                SetKey(actionName, key);
            else
                Debug.Log($"存档键位{save}无法解析，使用默认值");
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
    public Key container_Call_key = Key.B;
    [Header("容器界面物体旋转")]
    public Key item_Rotation_key = Key.R;
    
    //新增了仅仅储存名字的常量表
    public static readonly string[] AllActionNames =
    {
        "player_MoveUp_key",
        "player_MoveDown_key",
        "player_MoveLeft_key",
        "player_MoveRight_key",
        "player_Interact_key",
        "game_PauseOrContinue_key",
        "container_Call_key",
        "item_Rotation_key"
    };
    #endregion

    #region 修改键位与读写 

    public Key GetKey(string actionName)
    {
        switch (actionName)
        {
            case "player_MoveUp_key": return player_MoveUp_key;
            case "player_MoveDown_key": return player_MoveDown_key;
            case "player_MoveLeft_key": return player_MoveLeft_key;
            case "player_MoveRight_key": return player_MoveRight_key;
            case "player_Interact_key": return player_Interact_key;
            case "game_PauseOrContinue_key": return game_PauseOrContinue_key;
            case "container_Call_key": return container_Call_key;
            case "item_Rotation_key": return item_Rotation_key;
            default:
                Debug.Log($"未知的动作{actionName}");
                return Key.None;
        }
    }

    public bool SetKey(string actionName, Key newKey)
    {
        Key currentKey = GetKey(actionName);
        if (currentKey == Key.None) 
            return false;
        if (newKey == Key.None) 
            return false;
        if(!CanKeyBeChanged(currentKey,newKey)) 
            return false;

        switch (actionName)
        {
            case "player_MoveUp_key": player_MoveUp_key = newKey; break;
            case "player_MoveDown_key": player_MoveDown_key = newKey; break;
            case "player_MoveLeft_key": player_MoveLeft_key = newKey; break;
            case "player_MoveRight_key": player_MoveRight_key = newKey; break;
            case "player_Interact_key": player_Interact_key = newKey; break;
            case "game_PauseOrContinue_key": game_PauseOrContinue_key = newKey; break;
            case "container_Call_key": container_Call_key = newKey; break;
            case "item_Rotation_key": item_Rotation_key = newKey; break;
            default: return false;
        }
        
        PlayerPrefs.SetString(PlayerPrefsKey(actionName), newKey.ToString());
        PlayerPrefs.Save();
        Debug.Log($"{actionName}对应键已经改为{newKey}");
        return true;
    }
    
    public void ResetAllKeys()                                                                                                                                                   
      {                                                                                                                                                                            
          player_MoveUp_key = Key.W;                                                                                                                                               
          player_MoveDown_key = Key.S;                                                                                                                                             
          player_MoveLeft_key = Key.A;                                                                                                                                             
          player_MoveRight_key = Key.D;                                                                                                                                            
          player_Interact_key = Key.E;                                                                                                                                             
          game_PauseOrContinue_key = Key.Escape;                                                                                                                                   
          container_Call_key = Key.B;                                                                                                                                              
          item_Rotation_key = Key.R;                                                                                                                                               
                                                                                                                                                                                   
          foreach (string action in AllActionNames)                                                                                                                                
          {                                                                                                                                                                        
              PlayerPrefs.DeleteKey(PlayerPrefsKey(action));                                                                                                                       
          }                                                                                                                                                                        
          PlayerPrefs.Save();                                                                                                                                                      
                                                                                                                                                                                   
          Debug.Log("所有键位已恢复默认");                                                                                                                                         
      }
    
    private static string PlayerPrefsKey(string actionName) => "KeyRebind_" + actionName;

    #endregion
    #region 改键冲突检测 - 主要供负责设置选项的组员使用
    public bool CanKeyBeChanged(Key key_ToChange , Key newKey)
    {
        if (newKey == Key.None) return true;
        
        //因为有了一个常量表，所以直接读常量表即可
        foreach (string actionName in AllActionNames)
        {
            Key key = GetKey(actionName);
            if (newKey == key && key != key_ToChange)
            {
                Debug.Log($"按键 {newKey} 已被其他功能占用");
                return false;
            }
        }
        return true;
    }
    #endregion
}
