// 存档Json文件命名：awotr_save_编号

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    private string jsonFilePrefix = "awotr_save_";
    private string gameSaveFolder => Path.Combine(Application.persistentDataPath, "gamesave"); 

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
    public void SaveGame(GameSaveData newSaveData)
    {
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
    public void CreateNewGameSave()
    {
        // 当存档列表不为空：
        if (saveDatas.Count >= 1)
        {
            int newID = saveDatas[saveDatas.Count - 1].saveID + 1;
            GameSaveData newSaveData = new GameSaveData(newID , Path.Combine(gameSaveFolder, $"{jsonFilePrefix}{newID}.json"));
            saveDatas.Add(newSaveData);
        }
        
        // 若没有存档（第一次进行游玩）：
        else
        {
            int newID = 0;
            GameSaveData newSaveData = new GameSaveData(newID , Path.Combine(gameSaveFolder, $"{jsonFilePrefix}{newID}.json"));
            saveDatas.Add(newSaveData);
        }
    }

    // 删除存档
    public void DeleteGameSave(GameSaveData saveData_ToDelete)
    {
        File.Delete(saveData_ToDelete.savePath);
        Debug.Log($"已删除存档文件 {saveData_ToDelete.savePath}");
        saveDatas.Remove(saveData_ToDelete);
        Debug.Log("游戏存档移除成功！");
    }
    #endregion
}
