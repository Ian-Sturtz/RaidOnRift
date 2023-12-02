using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    // How many pieces have been added to the game so far
    public int PIECES_ADDED;

    // Board Information
    public float tile_size = 1f;
    public float tile_size_margins = 1.05f;
    public float game_board_size = 10.55f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    private GameObject gameBoard;   // Reference to gameBoard object

    // Game State Information
    public bool gameWon = false;
    public bool navyTurn = true;
    public GameObject[,] tiles; // All game squares
    public Piece[] NavyPieces;      // All Navy game pieces
    public Piece[] PiratePieces;    // All Pirate game pieces


    public GameObject JailCells;
    public JailBoard jail;
    public int teamSize = 30;

    private int[,] moveAssessment;  // All legal moves of a clicked-on piece
    public bool squareSelected = false;
    public GameObject tileSelected;
    public GameObject storedTileSelected;


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

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<JailBoard>();
        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();

        // Test spawns one piece of each type in a random spot on the board
        NavyPieces[0] = SpawnPiece(PieceType.Ore, true, 3, 0);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 1, 2);
        NavyPieces[2] = SpawnPiece(PieceType.LandMine, true, 7, 6);
        NavyPieces[3] = SpawnPiece(PieceType.Mate, true, 4, 4);
        NavyPieces[4] = SpawnPiece(PieceType.Royal1, true, 0, 3);

        PiratePieces[0] = SpawnPiece(PieceType.Ore, false, 7, 9);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 9, 9);
        PiratePieces[2] = SpawnPiece(PieceType.LandMine, false, 5, 5);
        PiratePieces[3] = SpawnPiece(PieceType.Royal1, false, 8, 6);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        // Check for a game win
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Piece checkPiece = tiles[i, j].GetComponent<Square>().currentPiece;

                if(checkPiece != null)
                {
                    if (checkPiece.hasOre && checkPiece.isNavy)
                    {
                        gameWon = true;
                    }
                }
            }

            for (int j = 9; j > 6; j--)
            {
                Piece checkPiece = tiles[i, j].GetComponent<Square>().currentPiece;

                if (checkPiece != null)
                {
                    if (checkPiece.hasOre && !checkPiece.isNavy)
                    {
                        gameWon = true;
                    }
                }
            }
        }

        // Detect a square has been clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // A square has been clicked
            if (hit.collider != null)
            {
                tileSelected = GameObject.Find(hit.collider.name);

                // A jail cell was clicked
                if(tileSelected.tag == "JailCell")
                {

                }

                // No square had previously been clicked
                else if (!squareSelected)
                {
                    Square current_square = tileSelected.GetComponent<Square>();
                    if (current_square.currentPiece != null)
                    {
                        if(current_square.currentPiece.isNavy != navyTurn)
                        {
                            Square selectedTile = tileSelected.GetComponent<Square>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);
                        }
                        else
                        {
                            storedTileSelected = tileSelected;
                            squareSelected = true;
                            current_square.SquareHasBeenClicked = true;
                            Debug.Log(tileSelected + " " + current_square + " " +  current_square.currentPiece);
                            DetectLegalMoves(tileSelected, current_square.currentPiece);
                        }
                        
                    }
                }
                // A square had been previously clicked
                else
                {
                    // A moveable square has been clicked
                    if(tileSelected.tag == "MoveableSquare")
                    {
                        Square currentSquare = storedTileSelected.GetComponent<Square>();
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        MovePiece(currentSquare.currentPiece, moveCoordinates.x, moveCoordinates.y);
                        ResetBoardMaterials();
                        NextTurn();
                        Square selectedTile = tileSelected.GetComponent<Square>();
                        selectedTile.FlashMaterial(selectedTile.moveableBoardMaterial, 2);
                        squareSelected = false;
                        currentSquare.SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                    }

                    // An enemy piece is being captured
                    else if(tileSelected.tag == "CaptureSquare")
                    {
                        Square currentSquare = storedTileSelected.GetComponent<Square>();
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        // Capture that piece
                        Piece capturedPiece = tiles[moveCoordinates.x, moveCoordinates.y].GetComponent<Square>().currentPiece;
                        if (capturedPiece.type == PieceType.Ore)
                            currentSquare.currentPiece.hasOre = true;

                        if (capturedPiece.hasOre)
                        {
                            // Captured the orebearer, need to place the ore back on the board
                        }

                        jail.InsertAPiece(capturedPiece);
                        Debug.Log(capturedPiece.name + " has been captured");
                        capturedPiece.destroyPiece();
                        currentSquare.currentPiece.hasCaptured = true;

                        // Move current piece to that square
                        MovePiece(currentSquare.currentPiece, moveCoordinates.x, moveCoordinates.y);
                        
                        // Clean up board now that move has completed
                        ResetBoardMaterials();
                        NextTurn();
                        Square selectedTile = tileSelected.GetComponent<Square>();
                        selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 2);
                        squareSelected = false;
                        currentSquare.SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                    }

                    // The same square was clicked twice (cancel the square selection)
                    else if(tileSelected.GetComponent<Square>().SquareHasBeenClicked)
                    {
                        ResetBoardMaterials();
                        squareSelected = false;
                        tileSelected.GetComponent<Square>().SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                    }
                }
            }
        }
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

    private void ResetBoardMaterials()
    {
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
            MovePiece(cp, startingX, startingY, false);
        }

        return cp;
    }

    public void MovePiece(Piece piece, int x, int y, bool lerpMove = true)
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


        if (lerpMove)
        {
            StartCoroutine(LerpPosition(piece, targetPosition));
        }
        else
        {
            piece.transform.position = targetPosition;
        }
    }

    IEnumerator LerpPosition(Piece piece, Vector3 targetPosition, float duration = 0.1f)
    {
        float time = 0;
        Vector3 startPosition = piece.transform.position;
        while (time < duration)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = targetPosition;
    }

    private void DetectLegalMoves(GameObject current, Piece piece)
    {
        Square current_square;
        int current_x = IdentifyThisBoardSquare(current).x;
        int current_y = IdentifyThisBoardSquare(current).y;

        moveAssessment = new int[TILE_COUNT_X, TILE_COUNT_Y];

        current_square = tiles[current_x, current_y].GetComponent<Square>();

        switch (piece.type)
        {
            case PieceType.Ore:
                squareSelected = false;
                current_square.SquareHasBeenClicked = false;
                current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<Square>().clickedBoardMaterial, 3);
                tileSelected = null;
                break;
            case PieceType.LandMine:
                squareSelected = false;
                current_square.SquareHasBeenClicked = false;
                current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<Square>().clickedBoardMaterial, 3);
                tileSelected = null;
                break;
            case PieceType.Mate:
                moveAssessment = piece.GetComponent<Mate>().GetValidMoves(tiles);
                break;
            case PieceType.Royal1:
                if (piece.isNavy)
                {
                    moveAssessment = piece.GetComponent<Admiral>().GetValidMoves(tiles);
                }
                else
                {
                    moveAssessment = piece.GetComponent<Captain>().GetValidMoves(tiles);
                }
                break;
            default:
                Debug.Log("Cannot determine piece moveset");
                break;
        }

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(moveAssessment[x,y] == 1)
                {
                    Piece possiblePiece = tiles[x, y].GetComponent<Square>().currentPiece;

                    // There is a piece in that possible move square
                    if(possiblePiece != null)
                    {
                        // That piece is on the same team (can't move there)
                        if(piece.isNavy == possiblePiece.isNavy)
                        {
                            moveAssessment[x, y] = -1;
                        }
                        // That piece is a land mine (can't move there)
                        else if(possiblePiece.type == PieceType.LandMine)
                        {
                            moveAssessment[x, y] = -1;
                        }
                        // That piece is an enemy piece that can be captured
                        else
                        {
                            moveAssessment[x, y] = 2;
                        }
                    }
                }
            }
        }

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {                
                if(moveAssessment[x,y] == 1)
                {
                    tiles[x, y].tag = "MoveableSquare";
                    Square activeSquare = tiles[x,y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.moveableBoardMaterial);
                }
                else if (moveAssessment[x,y] == 2)
                {
                    tiles[x, y].tag = "CaptureSquare";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                }
            }
        }
    }

    private void NextTurn()
    {
        if (navyTurn)
        {
            navyTurn = false;
        }
        else
        {
            navyTurn = true;
        }
    }
}
