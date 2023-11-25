using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Ore = 0,
    LandMine = 1,
    Mate = 2,
}

public enum Team
{
    Pirate = 0,
    Navy = 1,
}

public class Piece : MonoBehaviour
{
    public Material piratePiece;
    public Material navyPiece;
    public Material NavyOre;
    public Material PirateOre;

    public PieceType type;
    public bool isNavy;
    public bool hasCaptured;
    public bool hasOre;
    public int currentX = -1;
    public int currentY = -1;

    private Vector3 desiredPosition;
    private Vector3 desiredScale;

    protected bool IsSquareOnBoard(int x, int y)
    {
        return (x >= 0 && x < 10 && y >= 0 && y < 10) ;
    }
}
