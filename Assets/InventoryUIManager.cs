using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;
    void Awake() => Instance = this;

    [Header("UI")]
    public GameObject windowRoot;
    public RectTransform leftPanel;
    public RectTransform rightPanel;
    public GameObject iconPrefab;
    public GameObject cellBgPrefab;

    [Header("格子参数")]
    public float cellSize = 60;
    public float spacing = 5;

    [HideInInspector] public Inventory externalPlayerBag; 
    [HideInInspector] public Inventory currentTrash;

    // 拖拽数据
    private Inventory dragSource;
    private InventorySlotItem dragItem;
    private int dragStartX, dragStartY;

    void Start()
    {
        windowRoot.SetActive(false);
    }

    public void SetPlayerInventory(Inventory playerInv)
    {
        externalPlayerBag = playerInv;
    }

    public void Open(Inventory trash)
    {
        currentTrash = trash;
        windowRoot.SetActive(true);
        RefreshAll();
    }

    public void Close()
    {
        windowRoot.SetActive(false);
        dragItem = null;
        dragSource = null;
    }

    public void RefreshAll()
    {
        ClearAllIcons(leftPanel);
        ClearAllIcons(rightPanel);
        RenderInventory(currentTrash, leftPanel);
        RenderInventory(externalPlayerBag, rightPanel);
    }

    void ClearAllIcons(RectTransform panel)
    {
        for (int i = panel.childCount - 1; i >= 0; i--)
        {
            Destroy(panel.GetChild(i).gameObject);
        }
    }

    void RenderInventory(Inventory inv, RectTransform panel)
    {
        for (int y = 0; y < inv.height; y++)
        {
            for (int x = 0; x < inv.width; x++)
            {
                GameObject bgObj = Instantiate(cellBgPrefab, panel);
                RectTransform rt = bgObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cellSize, cellSize);
                float px = x * (cellSize + spacing);
                float py = -y * (cellSize + spacing);
                rt.anchoredPosition = new Vector2(px, py);
            }
        }

        foreach (var slot in inv.itemList)
        {
            GameObject iconObj = Instantiate(iconPrefab, panel);
            RectTransform rt = iconObj.GetComponent<RectTransform>();

            float totalWidth = slot.itemData.boundWidth * cellSize + (slot.itemData.boundWidth - 1) * spacing;
            float totalHeight = slot.itemData.boundHeight * cellSize + (slot.itemData.boundHeight - 1) * spacing;
            rt.sizeDelta = new Vector2(totalWidth, totalHeight);

            float x = slot.anchorX * (cellSize + spacing);
            float y = -slot.anchorY * (cellSize + spacing);
            rt.anchoredPosition = new Vector2(x, y);

            Image img = iconObj.GetComponent<Image>();
            img.sprite = slot.itemData.icon;
        }
    }

    public bool PosToGrid(RectTransform panel, Vector2 screenPos, out int gx, out int gy)
    {
        gx = 0; gy = 0;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, screenPos, null, out Vector2 local))
            return false;

        if (!panel.rect.Contains(local))
            return false;

        gx = Mathf.FloorToInt(local.x / (cellSize + spacing));
        gy = Mathf.FloorToInt(-local.y / (cellSize + spacing));

        return true;
    }

    void Update()
    {
        if (!windowRoot.activeSelf) return;

        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            TryPickItem(leftPanel, currentTrash, mousePos);
            if (dragItem == null)
                TryPickItem(rightPanel, externalPlayerBag, mousePos);
        }

        if (Input.GetMouseButtonUp(0) && dragItem != null)
        {
            bool placed = false;
            if (PosToGrid(leftPanel, mousePos, out int lx, out int ly))
            {
                placed = TryDropItem(currentTrash, lx, ly);
            }
            if (!placed && PosToGrid(rightPanel, mousePos, out int rx, out int ry))
            {
                placed = TryDropItem(externalPlayerBag, rx, ry);
            }

            dragItem = null;
            dragSource = null;
            RefreshAll();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    void TryPickItem(RectTransform panel, Inventory inv, Vector2 mouse)
    {
        if (dragItem != null) return;
        if (!PosToGrid(panel, mouse, out int x, out int y)) return;

        InventorySlotItem slot = inv.GetItemAtGrid(x, y);
        if (slot != null)
        {
            dragSource = inv;
            dragItem = slot;
            dragStartX = slot.anchorX;
            dragStartY = slot.anchorY;
        }
    }

    bool TryDropItem(Inventory targetInv, int x, int y)
    {
        if (targetInv == dragSource && x == dragStartX && y == dragStartY)
            return false;

        bool canPlace = targetInv.TryPlaceItem(dragItem.itemData, x, y);
        if (canPlace)
        {
            dragSource.RemoveItem(dragItem);
            return true;
        }
        return false;
    }
}