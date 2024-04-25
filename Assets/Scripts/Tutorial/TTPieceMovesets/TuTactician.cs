using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuTactician : TTPiece
{
    public bool[] mimicPieces = new bool[9];    // Boolean for whether he can mimic those pieces

    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        GameObject GameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        TTGameBoard board = GameBoard.GetComponent<TTGameBoard>();

        GameObject JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        TTJailBoard jail = JailCells.GetComponent<TTJailBoard>();

        int interactableZone = 0;

        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;

        for (int i = 0; i < 9; i++)
        {
            mimicPieces[i] = false;
        }

        moveAssessment = new int[10, 10];

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                moveAssessment[i, j] = -1;

        // Tactician's default moveset
        for(int i = 1; i <= 2; i++)
        {
            TTSquare possibleSquare;
            Debug.Log("currentX: " + currentX + " || currentY: " + currentY);

            // Right
            if (IsSquareOnBoard(currentX + i, currentY) && right)
            {
                possibleSquare = tiles[currentX + i, currentY].GetComponent<TTSquare>();

                if(possibleSquare.currentPiece != null)
                {
                    if(!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != TTPieceType.LandMine)
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
                possibleSquare = tiles[currentX - i, currentY].GetComponent<TTSquare>();

                if (possibleSquare.currentPiece != null)
                {
                    if (!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != TTPieceType.LandMine)
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
                possibleSquare = tiles[currentX, currentY + i].GetComponent<TTSquare>();

                if (possibleSquare.currentPiece != null)
                {
                    if (!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != TTPieceType.LandMine)
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
                possibleSquare = tiles[currentX, currentY - i].GetComponent<TTSquare>();

                if (possibleSquare.currentPiece != null)
                {
                    if (!possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != TTPieceType.LandMine)
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

        while(y <= upperBound)
        {
            for (int x = 0; x < 10; x++)
            {
                TTSquare squareInRange = tiles[x, y].GetComponent<TTSquare>();
                if (squareInRange.currentPiece != null)
                {
                    // Isolates non-orebearer pirates
                    if (!squareInRange.currentPiece.isNavy && !squareInRange.currentPiece.hasOre)
                    {
                        if(squareInRange.currentPiece.type == TTPieceType.Mate && !mimicPieces[0])
                        {
                            mimicPieces[0] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Bomber && !mimicPieces[1])
                        {
                            mimicPieces[1] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Vanguard && !mimicPieces[2])
                        {
                            mimicPieces[2] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Navigator && !mimicPieces[3])
                        {
                            mimicPieces[3] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Gunner && !mimicPieces[4])
                        {
                            mimicPieces[4] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Cannon && !mimicPieces[5])
                        {
                            mimicPieces[5] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Quartermaster && !mimicPieces[6])
                        {
                            mimicPieces[6] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Royal2 && !mimicPieces[7])
                        {
                            mimicPieces[7] = true;
                        }
                        else if (squareInRange.currentPiece.type == TTPieceType.Royal1 && !mimicPieces[8])
                        {
                            mimicPieces[8] = true;
                        }
                        else
                        {
                            //Default case here
                        }
                    }
                }
            }

            y++;
        }

        for (int i = 0; i < 9; i++)
        {
            if (mimicPieces[i])
            {
                
                TTPiece inheritPiece = Instantiate(board.PiecePrefabs[i + 13].GetComponent<TTPiece>());

                jail.InsertAPiece(inheritPiece, true);
                inheritPiece.destroyPiece();
                mimicPieces[i] = false;
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if(jail.TacticianMimicCells[i].GetComponent<TTJailCell>().currentPiece != null)
            {
                TTPiece currentPiece = jail.TacticianMimicCells[i].GetComponent<TTJailCell>().currentPiece;

                currentPiece.currentX = currentX;
                currentPiece.currentY = currentY;

                // Spawns an unloaded Gunner if the tactician has already gunner captured
                if(currentPiece.type == TTPieceType.Gunner)
                {
                    currentPiece.hasCaptured = board.tacticianGunnerCapture;
                }else if(currentPiece.type == TTPieceType.Royal2)
                {
                    currentPiece.GetComponent<TuCorsair>().canJump = (board.tacticianCorsairJump == 0);
                }
            }
        }
        Debug.Log("currentX: " + currentX + " || currentY: " + currentY);

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
