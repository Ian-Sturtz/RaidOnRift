using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Engineer : Piece
{
    public Piece capturedBomb = null;

    [SerializeField] private SpriteRenderer PieceImage;
    [SerializeField] private Sprite regularSprite;
    [SerializeField] private Sprite capturedSprite;

    protected override void Update()
    {
        base.Update();

        if (capturedBomb == null)
        {
            PieceImage.sprite = regularSprite;
        }
        else
        {
            PieceImage.sprite = capturedSprite;
        }
    }

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

        if (SceneManager.GetActiveScene().name == "Board")
        {
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
                            if(possibleSquare.currentPiece.type != PieceType.EnergyShield){
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX, currentY + change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if(capturedBomb != null)
                                {
                                    moveAssessment[currentX, currentY + change] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX, currentY - change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX, currentY - change] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX + change, currentY] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX + change, currentY] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX - change, currentY] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX - change, currentY] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX + change, currentY + change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX + change, currentY + change] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX - change, currentY + change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX - change, currentY + change] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX + change, currentY - change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX + change, currentY - change] = -1;
                                }
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
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX - change, currentY - change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX - change, currentY - change] = -1;
                                }
                            }
                        }
                    }
                    else
                    {
                        down_left = false;
                    }
                }
            }
        }
        // Tutorial scene
        else
        {
            for (int change = 1; (up || down || left || right || up_right || up_left || down_right || down_left) && change <= 2; change++)
            {
                if (up)
                {
                    if (IsSquareOnBoard(currentX, currentY + change))
                    {
                        moveAssessment[currentX, currentY + change] = 1;

                        TTSquare possibleSquare = tiles[currentX, currentY + change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            up = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX, currentY + change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX, currentY + change] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX, currentY - change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            down = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX, currentY - change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX, currentY - change] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX + change, currentY].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            right = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX + change, currentY] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX + change, currentY] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX - change, currentY].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            left = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX - change, currentY] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX - change, currentY] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX + change, currentY + change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            up_right = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX + change, currentY + change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX + change, currentY + change] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX - change, currentY + change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            up_left = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX - change, currentY + change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX - change, currentY + change] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX + change, currentY - change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            down_right = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX + change, currentY - change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX + change, currentY - change] = -1;
                                }
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

                        TTSquare possibleSquare = tiles[currentX - change, currentY - change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece != null)
                        {
                            down_left = false;
                            if (possibleSquare.currentPiece.type != PieceType.EnergyShield)
                            {
                                if (possibleSquare.currentPiece.type != PieceType.Ore)
                                {
                                    if (!possibleSquare.currentPiece.hasOre && capturedBomb == null)
                                    {
                                        moveAssessment[currentX - change, currentY - change] = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (capturedBomb != null)
                                {
                                    moveAssessment[currentX - change, currentY - change] = -1;
                                }
                            }
                        }
                    }
                    else
                    {
                        down_left = false;
                    }
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

        bool left = true;
        bool right = true;
        bool up = true;
        bool down = true;

        if (SceneManager.GetActiveScene().name == "Board")
        {
            for (int change = 1; change <= 2; change++)
            {
                Square possibleSquare;

                if (left)
                {
                    if (IsSquareOnBoard(currentX - change, currentY))
                    {
                        possibleSquare = tiles[currentX - change, currentY].GetComponent<Square>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX - change, currentY] = 6;
                        }
                        else
                        {
                            left = false;
                        }
                    }
                    else
                    {
                        left = false;
                    }
                }

                if (right)
                {
                    if (IsSquareOnBoard(currentX + change, currentY))
                    {
                        possibleSquare = tiles[currentX + change, currentY].GetComponent<Square>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX + change, currentY] = 6;
                        }
                        else
                        {
                            right = false;
                        }
                    }
                    else
                    {
                        right = false;
                    }
                }

                if (up)
                {
                    if (IsSquareOnBoard(currentX, currentY + change))
                    {
                        possibleSquare = tiles[currentX, currentY + change].GetComponent<Square>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX, currentY + change] = 6;
                        }
                        else
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
                        possibleSquare = tiles[currentX, currentY - change].GetComponent<Square>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX, currentY - change] = 6;
                        }
                        else
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
        }
        // Tutorial scene
        else
        {
            for (int change = 1; change <= 2; change++)
            {
                TTSquare possibleSquare;

                if (left)
                {
                    if (IsSquareOnBoard(currentX - change, currentY))
                    {
                        possibleSquare = tiles[currentX - change, currentY].GetComponent<TTSquare>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX - change, currentY] = 6;
                        }
                        else
                        {
                            left = false;
                        }
                    }
                    else
                    {
                        left = false;
                    }
                }

                if (right)
                {
                    if (IsSquareOnBoard(currentX + change, currentY))
                    {
                        possibleSquare = tiles[currentX + change, currentY].GetComponent<TTSquare>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX + change, currentY] = 6;
                        }
                        else
                        {
                            right = false;
                        }
                    }
                    else
                    {
                        right = false;
                    }
                }

                if (up)
                {
                    if (IsSquareOnBoard(currentX, currentY + change))
                    {
                        possibleSquare = tiles[currentX, currentY + change].GetComponent<TTSquare>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX, currentY + change] = 6;
                        }
                        else
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
                        possibleSquare = tiles[currentX, currentY - change].GetComponent<TTSquare>();

                        if (possibleSquare.currentPiece == null)
                        {
                            moveAssessment[currentX, currentY - change] = 6;
                        }
                        else
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
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
