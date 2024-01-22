using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tactician : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
