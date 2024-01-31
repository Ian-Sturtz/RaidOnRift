using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public static PieceManager instance;

    public int navyRoyal1;
    public int navyRoyal2;
    public int navyMate;
    public int navyQuartermaster;
    public int navyCannon;
    public int navyBomber;
    public int navyVanguard;
    public int navyNavigator;
    public int navyGunner;

    public int pirateRoyal1;
    public int pirateRoyal2;
    public int pirateMate;
    public int pirateQuartermaster;
    public int pirateCannon;
    public int pirateBomber;
    public int pirateVanguard;
    public int pirateNavigator;
    public int pirateGunner;

    public bool navyFirst;

    public Piece[] pieces = new Piece[60];

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
}
