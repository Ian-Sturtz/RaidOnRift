using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Ore = 0,
    LandMine = 1,
    Mate = 2,
    Vanguard = 3,
    Navigator = 4,
    Gunner = 5,
    Royal1 = 6,
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

    public void destroyPiece()
    {
        Destroy(gameObject);
    }

    protected bool IsSquareOnBoard(int x, int y)
    {
        return (x >= 0 && x < 10 && y >= 0 && y < 10) ;
    }

    public int[,] GetValidMovesOre(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        GameObject currentSquare = tiles[currentX, currentY];

        // For all squares +/- 1 away from current position
        for (int x_change = -1; x_change < 2; x_change++)
        {
            for (int y_change = -1; y_change < 2; y_change++)
            {
                if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                {
                    moveAssessment[currentX + x_change, currentY + y_change] = 1;
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
