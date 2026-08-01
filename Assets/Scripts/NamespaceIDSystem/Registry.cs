using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Registry : MonoBehaviour
{
    public static Registry Instance {get; private set;}

    private Dictionary<NamespaceID , ItemData> items = new Dictionary<NamespaceID, ItemData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadItems();
    }

    #region 加载物品
    void LoadItems()
    {
        ItemData[] itemDatas = Resources.LoadAll<ItemData>("Items");
        foreach (ItemData item in itemDatas)
        {
            RegisterItem(item);
        }
    }
    #endregion

    #region 注册单个物品
    public void RegisterItem(ItemData item)
    {
        if (items.ContainsKey(item.id))
        {
            return; // 物品存在则跳过注册
        }

        items[item.id] = item;
    }
    #endregion

    #region 通过ID获取物品
    public ItemData GetItem(NamespaceID id)
    {
        if (items.TryGetValue(id , out ItemData item))
        {
            return item;
        }
        return null;
    }
    #endregion
}
