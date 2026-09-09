using System;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingPageSwitch : MonoBehaviour
    {
        [SerializeField] private GameObject mainSettingsPage;
        [SerializeField] private GameObject keybindPage;
        
        [SerializeField] private Button toKeybindButton;     
        [SerializeField] private Button backButton; 
        
        private void Awake()
        {
            if (toKeybindButton != null)
                toKeybindButton.onClick.AddListener(ShowKeybindPage);
            if (backButton != null)
                backButton.onClick.AddListener(ShowMainPage);
        }

        private void OnEnable()
        {
            ShowMainPage();
        }

        public void ShowKeybindPage()
        {
            if (mainSettingsPage != null) mainSettingsPage.SetActive(false);
            if (keybindPage != null) keybindPage.SetActive(true);
        }

        public void ShowMainPage()
        {
            if (mainSettingsPage != null) mainSettingsPage.SetActive(true);
            if (keybindPage != null) keybindPage.SetActive(false);
        }
    }
}