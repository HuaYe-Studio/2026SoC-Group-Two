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
    private RectTransform dragIconTransform; 

private InventorySlotItem selectItem;
private Inventory selectSourceInv;
private int selectGridX;
private int selectGridY;
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

            float totalWidth = slot.CurrentWidth * cellSize + (slot.CurrentWidth - 1) * spacing;
float totalHeight = slot.CurrentHeight * cellSize + (slot.CurrentHeight - 1) * spacing;
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
        TrySelectItem(leftPanel, currentTrash, mousePos);
        if (selectItem == null)
            TrySelectItem(rightPanel, externalPlayerBag, mousePos);

        TryPickItem(leftPanel, currentTrash, mousePos);
        if (dragItem == null)
            TryPickItem(rightPanel, externalPlayerBag, mousePos);
    }

    if (dragItem != null && dragIconTransform != null)
    {
        RectTransform parentCanvas = dragIconTransform.parent as RectTransform;
        if (parentCanvas != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas, mousePos, null, out Vector2 localPoint))
        {
            dragIconTransform.localPosition = localPoint;
        }
    }

    if (Input.GetMouseButtonUp(0) && dragItem != null)
    {
        bool dropped = false;

        if (PosToGrid(leftPanel, mousePos, out int lx, out int ly))
        {
            dropped = TryDropItem(currentTrash, lx, ly);
        }

        if (!dropped && PosToGrid(rightPanel, mousePos, out int rx, out int ry))
        {
            dropped = TryDropItem(externalPlayerBag, rx, ry);
        }

        if (!dropped)
        {
            dragSource.TryPlaceItem(dragItem, dragStartX, dragStartY);
        }

        DestroyDragVisual();
        dragItem = null;
        dragSource = null;
        dragIconTransform = null;
        RefreshAll();
    }

    if (Input.GetKeyDown(KeyCode.R) && selectItem != null)
    {
        int curW = selectItem.CurrentWidth;
        int curH = selectItem.CurrentHeight;
        int nextW = curH;
        int nextH = curW;

        bool canRotate = selectSourceInv.CanPlaceAt(selectItem.anchorX, selectItem.anchorY, nextW, nextH);

        if (canRotate)
        {
            selectItem.ToggleRotate();
            RefreshAll();
        }
        else
        {
            Debug.Log("物品当前位置无法旋转");
        }
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

            dragSource.RemoveItem(dragItem);
            CreateDragVisual(panel, slot);
        }
    }

    void CreateDragVisual(RectTransform parentPanel, InventorySlotItem slot)
    {
        GameObject visual = Instantiate(iconPrefab, parentPanel);
        dragIconTransform = visual.GetComponent<RectTransform>();

        float totalWidth = slot.CurrentWidth * cellSize + (slot.CurrentWidth - 1) * spacing;
        float totalHeight = slot.CurrentHeight * cellSize + (slot.CurrentHeight - 1) * spacing;
        dragIconTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

        Image img = visual.GetComponent<Image>();
        if (img != null && slot.itemData != null)
            img.sprite = slot.itemData.icon;

        visual.transform.SetAsLastSibling();
    }

    void DestroyDragVisual()
    {
        if (dragIconTransform != null)
        {
            Destroy(dragIconTransform.gameObject);
            dragIconTransform = null;
        }
    }
    void TrySelectItem(RectTransform panel, Inventory inv, Vector2 mouse)
{
    if (!PosToGrid(panel, mouse, out int x, out int y)) 
        return;

    InventorySlotItem slot = inv.GetItemAtGrid(x, y);
    if (slot != null)
    {
        selectItem = slot;
        selectSourceInv = inv;
        selectGridX = x;
        selectGridY = y;
    }
    else
    {
        selectItem = null;
        selectSourceInv = null;
    }
}

    bool TryDropItem(Inventory targetInv, int x, int y)
{
    if (targetInv == dragSource && x == dragStartX && y == dragStartY)
        return false;
    bool canPlace = targetInv.TryPlaceItem(dragItem, x, y);
    return canPlace;
}
}
