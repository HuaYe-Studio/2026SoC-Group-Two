using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class HUDEmerge : MonoBehaviour
{
    public Canvas canvas;
    private CanvasGroup group;
    private PlayerInput input;
    private bool isIdle;
    private float idleTime;
    private void Start()
    {
        group = canvas.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        input = gameObject.GetComponent<PlayerInput>();
        idleTime = 0f;
        isIdle = false;
    }
    private void Update()
    {
        idleTime += Time.deltaTime;
        if(idleTime > 1.5f&&!isIdle)
        {
            isIdle=true;
            idleTime = 0f;
            StartCoroutine(HUDFadeIn());


        }
    }
    public void InterruptIdle()
    {
        idleTime = 0f;
        isIdle=false;
        group.alpha = 0f;
    }

    private void OnMouseEnter()
    {
        isIdle = true;
        StartCoroutine(HUDFadeIn());
        
    }
    private IEnumerator HUDFadeIn()
    {
        while (group.alpha < 1&&isIdle)
        {
            group.alpha += Time.deltaTime;
            yield return null;
        }
    }
}
