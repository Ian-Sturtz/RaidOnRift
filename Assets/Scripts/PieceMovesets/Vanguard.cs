using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanguard : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        GameObject currentSquare = tiles[currentX, currentY];

        bool left = true;
        bool right = true;

        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                if (IsSquareOnBoard(currentX + i, currentY + j))
                    moveAssessment[currentX + i, currentY + j] = 1;

        for (int change = 1; right || left; change++)
        {
            if (right)
            {
                if (IsSquareOnBoard(currentX + change, currentY))
                {
                    moveAssessment[currentX + change, currentY] = 1;

                    if (tiles[currentX + change, currentY].GetComponent<Square>().currentPiece != null)
                    {
                        right = false;
                    }
                }
                else
                {
                    right = false;
                }
            }

            if (left)
            {
                if (IsSquareOnBoard(currentX - change, currentY))
                {
                    moveAssessment[currentX - change, currentY] = 1;

                    if (tiles[currentX - change, currentY].GetComponent<Square>().currentPiece != null)
                    {
                        left = false;
                    }
                }
                else
                {
                    left = false;
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
