using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TicketRecord
{
    public string completionTimeBinary;
    public int completedWorldIndex;
    public string worldName;

    public TicketRecord()
    {
        completionTimeBinary = DateTime.Now.ToBinary().ToString();
        completedWorldIndex = 0;
        worldName = "";
    }
}

[System.Serializable]
public class ProfileData
{
    [Header("档位索引")]
    public int slotIndex;

    [Header("基本信息")]
    public string profileName;
    public bool isEmpty;

    [Header("游戏进度")]
    public int currentWorldIndex;
    public string nightSaveTimeBinary;
    public string lastPlayedTimeBinary;

    [Header("收集品")]
    public List<string> unlockedCollectibleIds;

    [Header("角色数据")]
    public string characterAppearanceJson;

    [Header("通关车票")]
    public List<TicketRecord> ticketRecords;

    public ProfileData(int index)
    {
        slotIndex = index;
        isEmpty = true;
        profileName = "存档 " + (index + 1);
        currentWorldIndex = 0;
        nightSaveTimeBinary = "0";
        lastPlayedTimeBinary = DateTime.Now.ToBinary().ToString();
        unlockedCollectibleIds = new List<string>();
        ticketRecords = new List<TicketRecord>();
        characterAppearanceJson = "";
    }
}
