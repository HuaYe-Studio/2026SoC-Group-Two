using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldTime;

namespace Scavenge
{
    public class ScavengeSpot : MonoBehaviour
    {
        [Header("拾荒键")]
        [Tooltip("开始拾荒按键")]
        public KeyCode interactKey = KeyCode.E;
        [Tooltip("互动距离")]
        public float interactRange = 3f;
        [Tooltip("拾荒键随机出现检查间隔(秒)")]
        public float spawnInterval = 30f;
        [Tooltip("每次检查时出现的概率(0~1)")]
        [Range(0f, 1f)] public float spawnChance = 0.5f;

        [Header("拾荒键光提示")]
        public Color keyLightColor = new Color(1f, 0.85f, 0.3f, 1f);
        public float keyLightIntensity = 2.5f;
        public float keyLightRange = 6f;
        public Vector3 keyLightOffset = new Vector3(0f, 0.8f, 0f);

        [Header("对应游戏")]
        [Tooltip("拖入该容器对应的小游戏组件")]
        public ScavengeGameBase game;

        [Header("容器")]
        public int containerWidth = 6;
        public int containerHeight = 3;

        [Header("刷新周期")]
        [Tooltip("已被拾荒过的容器。\n用天数变化当作存档时机")]
        public bool refreshOnDayChanged = true;

        [HideInInspector] public Inventory containerInv;

        private Transform player;
        private Light keyLight;
        private bool keyActive;
        private bool gameRunning;
        private bool scavengedThisCycle;
        private float nextRollTime;
        private float playerFindTimer;
        private bool warnedNoGame;
        private bool containerWasOpen; 

        // 靠近提示
        private Canvas hintCanvas;
        private Text hintLabel;
        private bool hintVisible;

        void Awake()
        {
            containerInv = new Inventory(containerWidth, containerHeight);
            nextRollTime = Time.time + Random.Range(0f, spawnInterval);
            CreateKeyLight();
            CreateHintUI();
            ScavengeSaveEvents.OnWorldSaved += OnWorldSaved;
        }

        void Start()
        {

            if (refreshOnDayChanged && TimeManager.Instance != null)
                TimeManager.Instance.OnDayChanged += OnDayChangedStandIn;

            ScavengePlayerBag.EnsureWired(); // 提前接好场景背包，容器UI随时显示真实玩家背包
        }

        void OnDestroy()
        {
            ScavengeSaveEvents.OnWorldSaved -= OnWorldSaved;
            if (TimeManager.Instance != null)
                TimeManager.Instance.OnDayChanged -= OnDayChangedStandIn;
        }

       
        private void OnWorldSaved()
        {
            scavengedThisCycle = false;
            nextRollTime = Time.time + Random.Range(0f, spawnInterval);
        }

        private void OnDayChangedStandIn(System.DateTime currentTime)
        {
            OnWorldSaved();
        }

        void Update()
        {
            FindPlayer();

            if (!keyActive && !gameRunning && !scavengedThisCycle && Time.time >= nextRollTime)
            {
                nextRollTime = Time.time + spawnInterval;
                if (Random.value < spawnChance)
                    SetKeyActive(true);
            }

            if (keyActive && player != null)
            {
                bool inRange = Vector3.Distance(transform.position, player.position) <= interactRange;
                SetHintVisible(inRange);
                if (inRange && Input.GetKeyDown(interactKey))
                    StartGame();
            }
            else
            {
                SetHintVisible(false);
            }

            if (hintVisible && Camera.main != null)
                hintCanvas.transform.rotation = Camera.main.transform.rotation;

            SyncBackpackWithContainer();
        }

        /// <summary>轮询容器窗口开关触发背包双向同步（开：场景→数据；关：数据→场景）</summary>
        private void SyncBackpackWithContainer()
        {
            var uiMgr = InventoryUIManager.Instance;
            if (uiMgr == null) return;

            bool open = uiMgr.windowRoot != null && uiMgr.windowRoot.activeSelf;
            if (open == containerWasOpen) return;
            containerWasOpen = open;

            if (open)
                ScavengePlayerBag.OnContainerOpened();
            else
                ScavengePlayerBag.OnContainerClosed();
        }

        private void SetKeyActive(bool active)
        {
            keyActive = active;
            keyLight.enabled = active;
            if (!active)
                SetHintVisible(false);
        }

        private void CreateKeyLight()
        {
            var go = new GameObject("ScavengeKeyLight");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = keyLightOffset;

            keyLight = go.AddComponent<Light>();
            keyLight.type = LightType.Point;
            keyLight.color = keyLightColor;
            keyLight.intensity = keyLightIntensity;
            keyLight.range = keyLightRange;
            keyLight.enabled = false;
        }

        private void CreateHintUI()
        {
            var go = new GameObject("InteractHint", typeof(RectTransform), typeof(Canvas));
            go.transform.SetParent(transform, false);
            go.transform.localPosition = keyLightOffset + Vector3.up * 0.9f;

            hintCanvas = go.GetComponent<Canvas>();
            hintCanvas.renderMode = RenderMode.WorldSpace;
            hintCanvas.sortingOrder = 1;

            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(300f, 50f);
            go.transform.localScale = Vector3.one * 0.01f; // 300*0.01 = 3世界单位宽

            hintLabel = ScavengeUI.CreateText(go.transform, "Label", $"按 {interactKey} 拾荒",
                Color.white, 30, TextAnchor.MiddleCenter);
            ScavengeUI.AddOutline(hintLabel);
            go.SetActive(false);
        }

        private void SetHintVisible(bool visible)
        {
            if (hintVisible == visible) return;
            hintVisible = visible;
            hintCanvas.gameObject.SetActive(visible);
        }

        private void StartGame()
        {
            if (game == null)
            {
                if (!warnedNoGame)
                {
                    Debug.LogError("[Scavenge] 未指定该容器对应的小游戏");
                    warnedNoGame = true;
                }
                return;
            }

            SetKeyActive(false);
            gameRunning = true;

            game.OnSettled -= OnGameSettled;
            game.OnSettled += OnGameSettled;
            game.BeginGame();
        }

        
        private void OnGameSettled(List<Item> obtainedItems, bool completed)
        {
            gameRunning = false;
           
            scavengedThisCycle = true;

            containerInv.itemList.Clear();
            foreach (Item item in obtainedItems)
            {
                if (!TryPlaceAnywhere(containerInv, item))
                    Debug.LogWarning($"[Scavenge] 容器({containerWidth}x{containerHeight})已满，{item.itemName} 放不下");
            }

            var uiMgr = InventoryUIManager.Instance;
            if (uiMgr == null)
            {
                Debug.LogWarning("[Scavenge] 找不到 InventoryUIManager");
                return;
            }
            if (ScavengePlayerBag.Get() == null)
            {
                Debug.LogWarning("[Scavenge] 场景中没有背包面板");
                uiMgr.SetPlayerInventory(new Inventory(6, 4));
            }
            uiMgr.Open(containerInv);
        }

        private static bool TryPlaceAnywhere(Inventory inv, Item item)
        {
            for (int y = 0; y < inv.height; y++)
                for (int x = 0; x < inv.width; x++)
                    if (inv.TryPlaceItem(item, x, y))
                        return true;
            return false;
        }

        private void FindPlayer()
        {
            if (player != null) return;
            playerFindTimer -= Time.deltaTime;
            if (playerFindTimer > 0f) return;
            playerFindTimer = 1f;
            var go = GameObject.FindWithTag("Player");
            if (go != null)
                player = go.transform;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
