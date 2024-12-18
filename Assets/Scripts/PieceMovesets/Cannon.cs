using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cannon : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;
        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        if (SceneManager.GetActiveScene().name == "Board")
        {
            // Movement
            for (int x_change = -1; x_change <= 1; x_change++)
                for (int y_change = -1; y_change <= 1; y_change++)
                    if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                        if (tiles[currentX + x_change, currentY + y_change].GetComponent<Square>().currentPiece == null)
                            moveAssessment[currentX + x_change, currentY + y_change] = 1;

            // Capturing
            for (int change = 1; up || down || left || right; change++)
            {
                // Searching for enemy pieces in the up direction
                if (up)
                {
                    if(IsSquareOnBoard(currentX, currentY + change))
                    {
                        Square possibleSquare = tiles[currentX, currentY + change].GetComponent<Square>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            up = false;

                            if (IsSquareOnBoard(currentX, currentY + change + 1))
                            {
                                Square possibleDestination = tiles[currentX, currentY + change + 1].GetComponent<Square>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX, currentY + change] = 4;
                                    moveAssessment[currentX, currentY + change + 1] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        up = false;
                    }
                }

                // Searching for enemy pieces in the down direction
                if (down)
                {
                    if (IsSquareOnBoard(currentX, currentY - change))
                    {
                        Square possibleSquare = tiles[currentX, currentY - change].GetComponent<Square>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            down = false;

                            if (IsSquareOnBoard(currentX, currentY - change - 1))
                            {
                                Square possibleDestination = tiles[currentX, currentY - change - 1].GetComponent<Square>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX, currentY - change] = 4;
                                    moveAssessment[currentX, currentY - change - 1] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        down = false;
                    }
                }

                // Searching for enemy pieces in the left direction
                if (left)
                {
                    if (IsSquareOnBoard(currentX - change, currentY))
                    {
                        Square possibleSquare = tiles[currentX - change, currentY].GetComponent<Square>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            left = false;

                            if (IsSquareOnBoard(currentX - change - 1, currentY))
                            {
                                Square possibleDestination = tiles[currentX - change - 1, currentY].GetComponent<Square>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX - change, currentY] = 4;
                                    moveAssessment[currentX - change - 1, currentY] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        left = false;
                    }
                }

                // Searching for enemy pieces in the right direction
                if (right)
                {
                    if (IsSquareOnBoard(currentX + change, currentY))
                    {
                        Square possibleSquare = tiles[currentX + change, currentY].GetComponent<Square>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            right = false;

                            if (IsSquareOnBoard(currentX + change + 1, currentY))
                            {
                                Square possibleDestination = tiles[currentX + change + 1, currentY].GetComponent<Square>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX + change, currentY] = 4;
                                    moveAssessment[currentX + change + 1, currentY] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        right = false;
                    }
                }
            }
        }
        // Tutorial scene
        else
        {
            // Movement
            for (int x_change = -1; x_change <= 1; x_change++)
                for (int y_change = -1; y_change <= 1; y_change++)
                    if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                        if (tiles[currentX + x_change, currentY + y_change].GetComponent<TTSquare>().currentPiece == null)
                            moveAssessment[currentX + x_change, currentY + y_change] = 1;

            // Capturing
            for (int change = 1; up || down || left || right; change++)
            {
                // Searching for enemy pieces in the up direction
                if (up)
                {
                    if (IsSquareOnBoard(currentX, currentY + change))
                    {
                        TTSquare possibleSquare = tiles[currentX, currentY + change].GetComponent<TTSquare>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            up = false;

                            if (IsSquareOnBoard(currentX, currentY + change + 1))
                            {
                                TTSquare possibleDestination = tiles[currentX, currentY + change + 1].GetComponent<TTSquare>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX, currentY + change] = 4;
                                    moveAssessment[currentX, currentY + change + 1] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        up = false;
                    }
                }

                // Searching for enemy pieces in the down direction
                if (down)
                {
                    if (IsSquareOnBoard(currentX, currentY - change))
                    {
                        TTSquare possibleSquare = tiles[currentX, currentY - change].GetComponent<TTSquare>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            down = false;

                            if (IsSquareOnBoard(currentX, currentY - change - 1))
                            {
                                TTSquare possibleDestination = tiles[currentX, currentY - change - 1].GetComponent<TTSquare>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX, currentY - change] = 4;
                                    moveAssessment[currentX, currentY - change - 1] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        down = false;
                    }
                }

                // Searching for enemy pieces in the left direction
                if (left)
                {
                    if (IsSquareOnBoard(currentX - change, currentY))
                    {
                        TTSquare possibleSquare = tiles[currentX - change, currentY].GetComponent<TTSquare>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            left = false;

                            if (IsSquareOnBoard(currentX - change - 1, currentY))
                            {
                                TTSquare possibleDestination = tiles[currentX - change - 1, currentY].GetComponent<TTSquare>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX - change, currentY] = 4;
                                    moveAssessment[currentX - change - 1, currentY] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        left = false;
                    }
                }

                // Searching for enemy pieces in the right direction
                if (right)
                {
                    if (IsSquareOnBoard(currentX + change, currentY))
                    {
                        TTSquare possibleSquare = tiles[currentX + change, currentY].GetComponent<TTSquare>();

                        // Piece has been found
                        if (possibleSquare.currentPiece != null)
                        {
                            right = false;

                            if (IsSquareOnBoard(currentX + change + 1, currentY))
                            {
                                TTSquare possibleDestination = tiles[currentX + change + 1, currentY].GetComponent<TTSquare>();

                                // There is an appropriate square to jump into
                                if (possibleDestination.currentPiece == null)
                                {
                                    moveAssessment[currentX + change, currentY] = 4;
                                    moveAssessment[currentX + change + 1, currentY] = 5;
                                }
                            }
                        }
                    }
                    else
                    {
                        right = false;
                    }
                }
            }
        }


        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
