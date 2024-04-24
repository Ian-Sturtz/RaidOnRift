using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using Unity.Networking.Transport;

public class PPGameBoard : MonoBehaviour
{
    #region GameInfo
    // How many pieces have been added to the game so far
    public int PIECES_ADDED;

    // Game State Information
    public bool navyTurn = true;
    public GameObject[,] tiles;     // All game squares
    public Piece[] NavyPieces;      // All Navy game pieces
    public Piece[] PiratePieces;    // All Pirate game pieces

    public bool playerIsNavy = false;

    public Bar PlacementTimer;
    public GameObject timer;

    #endregion

    #region BoardInfo
    // Board Information
    public float tile_size = 1f;
    public float tile_size_margins = 1.05f;
    public float game_board_size = 10.55f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    private GameObject gameBoard;   // Reference to gameBoard object

    [SerializeField] private GameObject viewBlocker;
    [SerializeField] private GameObject pirateViewBlockZone;
    [SerializeField] private GameObject navyViewBlockZone;

    private BoardUI boardUI;
    private string defaultText = "Click on a piece to spawn it!";

    [SerializeField] private GameObject PiecePlacerObject;
    [SerializeField] private PiecePlacement piecePlacer;
    [SerializeField] private bool navyDone = true;
    [SerializeField] private bool pirateDone = true;
    [SerializeField] private bool piecesDone = false;
    [SerializeField] private bool oreSpawned = false;

    public bool pieceMoving = false;

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

    private void Awake()
    {
        RegisterEvents();
    }

