using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotItem
{
    public Item itemData;
    public int anchorX;
    public int anchorY;
    public bool isRotated =false;
    public int currentWidth => isRotated ? itemData.boundHeight : itemData.boundWidth;
    public int currentHeight => isRotated ? itemData.boundWidth : itemData.boundHeight;
    public InventorySlotItem(Item itemData, int anchorX, int anchorY)
    {
        this.itemData = itemData;
        this.anchorX = anchorX;
        this.anchorY = anchorY;
    }
    public bool ContainsGrid(int gridX, int gridY)
    {
        bool containsX = gridX >= anchorX && gridX < anchorX + currentWidth;
        bool containsY = gridY >= anchorY && gridY < anchorY + currentHeight;
        return containsX && containsY;
    }
    
    public InventorySlotItem(Item itemData, int anchorX, int anchorY, bool isRotated)
    {
        this.itemData = itemData;
        this.anchorX = anchorX;
        this.anchorY = anchorY;
        this.isRotated = isRotated;
    }
    public void ToggleRotate()
    {
        isRotated = !isRotated;
    }
}
public class Inventory
{
    public List<InventorySlotItem> itemList;
    public int width;
    public int height;
    public Inventory(int width, int height)
    {
        this.width = width;
        this.height = height;
        itemList = new List<InventorySlotItem>();
    }
    public InventorySlotItem GetItemAtGrid(int gridX, int gridY)
    {
        foreach(InventorySlotItem slot in itemList)
        {
            if(slot.ContainsGrid(gridX, gridY))
            {
                return slot;
            }
        }
        return null;
    }

    public bool TryPlaceItem(Item item, int anchorX, int anchorY)
    {
        bool can = CanPlaceAt(anchorX, anchorY, item.boundWidth, item.boundHeight);
        if(!can)
        {
            return false;
        }
        InventorySlotItem newSlot = new InventorySlotItem(item, anchorX, anchorY);
        itemList.Add(newSlot);
        return true;
    }

    public bool TryPlaceItem(InventorySlotItem slot, int anchorX, int anchorY)
    {
        bool can = CanPlaceAt(anchorX, anchorY, slot.currentWidth, slot.currentHeight);
        if(!can)
        {
            return false;
        }
        InventorySlotItem newSlot = new InventorySlotItem(slot.itemData, anchorX, anchorY, slot.isRotated);
        itemList.Add(newSlot);
        return true;
    }
    public bool RemoveItem(InventorySlotItem slot)
    {
        return itemList.Remove(slot);
    }
    public bool CanPlaceAt(int anchorX, int anchorY, int occupyW, int occupyH)
    {
        for(int dx=0;dx<occupyW;dx++)
        {
            for(int dy=0;dy<occupyH;dy++)
            {
                int gx = anchorX + dx;
                int gy = anchorY + dy;
                if(gx < 0 || gx >= width || gy < 0 || gy >= height)
                {
                    return false;
                }
                if(GetItemAtGrid(gx, gy) != null)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
public class Item
{
    public string itemName;
    public Sprite icon;
    public int boundWidth;
    public int boundHeight;
}

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
