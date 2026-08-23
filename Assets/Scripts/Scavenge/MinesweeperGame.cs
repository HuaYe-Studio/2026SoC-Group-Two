using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scavenge
{
    public class MinesweeperGame : ScavengeGameBase
    {
        [Header("棋盘设置")]
        [Tooltip("排数")]
        public int rows = 9;
        [Tooltip("列数")]
        public int cols = 9;
        [Tooltip("雷数量")]
        public int mineCount = 12;

        [Header("物品")]
        [Tooltip("可能出现的物品池（用 ItemDef 资产配置）")]
        public List<ItemDef> itemDefCandidates = new List<ItemDef>();
        [Tooltip("布置的物品数量")]
        public int itemCount = 2;

        [Header("能量")]
        public float initialEnergy = 100f;
        [Tooltip("每次点击消耗的能量")]
        public float clickEnergyCost = 3f;

        [Header("踩雷惩罚")]
        [Tooltip("随机扣除能量值")]
        public float[] mineEnergyPenaltyOptions = { 10f, 15f, 20f };

        [Header("界面")]
        public float cellSize = 80f;
        public float cellSpacing = 4f;
        public Color hiddenCellColor = new Color(0.32f, 0.36f, 0.43f, 1f);
        public Color revealedCellColor = new Color(0.55f, 0.60f, 0.68f, 1f);
        public Color mineCellColor = new Color(0.65f, 0.25f, 0.25f, 1f);
        [Tooltip("数字颜色（浅色）")]
        public Color numberColor = new Color(0.88f, 0.93f, 1f, 1f);
        public Color panelColor = new Color(0.09f, 0.10f, 0.13f, 0.96f);

        private enum CellType { Normal, Mine, Item }

        private class PlacedItem
        {
            public Item def;
            public int anchorX;   
            public int anchorY;   
            public int revealedCells; 
            public bool obtained;
            public Image overlay; 
        }

        private CellType[,] board;
        private int[,] itemIndexAt;  
        private bool[,] revealed;
        private Button[,] cellButtons;
        private Text[,] cellNumberTexts;
        private readonly List<PlacedItem> placedItems = new List<PlacedItem>();
        private readonly List<Item> itemPool = new List<Item>();

        private bool boardGenerated;
        private float energy;
        private int revealedNonMineCount;
        private int totalNonMineCount;

        private RectTransform boardRect;
        private Text energyText;
        private Text hintText;

        private float CellStep => cellSize + cellSpacing;

        protected override void BuildGameUI()
        {
            boardGenerated = false;
            board = new CellType[rows, cols];
            itemIndexAt = new int[rows, cols];
            revealed = new bool[rows, cols];
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    itemIndexAt[y, x] = -1;
            placedItems.Clear();
            BuildItemPool();
            energy = initialEnergy;
            revealedNonMineCount = 0;
            totalNonMineCount = 0;

            float boardW = cols * cellSize + (cols - 1) * cellSpacing;
            float boardH = rows * cellSize + (rows - 1) * cellSpacing;
            const float pad = 24f;

            RectTransform panel = CreatePanel("MinesweeperPanel",
                new Vector2(boardW + pad * 2f, boardH + 150f + pad * 2f), panelColor);

            Text title = ScavengeUI.CreateText(panel, "Title", "扫 荒", Color.white, 34);
            RectTransform titleRt = (RectTransform)title.transform;
            titleRt.anchorMin = titleRt.anchorMax = new Vector2(0.5f, 1f);
            titleRt.sizeDelta = new Vector2(320f, 40f);
            titleRt.anchoredPosition = new Vector2(0f, -pad - 20f);

            energyText = ScavengeUI.CreateText(panel, "EnergyText", "", Color.white, 22);
            RectTransform energyRt = (RectTransform)energyText.transform;
            energyRt.anchorMin = energyRt.anchorMax = new Vector2(0.5f, 1f);
            energyRt.sizeDelta = new Vector2(400f, 28f);
            energyRt.anchoredPosition = new Vector2(0f, -pad - 56f);
            UpdateEnergyUI();

            var boardGo = new GameObject("Board", typeof(RectTransform));
            boardGo.transform.SetParent(panel, false);
            boardRect = (RectTransform)boardGo.transform;
            boardRect.anchorMin = boardRect.anchorMax = new Vector2(0.5f, 1f);
            boardRect.pivot = new Vector2(0.5f, 1f);
            boardRect.sizeDelta = new Vector2(boardW, boardH);
            boardRect.anchoredPosition = new Vector2(0f, -pad - 92f);

            cellButtons = new Button[rows, cols];
            cellNumberTexts = new Text[rows, cols];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    cellButtons[y, x] = CreateCellButton(x, y);
                    cellNumberTexts[y, x] = CreateCellNumberText(cellButtons[y, x].transform);
                }
            }

            hintText = ScavengeUI.CreateText(panel, "HintText",
                $"消耗{clickEnergyCost:0.#}能量 | 踩雷随机惩罚 | Esc随时退出",
                new Color(0.72f, 0.76f, 0.84f, 1f), 18);
            RectTransform hintRt = (RectTransform)hintText.transform;
            hintRt.anchorMin = hintRt.anchorMax = new Vector2(0.5f, 0f);
            hintRt.sizeDelta = new Vector2(boardW, 28f);
            hintRt.anchoredPosition = new Vector2(0f, pad + 6f);
        }

        private void BuildItemPool()
        {
            itemPool.Clear();
            foreach (ItemDef def in itemDefCandidates)
                if (def != null)
                    itemPool.Add(def.ToItem());

            if (itemCount > 0 && itemPool.Count == 0)
                Debug.LogWarning("[Scavenge] 本局没有物品");
        }

        private Button CreateCellButton(int x, int y)
        {
            var go = new GameObject($"Cell_{x}_{y}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(boardRect, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.sizeDelta = new Vector2(cellSize, cellSize);
            rt.anchoredPosition = new Vector2(x * CellStep, -y * CellStep);

            var img = go.GetComponent<Image>();
            img.color = hiddenCellColor;

            var btn = go.GetComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.targetGraphic = img;

            int cx = x, cy = y;
            btn.onClick.AddListener(() => OnCellClicked(cx, cy));
            return btn;
        }

        private Text CreateCellNumberText(Transform cell)
        {
            Text text = ScavengeUI.CreateText(cell, "Number", "", numberColor,
                Mathf.RoundToInt(cellSize * 0.42f), TextAnchor.MiddleCenter, FontStyle.Bold);
            ScavengeUI.AddOutline(text);
            text.gameObject.SetActive(false);
            return text;
        }
        private void OnCellClicked(int x, int y)
        {
            if (isSettled || revealed[y, x]) return;

            if (!boardGenerated)
                GenerateBoard(x, y);

            energy = Mathf.Max(0f, energy - clickEnergyCost);
            UpdateEnergyUI();
            if (energy <= 0f)
            {
                SetHint("能量耗尽");
                Settle(false);
                return;
            }

            if (board[y, x] == CellType.Mine)
            {
                ApplyMinePunishment();
                RevealCell(x, y);
                if (energy <= 0f)
                {
                    SetHint("能量耗尽");
                    Settle(false);
                }
                return;
            }

            RevealCell(x, y);

            if (revealedNonMineCount >= totalNonMineCount)
            {
                SetHint("额外奖励！");
                GrantExtraReward();
                Settle(true);
            }
        }

    
        private void ApplyMinePunishment()
        {
            if (Random.Range(0, 2) == 0 && ScavengePlayerBag.RemoveRandomItem(out Item lost))
            {
                SetHint($"踩雷，丢失背包物品：{lost.itemName}");
                Debug.Log($"[Scavenge] 踩雷惩罚：丢失背包物品 {lost.itemName}");
                return;
            }

            float penalty = mineEnergyPenaltyOptions.Length > 0
                ? mineEnergyPenaltyOptions[Random.Range(0, mineEnergyPenaltyOptions.Length)]
                : 15f;
            energy = Mathf.Max(0f, energy - penalty);
            UpdateEnergyUI();
            SetHint($"踩雷，能量 -{penalty:0.#}");
            Debug.Log($"[Scavenge] 踩雷惩罚：能量 -{penalty:0.#}");
        }

        private void RevealCell(int x, int y)
        {
            revealed[y, x] = true;
            cellButtons[y, x].interactable = false; 

            var img = cellButtons[y, x].GetComponent<Image>();
            if (board[y, x] == CellType.Mine)
            {
                img.color = mineCellColor;
                ShowCellText(y, x, "雷", Color.white);
                return;
            }

            img.color = revealedCellColor;
            revealedNonMineCount++;

            int n = CountAdjacentMines(x, y);
            if (n > 0)
                ShowCellText(y, x, n.ToString(), numberColor);

            if (board[y, x] == CellType.Item)
            {
                var placed = placedItems[itemIndexAt[y, x]];
                if (placed.overlay == null)
                    CreateItemOverlay(placed);
                if (n > 0)
                    CreateOverlayNumberChip(placed, x, y, n); 
                placed.revealedCells++;

                if (!placed.obtained && placed.revealedCells >= placed.def.boundWidth * placed.def.boundHeight)
                {
                    placed.obtained = true;
                    obtainedItems.Add(placed.def);
                    if (placed.overlay != null)
                        Destroy(placed.overlay.gameObject);
                    SetHint($"获得{placed.def.itemName}");
                    Debug.Log($"[Scavenge] 获得{placed.def.itemName}");
                }
            }
        }

        private void GenerateBoard(int firstX, int firstY)
        {
            int total = rows * cols;
            mineCount = Mathf.Clamp(mineCount, 1, total - 9); 
            totalNonMineCount = total - mineCount;

            board = new CellType[rows, cols]; 
            itemIndexAt = new int[rows, cols];
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    itemIndexAt[y, x] = -1;

            var mineCandidates = new List<Vector2Int>();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (Mathf.Abs(x - firstX) <= 1 && Mathf.Abs(y - firstY) <= 1) continue;
                    mineCandidates.Add(new Vector2Int(x, y));
                }
            }
            for (int i = 0; i < mineCount && mineCandidates.Count > 0; i++)
            {
                int idx = Random.Range(0, mineCandidates.Count);
                Vector2Int pos = mineCandidates[idx];
                mineCandidates.RemoveAt(idx);
                board[pos.y, pos.x] = CellType.Mine;
            }

            int placedCount = 0;
            for (int i = 0; i < itemCount && itemPool.Count > 0; i++)
            {
                bool ok = false;
                for (int attempt = 0; attempt < 60 && !ok; attempt++)
                {
                    Item def = itemPool[Random.Range(0, itemPool.Count)];
                    int w = def.boundWidth, h = def.boundHeight;
                    if (w <= 0 || h <= 0 || w > cols || h > rows) continue;

                    int ax = Random.Range(0, cols - w + 1);
                    int ay = Random.Range(0, rows - h + 1);
                    if (!IsAreaFree(ax, ay, w, h)) continue;

                    for (int dy = 0; dy < h; dy++)
                        for (int dx = 0; dx < w; dx++)
                        {
                            board[ay + dy, ax + dx] = CellType.Item;
                            itemIndexAt[ay + dy, ax + dx] = placedCount;
                        }
                    placedItems.Add(new PlacedItem { def = def, anchorX = ax, anchorY = ay });
                    placedCount++;
                    ok = true;
                }
                if (!ok)
                    Debug.LogWarning($"[Scavenge] 第{i + 1}个物品布置失败，本局物品 {placedCount}/{itemCount}");
            }

            boardGenerated = true;
        }

        private bool IsAreaFree(int anchorX, int anchorY, int w, int h)
        {
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                    if (board[anchorY + dy, anchorX + dx] != CellType.Normal)
                        return false;
            return true;
        }

        private int CountAdjacentMines(int x, int y)
        {
            int n = 0;
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (nx < 0 || nx >= cols || ny < 0 || ny >= rows) continue;
                    if (board[ny, nx] == CellType.Mine) n++;
                }
            }
            return n;
        }

        private void CreateItemOverlay(PlacedItem placed)
        {
            var go = new GameObject("ItemOverlay_" + placed.def.itemName, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(boardRect, false);
            go.transform.SetAsLastSibling(); 

            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            float w = placed.def.boundWidth, h = placed.def.boundHeight;
            rt.anchoredPosition = new Vector2(placed.anchorX * CellStep, -placed.anchorY * CellStep);
            rt.sizeDelta = new Vector2(w * cellSize + (w - 1) * cellSpacing, h * cellSize + (h - 1) * cellSpacing);

            var img = go.GetComponent<Image>();
            img.sprite = placed.def.icon;
            img.color = placed.def.icon != null ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            img.raycastTarget = false; 

            placed.overlay = img;
        }

        private void CreateOverlayNumberChip(PlacedItem placed, int x, int y, int n)
        {
            Text chip = ScavengeUI.CreateText(placed.overlay.transform, $"Chip_{x}_{y}", n.ToString(),
                numberColor, Mathf.RoundToInt(cellSize * 0.42f), TextAnchor.MiddleCenter, FontStyle.Bold);
            ScavengeUI.AddOutline(chip);

            var rt = (RectTransform)chip.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(cellSize, cellSize);

            Vector2 overlaySize = placed.overlay.rectTransform.sizeDelta;
            float localX = (x - placed.anchorX) * CellStep + cellSize * 0.5f;
            float localY = -((y - placed.anchorY) * CellStep + cellSize * 0.5f);
            rt.anchoredPosition = new Vector2(localX - overlaySize.x * 0.5f, localY + overlaySize.y * 0.5f);
        }

        private void ShowCellText(int row, int col, string content, Color color)
        {
            var t = cellNumberTexts[row, col];
            t.gameObject.SetActive(true);
            t.text = content;
            t.color = color;
        }

        private void UpdateEnergyUI()
        {
            if (energyText != null)
                energyText.text = $"能量：{energy:0} / {initialEnergy:0}";
        }

        private void SetHint(string msg)
        {
            if (hintText != null)
                hintText.text = msg;
        }
        
    }
}
