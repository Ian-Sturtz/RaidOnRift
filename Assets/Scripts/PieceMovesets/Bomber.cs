using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles, bool bombInJail = false)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;
        bool up_right = true;
        bool up_left = true;
        bool down_right = true;
        bool down_left = true;

        for (int change = 1; (up || down || left || right || up_right || up_left || down_right || down_left) && change <= 2; change++)
        {
            if (up)
            {
                if (IsSquareOnBoard(currentX, currentY + change))
                {
                    moveAssessment[currentX, currentY + change] = 1;

                    Square possibleSquare = tiles[currentX, currentY + change].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        up = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX, currentY + change] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX, currentY + change] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX, currentY - change].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        down = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX, currentY - change] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX, currentY - change] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX + change, currentY].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        right = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX + change, currentY] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX + change, currentY] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX - change, currentY].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        left = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX - change, currentY] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX - change, currentY] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX + change, currentY + change].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        up_right = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX + change, currentY + change] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX + change, currentY + change] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX - change, currentY + change].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        up_left = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX - change, currentY + change] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX - change, currentY + change] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX + change, currentY - change].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        down_right = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX + change, currentY - change] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX + change, currentY - change] = -1;
                        }
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

                    Square possibleSquare = tiles[currentX - change, currentY - change].GetComponent<Square>();
                    if (possibleSquare.currentPiece != null)
                    {
                        down_left = false;
                        if (!bombInJail && possibleSquare.currentPiece.type != PieceType.LandMine)
                        {
                            moveAssessment[currentX - change, currentY - change] = -1;
                        }
                        else if (possibleSquare.currentPiece.type != PieceType.LandMine && !possibleSquare.currentPiece.hasOre && !hasCaptured)
                        {
                            moveAssessment[currentX - change, currentY - change] = -1;
                        }
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

    // Checks for legal squares to deploy a land mine to
    public int[,] DetectBombDeploy(GameObject[,] tiles)
    {
        Debug.Log("Checking for legal bomb deployments");
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        // Checks square to the left
        if(tiles[currentX -1, currentY].GetComponent<Square>().currentPiece == null)
        {
            moveAssessment[currentX - 1, currentY] = 6;
        }

        // Checks square to the right
        if (tiles[currentX + 1, currentY].GetComponent<Square>().currentPiece == null)
        {
            moveAssessment[currentX + 1, currentY] = 6;
        }

        // Checks square above
        if (tiles[currentX, currentY + 1].GetComponent<Square>().currentPiece == null)
        {
            moveAssessment[currentX, currentY + 1] = 6;
        }

        // Checks square below
        if (tiles[currentX, currentY - 1].GetComponent<Square>().currentPiece == null)
        {
            moveAssessment[currentX, currentY - 1] = 6;
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
