using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuQuartermaster : TTPiece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        // Upper Right direction
        if (IsSquareOnBoard(currentX + 2, currentY + 1))
            moveAssessment[currentX + 2, currentY + 1] = 1;
        
        if (IsSquareOnBoard(currentX + 1, currentY + 2))
            moveAssessment[currentX + 1, currentY + 2] = 1;

        // Upper Left direction
        if (IsSquareOnBoard(currentX - 2, currentY + 1))
            moveAssessment[currentX - 2, currentY + 1] = 1;

        if (IsSquareOnBoard(currentX - 1, currentY + 2))
            moveAssessment[currentX - 1, currentY + 2] = 1;

        // Lower Right direction
        if (IsSquareOnBoard(currentX + 2, currentY - 1))
            moveAssessment[currentX + 2, currentY - 1] = 1;

        if (IsSquareOnBoard(currentX + 1, currentY - 2))
            moveAssessment[currentX + 1, currentY - 2] = 1;

        // Lower Left direction
        if (IsSquareOnBoard(currentX - 2, currentY - 1))
            moveAssessment[currentX - 2, currentY - 1] = 1;

        if (IsSquareOnBoard(currentX - 1, currentY - 2))
            moveAssessment[currentX - 1, currentY - 2] = 1;

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
