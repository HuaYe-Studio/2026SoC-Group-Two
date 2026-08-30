using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 存储各种游戏数据类型（包括物品、时间、玩家状态、场景等）
[System.Serializable]
public class GameSaveData
{
    public bool isGameSaveActive; // 存档是否在启用中
    public int saveID; // 存档编号
    public string savePath; // 存档文件路径
    public bool isAutoSave; // 存档方式（是否为自动存档）
    public TimeData timeData;
    public List<ContainerData> containersData = new List<ContainerData>();
    public PlayerData playerData;

    // 构造函数
    public GameSaveData(int id = 0 , string path = "no path" , bool autoSave = false)
    {
        saveID = id;
        savePath = path;
        isAutoSave = autoSave;
    }
}

[System.Serializable]
public class TimeData
{
    public int game_Day; // 游戏内天数
    public int game_Hour; // 游戏内小时
    public int game_Minute; // 游戏内分钟

    // 更新时间数据
    public TimeData(int day , int hour , int min)
    {
        game_Day = day;
        game_Hour = hour;
        game_Minute = min;
    }
}

[System.Serializable]
public class ContainerData
{
    public string containerName; // 容器对象名，用于恢复时找到对应容器
    public List<ContainerItemData> itemPivotsInContainer = new List<ContainerItemData>();

    public ContainerData()
    {
    }

    public ContainerData(string containerNameToSave, List<ContainerItemData> itemPivotsToSave)
    {
        containerName = containerNameToSave;
        itemPivotsInContainer = itemPivotsToSave ?? new List<ContainerItemData>();
    }
}

[System.Serializable]
public class ContainerItemData
{
    public string itemName;
    public Vector2 pivotPositionInContainer;
    public List<Vector2> itemMeshPositions = new List<Vector2>();
    public bool isActive;

    public ContainerItemData()
    {
    }

    public ContainerItemData(ItemPivot itemPivot)
    {
        if (itemPivot == null)
        {
            return;
        }

        itemName = itemPivot.gameObject != null ? itemPivot.gameObject.name : string.Empty;
        pivotPositionInContainer = itemPivot.pivotPositionInContainer;
        itemMeshPositions = itemPivot.itemMeshPositions != null ? new List<Vector2>(itemPivot.itemMeshPositions) : new List<Vector2>();
        isActive = itemPivot.gameObject != null && itemPivot.gameObject.activeSelf;
    }
}

[System.Serializable]
public class PlayerData
{
    public float playerStatus_Health; // 玩家健康值
    public float playerStatus_Stamina; // 玩家体力值
    public float playerStatus_Hungry; // 玩家饱腹值
    public float playerStatus_Mental; // 玩家心态值
    public Vector3 playerPosition; // 玩家坐标
    public Vector3 playerRotation; // 玩家旋转角度

    public PlayerData()
    {
        playerPosition = Vector3.zero;
        playerRotation = Vector3.zero;
    }

    // 更新玩家数据
    public PlayerData(float health , float stamina , float hungry , float mental , Transform transform)
    {
        playerStatus_Health = health;
        playerStatus_Hungry = hungry;
        playerStatus_Mental = mental;
        playerStatus_Stamina = stamina;

        if (transform != null)
        {
            playerPosition = transform.position;
            playerRotation = transform.eulerAngles;
        }
        else
        {
            playerPosition = Vector3.zero;
            playerRotation = Vector3.zero;
        }
    }
}
