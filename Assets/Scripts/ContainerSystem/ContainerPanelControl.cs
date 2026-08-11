using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BackpackKeyboradInput : MonoBehaviour
{
    [Header("背包面板画布")]
    public Canvas backpackPanel;
    [Header("背包面板召唤/隐藏按键")]
    public KeyCode backpack_KeyCode;
    private bool isBackpackOpened = false;

    void Start()
    {
        isBackpackOpened = false;
    }

    void Update()
    {
        Open_Close_Backpack();
    }

    #region 背包按键输入控制
    void Open_Close_Backpack()
    {
        if (Input.GetKeyDown(backpack_KeyCode))
        {
            isBackpackOpened = !isBackpackOpened;

            if (isBackpackOpened)
            {
                backpackPanel.gameObject.SetActive(true);
            }
            else
            {
                backpackPanel.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region 关闭按钮事件
    public void onCloseBtnClick()
    {
        isBackpackOpened = false;
        backpackPanel.gameObject.SetActive(false);
    }
    #endregion
}
