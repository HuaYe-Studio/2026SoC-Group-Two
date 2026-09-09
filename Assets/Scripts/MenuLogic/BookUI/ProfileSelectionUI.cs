using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSelectionUI : MonoBehaviour
{
    [Header("三本书")]
    public ProfileBook[] profileBooks = new ProfileBook[3];

    [Header("收集品")]
    public Transform collectibleParent;
    public GameObject[] collectiblePrefabs;
    public float collectibleSpacing = 0.4f;

    [Header("新书生成")]
    public GameObject freshBookPrefab;
    public Transform bookSpawnParent;
    public float newBookLandDuration = 0.5f;

    [Header("车票")]
    public GameObject ticketPrefab;
    public Transform ticketSpawnParent;
    public float ticketSpacing = 0.5f;
    public Vector3 ticketStartOffset = new Vector3(-0.5f, 0, 0);

    public bool IsAnimating { get; private set; } = false;

    private List<GameObject> activeCollectibles = new List<GameObject>();
    private List<GameObject> activeTickets = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < profileBooks.Length; i++)
        {
            if (profileBooks[i] != null)
            {
                profileBooks[i].slotIndex = i;
                profileBooks[i].SetUICoordinator(this);
            }
        }
        StartCoroutine(InitializeScene());
    }

    private IEnumerator InitializeScene()
    {
        IsAnimating = true;
        yield return null;

        int activeSlot = ProfileManager.Instance.ActiveSlotIndex;

        for (int i = 0; i < profileBooks.Length; i++)
        {
            if (profileBooks[i] != null)
                profileBooks[i].SnapToClosed();
        }

        if (activeSlot >= 0 && activeSlot < profileBooks.Length && profileBooks[activeSlot] != null)
            profileBooks[activeSlot].SnapToOpen();

        RefreshCollectiblesInstant(activeSlot);
        RefreshTickets(activeSlot);

        IsAnimating = false;
    }

    public void StartProfileSwitch(int targetSlot)
    {
        if (IsAnimating) return;
        int fromSlot = ProfileManager.Instance.ActiveSlotIndex;
        if (fromSlot == targetSlot) return;
        StartCoroutine(SwitchProfileRoutine(fromSlot, targetSlot));
    }

    public void RequestDeleteProfile(int targetSlot)
    {
        if (IsAnimating) return;
        StartCoroutine(DeleteProfileRoutine(targetSlot));
    }

    private IEnumerator SwitchProfileRoutine(int fromSlot, int toSlot)
    {
        IsAnimating = true;
        Debug.Log("开始切换: 档位 " + fromSlot + " → " + toSlot);

        ClearCollectibles();

        if (fromSlot >= 0 && fromSlot < profileBooks.Length && profileBooks[fromSlot] != null)
            profileBooks[fromSlot].CloseBook();

        yield return new WaitForSeconds(0.2f);

        if (toSlot >= 0 && toSlot < profileBooks.Length && profileBooks[toSlot] != null)
        {
            ProfileData targetData = ProfileManager.Instance.GetProfile(toSlot);
            if (targetData.isEmpty)
                ProfileManager.Instance.CreateNewProfile(toSlot);
            profileBooks[toSlot].OpenBook();
        }

        RefreshCollectiblesInstant(toSlot);
        RefreshTickets(toSlot);

        ProfileManager.Instance.SwitchActiveProfile(toSlot);

        IsAnimating = false;
        Debug.Log("切换: 档位 " + toSlot);
    }

    private IEnumerator DeleteProfileRoutine(int targetSlot)
    {
        IsAnimating = true;
        int activeSlot = ProfileManager.Instance.ActiveSlotIndex;
        bool isDeletingActive = (targetSlot == activeSlot);
        Debug.Log("删除: 档位 " + targetSlot + (isDeletingActive ? " (当前活跃档)" : ""));

        if (isDeletingActive)
        {
            ClearCollectibles();
            if (profileBooks[targetSlot] != null)
                profileBooks[targetSlot].CloseBook();
        }

        ProfileBook targetBook = profileBooks[targetSlot];
        if (targetBook != null)
            yield return targetBook.ThrowOutLeft();

        yield return new WaitForSeconds(0.3f);

        ProfileManager.Instance.DeleteProfile(targetSlot);

        yield return SpawnNewBook(targetSlot);

        int newActiveSlot = ProfileManager.Instance.ActiveSlotIndex;

        if (isDeletingActive)
        {
            if (newActiveSlot >= 0 && newActiveSlot < profileBooks.Length && profileBooks[newActiveSlot] != null)
                profileBooks[newActiveSlot].OpenBook();

            RefreshCollectiblesInstant(newActiveSlot);
            RefreshTickets(newActiveSlot);
        }

        IsAnimating = false;
        Debug.Log("删除成功: 档位 " + targetSlot + " → 新活跃档: " + newActiveSlot);
    }

    private void RefreshCollectiblesInstant(int slotIndex)
    {
        ClearCollectibles();

        ProfileData data = ProfileManager.Instance.GetProfile(slotIndex);
        if (data == null || data.unlockedCollectibleIds == null || data.unlockedCollectibleIds.Count == 0)
            return;

        Vector3 basePos = collectibleParent != null ? collectibleParent.position : Vector3.zero;

        for (int i = 0; i < data.unlockedCollectibleIds.Count; i++)
        {
            GameObject prefab = FindCollectiblePrefab(data.unlockedCollectibleIds[i]);
            if (prefab == null) continue;

            Vector3 spawnPos = basePos + Vector3.right * (i - (data.unlockedCollectibleIds.Count - 1) * 0.5f) * collectibleSpacing;
            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity,
                collectibleParent != null ? collectibleParent : transform);
            item.name = "Collectible_" + data.unlockedCollectibleIds[i];
            activeCollectibles.Add(item);
        }
    }

    private void ClearCollectibles()
    {
        foreach (GameObject obj in activeCollectibles)
        {
            if (obj != null)
                Destroy(obj);
        }
        activeCollectibles.Clear();
    }

    private GameObject FindCollectiblePrefab(string id)
    {
        foreach (GameObject prefab in collectiblePrefabs)
        {
            if (prefab != null && prefab.name == id)
                return prefab;
        }
        Debug.LogWarning("未找到收集品: " + id);
        return null;
    }

    private void RefreshTickets(int slotIndex)
    {
        ClearTickets();

        if (ticketPrefab == null) return;

        ProfileData data = ProfileManager.Instance.GetProfile(slotIndex);
        if (data == null || data.ticketRecords == null || data.ticketRecords.Count == 0)
            return;

        Transform parent = ticketSpawnParent != null ? ticketSpawnParent : transform;
        Vector3 basePos = parent.position;

        for (int i = 0; i < data.ticketRecords.Count; i++)
        {
            Vector3 spawnPos = basePos + ticketStartOffset + Vector3.right * i * ticketSpacing;
            GameObject ticketObj = Instantiate(ticketPrefab, spawnPos, Quaternion.identity, parent);
            ticketObj.name = "Ticket_" + i;

            TicketDisplay display = ticketObj.GetComponent<TicketDisplay>();
            if (display == null)
                display = ticketObj.AddComponent<TicketDisplay>();

            display.ticketData = data.ticketRecords[i];
            display.SetGlobalAnimLock(() => IsAnimating);

            activeTickets.Add(ticketObj);
        }
    }

    private void ClearTickets()
    {
        foreach (GameObject obj in activeTickets)
        {
            if (obj != null)
                Destroy(obj);
        }
        activeTickets.Clear();
    }

    private IEnumerator SpawnNewBook(int slotIndex)
    {
        if (freshBookPrefab == null)
        {
            if (slotIndex < profileBooks.Length && profileBooks[slotIndex] != null)
                profileBooks[slotIndex].ResetToClosed();
            yield break;
        }

        Transform parent = bookSpawnParent != null ? bookSpawnParent : transform;

        Vector3 targetPos = parent.position;
        ProfileBook oldBook = profileBooks[slotIndex];
        if (oldBook != null)
            targetPos = oldBook.transform.position;

        Vector3 spawnPos = targetPos + Vector3.up * 0.5f;
        GameObject newBookObj = Instantiate(freshBookPrefab, spawnPos, Quaternion.identity, parent);
        ProfileBook newBook = newBookObj.GetComponent<ProfileBook>();

        if (newBook == null)
            newBook = newBookObj.AddComponent<ProfileBook>();

        newBook.slotIndex = slotIndex;
        newBook.SetUICoordinator(this);
        profileBooks[slotIndex] = newBook;

        float elapsed = 0f;
        while (elapsed < newBookLandDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / newBookLandDuration;
            t = 1f - (1f - t) * (1f - t);
            newBookObj.transform.position = Vector3.Lerp(spawnPos, targetPos, t);
            yield return null;
        }
        newBookObj.transform.position = targetPos;
    }
}
