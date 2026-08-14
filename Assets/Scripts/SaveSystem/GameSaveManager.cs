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

    // 存档列表
    public List<GameSaveData> saveDatas = new List<GameSaveData>();

    #region 公共方法
    // 读取所有游戏存档
    public void LoadAllGameSave()
    {
        
    }
    // 保存游戏数据
    public void SaveGame(GameSaveData newSaveData)
    {
        string json = JsonUtility.ToJson(newSaveData , true);

        // 当Json文件不存在：
        if (!File.Exists(newSaveData.savePath))
        {
            Debug.LogError($"文件 {newSaveData.savePath} 不存在！");
        }

        // 更新保存已有存档：
        else
        {
            File.WriteAllText(newSaveData.savePath , json);
            Debug.Log($"存档文件 {newSaveData.savePath} 已保存");
        }
    }

    // 加载游戏数据
    public void LoadGame(GameSaveData saveData_toLoad)
    {
        string jsonString = File.ReadAllText(GetFilePath(saveData_toLoad));
        GameSaveData gameData = JsonUtility.FromJson<GameSaveData>(jsonString);
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
        int newID = saveDatas[saveDatas.Count - 1].saveID + 1;
        GameSaveData newSaveData = new GameSaveData(newID , Path.Combine(gameSaveFolder, $"{jsonFilePrefix}{newID}.json"));
        saveDatas.Add(newSaveData);
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

    #region 应用存档数据
    void ApplyGameSaveData(GameSaveData gameSaveData)
    {
        
    }
    #endregion
}
