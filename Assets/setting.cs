using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setting : MonoBehaviour
{
    public GameObject settingPanel;
    void Start()
    {
        if(settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }
    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                if(hit.collider.gameObject == this.gameObject)
                {
                    settingPanel.SetActive(true);
                }
            }
        }
    }
}
