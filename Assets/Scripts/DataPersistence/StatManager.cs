using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour, IDataPersistence
{
    public static StatManager instance;

    public int numBattles = 0;
    public int numWins = 0;

    public bool completedTutorial = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncreaseNumBattles()
    {
        numBattles++;
    }

    public void IncreaseNumWins()
    {
        numWins++;
    }

    public void FinishedTutorial()
    {
        completedTutorial = true;
    }

    public void LoadData(GameData data)
    {
        numBattles = data.numBattles;
        numWins = data.numWins;
        completedTutorial = data.completedTutorial;
    }
    public void SaveData(ref GameData data)
    {
        data.numBattles = numBattles;
        data.numWins = numWins;
        data.completedTutorial = completedTutorial;
    }
}
