using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class giveup : MonoBehaviour
{
    [Header("车票")]
    public Transform ticketTransform;

    [Header("升起")]
    [Tooltip("上升高度")]
    public Vector3 raisedOffset = new Vector3(0, 0.3f, -0.2f);
    [Tooltip("升起/降下时长")]
    public float raiseDuration = 0.4f;

    [Header("飞出")]
    [Tooltip("飞出时长")]
    public float throwOutDuration = 0.6f;
    [Tooltip("向右飞出距离")]
    public Vector3 throwOutOffset = new Vector3(5f, 2f, -3f);

    [Header("黑屏")]
    [Tooltip("全屏黑色 Image")]
    public Image blackScreen;
    [Tooltip("黑屏渐入时长")]
    public float fadeDuration = 1.5f;
    [Tooltip("主菜单场景")]
    public string mainMenuSceneName = "MainMenu";

    private Vector3 restLocalPos;
    private Vector3 raisedLocalPos;
    private bool isRaised = false;
    private bool isAnimating = false;

    void Start()
    {
        if (ticketTransform == null)
            ticketTransform = transform;

        restLocalPos = ticketTransform.localPosition;
        raisedLocalPos = restLocalPos + raisedOffset;

        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
            blackScreen.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isAnimating) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetHit(out _))
            {
                if (!isRaised)
                    StartCoroutine(RaiseRoutine());
                else
                    StartCoroutine(LowerRoutine());
            }
            else if (isRaised)
            {
                StartCoroutine(LowerRoutine());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isRaised && TryGetHit(out _))
            {
                StartCoroutine(GiveUpRoutine());
            }
        }
    }

    private bool TryGetHit(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject;
    }

    private IEnumerator RaiseRoutine()
    {
        isAnimating = true;
        Vector3 startPos = ticketTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < raiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / raiseDuration;
            t = 1f - (1f - t) * (1f - t);
            ticketTransform.localPosition = Vector3.Lerp(startPos, raisedLocalPos, t);
            yield return null;
        }

        ticketTransform.localPosition = raisedLocalPos;
        isRaised = true;
        isAnimating = false;
    }

    private IEnumerator LowerRoutine()
    {
        isAnimating = true;
        Vector3 startPos = ticketTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < raiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / raiseDuration;
            t = t * t * (3f - 2f * t);
            ticketTransform.localPosition = Vector3.Lerp(startPos, restLocalPos, t);
            yield return null;
        }

        ticketTransform.localPosition = restLocalPos;
        isRaised = false;
        isAnimating = false;
    }

    private IEnumerator GiveUpRoutine()
    {
        isAnimating = true;

        Vector3 startPos = ticketTransform.position;
        Vector3 targetPos = startPos + throwOutOffset;
        Quaternion startRot = ticketTransform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(
            Random.Range(30f, 120f),
            Random.Range(-60f, 60f),
            Random.Range(-30f, 30f)
        );

        float elapsed = 0f;
        while (elapsed < throwOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / throwOutDuration;
            t = t * t;
            ticketTransform.position = Vector3.Lerp(startPos, targetPos, t);
            ticketTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        ticketTransform.position = targetPos;
        ticketTransform.rotation = targetRot;

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            Color c = blackScreen.color;
            elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                blackScreen.color = c;
                yield return null;
            }
        }

        Debug.Log("放弃游戏");
        GiveUp();
    }

    public void GiveUp()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
