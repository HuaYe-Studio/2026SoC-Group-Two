using UnityEngine;
using TMPro;

public class alarm : MonoBehaviour
{
    [Header("Time Display")]
    [Tooltip("显示时间")]
    [SerializeField] private TextMeshPro timeDisplay;

    [Header("Time Settings")]
    [Tooltip("点击前时间")]
    [SerializeField] private string beforeClickTime = "5:59";
    [Tooltip("点击后时间")]
    [SerializeField] private string afterClickTime = "6:00";

    private bool hasTriggered = false;

    void Start()
    {
        if (timeDisplay != null)
        {
            timeDisplay.text = beforeClickTime;
        }
    }

    void OnMouseDown()
    {
        if (hasTriggered) return;

        hasTriggered = true;

        if (timeDisplay != null)
        {
            timeDisplay.text = afterClickTime;
        }

        Time.timeScale = 1f;

        Debug.Log("游戏继续");
    }
}
