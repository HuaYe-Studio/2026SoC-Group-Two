using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProfileSaveManager : MonoBehaviour
{
    private string profileSaveFolder => Path.Combine(Application.persistentDataPath, "profilesave");
    private string achievementsPath => Path.Combine(profileSaveFolder , "achievements.json");
    private ProfileSaveData profileSaveData;
    
    #region 单例实现
    private static ProfileSaveManager _instance;
    public static ProfileSaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ProfileSaveManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ProfileSaveManager");
                    _instance = go.AddComponent<ProfileSaveManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    void Awake()
    {
        LoadProfile();
    }

    #region 公共方法
    // 保存全局数据（建议在退出游戏时进行）
    public void SaveProfile()
    {
        // 保存解锁成就
        string json = JsonUtility.ToJson(profileSaveData.profile_achievementsUnlocked , true);
        File.WriteAllText(achievementsPath , json);
        Debug.Log($"已保存成就信息！保存位置{achievementsPath}");

        // 保存收藏物品
    }

    // 加载全局数据
    public void LoadProfile()
    {
        // 加载成就
        string jsonString = File.ReadAllText(achievementsPath);
        profileSaveData.profile_achievementsUnlocked = JsonUtility.FromJson<List<Achievement>>(jsonString);
    }
    #endregion
}
