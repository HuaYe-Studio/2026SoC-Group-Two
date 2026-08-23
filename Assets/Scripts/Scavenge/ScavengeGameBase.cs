using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scavenge
{
    public abstract class ScavengeGameBase : MonoBehaviour
    {
        public event Action<List<Item>, bool> OnSettled;

        [Header("通用界面")]
        [Tooltip("遮罩不透明度(0~1)")]
        [Range(0f, 1f)] public float maskAlpha = 0.7f;
        [Tooltip("游戏画布排序层级")]
        public int canvasSortingOrder = 1000;

        protected readonly List<Item> obtainedItems = new List<Item>();
        protected bool isSettled { get; private set; }

        protected Canvas overlayCanvas;

        private static int playerLockCount;
        private static bool playerWasEnabled; 
        private bool gameStarted;

        public void BeginGame()
        {
            if (gameStarted && !isSettled)
            {
                Debug.LogWarning("[Scavenge] 游戏运行中");
                return;
            }
            gameStarted = true;
            isSettled = false;
            obtainedItems.Clear();

            BuildOverlayCanvas();
            BuildGameUI();
            LockPlayer(true);
        }

        protected abstract void BuildGameUI();

        private void Update()
        {
            if (!gameStarted || isSettled) return;

            if (Input.GetKeyDown(KeyCode.Escape))
                Settle(false);
        }

        protected void Settle(bool completed)
        {
            if (isSettled) return;
            isSettled = true;

            if (overlayCanvas != null)
                Destroy(overlayCanvas.gameObject);

            LockPlayer(false);
            OnSettled?.Invoke(new List<Item>(obtainedItems), completed);
        }

        protected virtual void GrantExtraReward()
        {
            Debug.Log("[Scavenge] 通关额外奖励触发");
        }

        protected RectTransform CreatePanel(string name, Vector2 size, Color bgColor)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(overlayCanvas.transform, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;

            var img = go.GetComponent<Image>();
            img.color = bgColor;
            img.raycastTarget = true;
            return rt;
        }

        private void BuildOverlayCanvas()
        {
            var canvasGo = new GameObject("ScavengeOverlayCanvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);

            overlayCanvas = canvasGo.GetComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = canvasSortingOrder;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var maskGo = new GameObject("Mask", typeof(RectTransform), typeof(Image));
            maskGo.transform.SetParent(canvasGo.transform, false);

            var maskRt = (RectTransform)maskGo.transform;
            maskRt.anchorMin = Vector2.zero;
            maskRt.anchorMax = Vector2.one;
            maskRt.offsetMin = Vector2.zero;
            maskRt.offsetMax = Vector2.zero;

            var maskImg = maskGo.GetComponent<Image>();
            maskImg.color = new Color(0f, 0f, 0f, maskAlpha);
            maskImg.raycastTarget = true;
        }

        protected static void LockPlayer(bool locked)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("[Scavenge] 未找到玩家");
                return;
            }
            var pc = player.GetComponent<PlayerController>();
            if (pc == null)
            {
                Debug.LogWarning("[Scavenge] 未找到PlayerController组件");
                return;
            }
            int prevCount = playerLockCount;
            playerLockCount = Mathf.Max(0, playerLockCount + (locked ? 1 : -1));
            if (prevCount == 0 && playerLockCount > 0)
            {
                playerWasEnabled = pc.enabled;
                pc.enabled = false;
            }
            else if (prevCount > 0 && playerLockCount == 0)
            {
                pc.enabled = playerWasEnabled;
            }
        }
    }
}
