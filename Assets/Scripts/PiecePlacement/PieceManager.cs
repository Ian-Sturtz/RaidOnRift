using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PieceManager : MonoBehaviour
{
    public static PieceManager instance;
    public bool isActive = false;
    public bool onlineMultiplayer = false;
    public bool startingFromBoard = false;

    public int navyRoyal1;
    public int navyRoyal2;
    public int navyMate;
    public int navyQuartermaster;
    public int navyCannon;
    public int navyEngineer;
    public int navyVanguard;
    public int navyNavigator;
    public int navyGunner;

    public int pirateRoyal1;
    public int pirateRoyal2;
    public int pirateMate;
    public int pirateQuartermaster;
    public int pirateCannon;
    public int pirateEngineer;
    public int pirateVanguard;
    public int pirateNavigator;
    public int pirateGunner;

    public bool navyFirst;

    public PieceType[] pieceTypes = new PieceType[60];
    public bool[] factions = new bool[60];
    public int[,] pieceCoords = new int[60,2];
    public int totalPieces;

    private void Awake()
    {
        if (instance != null)
        {
            if(PieceManager.instance.startingFromBoard)
            {
                Destroy(instance.gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        instance = this;
        isActive = true;
        DontDestroyOnLoad(gameObject);
    }
}
