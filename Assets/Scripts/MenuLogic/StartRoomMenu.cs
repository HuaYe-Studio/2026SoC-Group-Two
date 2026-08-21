using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Random;

public class StartRoomMenu : MonoBehaviour
{
    [Header("场景配置")]
    public string stationSceneName = "02_Worlds/StationPlatform";
    [Header("列车物体")]
    public GameObject trainModel;
    [Header("捏人阶段标记")]
    public bool isCharacterCreateState = true;
    [Header("列车模型预制体")]
    public GameObject[] randomTrainPrefabs;
    public Transform trainSpawnPoint;

    void Start()
    {
        SceneManager.LoadSceneAsync(stationSceneName, LoadSceneMode.Additive);
        if (isCharacterCreateState && trainModel != null)
        {
            trainModel.SetActive(false);
        }
        else
        {
            SpawnRandomTrain();
        }
    }

    public void SpawnRandomTrain()
    {
        if (randomTrainPrefabs == null || randomTrainPrefabs.Length == 0 || trainSpawnPoint == null)
            return;

        int randomIndex = Random.Range(0, randomTrainPrefabs.Length);
        Instantiate(randomTrainPrefabs[randomIndex], trainSpawnPoint.position, trainSpawnPoint.rotation);
        Debug.Log("正在生成列车：" + randomTrainPrefabs[randomIndex].name);
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey("NightSaveTime");
        PlayerPrefs.DeleteKey("CurrentWorldIndex");
        PlayerPrefs.Save();
        SceneManager.LoadScene("02_Worlds/StartRoom");
    }

    public void OnCharacterCreateFinish()
    {
        isCharacterCreateState = false;
        if (trainModel != null) trainModel.SetActive(true);
        SpawnRandomTrain();
    }
}