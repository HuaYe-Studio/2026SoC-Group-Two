using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 此SO用于设置物品属性
[CreateAssetMenu(fileName = "NewItem" , menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public NamespaceID id; // 物品ID
    public string displayName;
    public Sprite image;

    //物品管理器需要用到这些属性
    public GameObject prefab;

}
