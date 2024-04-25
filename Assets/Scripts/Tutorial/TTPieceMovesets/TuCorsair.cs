using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuCorsair : TTPiece
{
    
    [SerializeField] private GameObject PieceImage;
    [SerializeField] private Sprite jumpReady;
    [SerializeField] private Sprite cooldown;

    [SerializeField] private GameObject GameBoard;
    [SerializeField] private TTGameBoard board;
    public bool canJump;
    private bool needsChange = true;
    private bool pieceSelectionScene = false;

    private void Start()
    {
        GameBoard = GameObject.FindGameObjectWithTag("GameBoard");

        board = GameBoard.GetComponent<TTGameBoard>();

        if (board == null)
        {
            pieceSelectionScene = true;
        }
    }

    private void Update()
    {
        if(!pieceSelectionScene)
            if(!board.tacticianSelected)
                canJump = (board.jumpCooldown == 0);

        if (canJump)
        {
            PieceImage.GetComponent<SpriteRenderer>().sprite = jumpReady;
        }
        else
        {
            PieceImage.GetComponent<SpriteRenderer>().sprite = cooldown;
        }
    }


    public int[,] GetValidMoves(GameObject[,] tiles, bool canJump = true)
    {
        int[,] moveAssessment;
        
        bool up_right = true;
        bool up_left = true;
        bool down_right = true;
        bool down_left = true;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        for (int x = 0; x < 10 && canJump; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                TTSquare possibleSquare = tiles[x, y].GetComponent<TTSquare>();
                if (possibleSquare.currentPiece == null)
                {
                    moveAssessment[x, y] = 8;
                }
            }
        }
        for (int i = 1; i <= 2; i++)
        {

            // Upper Right
            if (IsSquareOnBoard(currentX + i, currentY + i) && up_right)
            {
                TTSquare possibleSquare = tiles[currentX + i, currentY + i].GetComponent<TTSquare>();
                moveAssessment[currentX + i, currentY + i] = 1;
                if (possibleSquare.currentPiece != null)
                {
                    up_right = false;
                }
            }
            else
            {
                up_right = false;
            }

            // Upper Left
            if (IsSquareOnBoard(currentX - i, currentY + i) && up_left)
            {
                TTSquare possibleSquare = tiles[currentX - i, currentY + i].GetComponent<TTSquare>();
                moveAssessment[currentX - i, currentY + i] = 1;
                if (possibleSquare.currentPiece != null)
                {
                    up_left = false;
                }
            }
            else
            {
                up_left = false;
            }

            // Lower Right
            if (IsSquareOnBoard(currentX + i, currentY - i) && down_right)
            {
                TTSquare possibleSquare = tiles[currentX + i, currentY - i].GetComponent<TTSquare>();
                moveAssessment[currentX + i, currentY - i] = 1;
                if (possibleSquare.currentPiece != null)
                {
                    down_right = false;
                }
            }
            else
            {
                down_right = false;
            }

            // Lower Left
            if (IsSquareOnBoard(currentX - i, currentY - i) && down_left)
            {
                TTSquare possibleSquare = tiles[currentX - i, currentY - i].GetComponent<TTSquare>();
                moveAssessment[currentX - i, currentY - i] = 1;
                if (possibleSquare.currentPiece != null)
                {
                    down_left = false;
                }
            }
            else
            {
                down_left = false;
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
