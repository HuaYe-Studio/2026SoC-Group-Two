using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement
{
    [Header("图标")]
    public Image achievement_Icon;
    [Header("成就内容")]
    public string achievemnt_Content;
    [Header("成就解锁状况")]
    public bool isAchievementUnlocked;
}
