using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Settings
{
    public class KeyRebindUI:MonoBehaviour
    {
        [Serializable]
        public class KeyRebindUIData
        {
            public string actionName;
            public TMP_Text keyText;
            public Button rebindButton;
        }
        
        [SerializeField]private List<KeyRebindUIData> KeyRebindUIDatas = new List<KeyRebindUIData>();
        [SerializeField] private Button resetButton;
        
        private string waitingText = "请按下新的按键……";
        private string conflictText = "按键冲突，请更换";
        private string idleText = "修改";
        
        private bool isWaiting = false;
        //-1 为不在等待
        private int waitingIndex = -1;

        private void Awake()
        {
            for (int i = 0; i < KeyRebindUIDatas.Count; i++)
            {
                int index = i;
                KeyRebindUIDatas[i].rebindButton.onClick.AddListener(() => OnRebindButtonClick(index));
            }
            
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);
        }

        private void Update()
        {
            if(!isWaiting||waitingIndex < 0)
                return;
            if (Keyboard.current == null) 
                return; 

            foreach (KeyControl control in Keyboard.current.allKeys)
            {
                if(control==null)
                    continue;
                if (!control.wasPressedThisFrame)
                    continue;
                if (IsModifierKey(control.keyCode))                                                                                                
                    continue;
                TryRebindKey(control.keyCode);
                return;
            }
        }

        private void OnEnable()
        {
            RefreshKeyText();
        }

        private void OnDisable()
        {
            CancelWaiting();
        }

        private void OnRebindButtonClick(int index)
        {
            if (isWaiting)
            {
                if (waitingIndex == index)
                    CancelWaiting();
                return;
            }

            if (index < 0 || index >= KeyRebindUIDatas.Count)
                return;
            if (String.IsNullOrEmpty(KeyRebindUIDatas[index].actionName))
                return;
            
            isWaiting = true;
            waitingIndex = index;
            SetButtonText(index,waitingText);
        }

        private void TryRebindKey(Key newKey)
        {
            KeyRebindUIData data = KeyRebindUIDatas[waitingIndex];
            bool isChanged = false;
            
            if (SettingManager.Instance != null)
            {
                isChanged = SettingManager.Instance.RebindKey(data.actionName, newKey);
            }

            if (isChanged)
            {
                data.keyText.text = newKey.ToString();
                isWaiting = false;
                waitingIndex = -1;
                SetButtonText(KeyRebindUIDatas.IndexOf(data),idleText);
            }
            else
            {
                SetButtonText(waitingIndex,conflictText);
            }
        }
        
        private void SetButtonText(int index,string text)
        {
            if (index < 0 || index >= KeyRebindUIDatas.Count)
                return;
            TMP_Text label = KeyRebindUIDatas[index].rebindButton.GetComponentInChildren<TMP_Text>();
            if(label!=null)
                label.text = text;
        }

        private void RefreshKeyText()
        {
            if(KeyManager.Instance==null)
                return;
            foreach (KeyRebindUIData data in KeyRebindUIDatas)
            {
                if(data == null || data.keyText.text == null)
                    continue;
                if(string.IsNullOrEmpty(data.actionName))
                    continue;
                data.keyText.text = KeyManager.Instance.GetKey(data.actionName).ToString();
            }
        }
        
        private void CancelWaiting()
        {
            if(!isWaiting||waitingIndex < 0)
                return;
            SetButtonText(waitingIndex,idleText);
            isWaiting = false;
            waitingIndex = -1;
        }
        
        private void OnResetClicked()
        {
            CancelWaiting();                    
            if (KeyManager.Instance != null)
                KeyManager.Instance.ResetAllKeys();
            RefreshKeyText();        
        }
        
        //不绑定修饰键
        private static bool IsModifierKey(Key key)
        {
            return key == Key.LeftShift || key == Key.RightShift
                                        || key == Key.LeftCtrl  || key == Key.RightCtrl
                                        || key == Key.LeftAlt   || key == Key.RightAlt;
        }
    }
}