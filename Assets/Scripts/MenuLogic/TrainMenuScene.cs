using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System;

public class TrainMenuScene : MonoBehaviour
{
    [Header("相机动画")]
    public Transform mainCamera;
    public Transform playerTargetPoint;
    public float cameraMoveSpeed = 8f;
    [Header("UI面板")]
    public GameObject gameMainUI;
    [Header("列车远景场景列表")]
    public string[] worldBackgroundScenes;

    private string saveWorldIndexKey = "CurrentWorldIndex";
    private bool cameraMoveComplete = false;

    void Start()
    {
        LoadSaveWorldBackground();
        Invoke(nameof(LoadTrainCoreScene), 0.5f);
    }

    void Update()
    {
        if (!cameraMoveComplete && mainCamera != null && playerTargetPoint != null)
        {
            mainCamera.position = Vector3.Lerp(mainCamera.position, playerTargetPoint.position, Time.deltaTime * cameraMoveSpeed);
            mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, playerTargetPoint.rotation, Time.deltaTime * cameraMoveSpeed);
            if (Vector3.Distance(mainCamera.position, playerTargetPoint.position) < 0.1f)
            {
                cameraMoveComplete = true;
                gameMainUI.SetActive(true);
            }
        }
    }

    void LoadSaveWorldBackground()
    {
        int worldIdx = PlayerPrefs.GetInt(saveWorldIndexKey, 0);
        if (worldIdx >= 0 && worldIdx < worldBackgroundScenes.Length)
        {
            SceneManager.LoadSceneAsync(worldBackgroundScenes[worldIdx], LoadSceneMode.Additive);
        }
    }

    void LoadTrainCoreScene()
    {
        SceneManager.LoadSceneAsync("02_Worlds/Train", LoadSceneMode.Additive);
    }

    public void SaveCurrentWorldIndex(int index)
    {
        PlayerPrefs.SetInt(saveWorldIndexKey, index);
        PlayerPrefs.Save();
    }
}
