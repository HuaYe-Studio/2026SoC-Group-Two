using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour
{

    public List<ItemData> itemDatas = new List<ItemData>();
    private Dictionary<string, GameObject> itemDict = new Dictionary<string, GameObject>();
    private Dictionary<Guid, GameObject> items = new Dictionary<Guid, GameObject>();
    private void Awake()
    {
        foreach(ItemData item in itemDatas)
        {
            itemDict.Add(item.id.Path, item.prefab);
        }
    }
    public void CreateObj(string id)
    {
        GameObject obj = Instantiate(itemDict[id]);
        ObjData objData= obj.GetComponent<ObjData>();
        objData.guid=Guid.NewGuid();
        items.Add(objData.guid, obj);

    }
    public void ChangeCanUse(Guid guid, bool b)
    {
        ObjData objData=items[guid].GetComponent<ObjData>();
        objData.canUse=b;
    }
    public void ChangeContainer(Guid guid, string container)
    {
        ObjData objData = items[guid].GetComponent<ObjData>();
        objData.container = container;
    }
    public void ChangeQuantity(Guid guid, int q)
    {
        ObjData objData = items[guid].GetComponent<ObjData>();
        objData.quantity=q;
    }
   
}
