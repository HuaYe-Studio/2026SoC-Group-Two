using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllProfilesFromPrefs();
    }

    public event Action<int> OnActiveProfileChanged;

    public event Action<ProfileData> OnProfileSaved;

    public event Action<int> OnProfileDeleted;



    private const string PROFILE_KEY_PREFIX = "ProfileData_";
    private const string ACTIVE_SLOT_KEY = "ActiveProfileSlot";
    private const int MAX_SLOTS = 3;

    private ProfileData[] profiles = new ProfileData[MAX_SLOTS];
    private int activeSlotIndex = 0;

    public ProfileData ActiveProfile
    {
        get
        {
            if (profiles[activeSlotIndex] == null)
                profiles[activeSlotIndex] = new ProfileData(activeSlotIndex);
            return profiles[activeSlotIndex];
        }
    }

    public int ActiveSlotIndex => activeSlotIndex;

    public ProfileData GetProfile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
        {
            Debug.LogError("档位索引无效: " + slotIndex);
            return null;
        }
        if (profiles[slotIndex] == null)
            profiles[slotIndex] = new ProfileData(slotIndex);
        return profiles[slotIndex];
    }

    public List<ProfileData> GetAllProfiles()
    {
        List<ProfileData> list = new List<ProfileData>();
        for (int i = 0; i < MAX_SLOTS; i++)
            list.Add(GetProfile(i));
        return list;
    }



    private void LoadAllProfilesFromPrefs()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            profiles[i] = LoadProfileFromPrefs(i);
        }

        activeSlotIndex = PlayerPrefs.GetInt(ACTIVE_SLOT_KEY, 0);
        if (activeSlotIndex < 0 || activeSlotIndex >= MAX_SLOTS)
            activeSlotIndex = 0;

        Debug.Log("加载完成: " + activeSlotIndex);
    }

    private ProfileData LoadProfileFromPrefs(int slotIndex)
    {
        string key = PROFILE_KEY_PREFIX + slotIndex;
        if (!PlayerPrefs.HasKey(key))
            return new ProfileData(slotIndex);

        string json = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(json))
            return new ProfileData(slotIndex);

        try
        {
            ProfileData data = JsonUtility.FromJson<ProfileData>(json);
            if (data == null)
                return new ProfileData(slotIndex);

            data.slotIndex = slotIndex;
            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning("档位 " + slotIndex + " 数据损坏，已重置: " + e.Message);
            return new ProfileData(slotIndex);
        }
    }

    public void SaveProfile(ProfileData data)
    {
        if (data == null) return;

        string key = PROFILE_KEY_PREFIX + data.slotIndex;

        data.lastPlayedTimeBinary = DateTime.Now.ToBinary().ToString();
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();

        OnProfileSaved?.Invoke(data);
    }

    public void SaveActiveProfile()
    {
        SaveProfile(ActiveProfile);
    }



    public void SwitchActiveProfile(int targetSlotIndex)
    {
        if (targetSlotIndex < 0 || targetSlotIndex >= MAX_SLOTS)
        {
            Debug.LogError("目标不存在: " + targetSlotIndex);
            return;
        }

        if (targetSlotIndex == activeSlotIndex)
            return;

        activeSlotIndex = targetSlotIndex;
        PlayerPrefs.SetInt(ACTIVE_SLOT_KEY, activeSlotIndex);
        PlayerPrefs.Save();

        if (ActiveProfile.isEmpty)
        {
            CreateNewProfile(activeSlotIndex);
        }

        Debug.Log("切换到存档: " + activeSlotIndex);
        OnActiveProfileChanged?.Invoke(activeSlotIndex);
    }

    public void CreateNewProfile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
            return;

        ProfileData newData = new ProfileData(slotIndex);
        newData.isEmpty = false;
        newData.profileName = "存档 " + (slotIndex + 1);
        newData.currentWorldIndex = 0;
        newData.nightSaveTimeBinary = "0";
        newData.unlockedCollectibleIds = new List<string>();

        profiles[slotIndex] = newData;
        SaveProfile(newData);

        Debug.Log("新建存档: 档位 " + slotIndex);
    }

    public void DeleteProfile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
            return;

        profiles[slotIndex] = new ProfileData(slotIndex);
        PlayerPrefs.SetString(PROFILE_KEY_PREFIX + slotIndex, JsonUtility.ToJson(profiles[slotIndex]));
        PlayerPrefs.Save();

        OnProfileDeleted?.Invoke(slotIndex);

        if (slotIndex == activeSlotIndex)
        {
            int newActive = AutoSelectNewActiveSlot();
            activeSlotIndex = newActive;
            PlayerPrefs.SetInt(ACTIVE_SLOT_KEY, activeSlotIndex);
            PlayerPrefs.Save();

            if (ActiveProfile.isEmpty)
            {
                CreateNewProfile(activeSlotIndex);
            }

            OnActiveProfileChanged?.Invoke(activeSlotIndex);
        }

        Debug.Log("删除存档: 档位 " + slotIndex + ", 选择存档: " + activeSlotIndex);
    }

    private int AutoSelectNewActiveSlot()
    {

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (!profiles[i].isEmpty)
                return i;
        }

        return 0;
    }



    public void SyncActiveProfileProgress()
    {

        if (PlayerPrefs.HasKey("CurrentWorldIndex"))
        {
            ActiveProfile.currentWorldIndex = PlayerPrefs.GetInt("CurrentWorldIndex");
        }

        if (PlayerPrefs.HasKey("NightSaveTime"))
        {
            ActiveProfile.nightSaveTimeBinary = PlayerPrefs.GetString("NightSaveTime");
        }

        SaveActiveProfile();
        Debug.Log("进度同步完成: 档位 " + activeSlotIndex);
    }

    public TicketRecord AddTicketToActiveProfile(int worldIndex, string worldName = "")
    {
        if (ActiveProfile.ticketRecords == null)
            ActiveProfile.ticketRecords = new List<TicketRecord>();

        TicketRecord record = new TicketRecord
        {
            completedWorldIndex = worldIndex,
            worldName = worldName
        };
        ActiveProfile.ticketRecords.Add(record);
        SaveActiveProfile();
        Debug.Log("添加车票: 档位 " + activeSlotIndex + " 世界 " + worldIndex);
        return record;
    }
}
