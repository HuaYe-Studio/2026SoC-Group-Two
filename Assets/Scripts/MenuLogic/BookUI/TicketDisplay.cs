using System.Collections;
using UnityEngine;

public class TicketDisplay : MonoBehaviour
{
    [Tooltip("车票移动的 Transform")]
    public Transform ticketTransform;
    [Tooltip("拿起时上升的偏移量、")]
    public Vector3 raisedOffset = new Vector3(0, 0.3f, -0.2f);
    [Tooltip("升降时长")]
    public float animationDuration = 0.4f;

    [Header("数据")]
    public TicketRecord ticketData;

    public bool IsRaised { get; private set; } = false;

    public bool IsAnimating { get; private set; } = false;

    private Vector3 restLocalPos;
    private Vector3 raisedLocalPos;

    private static TicketDisplay currentlyRaised;

    public System.Func<bool> GetGlobalAnimLock;

    void Start()
    {
        if (ticketTransform == null)
            ticketTransform = transform;

        restLocalPos = ticketTransform.localPosition;
        raisedLocalPos = restLocalPos + raisedOffset;
    }

    void Update()
    {
        if (IsAnimating) return;

        if (GetGlobalAnimLock != null && GetGlobalAnimLock())
            return;

        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitThis = Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject;

        if (hitThis && !IsRaised)
        {

            if (currentlyRaised != null && currentlyRaised != this)
                StartCoroutine(currentlyRaised.LowerRoutine());
            StartCoroutine(RaiseRoutine());
        }
        else if (hitThis && IsRaised)
        {

            StartCoroutine(LowerRoutine());
        }
        else if (!hitThis && IsRaised)
        {

            StartCoroutine(LowerRoutine());
        }
    }

    private IEnumerator RaiseRoutine()
    {
        IsAnimating = true;

        Vector3 startPos = ticketTransform.localPosition;
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            t = 1f - (1f - t) * (1f - t);
            ticketTransform.localPosition = Vector3.Lerp(startPos, raisedLocalPos, t);
            yield return null;
        }
        ticketTransform.localPosition = raisedLocalPos;

        IsRaised = true;
        currentlyRaised = this;
        IsAnimating = false;
    }

    public IEnumerator LowerRoutine()
    {
        IsAnimating = true;

        Vector3 startPos = ticketTransform.localPosition;
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            t = t * t * (3f - 2f * t);
            ticketTransform.localPosition = Vector3.Lerp(startPos, restLocalPos, t);
            yield return null;
        }
        ticketTransform.localPosition = restLocalPos;

        IsRaised = false;
        if (currentlyRaised == this)
            currentlyRaised = null;
        IsAnimating = false;
    }

    public void SetGlobalAnimLock(System.Func<bool> lockFunc)
    {
        GetGlobalAnimLock = lockFunc;
    }

    public void SnapToRest()
    {
        ticketTransform.localPosition = restLocalPos;
        IsRaised = false;
        IsAnimating = false;
        if (currentlyRaised == this)
            currentlyRaised = null;
    }
}
