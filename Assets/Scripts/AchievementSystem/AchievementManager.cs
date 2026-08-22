using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  此成就管理器建议放于bootstrap场景中同其他管理器一同进行加载
public class AchievementManager : MonoBehaviour
{
    #region 单例实现
    private static AchievementManager _instance;
    public static AchievementManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AchievementManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AchievementManager");
                    _instance = go.AddComponent<AchievementManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("所有成就")]
    public List<Achievement> achievements = new List<Achievement>(); // 可以直接在Inspector中进行添加Achievement

    #region 从Profile加载成就状态
    #endregion
}
