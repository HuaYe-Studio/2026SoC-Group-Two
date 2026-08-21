using System.Collections;
using UnityEngine;

public class ProfileBook : MonoBehaviour
{
    [Header("书籍模型")]
    public GameObject bookClosedModel;
    public GameObject bookOpenModel;

    [Header("飞出方向")]
    public float throwOutDuration = 0.6f;
    public Vector3 throwOutOffset = new Vector3(-5f, 2f, -3f);

    public int slotIndex = 0;
    public bool IsOpen { get; private set; } = false;
    public bool IsAnimating { get; private set; } = false;

    public System.Func<bool> GetGlobalAnimLock;

    private ProfileSelectionUI uiCoordinator;

    void Start()
    {
        if (uiCoordinator == null)
            uiCoordinator = GetComponentInParent<ProfileSelectionUI>();
        ApplyClosedState();
        IsOpen = false;
    }

    void Update()
    {
        if (IsAnimating) return;
        if (GetGlobalAnimLock != null && GetGlobalAnimLock()) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetHit(out _))
                OnMouseLeftClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (TryGetHit(out _))
                OnMouseRightClick();
        }
    }

    private bool TryGetHit(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            return hit.collider.gameObject == gameObject;
        return false;
    }

    private void OnMouseLeftClick()
    {
        if (ProfileManager.Instance == null) return;
        if (slotIndex == ProfileManager.Instance.ActiveSlotIndex) return;

        if (uiCoordinator != null)
        {
            uiCoordinator.StartProfileSwitch(slotIndex);
        }
        else
        {
            Debug.LogWarning("未找到UI选择，切换档位");
            ProfileManager.Instance.SwitchActiveProfile(slotIndex);
            OpenBook();
        }
    }

    private void OnMouseRightClick()
    {
        if (ProfileManager.Instance == null) return;

        ProfileData data = ProfileManager.Instance.GetProfile(slotIndex);
        if (data.isEmpty) return;

        if (uiCoordinator != null)
        {
            uiCoordinator.RequestDeleteProfile(slotIndex);
        }
        else
        {
            Debug.LogWarning("未找到UI选择，无法删除");
        }
    }

    public void OpenBook()
    {
        if (IsOpen) return;
        if (bookClosedModel != null) bookClosedModel.SetActive(false);
        if (bookOpenModel != null) bookOpenModel.SetActive(true);
        IsOpen = true;
    }

    public void CloseBook()
    {
        if (!IsOpen) return;
        if (bookClosedModel != null) bookClosedModel.SetActive(true);
        if (bookOpenModel != null) bookOpenModel.SetActive(false);
        IsOpen = false;
    }

    public IEnumerator ThrowOutLeft()
    {
        IsAnimating = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + throwOutOffset;
        Quaternion startRot = transform.rotation;
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
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.position = targetPos;
        transform.rotation = targetRot;

        IsOpen = false;
        IsAnimating = false;
        gameObject.SetActive(false);
    }

    public void SnapToClosed()
    {
        ApplyClosedState();
        IsOpen = false;
    }

    public void SnapToOpen()
    {
        if (bookClosedModel != null) bookClosedModel.SetActive(false);
        if (bookOpenModel != null) bookOpenModel.SetActive(true);
        IsOpen = true;
    }

    private void ApplyClosedState()
    {
        if (bookClosedModel != null) bookClosedModel.SetActive(true);
        if (bookOpenModel != null) bookOpenModel.SetActive(false);
    }

    public void SetUICoordinator(ProfileSelectionUI coordinator)
    {
        uiCoordinator = coordinator;
    }

    public void ResetToClosed()
    {
        IsOpen = false;
        IsAnimating = false;
        ApplyClosedState();
        gameObject.SetActive(true);
    }
}
