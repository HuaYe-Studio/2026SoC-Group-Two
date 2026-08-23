using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        private GameObject currentOpenUI;
        public bool IsAnyUIOpen => currentOpenUI != null;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OpenUI(GameObject UI)
        {
            if (currentOpenUI == UI)
                return;
            
            CloseCurrent();
            UI.SetActive(true);
            if(GameFlowManager.Instance != null)
                GameFlowManager.Instance.PauseGameplay($"{UI.name}");
            currentOpenUI = UI;
        }

        public void CloseUI(GameObject UI)
        {
            if (currentOpenUI == null)
                return;
            UI.SetActive(false);
            if(GameFlowManager.Instance != null)
                GameFlowManager.Instance.ResumeGameplay($"{UI.name}");
            if (currentOpenUI == UI)
                currentOpenUI = null;
        }
        
        public void CloseCurrent()           
        {
            if (currentOpenUI == null) return;
            currentOpenUI.SetActive(false);
            if(GameFlowManager.Instance != null)
                GameFlowManager.Instance.ResumeGameplay($"{currentOpenUI.name}");
            currentOpenUI = null;
        }
    }
}