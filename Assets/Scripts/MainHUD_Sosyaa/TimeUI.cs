using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WorldTime;

public class TimeUI : MonoBehaviour
{
    private TMP_Text tMP_Text;
    private TimeManager timeManager;

    void Awake()
    {
        tMP_Text = GetComponent<TextMeshPro>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
    }

    void Update()
    {
        tMP_Text.text = timeManager.CurrentTime.Hour.ToString() + ":"
        + timeManager.CurrentTime.Minute.ToString() + ":"
        + timeManager.CurrentTime.Second.ToString();
    }
}
