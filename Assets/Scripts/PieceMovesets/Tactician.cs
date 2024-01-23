using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tactician : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        int interactableZone = 0;

        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;


        bool mate = false;
        bool bomber = false;
        bool vanguard = false;
        bool navigator = false;
        bool gunner = false;
        bool cannon = false;
        bool quartermaster = false;
        bool corsair = false;
        bool captain = false;

        moveAssessment = new int[10, 10];

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                moveAssessment[i, j] = -1;

        // Tactician's default moveset
        for(int i = 1; i <= 2; i++)
        {
            Square possibleSquare;

            // Right
            if (IsSquareOnBoard(currentX + i, currentY) && right)
            {
                possibleSquare = tiles[currentX + i, currentY].GetComponent<Square>();

                if(possibleSquare.currentPiece != null)
                {
                    if(!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != PieceType.LandMine)
                    {
                        moveAssessment[currentX + i, currentY] = 2;
                        right = false;
                    }
                    else
                    {
                        right = false;
                    }
                }
                else
                {
                    moveAssessment[currentX + i, currentY] = 1;
                }
            }
            else
            {
                right = false;
            }

            // Left
            if (IsSquareOnBoard(currentX - i, currentY) && left)
            {
                possibleSquare = tiles[currentX - i, currentY].GetComponent<Square>();

                if (possibleSquare.currentPiece != null)
                {
                    if (!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != PieceType.LandMine)
                    {
                        moveAssessment[currentX - i, currentY] = 2;
                        left = false;
                    }
                    else
                    {
                        left = false;
                    }
                }
                else
                {
                    moveAssessment[currentX - i, currentY] = 1;
                }
            }
            else
            {
                left = false;
            }

            // Up
            if (IsSquareOnBoard(currentX, currentY+ i) && up)
            {
                possibleSquare = tiles[currentX, currentY + i].GetComponent<Square>();

                if (possibleSquare.currentPiece != null)
                {
                    if (!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != PieceType.LandMine)
                    {
                        moveAssessment[currentX, currentY + i] = 2;
                        up = false;
                    }
                    else
                    {
                        up = false;
                    }
                }
                else
                {
                    moveAssessment[currentX, currentY + i] = 1;
                }
            }
            else
            {
                up = false;
            }

            // Down
            if (IsSquareOnBoard(currentX, currentY - i) && down)
            {
                possibleSquare = tiles[currentX, currentY - i].GetComponent<Square>();

                if (possibleSquare.currentPiece != null)
                {
                    if (!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != PieceType.LandMine)
                    {
                        moveAssessment[currentX, currentY - i] = 2;
                        down = false;
                    }
                    else
                    {
                        down = false;
                    }
                }
                else
                {
                    moveAssessment[currentX, currentY - i] = 1;
                }
            }
            else
            {
                down = false;
            }
        }

        // Tactician is in allied starting zone
        if (currentY <= 2)
        {
            interactableZone = 1;
        }
        // Tactician is in neutral zone
        else if (currentY <= 6)
        {
            interactableZone = 2;
        }
        // Tactician is in enemy starting zone
        else
        {
            interactableZone = 3;
        }

        int y = -1;
        int upperBound = -1;

        if (interactableZone == 1)
        {
            y = 0;
            upperBound = 2;
        }
        else if(interactableZone == 2)
        {
            y = 3;
            upperBound = 6;
        }
        else if(interactableZone == 3)
        {
            y = 7;
            upperBound = 9;
        }

        for (int j; y <= upperBound; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Square squareInRange = tiles[x, y].GetComponent<Square>();
                if (squareInRange.currentPiece != null)
                {
                    if (!squareInRange.currentPiece.isNavy)
                    {
                        switch (squareInRange.currentPiece.type)
                        {
                            case PieceType.Mate:
                                Debug.Log("Inheriting move from Mate on {" + x + ", " + y + "}");
                                mate = true;
                                break;
                            case PieceType.Bomber:
                                Debug.Log("Inheriting move from Bomber on {" + x + ", " + y + "}");
                                bomber = true;
                                break;
                            case PieceType.Vanguard:
                                Debug.Log("Inheriting move from Vanguard on {" + x + ", " + y + "}");
                                vanguard = true;
                                break;
                            case PieceType.Navigator:
                                Debug.Log("Inheriting move from Navigator on {" + x + ", " + y + "}");
                                navigator = true;
                                break;
                            case PieceType.Gunner:
                                Debug.Log("Inheriting move from Gunner on {" + x + ", " + y + "}");
                                gunner = true;
                                break;
                            case PieceType.Cannon:
                                Debug.Log("Inheriting move from Cannon on {" + x + ", " + y + "}");
                                cannon = true;
                                break;
                            case PieceType.Quartermaster:
                                Debug.Log("Inheriting move from Quartermaster on {" + x + ", " + y + "}");
                                quartermaster = true;
                                break;
                            case PieceType.Royal2:
                                Debug.Log("Inheriting move from Corsair on {" + x + ", " + y + "}");
                                corsair = true;
                                break;
                            case PieceType.Royal1:
                                Debug.Log("Inheriting move from Captain on {" + x + ", " + y + "}");
                                captain = true;
                                break;
                            default:
                                Debug.Log("Piece on {" + x + ", " + y + "} is a land mine or ore or unidentified");
                                break;
                        }
                    }
                }
            }
        }

        if(!mate && !bomber && !vanguard && !navigator && !gunner && !cannon && !quartermaster && !corsair && !captain)
        {
            Debug.Log("No moves to inherit");
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
