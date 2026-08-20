using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 存储各种游戏数据类型（包括物品、时间、玩家状态、场景等）
[System.Serializable]
public class GameSaveData
{
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
}

[System.Serializable]
public class ContainerData // 用于记录有哪些物品在容器中以及物品在容器中的位置，建议物品管理器可以根据存档数据在容器内动态生成物品
{
    public GameObject container; // 容器网格阵父级锚点（根据容器系统，此锚点正常来说为空物体）
    // 这里准备创建一个List用于存储物品数据（物品名字、图标、所处网格逻辑坐标等）
}

[System.Serializable]
public class PlayerData
{
    public int playerStatus_Health; // 玩家健康值
    public int playerStatus_Stamina; // 玩家体力值
    public int playerStatus_Hungry; // 玩家饱腹值
    public int playerStatus_Hygiene; // 玩家卫生值（可能需要更改为“心态”）
}
