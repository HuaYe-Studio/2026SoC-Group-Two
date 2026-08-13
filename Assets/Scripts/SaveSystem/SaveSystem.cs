using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    #region 单例实现
    private static SaveSystem _instance;
    public static SaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveSystem");
                    _instance = go.AddComponent<SaveSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    // 存档列表
    public Dictionary<SaveData , string> saveFiles = new Dictionary<SaveData, string>();

    #region Json文件读写
    // 保存游戏数据
    void SaveGame(string filePath , SaveData newSaveData)
    {
        string json = JsonUtility.ToJson(newSaveData , true);
        // 当Json文件不存在：
        if (!File.Exists(filePath))
        {
            Debug.Log($"文件 {filePath} 不存在，即将创建新文件...");
            File.WriteAllText(filePath , json);
            Debug.Log($"已成功创建文件 {filePath} 并写入");
        }

        // 更新保存已有存档：
        else
        {
            File.WriteAllText(filePath , json);
            Debug.Log($"存档文件 {filePath} 已保存");
        }
    }
    // 加载游戏数据
    void LoadGame(SaveData saveData_toLoad)
    {
        string jsonString = File.ReadAllText(GetFilePath(saveData_toLoad));
        SaveData gameData = JsonUtility.FromJson<SaveData>(jsonString);
    }
    #endregion

    #region 公共方法
    // 获取某个存档的文件路径
    public string GetFilePath(SaveData saveData)
    {
        if (saveFiles.ContainsKey(saveData))
        {
            return saveFiles[saveData];
        }
        else
        {
            Debug.LogError("无法获取存档文件路径！");
            return "No file path";
        }
    }
    // 新建存档
    public void CreateNewSave()
    {
        
    }
    // 覆盖存档
    public void OverwirteSave()
    {
        
    }
    // 删除存档
    public void DeleteSave()
    {
        
    }
    #endregion
}
