using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 此物品管理器应在游戏场景中进行加载
public class ItemManager : MonoBehaviour
{
    #region 单例实现
    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ItemManager");
                    _instance = go.AddComponent<ItemManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("所有物品对象池")]
    public Queue<ItemPivot> itemsPool;

    void Awake()
    {
        // 获取场景中的所有物品 - 初始化物品对象池
        GameObject[] itemsInScene = GameObject.FindGameObjectsWithTag("item");
        foreach (GameObject item in itemsInScene)
        {
            itemsPool.Enqueue(item.GetComponent<ItemPivot>());
        }
    }

    #region 销毁物品
    public void DestoryItem(GameObject item_ToDestory)
    {
        item_ToDestory.SetActive(false);
        if (!itemsPool.Contains(item_ToDestory.GetComponent<ItemPivot>()))
        {
            itemsPool.Enqueue(item_ToDestory.GetComponent<ItemPivot>());
        }
    }
    #endregion

    #region 加载容器中物品
    public void LoadItemInConatiner(GameObject container , GameObject item , Vector2 itemPosInContainer)
    {
        if (!item.activeSelf)
        {
                
        }

        foreach (GameObject containerMesh in container.GetComponent<ContainerCreator>().containerMeshes)
        {
            if (containerMesh.GetComponent<ContainerMesh>().meshPos == itemPosInContainer && !containerMesh.GetComponent<ContainerMesh>().isMeshUsed)
            {
                item.transform.position = containerMesh.transform.position;
                item.SetActive(true);
            }
        }
    }
    #endregion
}
