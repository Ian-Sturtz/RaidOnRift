using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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

    [SerializeField] private bool navyDone = true;
    [SerializeField] private bool pirateDone = true;
    [SerializeField] private bool piecesDone = false;

    #endregion

    #region JailInfo

    // Jail State Information
    public GameObject JailCells;
    public PPJailBoard jail;
    public int teamSize = 30;
    [SerializeField] private int selectedCellIndex = -1;

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

    bool piecesadded = false;

    private void Start()
    {
        navyTurn = PieceManager.instance.navyFirst;
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
        if (navyTurn)
        {
            navyDone = true;
        }
        else
        {
            pirateDone = true;
        }

        if (navyTurn)
        {
            for (int i = 0; i < 30; i++)
            {
                if (jail.navyJailedPieces[i] != null)
                {
                    navyDone = false;
                    if (!squareSelected)
                    {
                        jail.PirateJailCells[i].GetComponent<PPJailCell>().interactable = true;
                    }
                }
            }
        }
        else if (!navyTurn)
        {
            for (int i = 0; i < 30; i++)
            {
                if (jail.pirateJailedPieces[i] != null)
                {
                    pirateDone = false;
                    if (!squareSelected)
                    {
                        jail.NavyJailCells[i].GetComponent<PPJailCell>().interactable = true;
                    }
                }

            }
        }

        // Both teams are done being placed
        if(navyDone && pirateDone && !piecesadded)
        {
            int totalPieces = 0;
            piecesDone = true;
            Debug.Log("Pieces placed");

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    PPSquare activeSquare = tiles[x, y].GetComponent<PPSquare>();

                    if (activeSquare.currentPiece != null)
                    {
                        Piece activePiece = activeSquare.currentPiece;

                        PieceManager.instance.pieceTypes[totalPieces] = activePiece.type;
                        PieceManager.instance.factions[totalPieces] = activePiece.isNavy;
                        PieceManager.instance.pieceCoords[totalPieces, 0] = activePiece.currentX;
                        PieceManager.instance.pieceCoords[totalPieces, 1] = activePiece.currentY;

                        Debug.Log(PieceManager.instance.pieceTypes[totalPieces] + " " + PieceManager.instance.factions[totalPieces] + " {" + PieceManager.instance.pieceCoords[totalPieces, 0] + "," + PieceManager.instance.pieceCoords[totalPieces, 1] + "}");
                        totalPieces++;
                    }
                }
            }

            PieceManager.instance.totalPieces = totalPieces;
            piecesadded = true;
            SceneManager.LoadScene("Board");
        }

        // Navy finished placing their pieces (skip their placement turn)
        else if (navyTurn && navyDone)
        {
            NextTurn();
        }

        // Pirates finished placing their pieces (skip their placement turn)
        else if(!navyTurn && pirateDone)
        {
            NextTurn();
        }

        // Mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // A square has been clicked
            if (hit.collider != null)
            {
                tileSelected = GameObject.Find(hit.collider.name);

                if (tileSelected.tag == "JailCell")
                {
                    // Do nothing
                }
                // No valid cell had been clicked yet
                else if (!squareSelected)
                {
                    // An interactable jail cell was clicked
                    if (tileSelected.tag == "InteractablePiece")
                    {
                        PPJailCell currentSquare = tileSelected.GetComponent<PPJailCell>();
                        ResetBoardMaterials();
                        currentSquare.clicked = true;
                        currentSquare.tag = "InteractablePiece";
                        squareSelected = true;
                        storedTileSelected = tileSelected;

                        DetectValidSquares(currentSquare);
                    }

                    // The same piece was clicked twice (cancel piece selection)
                } else if (tileSelected.tag == "InteractablePiece")
                {
                    boardUI.GoalText(defaultText);
                    ResetBoardMaterials();
                    storedTileSelected = null;
                    squareSelected = false;
                    tileSelected = null;
                    selectedCellIndex = -1;

                    // A legal placement square has been selected (spawn the selected piece there)
                } else if (tileSelected.tag == "MoveableSquare")
                {
                    Vector2Int spawnCoordinates = IdentifyThisBoardSquare(tileSelected);
                    tileSelected.GetComponent<PPSquare>().FlashMaterial(tileSelected.GetComponent<PPSquare>().moveableBoardMaterial, 2);
                    // Finds which cell the target piece is in
                    for (int i = 0; i < teamSize; i++)
                    {
                        if (navyTurn)
                        {
                            PPJailCell activeCell;
                            activeCell = jail.PirateJailCells[i].GetComponent<PPJailCell>();
                            if (activeCell.tag == "InteractablePiece")
                            {
                                selectedCellIndex = i;
                            }
                        }
                        else
                        {
                            PPJailCell activeCell;
                            activeCell = jail.NavyJailCells[i].GetComponent<PPJailCell>();
                            if (activeCell.tag == "InteractablePiece")
                            {
                                selectedCellIndex = i;
                            }
                        }
                    }

                    PPSquare currentSquare = tileSelected.GetComponent<PPSquare>();
                    PPJailCell currentCell = storedTileSelected.GetComponent<PPJailCell>();
                    Piece currentPiece = currentCell.currentPiece;

                    
                    SpawnPiece(currentPiece.type, currentPiece.isNavy, spawnCoordinates.x, spawnCoordinates.y);

                    ResetBoardMaterials();
                    currentPiece.destroyPiece();
                    currentCell.resetCell();
                    
                    if (navyTurn)
                    {
                        jail.navyJailedPieces[selectedCellIndex] = null;
                    }
                    else
                    {
                        jail.pirateJailedPieces[selectedCellIndex] = null;
                    }

                    currentSquare.SquareHasBeenClicked = false;
                    storedTileSelected = null;
                    tileSelected = null;
                    squareSelected = false;
                    selectedCellIndex = -1;
                    NextTurn();
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
                PPSquare square = tiles[x, y].GetComponent<PPSquare>();
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
        tiles[x, y].GetComponent<PPSquare>().currentPiece = piece;
    }

    private void NullCurrentPiece(int x, int y)
    {
        tiles[x, y].GetComponent<PPSquare>().currentPiece = null;
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
        tiles[x, y].GetComponent<PPSquare>().currentPiece = piece;
        piece.currentX = x;
        piece.currentY = y;
        SetCurrentPiece(piece, x, y);


        piece.transform.position = targetPosition;
    }

    public void DetectValidSquares(PPJailCell currentSquare)
    {
        Piece currentPiece = currentSquare.currentPiece;
        int startingRow;
        int endRow;
        int rowSpacesRemaining = 0;
        int spacesNeeded = 2;
        Piece possiblePiece;

        boardUI.GoalText("");

        if (currentPiece.type == PieceType.LandMine)
        {
            boardUI.GoalText("This is a Land Mine. It must go in the Neutral Zone (rows 4-7).", true);
            startingRow = 3;
            endRow = 7;
        }else if (currentPiece.isNavy)
        {
            startingRow = 0;
            if (currentPiece.type == PieceType.Royal1 || currentPiece.type == PieceType.Ore)
            {
                if(currentPiece.type == PieceType.Royal1)
                {
                    boardUI.GoalText("Team leaders need to go in your Commander Zone (row 1).", true);
                }
                else
                {
                    boardUI.GoalText("The Ore must go in your Commander Zone (row 1).", true);
                }
                endRow = 1;
            }
            else
            {
                boardUI.GoalText("Pieces on your team can go in your Start Zone (rows 1-3).", true);
                for (int i = 0; i < 10; i++)
                {
                    possiblePiece = tiles[i, 0].GetComponent<PPSquare>().currentPiece;
                    if(possiblePiece == null)
                    {
                        rowSpacesRemaining++;
                    }
                    else if (possiblePiece.type == PieceType.Royal1 || possiblePiece.type == PieceType.Ore) 
                    {
                        spacesNeeded--;
                    }
                }

                if(rowSpacesRemaining <= spacesNeeded)
                {
                    boardUI.GoalText("Remember to save some space for your Team Leader and Ore!", true);
                    startingRow = 1;
                }

                endRow = 3;
            }
        }
        else
        {
            endRow = 10;
            if (currentPiece.type == PieceType.Royal1 || currentPiece.type == PieceType.Ore)
            {
                if (currentPiece.type == PieceType.Royal1)
                {
                    boardUI.GoalText("Team leaders need to go in your Commander Zone (row 9).", true);
                }
                else
                {
                    boardUI.GoalText("The ore must go in your Commander Zone (row 9).", true);
                }
                startingRow = 9;
            }
            else
            {
                boardUI.GoalText("Pieces on your team can go in your Start Zone (rows 8-10).", true);
                for (int i = 0; i < 10; i++)
                {
                    possiblePiece = tiles[i, 9].GetComponent<PPSquare>().currentPiece;
                    if (possiblePiece == null)
                    {
                        rowSpacesRemaining++;
                    }
                    else if (possiblePiece.type == PieceType.Royal1 || possiblePiece.type == PieceType.Ore)
                    {
                        spacesNeeded--;
                    }
                }

                if (rowSpacesRemaining <= spacesNeeded)
                {
                    boardUI.GoalText("Remember to save some space in your Commander Zone for your Team Leader and Ore!", true);
                    endRow = 9;
                }

                startingRow = 7;
            }
        }

        for (int x = 0; x < 10; x++)
        {
            for (int y = startingRow; y < endRow; y++)
            {
                possiblePiece = tiles[x, y].GetComponent<PPSquare>().currentPiece;

                if(possiblePiece == null)
                {
                    PPSquare activeSquare = tiles[x, y].GetComponent<PPSquare>();
                    activeSquare.tag = "MoveableSquare";
                    activeSquare.SetMaterial(activeSquare.moveableBoardMaterial);
                }
            }
        }

        boardUI.GoalText("Click on a green square to place that piece there,", true);
        boardUI.GoalText("or click the piece again to cancel.", true);
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
