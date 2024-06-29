using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour, IDataPersistence
{
    public static StatManager instance;

    public int numBattles = 0;
    public int numOfflineBattles = 0;
    public int numOnlineBattles = 0;
    public int numWins = 0;
    public int numLosses = 0;
    public int numStalemates = 0;

    public bool completedTutorial = false;

    public string[] presetNames = new string[10];
    public int[] presetRoyal1 = new int[10];
    public int[] presetRoyal2 = new int[10];
    public int[] presetMate = new int[10];
    public int[] presetQuartermaster = new int[10];
    public int[] presetCannon = new int[10];
    public int[] presetEngineer = new int[10];
    public int[] presetVanguard = new int[10];
    public int[] presetNavigator = new int[10];
    public int[] presetGunner = new int[10];

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

    public void IncreaseNumOfflineBattles()
    {
        numOfflineBattles++;
    }

    public void IncreaseNumOnlineBattles()
    {
        numOnlineBattles++;
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
        numOfflineBattles = data.numOfflineBattles;
        numOnlineBattles = data.numOnlineBattles;
        numWins = data.numWins;
        numLosses = data.numLosses;
        numStalemates = data.numStalemates;

        completedTutorial = data.completedTutorial;

        presetNames = data.presetNames;
        presetRoyal1 = data.presetRoyal1;
        presetRoyal2 = data.presetRoyal2;
        presetMate = data.presetMate;
        presetQuartermaster = data.presetQuartermaster;
        presetCannon = data.presetCannon;
        presetEngineer = data.presetEngineer;
        presetVanguard = data.presetVanguard;
        presetNavigator = data.presetNavigator;
        presetGunner = data.presetGunner;
    }
    public void SaveData(ref GameData data)
    {
        data.numBattles = numBattles;
        data.numOfflineBattles = numOfflineBattles;
        data.numOnlineBattles = numOnlineBattles;
        data.numWins = numWins;
        data.numLosses = numLosses;
        data.numStalemates = numStalemates;

        data.completedTutorial = completedTutorial;

        data.presetNames = presetNames;
        data.presetRoyal1 = presetRoyal1;
        data.presetRoyal2 = presetRoyal2;
        data.presetMate = presetMate;
        data.presetQuartermaster = presetQuartermaster;
        data.presetCannon = presetCannon;
        data.presetEngineer = presetEngineer;
        data.presetVanguard = presetVanguard;
        data.presetNavigator = presetNavigator;
        data.presetGunner = presetGunner;
    }
}
