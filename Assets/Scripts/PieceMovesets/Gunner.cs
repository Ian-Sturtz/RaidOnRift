using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;
        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;
        bool up_right = true;
        bool up_left = true;
        bool down_right = true;
        bool down_left = true;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        // Movement
        for (int x_change = -1; x_change <= 1; x_change++)
            for (int y_change = -1; y_change <= 1; y_change++)
                if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                    if(tiles[currentX + x_change, currentY + y_change].GetComponent<Square>().currentPiece == null)
                        moveAssessment[currentX + x_change, currentY + y_change] = 1;

        // Capturing
        if (!hasCaptured)
        {
            Square possibleSquare;

            // Iterates through all squares within attack range
            for(int x_change = 0; x_change <= 3; x_change++)
            {
                int y_change = x_change;
                if (x_change != 0) {
                    // Continues searching straight up for obstructions
                    if (up)
                    {
                        if (IsSquareOnBoard(currentX, currentY + y_change))
                        {
                            possibleSquare = tiles[currentX, currentY + y_change].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                up = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX, currentY + y_change] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching up-left for obstructions
                    if (up_left)
                    {
                        if (IsSquareOnBoard(currentX - x_change, currentY + y_change))
                        {
                            possibleSquare = tiles[currentX - x_change, currentY + y_change].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                up_left = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX - x_change, currentY + y_change] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching up-right for obstructions
                    if (up_right)
                    {
                        if (IsSquareOnBoard(currentX + x_change, currentY + y_change)){
                            possibleSquare = tiles[currentX + x_change, currentY + y_change].GetComponent<Square>();

                            // Obstruction found
                            if(possibleSquare.currentPiece != null)
                            {
                                up_right = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX + x_change, currentY + y_change] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching straight down for obstructions
                    if (down)
                    {
                        if (IsSquareOnBoard(currentX, currentY - y_change))
                        {
                            possibleSquare = tiles[currentX, currentY - y_change].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                down = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX, currentY - y_change] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching down-left for obstructions
                    if (down_left)
                    {
                        if (IsSquareOnBoard(currentX - x_change, currentY - y_change))
                        {
                            possibleSquare = tiles[currentX - x_change, currentY - y_change].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                down_left = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX - x_change, currentY - y_change] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching down-right for obstructions
                    if (down_right)
                    {
                        if (IsSquareOnBoard(currentX + x_change, currentY - y_change))
                        {
                            possibleSquare = tiles[currentX + x_change, currentY - y_change].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                down_right = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX + x_change, currentY - y_change] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching right for obstructions
                    if (right)
                    {
                        if (IsSquareOnBoard(currentX + x_change, currentY))
                        {
                            possibleSquare = tiles[currentX + x_change, currentY].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                right = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX + x_change, currentY] = 3;
                                    }
                                }
                            }
                        }
                    }

                    // Continues searching right for obstructions
                    if (left)
                    {
                        if (IsSquareOnBoard(currentX - x_change, currentY))
                        {
                            possibleSquare = tiles[currentX - x_change, currentY].GetComponent<Square>();

                            // Obstruction found
                            if (possibleSquare.currentPiece != null)
                            {
                                left = false; // Must stop searching in this direction

                                // Obstruction is an enemy piece
                                if (isNavy != possibleSquare.currentPiece.GetComponent<Piece>().isNavy)
                                {
                                    // Obstruction is not a land mine
                                    if (possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.LandMine && possibleSquare.currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                                    {
                                        moveAssessment[currentX - x_change, currentY] = 3;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
