using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Admiral : Piece
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
        bool left = true;
        bool right = true;
        bool up_right = true;
        bool up_left = true;
        bool down_right = true;
        bool down_left = true;

        for (int change = 1; up || down || left || right || up_right || up_left || down_right || down_left; change++)
        {
            if (up)
            {
                if (IsSquareOnBoard(currentX, currentY + change))
                {
                    moveAssessment[currentX, currentY + change] = 1;

                    if (tiles[currentX, currentY + change].GetComponent<Square>().currentPiece != null)
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

                    if (tiles[currentX, currentY - change].GetComponent<Square>().currentPiece != null)
                    {
                        down = false;
                    }
                }
                else
                {
                    down = false;
                }
            }

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

            if (up_right)
            {
                if (IsSquareOnBoard(currentX + change, currentY + change))
                {
                    moveAssessment[currentX + change, currentY + change] = 1;

                    if (tiles[currentX + change, currentY + change].GetComponent<Square>().currentPiece != null)
                    {
                        up_right = false;
                    }
                }
                else
                {
                    up_right = false;
                }
            }

            if (up_left)
            {
                if (IsSquareOnBoard(currentX - change, currentY + change))
                {
                    moveAssessment[currentX - change, currentY + change] = 1;

                    if (tiles[currentX - change, currentY + change].GetComponent<Square>().currentPiece != null)
                    {
                        up_left = false;
                    }
                }
                else
                {
                    up_left = false;
                }
            }

            if (down_right)
            {
                if (IsSquareOnBoard(currentX + change, currentY - change))
                {
                    moveAssessment[currentX + change, currentY - change] = 1;

                    if (tiles[currentX + change, currentY - change].GetComponent<Square>().currentPiece != null)
                    {
                        down_right = false;
                    }
                }
                else
                {
                    down_right = false;
                }
            }

            if (down_left)
            {
                if (IsSquareOnBoard(currentX - change, currentY - change))
                {
                    moveAssessment[currentX - change, currentY - change] = 1;

                    if (tiles[currentX - change, currentY - change].GetComponent<Square>().currentPiece != null)
                    {
                        down_left = false;
                    }
                }
                else
                {
                    down_left = false;
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
