using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuNavigator : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        GameObject currentSquare = tiles[currentX, currentY];

        bool up = true;
        bool down = true;

        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                if (IsSquareOnBoard(currentX + i, currentY + j))
                    moveAssessment[currentX + i, currentY + j] = 1;

        for (int change = 1; up || down; change++)
        {
            if (up)
            {
                if (IsSquareOnBoard(currentX, currentY + change))
                {
                    moveAssessment[currentX, currentY + change] = 1;

                    if (tiles[currentX, currentY + change].GetComponent<TTSquare>().currentPiece != null)
                    {
                        up = false;
                    }
                }
                else
                {
                    up = false;
                }
            }

            if (down)
            {
                if (IsSquareOnBoard(currentX, currentY - change))
                {
                    moveAssessment[currentX, currentY - change] = 1;

                    if (tiles[currentX, currentY - change].GetComponent<TTSquare>().currentPiece != null)
                    {
                        down = false;
                    }
                }
                else
                {
                    down = false;
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
