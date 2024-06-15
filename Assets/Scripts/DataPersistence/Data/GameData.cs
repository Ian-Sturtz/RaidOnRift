using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int numBattles;
    public int numWins;

    public bool completedTutorial;

    public GameData()
    { 
        this.numBattles = 0;
        this.numWins = 0;

        this.completedTutorial = false;
    }
}
