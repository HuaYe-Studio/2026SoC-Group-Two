using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationInfo : MonoBehaviour
{
    public string stationName;
    public float arriveTime;
    public float stopTime;
    public int myX;
    public Sprite sprite;
    public Text checkText;
    private void Start()
    {
        checkText.enabled = false;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(myX, -120, 0);
        Image image=GetComponent<Image>();
        image.sprite = sprite;
    }
    StationInfo(string name, float arrtime, float stime,int myx, Sprite sp)
    {
        stationName = name;
        arriveTime = arrtime;
        stopTime = stime;
        myX = myx;
        checkText.text = $"{name}站\n到达此站还需要：{arrtime}\n停靠此站的时间：{stime}";
        sprite = sp;
    }
    private void OnMouseOver()
    {
        checkText.enabled=true;
    }
    private void OnMouseExit()
    {
        checkText.enabled=false;
    }
}
