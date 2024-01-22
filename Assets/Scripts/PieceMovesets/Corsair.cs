using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corsair : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Square possibleSquare = tiles[x, y].GetComponent<Square>();
                if (possibleSquare.currentPiece == null)
                {
                    moveAssessment[x, y] = 8;
                }
            }
        }

        // Upper Right
        if (IsSquareOnBoard(currentX + 1, currentY + 1))
        {
            moveAssessment[currentX + 1, currentY + 1] = 1;
        }

        // Upper Left
        if (IsSquareOnBoard(currentX - 1, currentY + 1))
        {
            moveAssessment[currentX - 1, currentY + 1] = 1;
        }

        // Lower Right
        if (IsSquareOnBoard(currentX + 1, currentY - 1))
        {
            moveAssessment[currentX + 1, currentY - 1] = 1;
        }

        // Lower Left
        if (IsSquareOnBoard(currentX - 1, currentY - 1))
        {
            moveAssessment[currentX - 1, currentY - 1] = 1;
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
