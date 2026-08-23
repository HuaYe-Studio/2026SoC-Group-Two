using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scavenge
{
    public class BackpackSystemAdapter
    {
        public bool HasBackpackSystem => creator != null;
        public Inventory Data { get; private set; }

        private ContainerCreator creator;
        private Canvas backpackCanvas;
        private readonly Dictionary<InventorySlotItem, GameObject> slotToGo =
            new Dictionary<InventorySlotItem, GameObject>();

        public BackpackSystemAdapter()
        {
            creator = Object.FindObjectOfType<ContainerCreator>();
            if (creator == null)
            {
                Debug.LogWarning("[Scavenge] 没有找到 ContainerCreator");
                return;
            }
            backpackCanvas = creator.GetComponentInParent<Canvas>();
            Data = new Inventory(creator.columnNumber, creator.rollNumber);
        }

        public void RebuildFromScene()
        {
            if (creator == null) return;
            Data.itemList.Clear();
            slotToGo.Clear();

            foreach (PlacedEntry p in CollectPlacedItems())
            {
                InventorySlotItem slot = new InventorySlotItem(p.item, p.anchorX, p.anchorY);
                Data.itemList.Add(slot);
                slotToGo[slot] = p.go;
            }
        }

        public void ApplyDataToScene()
        {
            if (creator == null) return;

            List<PlacedEntry> placed = CollectPlacedItems();

            foreach (PlacedEntry p in placed)
            {
                if (!DataHas(p)) DestroyPlacedItem(p);
            }

            slotToGo.Clear();
            foreach (InventorySlotItem slot in Data.itemList)
            {
                GameObject existing = null;
                foreach (PlacedEntry p in placed)
                {
                    if (p.anchorX == slot.anchorX && p.anchorY == slot.anchorY &&
                        p.width == slot.CurrentWidth && p.height == slot.CurrentHeight)
                    {
                        existing = p.go;
                        break;
                    }
                }
                slotToGo[slot] = existing != null ? existing : CreateItemGO(slot);
            }
        }

        public bool RemoveRandomItem(out Item removed)
        {
            removed = null;
            if (creator == null || Data == null || Data.itemList.Count == 0) return false;

            int idx = Random.Range(0, Data.itemList.Count);
            InventorySlotItem slot = Data.itemList[idx];
            removed = slot.itemData;

            if (slotToGo.TryGetValue(slot, out GameObject go) && go != null)
            {
                DestroySlotGo(go);
            }
            else
            {
                foreach (PlacedEntry p in CollectPlacedItems())
                {
                    if (p.anchorX == slot.anchorX && p.anchorY == slot.anchorY &&
                        p.width == slot.CurrentWidth && p.height == slot.CurrentHeight)
                    {
                        DestroyPlacedItem(p);
                        break;
                    }
                }
            }

            Data.itemList.RemoveAt(idx);
            slotToGo.Remove(slot);
            return true;
        }

        private struct PlacedEntry
        {
            public GameObject go;
            public ItemPivot pivot;
            public ItemMeshDetection det;
            public Item item;
            public int anchorX, anchorY, width, height;
        }

        /// <summary>收集背包里已放置的物品：优先走 Container_ItemManager 的 ItemPivot 登记，回退扫描 ItemMeshDetection</summary>
        private List<PlacedEntry> CollectPlacedItems()
        {
            List<PlacedEntry> result = new List<PlacedEntry>();
            HashSet<GameObject> seen = new HashSet<GameObject>();

            var itemMgr = creator != null ? creator.GetComponent<Container_ItemManager>() : null;
            if (itemMgr != null)
            {
                foreach (ItemPivot pivot in itemMgr.itemPivots)
                {
                    if (pivot == null || pivot.itemMeshPositions == null || pivot.itemMeshPositions.Count == 0) continue;
                    PlacedEntry entry = ReadPivot(pivot);
                    if (entry.go == null) continue;
                    seen.Add(pivot.gameObject);
                    result.Add(entry);
                }
            }

            foreach (ItemMeshDetection det in Object.FindObjectsOfType<ItemMeshDetection>())
            {
                if (det == null) continue;
                GameObject go = det.parentItem != null ? det.parentItem : det.gameObject;
                if (go == null || seen.Contains(go)) continue;
                if (!BelongsToThisContainer(det)) continue;

                PlacedEntry entry = ReadDetection(det, go);
                if (entry.go == null) continue;
                seen.Add(go);
                result.Add(entry);
            }

            return result;
        }

        private PlacedEntry ReadPivot(ItemPivot pivot)
        {
            int w = 1, h = 1;
            foreach (Vector2 pos in pivot.itemMeshPositions)
            {
                w = Mathf.Max(w, Mathf.RoundToInt(pos.x) + 1);
                h = Mathf.Max(h, Mathf.RoundToInt(-pos.y) + 1);
            }

            Sprite icon = pivot.itemImage != null ? pivot.itemImage.sprite : null;
            Item item = new Item
            {
                itemName = pivot.gameObject.name,
                icon = icon,
                boundWidth = w,
                boundHeight = h
            };

            return new PlacedEntry
            {
                go = pivot.gameObject,
                pivot = pivot,
                item = item,
                anchorX = Mathf.RoundToInt(pivot.pivotPositionInContainer.x),
                anchorY = Mathf.RoundToInt(-pivot.pivotPositionInContainer.y),
                width = w,
                height = h
            };
        }

        private PlacedEntry ReadDetection(ItemMeshDetection det, GameObject go)
        {
            Vector3 rootPos = det.parentItem != null ? det.parentItem.transform.position : det.transform.position;

            ContainerMesh pivotMesh = null;
            float best = float.MaxValue;
            foreach (GameObject gm in creator.containerMeshes)
            {
                if (gm == null) continue;
                ContainerMesh cm = gm.GetComponent<ContainerMesh>();
                if (cm == null) continue;
                float d = Vector3.Distance(gm.transform.position, rootPos);
                if (d < best) { best = d; pivotMesh = cm; }
            }
            if (pivotMesh == null) return new PlacedEntry();

            int anchorX = Mathf.RoundToInt(pivotMesh.meshPos.x);
            int anchorY = Mathf.RoundToInt(-pivotMesh.meshPos.y);

            ItemMeshCreator ic = go.GetComponent<ItemMeshCreator>();
            Image img = ic != null ? ic.itemImage : null;

            int w, h;
            if (ic != null && ic.meshNumber_Hor > 0 && ic.meshNumber_Ver > 0)
            {
                w = ic.meshNumber_Hor;
                h = ic.meshNumber_Ver;
            }
            else
            {
                w = 1; h = 1;
                foreach (ItemMesh m in go.GetComponentsInChildren<ItemMesh>())
                {
                    w = Mathf.Max(w, Mathf.RoundToInt(m.itemMeshPos.x) + 1);
                    h = Mathf.Max(h, Mathf.RoundToInt(-m.itemMeshPos.y) + 1);
                }
            }

            float z = det.parentItem != null ? det.parentItem.transform.localEulerAngles.z : det.transform.localEulerAngles.z;
            bool rotated = Mathf.Abs(NormalizeEulerZ(z)) > 45f;

            Item item = new Item
            {
                itemName = go.name,
                icon = img != null ? img.sprite : null,
                boundWidth = w,
                boundHeight = h
            };

            return new PlacedEntry
            {
                go = go,
                det = det,
                item = item,
                anchorX = anchorX,
                anchorY = anchorY,
                width = rotated ? h : w,
                height = rotated ? w : h
            };
        }

        private bool BelongsToThisContainer(ItemMeshDetection det)
        {
            if (det.usingContainerMeshes == null) return false;
            foreach (GameObject m in det.usingContainerMeshes)
            {
                ContainerMesh cm = m != null ? m.GetComponent<ContainerMesh>() : null;
                if (cm != null && cm.containerCreator == creator) return true;
            }
            return false;
        }

        private bool DataHas(PlacedEntry p)
        {
            foreach (InventorySlotItem slot in Data.itemList)
            {
                if (slot.anchorX == p.anchorX && slot.anchorY == p.anchorY &&
                    slot.CurrentWidth == p.width && slot.CurrentHeight == p.height)
                    return true;
            }
            return false;
        }

        private GameObject CreateItemGO(InventorySlotItem slot)
        {
            Item item = slot.itemData;
            int w = slot.CurrentWidth;
            int h = slot.CurrentHeight;

            GameObject go = new GameObject(item.itemName);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(creator.containerMeshPivotObj.transform.parent, false);

            Vector3 anchorPos = creator.containerMeshPivotObj.transform.position
                + new Vector3(slot.anchorX * creator.meshWidth, -slot.anchorY * creator.meshHeight, 0f);
            go.transform.position = anchorPos;

            GameObject imgGO = new GameObject("Icon", typeof(RectTransform));
            imgGO.transform.SetParent(go.transform, false);
            Image img = imgGO.AddComponent<Image>();
            img.sprite = item.icon;
            RectTransform imgRt = (RectTransform)imgGO.transform;
            imgRt.anchorMin = imgRt.anchorMax = new Vector2(0f, 1f);
            imgRt.pivot = new Vector2(0f, 1f);
            imgRt.anchoredPosition = Vector2.zero;
            imgRt.sizeDelta = new Vector2(w * creator.meshWidth, h * creator.meshHeight);

            ItemMeshCreator ic = go.AddComponent<ItemMeshCreator>();
            ic.containerCreator = creator;
            ic.itemMeshPrefab = creator.singleMeshPrefab;
            ic.itemImage = img;
            ic.itemMeshWidth = creator.meshWidth;
            ic.itemMeshHeight = creator.meshHeight;
            ic.meshNumber_Hor = item.boundWidth;
            ic.meshNumber_Ver = item.boundHeight;

            List<GameObject> occupied = new List<GameObject>();
            for (int gy = 0; gy < h; gy++)
            {
                for (int gx = 0; gx < w; gx++)
                {
                    GameObject m = Object.Instantiate(creator.singleMeshPrefab,
                        anchorPos + new Vector3(gx * creator.meshWidth, -gy * creator.meshHeight, 0f),
                        Quaternion.identity, go.transform);
                    ItemMesh mesh = m.GetComponent<ItemMesh>();
                    if (mesh == null) mesh = m.AddComponent<ItemMesh>();
                    mesh.itemMeshPos = new Vector2(gx, -gy);
                    mesh.itemMeshCreator = ic;
                    mesh.itemMeshPrefab = creator.singleMeshPrefab;
                    ic.itemMeshes.Add(m);

                    ContainerMesh cm = FindContainerMeshAt(slot.anchorX + gx, slot.anchorY + gy);
                    if (cm != null)
                    {
                        cm.isMeshUsed = true;
                        occupied.Add(cm.gameObject);
                    }
                }
            }

            ItemPivot pivot = go.AddComponent<ItemPivot>();
            pivot.itemImage = img;
            foreach (GameObject m in ic.itemMeshes)
                pivot.itemMeshPositions.Add(m.GetComponent<ItemMesh>().itemMeshPos);

            if (ContainerMeshTagDefined())
            {
                ItemMeshDetection det = imgGO.AddComponent<ItemMeshDetection>();
                det.canvas = backpackCanvas;
                det.parentItem = go;
                det.detectDistance = creator.meshWidth;
                det.usingContainerMeshes = occupied.ToArray();
                imgGO.AddComponent<ItemMeshImageDrag>();
            }
            else
            {
                Debug.LogWarning("[Scavenge] 未定义 containermesh 或 backpackmesh 标签");
            }

            var itemMgr = creator.GetComponent<Container_ItemManager>();
            if (itemMgr != null)
                itemMgr.AddItem(pivot, new Vector2(slot.anchorX, -slot.anchorY));

            return go;
        }

        private ContainerMesh FindContainerMeshAt(int gridX, int gridY)
        {
            foreach (GameObject gm in creator.containerMeshes)
            {
                ContainerMesh cm = gm != null ? gm.GetComponent<ContainerMesh>() : null;
                if (cm == null) continue;
                if (Mathf.RoundToInt(cm.meshPos.x) == gridX && Mathf.RoundToInt(-cm.meshPos.y) == gridY)
                    return cm;
            }
            return null;
        }

        private void DestroyPlacedItem(PlacedEntry p)
        {
            if (p.pivot != null)
            {
                var itemMgr = creator.GetComponent<Container_ItemManager>();
                if (itemMgr != null)
                    itemMgr.RemoveItem(p.pivot);
                else
                    FreePivotMeshes(p.pivot);
            }
            else if (p.det != null)
            {
                FreeDetMeshes(p.det);
            }
            Object.Destroy(p.go);
        }

        private void DestroySlotGo(GameObject go)
        {
            var itemMgr = creator.GetComponent<Container_ItemManager>();
            ItemPivot pivot = go.GetComponent<ItemPivot>();
            if (pivot != null && itemMgr != null)
            {
                itemMgr.RemoveItem(pivot);
                Object.Destroy(go);
                return;
            }

            ItemMeshDetection det = go.GetComponentInChildren<ItemMeshDetection>();
            FreeDetMeshes(det);
            Object.Destroy(go);
        }

        private static void FreePivotMeshes(ItemPivot pivot)
        {
            foreach (ContainerMesh cm in Object.FindObjectsOfType<ContainerMesh>())
            {
                if (cm == null) continue;
                if (pivot.itemMeshPositions.Contains(cm.meshPos - pivot.pivotPositionInContainer))
                    cm.isMeshUsed = false;
            }
        }

        private static void FreeDetMeshes(ItemMeshDetection det)
        {
            if (det == null || det.usingContainerMeshes == null) return;
            foreach (GameObject m in det.usingContainerMeshes)
            {
                ContainerMesh cm = m != null ? m.GetComponent<ContainerMesh>() : null;
                if (cm != null) cm.isMeshUsed = false;
            }
        }

        private static float NormalizeEulerZ(float z)
        {
            z = z % 360f;
            if (z > 180f) z -= 360f;
            if (z < -180f) z += 360f;
            return z;
        }

        private static bool? backpackMeshTagDefined;
        private static bool ContainerMeshTagDefined()
        {
            if (backpackMeshTagDefined == null)
            {
                try
                {
                    GameObject.FindGameObjectsWithTag("containermesh");
                    backpackMeshTagDefined = true;
                }
                catch (System.Exception)
                {
                    try
                    {
                        GameObject.FindGameObjectsWithTag("backpackmesh");
                        backpackMeshTagDefined = true;
                    }
                    catch (System.Exception)
                    {
                        backpackMeshTagDefined = false;
                    }
                }
            }
            return backpackMeshTagDefined.Value;
        }
    }
}
