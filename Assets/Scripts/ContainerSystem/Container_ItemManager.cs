using System.Collections.Generic;
using UnityEngine;

public class Container_ItemManager : MonoBehaviour
{
    [Header("此网格阵中放置的物品")]
    public List<ItemPivot> itemPivots = new List<ItemPivot>();
    private ContainerCreator containerCreator;

    void Awake()
    {
        containerCreator = GetComponent<ContainerCreator>();
    }

    #region 加载网格阵中物品
    public void LoadItemInContainer()
    {
        if (containerCreator == null)
        {
            return;
        }

        foreach (GameObject containerMesh in containerCreator.containerMeshes)
        {
            ContainerMesh mesh = containerMesh.GetComponent<ContainerMesh>();
            if (mesh != null)
            {
                mesh.isMeshUsed = false;
            }
        }

        for (int i = itemPivots.Count - 1; i >= 0; i--)
        {
            ItemPivot itemPivot = itemPivots[i];
            if (itemPivot == null)
            {
                itemPivots.RemoveAt(i);
                continue;
            }

            foreach (GameObject containerMesh in containerCreator.containerMeshes)
            {
                ContainerMesh mesh = containerMesh.GetComponent<ContainerMesh>();
                if (mesh != null && itemPivot.itemMeshPositions.Contains(mesh.meshPos - itemPivot.pivotPositionInContainer))
                {
                    mesh.isMeshUsed = true;
                }

                if (mesh != null && mesh.meshPos == itemPivot.pivotPositionInContainer)
                {
                    itemPivot.transform.position = mesh.transform.position;
                }
            }

            itemPivot.containerOfItem = gameObject;
            itemPivot.gameObject.SetActive(true);
        }
    }
    #endregion

    public void AddItem(ItemPivot itemPivot, Vector2 pivotPosition)
    {
        if (itemPivot == null)
        {
            return;
        }

        if (itemPivot.containerOfItem != null && itemPivot.containerOfItem != gameObject)
        {
            Container_ItemManager oldManager = itemPivot.containerOfItem.GetComponent<Container_ItemManager>();
            oldManager?.RemoveItem(itemPivot);
        }

        itemPivot.containerOfItem = gameObject;
        itemPivot.pivotPositionInContainer = pivotPosition;
        if (!itemPivots.Contains(itemPivot))
        {
            itemPivots.Add(itemPivot);
        }
    }

    public void RemoveItem(ItemPivot itemPivot)
    {
        if (itemPivot == null)
        {
            return;
        }

        if (containerCreator != null)
        {
            foreach (GameObject containerMesh in containerCreator.containerMeshes)
            {
                ContainerMesh mesh = containerMesh.GetComponent<ContainerMesh>();
                if (mesh != null && itemPivot.itemMeshPositions.Contains(mesh.meshPos - itemPivot.pivotPositionInContainer))
                {
                    mesh.isMeshUsed = false;
                }
            }
        }

        itemPivots.Remove(itemPivot);
        if (itemPivot.containerOfItem == gameObject)
        {
            itemPivot.containerOfItem = null;
        }
    }

    public void ClearDisplayedItems()
    {
        foreach (ItemPivot itemPivot in itemPivots)
        {
            if (itemPivot != null)
            {
                itemPivot.gameObject.SetActive(false);
            }
        }

        itemPivots.Clear();
    }

    #region 隐藏网格阵中物品
    public void HideItemInContainer()
    {
        foreach (ItemPivot itemPivot in itemPivots)
        {
            itemPivot.gameObject.SetActive(false);
        }
    }
    #endregion
}
