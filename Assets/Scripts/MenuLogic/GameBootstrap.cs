using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameBootstrap : MonoBehaviour
{
    private const string NewGameScenePath = "02_Worlds/StartRoom";
    private const string SaveGameScenePath = "02_Worlds/TrainMenu";

    void Start()
    {
        string saveTimeStr = PlayerPrefs.GetString("NightSaveTime", "0");
        long saveTimeStamp = long.Parse(saveTimeStr);
        bool useTrainStart = false;

        if (saveTimeStamp > 0)
        {
            System.DateTime saveTime = System.DateTime.FromBinary(saveTimeStamp);
            double saveDays = (System.DateTime.Now - saveTime).TotalDays;
            if (saveDays <= 7)
            {
                useTrainStart = true;
            }
        }

        if (useTrainStart)
        {
            SceneManager.LoadScene(SaveGameScenePath);
        }
        else
        {
            SceneManager.LoadScene(NewGameScenePath);
        }
    }
}