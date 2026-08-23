using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotItem
{
    public Item itemData;
    public int anchorX;
    public int anchorY;

    public bool isRotated = false;
    public int CurrentWidth => isRotated ? itemData.boundHeight : itemData.boundWidth;
    public int CurrentHeight => isRotated ? itemData.boundWidth : itemData.boundHeight;

    public InventorySlotItem(Item itemData, int anchorX, int anchorY)
    {
        this.itemData = itemData;
        this.anchorX = anchorX;
        this.anchorY = anchorY;
        this.isRotated = false;
    }

    public InventorySlotItem(Item itemData, int anchorX, int anchorY, bool rotated)
    {
        this.itemData = itemData;
        this.anchorX = anchorX;
        this.anchorY = anchorY;
        this.isRotated = rotated;
    }

    public void ToggleRotate()
    {
        isRotated = !isRotated;
    }

    public bool ContainsGrid(int gridX, int gridY)
    {
        bool containsX = gridX >= anchorX && gridX < anchorX + CurrentWidth;
        bool containsY = gridY >= anchorY && gridY < anchorY + CurrentHeight;
        return containsX && containsY;
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
        bool ok = CanPlaceAt(anchorX, anchorY, item.boundWidth, item.boundHeight);
    if (!ok) return false;
        InventorySlotItem newSlot = new InventorySlotItem(item, anchorX, anchorY);
        itemList.Add(newSlot);
        return true;
        
    }
    public bool TryPlaceItem(InventorySlotItem slot, int anchorX, int anchorY)
{
    bool ok = CanPlaceAt(anchorX, anchorY, slot.CurrentWidth, slot.CurrentHeight);
    if (!ok) return false;

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
    for (int dx = 0; dx < occupyW; dx++)
    {
        for (int dy = 0; dy < occupyH; dy++)
        {
            int gx = anchorX + dx;
            int gy = anchorY + dy;
            if (gx < 0 || gx >= width || gy < 0 || gy >= height)
                return false;
            if (GetItemAtGrid(gx, gy) != null)
                return false;
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
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}