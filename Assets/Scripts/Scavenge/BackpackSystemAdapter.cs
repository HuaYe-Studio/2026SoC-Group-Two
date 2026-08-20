using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scavenge
{
    public class BackpackSystemAdapter
    {
        public bool HasBackpackSystem => creator != null;
        public Inventory Data { get; private set; }

        private BackpackCreator creator;
        private Canvas backpackCanvas;
        private readonly Dictionary<InventorySlotItem, GameObject> slotToGo =
            new Dictionary<InventorySlotItem, GameObject>();

        public BackpackSystemAdapter()
        {
            creator = Object.FindObjectOfType<BackpackCreator>();
            if (creator == null)
            {
                Debug.LogWarning("[Scavenge] 没有找到 BackpackCreator");
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

            foreach (ItemMeshDetection det in Object.FindObjectsOfType<ItemMeshDetection>())
            {
                if (det == null || det.usingBackpackMeshes == null || det.usingBackpackMeshes.Length == 0)
                    continue;
                GameObject go = det.parentItem != null ? det.parentItem : det.gameObject;
                if (go == null || !BelongsToThisBackpack(det)) continue;

                ItemInfo info = ReadPlacedItem(det, go);
                if (info == null) continue;

                InventorySlotItem slot = new InventorySlotItem(info.item, info.anchorX, info.anchorY, info.rotated);
                Data.itemList.Add(slot);
                slotToGo[slot] = go;
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
                FreeAndDestroy(go);
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
            public ItemMeshDetection det;
            public int anchorX, anchorY, width, height;
        }

        private class ItemInfo
        {
            public Item item;
            public int anchorX, anchorY;
            public bool rotated;
        }

        private bool BelongsToThisBackpack(ItemMeshDetection det)
        {
            foreach (GameObject m in det.usingBackpackMeshes)
            {
                BackpackMesh bm = m != null ? m.GetComponent<BackpackMesh>() : null;
                if (bm != null && bm.backpackCreator == creator) return true;
            }
            return false;
        }

        private ItemInfo ReadPlacedItem(ItemMeshDetection det, GameObject go)
        {
            Vector3 rootPos = det.parentItem != null ? det.parentItem.transform.position : det.transform.position;

            BackpackMesh pivotMesh = null;
            float best = float.MaxValue;
            foreach (GameObject gm in creator.backpackMeshes)
            {
                if (gm == null) continue;
                BackpackMesh bm = gm.GetComponent<BackpackMesh>();
                if (bm == null) continue;
                float d = Vector3.Distance(gm.transform.position, rootPos);
                if (d < best) { best = d; pivotMesh = bm; }
            }
            if (pivotMesh == null) return null;

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
            return new ItemInfo { item = item, anchorX = anchorX, anchorY = anchorY, rotated = rotated };
        }

        private List<PlacedEntry> CollectPlacedItems()
        {
            List<PlacedEntry> result = new List<PlacedEntry>();
            foreach (ItemMeshDetection det in Object.FindObjectsOfType<ItemMeshDetection>())
            {
                if (det == null || det.usingBackpackMeshes == null || det.usingBackpackMeshes.Length == 0)
                    continue;
                GameObject go = det.parentItem != null ? det.parentItem : det.gameObject;
                if (go == null || !BelongsToThisBackpack(det)) continue;

                ItemInfo info = ReadPlacedItem(det, go);
                if (info == null) continue;
                result.Add(new PlacedEntry
                {
                    go = go,
                    det = det,
                    anchorX = info.anchorX,
                    anchorY = info.anchorY,
                    width = info.rotated ? info.item.boundHeight : info.item.boundWidth,
                    height = info.rotated ? info.item.boundWidth : info.item.boundHeight
                });
            }
            return result;
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
            bool rotated = slot.isRotated;
            int w = slot.CurrentWidth;  
            int h = slot.CurrentHeight; 

            GameObject go = new GameObject(item.itemName);
            RectTransform goRt = go.AddComponent<RectTransform>();
            go.transform.SetParent(creator.backpackMeshPivotObj.transform.parent, false);

            Vector3 anchorPos = creator.backpackMeshPivotObj.transform.position
                + new Vector3(slot.anchorX * creator.meshWidth, -slot.anchorY * creator.meshHeight, 0f);
            go.transform.position = anchorPos;
            if (rotated)
                go.transform.localEulerAngles = new Vector3(0f, 0f, -90f);

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
            ic.backpackCreator = creator;
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

                    BackpackMesh bm = FindBackpackMeshAt(slot.anchorX + gx, slot.anchorY + gy);
                    if (bm != null)
                    {
                        bm.isMeshUsed = true;
                        occupied.Add(bm.gameObject);
                    }
                }
            }

            if (BackpackMeshTagDefined())
            {
                ItemMeshDetection det = imgGO.AddComponent<ItemMeshDetection>();
                det.canvas = backpackCanvas;
                det.parentItem = go;
                det.itemImage = img;
                det.detectDistance = creator.meshWidth;
                det.rotate_KeyCode = KeyCode.R;
                det.usingBackpackMeshes = occupied.ToArray();
            }
            else
            {
                Debug.LogWarning("[Scavenge] 未定义 backpackmesh tag");
            }

            return go;
        }

        private BackpackMesh FindBackpackMeshAt(int gridX, int gridY)
        {
            foreach (GameObject gm in creator.backpackMeshes)
            {
                BackpackMesh bm = gm != null ? gm.GetComponent<BackpackMesh>() : null;
                if (bm == null) continue;
                if (Mathf.RoundToInt(bm.meshPos.x) == gridX && Mathf.RoundToInt(-bm.meshPos.y) == gridY)
                    return bm;
            }
            return null;
        }

        private void DestroyPlacedItem(PlacedEntry p)
        {
            if (p.det != null && p.det.usingBackpackMeshes != null)
            {
                foreach (GameObject m in p.det.usingBackpackMeshes)
                {
                    BackpackMesh bm = m != null ? m.GetComponent<BackpackMesh>() : null;
                    if (bm != null) bm.isMeshUsed = false;
                }
            }
            Object.Destroy(p.go);
        }

        private void FreeAndDestroy(GameObject go)
        {
            ItemMeshDetection det = go.GetComponentInChildren<ItemMeshDetection>();
            if (det != null && det.usingBackpackMeshes != null)
            {
                foreach (GameObject m in det.usingBackpackMeshes)
                {
                    BackpackMesh bm = m != null ? m.GetComponent<BackpackMesh>() : null;
                    if (bm != null) bm.isMeshUsed = false;
                }
            }
            Object.Destroy(go);
        }

        private static float NormalizeEulerZ(float z)
        {
            z = z % 360f;
            if (z > 180f) z -= 360f;
            if (z < -180f) z += 360f;
            return z;
        }

        private static bool? backpackMeshTagDefined;
        private static bool BackpackMeshTagDefined()
        {
            if (backpackMeshTagDefined == null)
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
            return backpackMeshTagDefined.Value;
        }
    }
}
