using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotItem
{
    public Item itemData;
    public int anchorX;
    public int anchorY;
    public InventorySlotItem(Item itemData, int anchorX, int anchorY)
    {
        this.itemData = itemData;
        this.anchorX = anchorX;
        this.anchorY = anchorY;
    }
    public bool ContainsGrid(int gridX, int gridY)
    {
        bool containsX = gridX >= anchorX && gridX < anchorX + itemData.boundWidth;
        bool containsY = gridY >= anchorY && gridY < anchorY + itemData.boundHeight;
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
        for(int dx=0;dx<item.boundWidth;dx++)
        {
            for(int dy=0;dy<item.boundHeight;dy++)
            {
                int gridX = anchorX + dx;
                int gridY = anchorY + dy;
                if(gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
                {
                    return false;
                }
                if(GetItemAtGrid(gridX, gridY) != null)
                {
                    return false;
                }
            }
        }
        InventorySlotItem newSlot = new InventorySlotItem(item, anchorX, anchorY);
        itemList.Add(newSlot);
        return true;
    }
    public bool RemoveItem(InventorySlotItem slot)
    {
        return itemList.Remove(slot);
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