    private void Start()
    {
        if (PieceManager.instance != null)
            navyTurn = PieceManager.instance.navyFirst;
        else
            navyTurn = true;

        if (PieceManager.instance.onlineMultiplayer)
        {
            playerIsNavy = (MultiplayerController.Instance.currentTeam == 0);
            navyTurn = playerIsNavy;
        }
        else
        {
            timer.SetActive(false);
        }


        PIECES_ADDED = Enum.GetValues(typeof(PieceType)).Length;

        //Initialize the game board and all variables
        {
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

            piecePlacer = PiecePlacerObject.GetComponent<PiecePlacement>();
            PlacementTimer.time = 120;
        }

        if (!PieceManager.instance.onlineMultiplayer && !navyTurn)
            StartCoroutine(RotateBoard(false));
        else if (PieceManager.instance.onlineMultiplayer && !playerIsNavy)
            StartCoroutine(RotateBoard(false));
    }
           
       
        

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void Update()
    {
        // Checks if the timer has run out
        if (PieceManager.instance.onlineMultiplayer && PlacementTimer.timeOver)
        {
            // Player is pirates and it's the navy's turn (game won)
            if (navyTurn && !playerIsNavy)
                MultiplayerController.Instance.gameWon = 1;
            // Player is navy and it's the pirate's turn (game won)
            else if (!navyTurn && playerIsNavy)
                MultiplayerController.Instance.gameWon = 1;
            // Navy turn and player is navy || Pirate turn and player is pirate (game loss)
            else
                MultiplayerController.Instance.gameWon = 0;

            // Ends the game
            MultiplayerController.Instance.ConnectionDropped();
        }

        // Shifts the view blocker around the board accordingly
        if (navyTurn && !oreSpawned)
        {
            if(PieceManager.instance.onlineMultiplayer && !playerIsNavy)
                viewBlocker.transform.position = navyViewBlockZone.transform.position;
            else
                viewBlocker.transform.position = pirateViewBlockZone.transform.position;
        }
        else if (!oreSpawned)
        {
            if (PieceManager.instance.onlineMultiplayer && playerIsNavy)
                viewBlocker.transform.position = pirateViewBlockZone.transform.position;
            else
                viewBlocker.transform.position = navyViewBlockZone.transform.position;
        }

        // Checks whether each player is done
        navyDone = true;
        for (int i = 0; i < 30; i++)
        {
            if(jail.navyJailedPieces[i] != null)
            {
                navyDone = false;
            }
        }
        pirateDone = true;
        for (int i = 0; i < 30; i++)
        {
            if (jail.pirateJailedPieces[i] != null)
            {
                pirateDone = false;
            }
        }

        if(PieceManager.instance.onlineMultiplayer)
        {
            if(navyDone && navyTurn)
            {
                NextTurn();
            }else if(pirateDone && !navyTurn)
            {
                NextTurn();
            }
        }
        else
        {
            if (navyDone && navyTurn)
            {
                NextTurn();
            }
            else if (pirateDone && !navyTurn)
            {
                NextTurn();
            }
        }

        if(PieceManager.instance.onlineMultiplayer)
        {
            if(navyDone && playerIsNavy)
            {
                MultiplayerController.Instance.gameWon = 1;
            }
            else if(pirateDone && !playerIsNavy)
            {
                MultiplayerController.Instance.gameWon = 1;
            }
        }

        boardUI.UpdateTurn(navyTurn);

        // Highlights cells of current team's jail
        if (navyTurn)
        {
            if(PieceManager.instance.onlineMultiplayer && !playerIsNavy)
            {
                boardUI.GoalText("Your opponent is placing their pieces...");
            }
            else
            {
                for (int i = 0; i < 30; i++)
                {
                    if (jail.navyJailedPieces[i] != null)
                    {
                        if (!squareSelected)
                        {
                            jail.PirateJailCells[i].GetComponent<PPJailCell>().interactable = true;
                        }
                    }
                }
            }
        }

        else if (!navyTurn)
        {
            if (PieceManager.instance.onlineMultiplayer && playerIsNavy)
            {
                boardUI.GoalText("Your opponent is placing their pieces...");
            }
            else
            {
                for (int i = 0; i < 30; i++)
                {
                    if (jail.pirateJailedPieces[i] != null)
                    {
                        if (!squareSelected)
                        {
                            jail.NavyJailCells[i].GetComponent<PPJailCell>().interactable = true;
                        }
                    }
                }
            }
        }

        // Both teams are done being placed
        if(navyDone && pirateDone && !piecesadded)
        {
            if (!oreSpawned)
            {
                viewBlocker.SetActive(false);
                piecePlacer.SpawnOresAndShields();
                if (PieceManager.instance.onlineMultiplayer)
                    MultiplayerController.Instance.gameWon = -1;
                oreSpawned = true;

                Debug.Log($"navy first: {PieceManager.instance.navyFirst}");

                navyTurn = PieceManager.instance.navyFirst;
                PlacementTimer.time = 30;
                PlacementTimer.ResetBar();
            }
            else
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

                            // Debug.Log(PieceManager.instance.pieceTypes[totalPieces] + " " + PieceManager.instance.factions[totalPieces] + " {" + PieceManager.instance.pieceCoords[totalPieces, 0] + "," + PieceManager.instance.pieceCoords[totalPieces, 1] + "}");
                            totalPieces++;
                        }
                    }
                }

                PieceManager.instance.totalPieces = totalPieces;
                piecesadded = true;
                SceneManager.LoadScene("Board");
            }
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

                    if (PieceManager.instance.onlineMultiplayer)
                    {
                        Debug.Log("Sending piece position to server");
                        NetPositionPiece pp = new NetPositionPiece();

                        if (currentPiece.isNavy)
                        {
                            pp.teamID = 0;
                        }
                        else
                        {
                            pp.teamID = 1;
                        }

                        pp.jailIndex = selectedCellIndex;
                        pp.targetX = spawnCoordinates.x;
                        pp.targetY = spawnCoordinates.y;

                        Debug.Log($"ID:{pp.teamID}, index: {pp.jailIndex}: {pp.targetX}, {pp.targetY}");

                        Client.Instance.SendToServer(pp);
                    }

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

                    if (oreSpawned)
                    {
                        NextTurn();
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
            cp.transform.Rotate(0f, 0f, 180f);
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
    IEnumerator RotateBoard(bool navyAtBottom)
    {
        Debug.Log($"Positioning the {navyAtBottom} pieces to the bottom of the screen");

        while (pieceMoving)
        {
            yield return new WaitForFixedUpdate();
        }

        if (navyAtBottom)
        {
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy))
            {
                Debug.Log("Putting the navy at the bottom of the screen");
                gameBoard.transform.Rotate(0f, 0f, -180f, Space.Self);

                for (int i = 0; i < teamSize; i++)
                {
                    if (NavyPieces[i] != null)
                    {
                        NavyPieces[i].transform.Rotate(0f, 0f, -180f, Space.Self);
                    }

                    if (PiratePieces[i] != null)
                    {
                        PiratePieces[i].transform.Rotate(0f, 0f, -180f, Space.Self);
                    }
                }
            }
        }
        else
        {
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && !playerIsNavy))
            {
                Debug.Log("Putting the pirates at the bottom of the screen");
                gameBoard.transform.Rotate(0f, 0f, 180f, Space.Self);

                for (int i = 0; i < teamSize; i++)
                {
                    if (NavyPieces[i] != null)
                    {
                        NavyPieces[i].transform.Rotate(0f, 0f, 180f, Space.Self);
                    }

                    if (PiratePieces[i] != null)
                    {
                        PiratePieces[i].transform.Rotate(0f, 0f, 180f, Space.Self);
                    }
                }
            }
        }

        yield return null;
    }

    // Changes the turn from one player to the next
    private void NextTurn()
    {
        ResetBoardMaterials(true);

        PlacementTimer.ResetBar();

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

    #region Events
    private void RegisterEvents()
    {
        NetUtility.S_POSITION_PIECE += OnPositionPieceServer;

        NetUtility.C_POSITION_PIECE += OnPositionPieceClient;
    }

    private void UnRegisterEvents()
    {
        NetUtility.S_POSITION_PIECE -= OnPositionPieceServer;

        NetUtility.C_POSITION_PIECE -= OnPositionPieceClient;
    }

    // Server
    private void OnPositionPieceServer(NetMessage msg, NetworkConnection cnn)
    {
        NetPositionPiece pp = msg as NetPositionPiece;

        Server.Instance.Broadcast(pp);
    }

    // Client
    private void OnPositionPieceClient(NetMessage msg)
    {
        NetPositionPiece pp = msg as NetPositionPiece;

        Debug.Log($"{pp.teamID} at index {pp.jailIndex} spawning at {pp.targetX} {pp.targetY}");

        PPJailCell currentCell;

        if ((pp.teamID == 0 && !playerIsNavy) || (pp.teamID == 1 && playerIsNavy))
        {
            if (pp.teamID == 0)
            {
                currentCell = jail.PirateJailCells[pp.jailIndex].GetComponent<PPJailCell>();
            }
            else
            {
                currentCell = jail.NavyJailCells[pp.jailIndex].GetComponent<PPJailCell>();
            }

            Piece currentPiece = currentCell.currentPiece;

            SpawnPiece(currentPiece.type, currentPiece.isNavy, pp.targetX, pp.targetY);

            ResetBoardMaterials();

            if (currentPiece.type == PieceType.Ore || currentPiece.type == PieceType.LandMine)
                NextTurn();

            currentPiece.destroyPiece();
            currentCell.resetCell();

            if (pp.teamID == 0)
            {
                jail.navyJailedPieces[pp.jailIndex] = null;
            }
            else
            {
                jail.pirateJailedPieces[pp.jailIndex] = null;
            }
        }
    }
    #endregion
}
