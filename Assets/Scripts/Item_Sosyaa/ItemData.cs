using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Status;

// 物品种类枚举
public enum ItemKind
{
    Food, // 食物
    Medic, // 医疗品
    Drink, // 饮料
    Junk, // 废品
}

// 此SO用于设置物品属性
[CreateAssetMenu(fileName = "NewItem" , menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public NamespaceID id; // 物品ID
    public string displayName;
    public Sprite image;
    public float itemPrice; // 物品价值
    public ItemKind itemKind; // 物品种类
    public bool isUsable; // 物品是否可以被使用
    // 这里暂时先不添加物品能否被出售的属性，默认都能出售（因为会涉及到容器系统的修改，在临近验收阶段尽量避免动我做的狗屎系统）
    public UnityEvent itemUsedEvent; // 物品使用后事件 - 这里仅考虑简单的事件，例如玩家属性变化
    public StatusType statusType; // 物品影响的属性
    public float changeAmount; // 属性修改值

    // 添加事件
    void ChangeStatus_Item()
    {
        StatusManager statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();
        statusManager.ChangeStatusValue(statusType, changeAmount);
    }

    void OnEnable()
    {
        itemUsedEvent.AddListener(ChangeStatus_Item);
    }
}
