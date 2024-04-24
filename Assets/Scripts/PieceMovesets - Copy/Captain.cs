using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                moveAssessment[i, j] = -1;

        int x = currentX;
        int y = currentY;

        for (int x_change = 0; x_change <= 5; x_change++)
        {
            for (int y_change = 0; y_change <= 5; y_change++)
            {
                if(x_change + y_change <= 5)
                {
                    if((x_change + y_change) % 2 != 0)
                    {
                        if (IsSquareOnBoard(x+x_change, y + y_change)){
                            moveAssessment[x + x_change, y + y_change] = 1;
                        }

                        if (IsSquareOnBoard(x + x_change, y - y_change)){
                            moveAssessment[x + x_change, y - y_change] = 1;
                        }

                        if (IsSquareOnBoard(x - x_change, y + y_change)){
                            moveAssessment[x - x_change, y + y_change] = 1;
                        }

                        if (IsSquareOnBoard(x - x_change, y - y_change)){
                            moveAssessment[x - x_change, y - y_change] = 1;
                        }
                    }
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
