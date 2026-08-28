using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WorldTime;

public class TimeUI : MonoBehaviour
{
    private TMP_Text tMP_Text;

    void Awake()
    {
        tMP_Text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        if(TimeManager.Instance ==  null)
            Debug.LogWarning("TimeManager not found");
    }
    void Update()
    {
        tMP_Text.text = TimeManager.Instance.CurrentTime.Hour.ToString("00") + ":"
        + TimeManager.Instance.CurrentTime.Minute.ToString("00") ;
    }
}
