using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPGameBoard : MonoBehaviour
{
    #region GameInfo
    // How many pieces have been added to the game so far
    public int PIECES_ADDED;

    // Game State Information
    public bool navyTurn = true;
    public GameObject[,] tiles;     // All game squares
    public Piece[] NavyPieces;      // All Navy game pieces
    public Piece[] PiratePieces;    // All Pirate game

    #endregion

    #region BoardInfo
    // Board Information
    public float tile_size = 1f;
    public float tile_size_margins = 1.05f;
    public float game_board_size = 10.55f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    private GameObject gameBoard;   // Reference to gameBoard object

    private BoardUI boardUI;
    private string defaultText = "Click on a piece to spawn it!";

    #endregion

    #region JailInfo

    // Jail State Information
    public GameObject JailCells;
    public PPJailBoard jail;
    public int teamSize = 30;

    #endregion

    #region MovementInfo
    // Movement Information
    private int[,] moveAssessment;  // All legal moves of a clicked-on piece
    public bool squareSelected = false;
    public GameObject tileSelected;
    public GameObject storedTileSelected;

    #endregion

    // Piece prefabs to be spawned in as needed
    [Header("Prefabs and Materials")]
    [SerializeField] public GameObject[] PiecePrefabs;

    private void Start()
    {
        PIECES_ADDED = System.Enum.GetValues(typeof(PieceType)).Length;

        //Initialize the game board and all variables
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        game_board_size = gameBoard.transform.localScale.x;
        tile_size = gameBoard.transform.localScale.x / game_board_size;
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        teamSize = 30;

        boardUI = FindObjectOfType<BoardUI>();
        boardUI.GoalText(defaultText);

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<PPJailBoard>();
        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();
    }

    private void Update()
    {
        
    }

    private void IdentifyBoardSquares()
    {
        string piecename;

        for (int x = 1; x <= TILE_COUNT_X; x++)
        {
            for (int y = 1; y <= TILE_COUNT_Y; y++)
            {
                piecename = "{" + x + "," + y + "}";

                GameObject boardSquare = GameObject.Find(piecename);

                boardSquare.tag = "GameSquare";
                
                tiles[x - 1, y - 1] = boardSquare;
            }
        }
    }

    private void ResetBoardMaterials(bool resetJail = true)
    {
        if (resetJail)
        {
            // Resets materials for any jail cells that were interacted with
            jail.resetMaterials();
        }

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                tiles[x, y].tag = "GameSquare";
                Square square = tiles[x, y].GetComponent<Square>();
                square.SetMaterial(square.defaultBoardMaterial);
            }
        }
    }

    private int FindFirstOpenTeamSlot(bool teamNavy)
    {
        int index;

        if (teamNavy)
        {
            for (index = 0; index < teamSize; index++)
            {
                if(NavyPieces[index] == null)
                {
                    return index;
                }
            }
        }
        else
        {
            for (index = 0; index < teamSize; index++)
            {
                if (PiratePieces[index] == null)
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private Vector2Int IdentifyThisBoardSquare(GameObject square)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(tiles[x,y] == square)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);

    }

    private void SetCurrentPiece(Piece piece, int x, int y)
    {
        tiles[x, y].GetComponent<Square>().currentPiece = piece;
    }

    private void NullCurrentPiece(int x, int y)
    {
        tiles[x, y].GetComponent<Square>().currentPiece = null;
    }

    public Piece SpawnPiece(PieceType type, bool isNavy, int startingX = -1, int startingY = -1)
    {
        Piece cp;

        if (!isNavy)
        {
            cp = Instantiate(PiecePrefabs[(int)type + PIECES_ADDED], this.transform).GetComponent<Piece>();
        }
        else
        {
            cp = Instantiate(PiecePrefabs[(int)type], this.transform).GetComponent<Piece>();
        }

        cp.transform.localScale /= game_board_size;

        cp.type = type;
        cp.isNavy = isNavy;

        if(startingX != -1 && startingY != -1)
        {
            MovePiece(cp, startingX, startingY);
        }

        return cp;
    }

    public void MovePiece(Piece piece, int x, int y)
    {
        Vector3 targetPosition = tiles[x, y].transform.position;

        // Removes piece from original square and puts it in new square
        if(piece.currentX != -1 && piece.currentY != -1)
        {
            NullCurrentPiece(piece.currentX, piece.currentY);
        }
        tiles[x, y].GetComponent<Square>().currentPiece = piece;
        piece.currentX = x;
        piece.currentY = y;
        SetCurrentPiece(piece, x, y);


        piece.transform.position = targetPosition;
    }

    // Changes the turn from one player to the next
    private void NextTurn()
    {
        ResetBoardMaterials(true);

        if (navyTurn)
        {
            navyTurn = false;
        }
        else
        {
            navyTurn = true;
        }

        // Update UI
        boardUI.UpdateTurn(navyTurn);
        boardUI.GoalText(defaultText);
    }
}
