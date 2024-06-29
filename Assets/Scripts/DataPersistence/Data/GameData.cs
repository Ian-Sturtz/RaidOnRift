using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int numBattles;
    public int numOfflineBattles;
    public int numOnlineBattles;
    public int numWins;
    public int numLosses;
    public int numStalemates;

    public bool completedTutorial;

    public string[] presetNames;
    public int[] presetRoyal1;
    public int[] presetRoyal2;
    public int[] presetMate;
    public int[] presetQuartermaster;
    public int[] presetCannon;
    public int[] presetEngineer;
    public int[] presetVanguard;
    public int[] presetNavigator;
    public int[] presetGunner;

    public GameData()
    { 
        this.numBattles = 0;
        this.numOfflineBattles = 0;
        this.numOnlineBattles = 0;
        this.numWins = 0;
        this.numLosses = 0;
        this.numStalemates = 0;

        this.completedTutorial = false;

        this.presetNames = new string[10];
        this.presetRoyal1 = new int[10];
        this.presetRoyal2 = new int[10];
        this.presetMate = new int[10];
        this.presetQuartermaster = new int[10];
        this.presetCannon = new int[10];
        this.presetEngineer = new int[10];
        this.presetVanguard = new int[10];
        this.presetNavigator = new int[10];
        this.presetGunner = new int[10];
    }
}
