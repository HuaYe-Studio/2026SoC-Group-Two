using UnityEngine;
using UI;

namespace Settings
{
    public class SettingTest : MonoBehaviour
    {
        [SerializeField]private GameObject settingPanel;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                var panel = settingPanel.GetComponent<SettingUIPanels>();
                if (settingPanel.activeSelf) 
                    panel.CloseSettingPanel();
                else 
                    panel.OpenSettingPanel();
                
                Debug.Log("成功强行呼出设置界面");
            }
        }
    }
}