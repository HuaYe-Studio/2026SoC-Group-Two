using UnityEngine;

namespace Settings
{
    public class SettingTest : MonoBehaviour
    {
        [SerializeField]private GameObject settingPanel;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                settingPanel.SetActive(!settingPanel.activeSelf);
                
                Debug.Log("成功强行呼出设置界面");
            }
        }
    }
}