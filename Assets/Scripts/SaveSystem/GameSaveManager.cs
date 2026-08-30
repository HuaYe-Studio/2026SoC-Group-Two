// 存档Json文件命名：awotr_save_编号

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Status;
using UnityEngine;
using WorldTime;

public class GameSaveManager : MonoBehaviour
{
    private string jsonFilePrefix = "awotr_save_";
    private string gameSaveFolder => Path.Combine(Application.persistentDataPath, "gamesave"); 
    private GameSaveData currentGameSave; // 当前启用的存档


    #region 单例实现
    private static GameSaveManager _instance;
    public static GameSaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameSaveManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameSaveManager");
                    _instance = go.AddComponent<GameSaveManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    void Awake()
    {
        LoadAllGameSave();
    }

    // 存档列表
    public List<GameSaveData> saveDatas = new List<GameSaveData>();

    #region 载入所有game存档（考虑到开始菜单的要求）
    async void LoadAllGameSave()
    {
        // 无存档文件夹情况；
        if (!Directory.Exists(gameSaveFolder))
        {
            Debug.Log("gamesave 文件夹不存在，即将创建文件夹...");
            Directory.CreateDirectory(gameSaveFolder); // 创建gamesave文件夹
            if (Directory.Exists(gameSaveFolder))
            {
                Debug.Log($"成功创建文件夹：{gameSaveFolder}");
            }
            else
            {
                Debug.LogError("创建gamesave文件夹失败！");
            }
        }

        // 载入gamesave文件夹中的存档json文件：
        string[] gameSaveFiles = Directory.GetFiles(gameSaveFolder , "awotr_save_*.json");

        foreach (string filePath in gameSaveFiles)
        {
            string json = File.ReadAllText(filePath);
            GameSaveData gameSaveData = JsonUtility.FromJson<GameSaveData>(json);
            saveDatas.Add(gameSaveData);
            Debug.Log($"成功加载存档：{Path.GetFileName(filePath)}");
        }

        Debug.Log($"game存档加载完成，共加载{saveDatas.Count}个存档");
    }
    #endregion

    #region 公共方法
    // 保存游戏数据
    public async Task SaveGame(GameSaveData newSaveData)
    {
        if (newSaveData == null)
        {
            Debug.LogError("保存失败：新存档对象为空。");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (StatusManager.Instance != null)
        {
            newSaveData.playerData = new PlayerData(
                StatusManager.Instance.GetStatusModule(StatusType.Healthy).CurrentValue,
                StatusManager.Instance.GetStatusModule(StatusType.Stamina).CurrentValue,
                StatusManager.Instance.GetStatusModule(StatusType.Hungry).CurrentValue,
                StatusManager.Instance.GetStatusModule(StatusType.Mental).CurrentValue,
                player != null ? player.transform : null
            );
        }
        else if (player != null)
        {
            newSaveData.playerData = new PlayerData(0f, 0f, 0f, 0f, player.transform);
        }
        else
        {
            newSaveData.playerData = new PlayerData();
        }

        if (TimeManager.Instance != null)
        {
            newSaveData.timeData = new TimeData(
                TimeManager.Instance.CurrentTime.Day,
                TimeManager.Instance.CurrentTime.Hour,
                TimeManager.Instance.CurrentTime.Minute
            );
        }
        else
        {
            newSaveData.timeData = new TimeData(0, 0, 0);
        }

        newSaveData.containersData.Clear();
        GameObject[] containers = GameObject.FindGameObjectsWithTag("container");
        foreach (GameObject container in containers)
        {
            if (container == null)
            {
                continue;
            }

            Container_ItemManager itemManager = container.GetComponent<Container_ItemManager>();
            if (itemManager == null)
            {
                continue;
            }

            List<ContainerItemData> serializedItems = new List<ContainerItemData>();
            foreach (ItemPivot itemPivot in itemManager.itemPivots)
            {
                if (itemPivot == null)
                {
                    continue;
                }

                serializedItems.Add(new ContainerItemData(itemPivot));
            }

            newSaveData.containersData.Add(new ContainerData(container.name, serializedItems));
        }

        string json = JsonUtility.ToJson(newSaveData , true);

        File.WriteAllText(newSaveData.savePath , json);
        Debug.Log($"存档文件 {newSaveData.savePath} 已保存");
    }

    // 获取某个存档的文件路径
    public string GetFilePath(GameSaveData saveData)
    {
        if (saveDatas.Contains(saveData))
        {
            return saveData.savePath;
        }
        else
        {
            Debug.LogError("无法获取存档文件路径！");
            return "No file path";
        }
    }

    // 新建存档（游戏进度从0开始）
    public async void CreateNewGameSave()
    {
        // 当存档列表不为空：
        if (saveDatas.Count >= 1)
        {
            int newID = saveDatas[saveDatas.Count - 1].saveID + 1;
            GameSaveData newSaveData = new GameSaveData(newID , Path.Combine(gameSaveFolder, $"{jsonFilePrefix}{newID}.json"));
            saveDatas.Add(newSaveData);
            Debug.Log($"成功新建游戏存档：{newSaveData.savePath}");
        }
        
        // 若没有存档（第一次进行游玩）：
        else
        {
            int newID = 0;
            GameSaveData newSaveData = new GameSaveData(newID , Path.Combine(gameSaveFolder, $"{jsonFilePrefix}{newID}.json"));
            saveDatas.Add(newSaveData);
            Debug.Log($"成功新建游戏存档：{newSaveData.savePath}");
        }
    }

    // 删除存档
    public async void DeleteGameSave(GameSaveData saveData_ToDelete)
    {
        File.Delete(saveData_ToDelete.savePath);
        Debug.Log($"已删除存档文件 {saveData_ToDelete.savePath}");
        saveDatas.Remove(saveData_ToDelete);
        Debug.Log("游戏存档移除成功！");
    }
    #endregion

    // 获取当前启用的存档
    public GameSaveData CurrentGameSave() => currentGameSave;

    // 选择存档并开始游戏
    public void StartGameSave(GameSaveData selectedGameSave)
    {
        currentGameSave = selectedGameSave;
    }

    // 退出游戏（退出到主菜单）————关闭存档
    public async void CloseGameSave()
    {
        await SaveGame(currentGameSave);
        currentGameSave = null;
    }
}
