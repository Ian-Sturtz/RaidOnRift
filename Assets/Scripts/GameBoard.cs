using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public int PIECES_ADDED = 2;    //Increase this number for each new piece added

    public float tile_size = 1f;
    public float tile_size_margins = 1.05f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    public GameObject[,] tiles;
    private GameObject gameBoard;

    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] PiecePrefabs;

    private void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        tile_size = gameBoard.transform.localScale.x / 10.55f;
        IdentifyBoardSquares();
    }

    private void IdentifyBoardSquares()
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];

        string piecename;

        for (int x = 1; x <= TILE_COUNT_X; x++)
        {
            for (int y = 1; y <= TILE_COUNT_Y; y++)
            {
                piecename = "File" + x + "/Rank" + y;

                GameObject boardSquare = GameObject.Find(piecename);

                if (x == 1 && y == 1)
                {
                    boardSquare.tag = "BoardLowerLeftCorner";
                }
                else
                {
                    boardSquare.tag = "GameSquare";
                }
                
                tiles[x - 1, y - 1] = boardSquare;

            }
        }
    }
}
