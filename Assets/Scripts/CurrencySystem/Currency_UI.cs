using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// 同对应要显示的对象的货币系统绑定即可
public class Currency_UI : MonoBehaviour
{
    private TMP_Text tMP_Text;
    [Header("要绑定的货币系统组件")]
    public CurrencySystem currencySystem;

    void Start()
    {
        tMP_Text = GetComponent<TMP_Text>();
        if (tMP_Text == null) Debug.LogError("UI控件缺少 TMP_Text 组件！");
    }

    void Update()
    {
        ShowCurrencyAccount();
    }

    #region 显示货币数量
    void ShowCurrencyAccount()
    {
        tMP_Text.text = currencySystem.GetCurrencyInTotal().ToString();
    }
    #endregion
}
