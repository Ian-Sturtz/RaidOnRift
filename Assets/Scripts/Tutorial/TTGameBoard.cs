using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TTGameBoard : MonoBehaviour
{
    #region GameInfo

    // How many pieces have been added to the game so far
    public int PIECES_ADDED;

    // Game State Information
    [Header("Game State Information")]
    public bool gameWon = false;
    public bool gameOver { set; get; } = false;
    public bool stalemate = false;
    public bool navyTurn = true;
    public GameObject[,] tiles;     // All game squares
    public TTPiece[] NavyPieces;      // All Navy game pieces
    public TTPiece[] PiratePieces;    // All Pirate game

    // public Bar gameTimer;

    #endregion

    #region BoardInfo
    // Board Information
    [Header("Board Information")]
    public float tile_size = 1f;
    public float tile_size_margins = 1.05f;
    public float game_board_size = 10.55f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    private GameObject gameBoard;   // Reference to gameBoard object

    BoardUI boardUI;

    public bool pieceMoving = false;

    #endregion

    #region JailInfo

    // Jail State Information
    [Header("Jail Information")]
    public GameObject JailCells;
    public TTJailBoard jail;
    public int teamSize = 30;

    #endregion

    #region MovementInfo
    // Movement Information
    [Header("Movement Information")]
    private int[,] moveAssessment;  // All legal moves of a clicked-on piece
    public bool squareSelected = false;
    public GameObject tileSelected;
    public GameObject storedTileSelected;

    #endregion

    #region PieceInteractions
    [Header("Piece Interactions")]
    // For use with bomber interactions
    [SerializeField] private bool bomberSelected = false;
    [SerializeField] private bool landMineSelected = false;
    [SerializeField] private bool landMineInJail = false;
    private int cellToHighlight = -2;

    // For use with tactician interactions
    public bool tacticianSelected = false;
    public bool tacticianInheritSelected = false;
    public bool tacticianGunnerCapture = false;
    public int tacticianCorsairJump = 0;

    // Ore and Orebearer mechanics
    [SerializeField] private bool resetOre = false;
    [SerializeField] private bool orebearerSecondMove = false;

    // Corsair mechanics
    public int jumpCooldown = 0;

    #endregion

    #region GameAudio

    [Header("Game Audio clips")]
    [SerializeField] AudioSource movementAudio;
    [SerializeField] AudioSource gunnerAudio;
    [SerializeField] AudioSource gunnerRecharge;
    [SerializeField] AudioSource Capture;


    #endregion

    #region PiecePrefabs
    // Piece prefabs to be spawned in as needed
    [Header("Prefabs and Materials")]
    [SerializeField] public GameObject[] PiecePrefabs;
    #endregion

    #region Multiplayer

    [SerializeField] private bool playerIsNavy;
    private bool[] playerRematch = new bool[2];

    #endregion

    private void Awake()
    {
        if (StoryUI.tutorialToLoad >= 10)
        {
            navyTurn = false;
            playerIsNavy = false;
        }
        else
        if (StoryUI.tutorialToLoad >= 0)
        {
            navyTurn = true;
            playerIsNavy = true;
        }

        Debug.Log(navyTurn);
    }

    private void Start()
    {
        PIECES_ADDED = System.Enum.GetValues(typeof(TTPieceType)).Length;

        //Initialize the game board and all variables
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        game_board_size = gameBoard.transform.localScale.x;
        tile_size = gameBoard.transform.localScale.x / game_board_size;
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        teamSize = 30;

        boardUI = FindObjectOfType<BoardUI>();
        

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<TTJailBoard>();
        NavyPieces = new TTPiece[teamSize];
        PiratePieces = new TTPiece[teamSize];
        IdentifyBoardSquares();

        SpawnAllPieces();
    }

    private void OnDestroy()
    {
        // UnRegisterEvents();
    }

    private void Update()
    {
        // Check for a game win
        // if (gameTimer.timeOver && !gameWon)
        // {
        //     GameOver(!navyTurn);
        // }

        for (int i = 0; i < TILE_COUNT_X && !gameWon; i++)
        {
            TTPiece checkPiece = tiles[i, 0].GetComponent<TTSquare>().currentPiece;

            if (checkPiece != null)
            {
                if (checkPiece.hasOre && checkPiece.isNavy)
                {
                    checkPiece.gameWon = true;
                    GameOver(true);
                }
            }

            checkPiece = tiles[i, 9].GetComponent<TTSquare>().currentPiece;

            if (checkPiece != null)
            {
                if (checkPiece.hasOre && !checkPiece.isNavy)
                {
                    checkPiece.gameWon = true;
                    GameOver(false);
                }
            }
        }


        // The active player has selected a bomber or tactician to use this turn
        if (storedTileSelected != null)
        {
            boardUI.TTUpdateSelectedPiece(storedTileSelected.GetComponent<TTSquare>().currentPiece.type, storedTileSelected.GetComponent<TTSquare>().currentPiece.isNavy);

            // The active player has selected a bomber to use this turn
            if (storedTileSelected.GetComponent<TTSquare>().currentPiece.type == TTPieceType.Bomber) {
                bomberSelected = true;
            }

            // The active player has selected a tactician to use this turn
            else if (storedTileSelected.GetComponent<TTSquare>().currentPiece.type == TTPieceType.Royal2)
            {
                if (storedTileSelected.GetComponent<TTSquare>().currentPiece.isNavy)
                {
                    tacticianSelected = true;
                }
            }
        }
        // The active player hasn't selected a tactician or bomber to use this turn
        else
        {
            boardUI.HideSelectedPiece();

            bomberSelected = false;

            // Despawns all pieces out of Tactician inherit cells (if applicable)
            if (tacticianSelected)
            {
                tacticianSelected = false;
                tacticianInheritSelected = false;

                for (int i = 0; i < 9; i++)
                {

                    if (jail.tacticianMimicPieces[i] != null)
                        jail.tacticianMimicPieces[i].GetComponent<TTPiece>().destroyPiece();
                    jail.TacticianMimicCells[i].GetComponent<TTJailCell>().resetCell();
                    ResetBoardMaterials(true);
                }
            }
        }

        // Highlights a captured enemy land mines so the bomber can deploy them
        if (bomberSelected && cellToHighlight == -2)
        {
            // The selected bomber is Navy and can deploy Pirate Bombs
            if (tileSelected.GetComponent<TTSquare>().currentPiece.isNavy)
            {
                // The bomber hasn't captured a bomb yet
                if (tileSelected.GetComponent<TTSquare>().currentPiece.GetComponent<TuBomber>().capturedBomb == null)
                {
                    Debug.Log("No bombs captured, searching for one");
                    cellToHighlight = jail.FindPiece(TTPieceType.LandMine, false);
                }
                else
                {
                    Debug.Log("Bomb captured, locating its position");
                    // Searches jail for the corresponding captured bomb
                    for (int i = 0; i < teamSize; i++)
                    {
                        if (jail.NavyJailCells[i].GetComponent<TTJailCell>().currentPiece == tileSelected.GetComponent<TTSquare>().currentPiece.GetComponent<TuBomber>().capturedBomb)
                        {
                            cellToHighlight = i;
                        }
                    }
                }

                if (cellToHighlight >= 0)
                {
                    jail.NavyJailCells[cellToHighlight].GetComponent<TTJailCell>().interactable = true;
                }

            }
            // The selected bomber is Pirate and can deploy Navy Bombs
            else
            {
                // The bomber hasn't captured a bomb yet
                if (tileSelected.GetComponent<TTSquare>().currentPiece.GetComponent<TuBomber>().capturedBomb == null)
                {
                    Debug.Log("No bombs captured, searching for one");
                    cellToHighlight = jail.FindPiece(TTPieceType.LandMine, true);
                }
                else
                {
                    Debug.Log("Bomb captured, locating its position");
                    // Searches jail for the corresponding captured bomb
                    for (int i = 0; i < teamSize; i++)
                    {
                        if (jail.PirateJailCells[i].GetComponent<TTJailCell>().currentPiece == tileSelected.GetComponent<TTSquare>().currentPiece.GetComponent<TuBomber>().capturedBomb)
                        {
                            cellToHighlight = i;
                        }
                    }
                }

                if (cellToHighlight >= 0)
                {
                    jail.PirateJailCells[cellToHighlight].GetComponent<TTJailCell>().interactable = true;
                }
            }
        }

        // Highlights all possible pieces that tactician can inherit
        if (tacticianSelected)
        {
            for (int i = 0; i < 9; i++)
            {
                if (jail.TacticianMimicCells[i].GetComponent<TTJailCell>().hasPiece == true) {
                    // Tactician has inherited a piece
                    if (tacticianInheritSelected)
                    {
                        // Jail cell in question was not selected (stop flashing)
                        if (!jail.TacticianMimicCells[i].GetComponent<TTJailCell>().clicked)
                        {
                            jail.TacticianMimicCells[i].GetComponent<TTJailCell>().interactable = false;
                        }
                    }
                    // Tactician has not yet inherited a piece (start flashing)
                    else if (jail.TacticianMimicCells[i].GetComponent<TTJailCell>().interactable == false)
                    {
                        TTJailCell tacticianCell = jail.TacticianMimicCells[i].GetComponent<TTJailCell>();
                        tacticianCell.interactable = true;
                    }
                }
            }
        }

        // Highlights the captured ore that needs redeployment
        if (resetOre && cellToHighlight == -2)
        {
            // Navy Ore needs redeployment
            if (!tileSelected.GetComponent<TTSquare>().currentPiece.isNavy)
            {
                cellToHighlight = jail.FindPiece(TTPieceType.Ore, false);
                Debug.Log(cellToHighlight);
                if (cellToHighlight >= 0)
                {
                    jail.NavyJailCells[cellToHighlight].GetComponent<TTJailCell>().interactable = true;
                }
            }
            // Pirate Ore needs redeployment
            else
            {
                cellToHighlight = jail.FindPiece(TTPieceType.Ore, true);
                Debug.Log(cellToHighlight);
                if (cellToHighlight >= 0)
                {
                    jail.PirateJailCells[cellToHighlight].GetComponent<TTJailCell>().interactable = true;
                }
            }
        }

        // Detect a square has been clicked
        if (Input.GetMouseButtonDown(0) && !gameWon)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // A square has been clicked
            if (hit.collider != null)
            {
                tileSelected = GameObject.Find(hit.collider.name);

                // A jail cell was clicked
                if (tileSelected.tag == "JailCell")
                {
                    // Do nothing
                }

                // No square had previously been clicked
                else if (!squareSelected)
                {
                    TTSquare current_square = tileSelected.GetComponent<TTSquare>();
                    if (current_square.currentPiece != null)
                    {
                        // It's not your turn yet!
                        if (navyTurn != playerIsNavy)
                        {
                            boardUI.DisplayTempText("It's not your turn yet!", 1.5f);
                            Debug.Log("It's not your turn!");
                            TTSquare selectedTile = tileSelected.GetComponent<TTSquare>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);
                        }
                        else if (current_square.currentPiece.isNavy != navyTurn)
                        {
                            boardUI.DisplayTempText("That's not your piece!", 1.5f);
                            Debug.Log("Wrong piece selected!");
                            TTSquare selectedTile = tileSelected.GetComponent<TTSquare>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);
                        }
                        // The wrong team is trying to move
                        else if (current_square.currentPiece.isNavy != navyTurn)
                        {
                            boardUI.DisplayTempText("It's not your turn yet!", 1.5f);
                            Debug.Log("It's not your turn!");
                            TTSquare selectedTile = tileSelected.GetComponent<TTSquare>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);
                        }
                        // The right team is trying to move
                        else
                        {
                            if (current_square.currentPiece.type == TTPieceType.Royal2 && current_square.currentPiece.isNavy)
                            {
                                tacticianSelected = true;
                            }
                            if (current_square.currentPiece.type == TTPieceType.Bomber)
                            {
                                bomberSelected = true;
                            }

                            // Finds enemy bombs in jail cells
                            if (bomberSelected)
                            {
                                // Tactician is mimicking a bomber
                                if (tacticianInheritSelected)
                                {
                                    landMineInJail = jail.FindPiece(TTPieceType.LandMine, false) >= 0;
                                }
                                // The selected bomber is Navy and can deploy Pirate Bombs
                                else if (tileSelected.GetComponent<TTSquare>().currentPiece.isNavy)
                                {
                                    landMineInJail = jail.FindPiece(TTPieceType.LandMine, false) >= 0;
                                }
                                // The selected bomber is Pirate and can deploy Navy Bombs
                                else
                                {
                                    landMineInJail = jail.FindPiece(TTPieceType.LandMine, true) >= 0;
                                }
                            }

                            storedTileSelected = tileSelected;
                            squareSelected = true;
                            current_square.SquareHasBeenClicked = true;
                            DetectLegalMoves(tileSelected, current_square.currentPiece);
                        }

                    }
                }
                // A square had been previously clicked
                else
                {
                    // A moveable square has been clicked (or the corsair has jumped to an open space)
                    if (tileSelected.tag == "MoveableSquare" || tileSelected.tag == "CorsairJump")
                    {
                        TTSquare currentSquare = storedTileSelected.GetComponent<TTSquare>();
                        TTPiece currentPiece = currentSquare.currentPiece;
                        TTSquare targetSquare = tileSelected.GetComponent<TTSquare>();

                        Vector2Int currentSquareCoords = IdentifyThisBoardSquare(storedTileSelected);
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        GameplayMovePiece(currentSquare, targetSquare, currentPiece, currentSquareCoords, moveCoordinates, tileSelected.tag == "CorsairJump");
                    }

                    // A capturable piece has been clicked (regular pieces or gunners)
                    else if (tileSelected.tag == "CaptureSquare" || tileSelected.tag == "GunnerTarget")
                    {
                        TTSquare CurrentSquare = storedTileSelected.GetComponent<TTSquare>();
                        TTSquare targetSquare = tileSelected.GetComponent<TTSquare>();
                        TTPiece currentPiece = CurrentSquare.currentPiece;
                        TTPiece capturedPiece = targetSquare.currentPiece;
                        Vector2Int currentCoordinates = IdentifyThisBoardSquare(storedTileSelected);
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        NetCapturePiece cp = new NetCapturePiece();

                        Debug.Log("skip server msg");

                        GameplayCapturePiece(CurrentSquare, targetSquare, currentPiece, capturedPiece, moveCoordinates, tileSelected.tag == "GunnerTarget");
                    }

                    // Cannon is capturing a piece by jumping
                    else if (tileSelected.tag == "CannonDestination")
                    {
                        TTSquare CurrentSquare = storedTileSelected.GetComponent<TTSquare>();
                        TTSquare targetSquare = tileSelected.GetComponent<TTSquare>();
                        TTPiece currentPiece = CurrentSquare.currentPiece;
                        Vector2Int currentCoordinates = IdentifyThisBoardSquare(storedTileSelected);
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        // Find which piece is being captured
                        TTSquare captureSquare;
                        TTPiece capturedPiece = null;

                        int captureDirection = -1;

                        // Checks the square to the right of move square
                        if (moveCoordinates.x + 1 < 10)
                        {
                            Debug.Log("Checked Right");
                            if (tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<TTSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Right");
                                captureSquare = tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<TTSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 1;
                            }
                        }
                        // Checks the square to the left of move square
                        if (moveCoordinates.x - 1 >= 0 && capturedPiece == null)
                        {
                            Debug.Log("Checked Left");
                            if (tiles[moveCoordinates.x - 1, moveCoordinates.y].GetComponent<TTSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Left");
                                captureSquare = tiles[moveCoordinates.x - 1, moveCoordinates.y].GetComponent<TTSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 3;
                            }
                        }
                        // Checks the square above the move square
                        if (moveCoordinates.y + 1 < 10 && capturedPiece == null)
                        {
                            Debug.Log("Checked Up");
                            if (tiles[moveCoordinates.x, moveCoordinates.y + 1].GetComponent<TTSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Up");
                                captureSquare = tiles[moveCoordinates.x, moveCoordinates.y + 1].GetComponent<TTSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 0;
                            }
                        }
                        // Checks the square below the move square
                        if (moveCoordinates.y - 1 >= 0 && capturedPiece == null)
                        {
                            Debug.Log("Checked Down");
                            if (tiles[moveCoordinates.x, moveCoordinates.y - 1].GetComponent<TTSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Down");
                                captureSquare = tiles[moveCoordinates.x, moveCoordinates.y - 1].GetComponent<TTSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 2;
                            }
                        }

                        NetCannonCapture cc = new NetCannonCapture();

                        GameplayCannonCapture(CurrentSquare, targetSquare, currentPiece, capturedPiece, moveCoordinates);
                    }

                    // A landmine has been selected from jail or a tactician is mimicking a piece
                    else if (tileSelected.tag == "InteractablePiece")
                    {
                        // A landmine is being interacted with
                        if (bomberSelected)
                        {
                            // The user clicked on the landmine again (cancel selection and return to normal moveset)
                            if (landMineSelected)
                            {
                                landMineSelected = false;
                                cellToHighlight = -2;
                                ResetBoardMaterials();
                                tileSelected = storedTileSelected;
                                DetectLegalMoves(tileSelected, tileSelected.GetComponent<TTSquare>().currentPiece);
                            }
                            else
                            {
                                landMineSelected = true;
                                tileSelected.GetComponent<TTJailCell>().clicked = true;
                                ResetBoardMaterials(false);
                                DetectLegalMoves(storedTileSelected, storedTileSelected.GetComponent<TTSquare>().currentPiece);
                            }
                        }

                        // The tactician is trying to mimic a piece
                        if (tacticianSelected)
                        {
                            // The user clicked on the inherited piece again (cancel selection and return to normal moveset)
                            if (tacticianInheritSelected)
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    if (jail.TacticianMimicCells[i].GetComponent<TTJailCell>().hasPiece)
                                        jail.TacticianMimicCells[i].GetComponent<TTJailCell>().interactable = true;
                                }

                                tacticianInheritSelected = false;
                                cellToHighlight = -2;
                                tileSelected = storedTileSelected;
                                ResetBoardMaterials(true);
                                ResetBoardMaterials(true);
                                DetectLegalMoves(tileSelected, storedTileSelected.GetComponent<TTSquare>().currentPiece);
                            }
                            // The user clicked on a piece to inherit
                            else
                            {
                                tacticianInheritSelected = true;

                                for (int i = 0; i < 9; i++)
                                {
                                    jail.TacticianMimicCells[i].GetComponent<TTJailCell>().interactable = false;
                                }

                                tileSelected.GetComponent<TTJailCell>().clicked = true;
                                ResetBoardMaterials(false);

                                TTSquare currentSquare = storedTileSelected.GetComponent<TTSquare>();
                                TTPiece inheritingPiece = tileSelected.GetComponent<TTJailCell>().currentPiece;

                                Vector2Int currentPosition = IdentifyThisBoardSquare(storedTileSelected);

                                DetectLegalMoves(storedTileSelected, inheritingPiece);

                                tileSelected = storedTileSelected;
                            }
                        }
                    }

                    // A Land Mine is being redeployed to the board
                    else if (tileSelected.tag == "MineDeploy")
                    {
                        Debug.Log("Deploying mine");
                        TTSquare mineSquare = storedTileSelected.GetComponent<TTSquare>();
                        Vector2Int deployCoordinates = IdentifyThisBoardSquare(tileSelected);

                        TTPiece shieldPiece;
                        if (!mineSquare.currentPiece.isNavy)
                            shieldPiece = jail.navyJailedPieces[cellToHighlight].GetComponent<TTPiece>();
                        else
                            shieldPiece = jail.pirateJailedPieces[cellToHighlight].GetComponent<TTPiece>();

                        GameplayDeployPiece(shieldPiece, deployCoordinates, cellToHighlight, 0);
                    }

                    // The ore is being redeployed to the board
                    else if (tileSelected.tag == "OreDeploy")
                    {
                        Debug.Log("Redeploying Ore");
                        TTSquare pieceSquare = storedTileSelected.GetComponent<TTSquare>();
                        Vector2Int deployCoordinates = IdentifyThisBoardSquare(tileSelected);

                        cellToHighlight = jail.FindPiece(TTPieceType.Ore, pieceSquare.currentPiece.isNavy);

                        TTPiece orePiece;
                        if (pieceSquare.currentPiece.isNavy || tacticianSelected)
                            orePiece = jail.navyJailedPieces[cellToHighlight].GetComponent<TTPiece>();
                        else
                            orePiece = jail.pirateJailedPieces[cellToHighlight].GetComponent<TTPiece>();

                        Debug.Log(cellToHighlight);

                        int turnOver = orebearerSecondMove ? 2 : 1;

                        GameplayDeployPiece(orePiece, deployCoordinates, cellToHighlight, turnOver);

                        // Return control to the orebearer for a second turn
                        if(orebearerSecondMove)
                        {
                            pieceSquare.SquareHasBeenClicked = true;
                            DetectLegalMoves(storedTileSelected, pieceSquare.currentPiece);
                        }
                    }

                    // The same square was clicked twice (cancel the square selection)
                    else if (tileSelected.GetComponent<TTSquare>().SquareHasBeenClicked)
                    {
                        ResetBoardMaterials(true);
                        boardUI.GoalText("Click on a piece to move it!");

                        // Ends turn if orebearer decides not to move a second time
                        if (tileSelected.GetComponent<TTSquare>().currentPiece.hasOre && orebearerSecondMove)
                        {
                            // Send a blank move message to the opponent so they can end the turn
                            if (PieceManager.instance.onlineMultiplayer)
                            {
                                TTSquare currentSquare = tileSelected.GetComponent<TTSquare>();
                                TTPiece currentPiece = currentSquare.currentPiece;

                                NetMovePiece mp = new NetMovePiece();
                                mp.teamID = currentPiece.isNavy ? 0 : 1;
                                mp.originalX = currentPiece.currentX;
                                mp.originalY = currentPiece.currentY;
                                mp.targetX = currentPiece.currentX;
                                mp.targetY = currentPiece.currentY;
                                mp.corsairJump = 0;

                                Client.Instance.SendToServer(mp);
                            }

                            orebearerSecondMove = false;
                            string debug = "update turn";
                            NextTurn(debug);
                        }

                        landMineInJail = false;
                        squareSelected = false;
                        tileSelected.GetComponent<TTSquare>().SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                        bomberSelected = false;
                        landMineSelected = false;
                    }
                }
            }
        }
    }

    private void GameplayMovePiece(TTSquare currentSquare, TTSquare targetSquare, TTPiece currentPiece, Vector2Int currentSquareCoords, Vector2Int moveCoordinates, bool corsairJump = false)
    {
        movementAudio.Play();

        // Corsair jumping (or tactician imitating) requires a cooldown
        if (corsairJump)
        {
            // Corsair jumping requires 2 turns of cooldown before the next jump
            if (currentPiece.type == TTPieceType.Royal2 && !currentPiece.isNavy)
                jumpCooldown = 3;
            // Tactician inherits a corsair
            else
            {
                tacticianCorsairJump = 3;
            }
        }
        else
        {
            // Checks if the piece is a Tactician that has moved since mimicking a Gunner
            if (currentPiece.type == TTPieceType.Royal2 && currentPiece.isNavy)
            {
                tacticianGunnerCapture = false;
            }

            if (currentPiece.type == TTPieceType.Gunner && currentPiece.hasCaptured)
            {
                gunnerRecharge.Play();
                currentPiece.hasCaptured = false;
            }
        }

        MovePiece(currentPiece, moveCoordinates.x, moveCoordinates.y);

        ResetBoardMaterials();
        targetSquare.FlashMaterial(targetSquare.moveableBoardMaterial, 3);
        squareSelected = false;
        currentSquare.SquareHasBeenClicked = false;
        tileSelected = null;
        storedTileSelected = null;
        string debug = "GameplayMovePiece turn";
        NextTurn(debug);
    }

    private void GameplayCapturePiece(TTSquare CurrentSquare, TTSquare targetSquare, TTPiece currentPiece, TTPiece capturedPiece, Vector2Int moveCoordinates, bool gunnerCapture = false, bool turnOver = true)
    {
        // Check if capture target is the ore
        if (capturedPiece.type == TTPieceType.Ore)
        {
            currentPiece.hasOre = true;
            currentPiece.type = TTPieceType.Mate; // Gets rid of any fancy moves at their disposal

            // Update UI
        
            boardUI.UpdateGoal(navyTurn, true);
            Debug.Log("update ui");
        }

        // If the orebearer is being captured, the ore needs to be reset (handled by opponent in multiplayer)
        if (capturedPiece.hasOre)
        {
            resetOre = true;
            turnOver = false;
        }

        // Capture that Piece
        jail.InsertAPiece(capturedPiece);
        capturedPiece.destroyPiece();
        currentPiece.hasCaptured = true;

        // Links the engineer with his captured shield
        if (capturedPiece.type == TTPieceType.LandMine && currentPiece.type == TTPieceType.Bomber)
        {
            int bombJailIndex = jail.FindLastSlot(!currentPiece.isNavy);
            Debug.Log(bombJailIndex);
            if (currentPiece.isNavy)
            {
                currentPiece.GetComponent<TuBomber>().capturedBomb = jail.pirateJailedPieces[bombJailIndex];
            }
            else
            {
                currentPiece.GetComponent<TuBomber>().capturedBomb = jail.navyJailedPieces[bombJailIndex];
            }
        }

        // Prevents the Tactician from mimicking a Gunner and capturing twice
        if (gunnerCapture)
        {
            gunnerAudio.Play();

            if (currentPiece.type == TTPieceType.Royal2 && currentPiece.isNavy)
            {
                tacticianGunnerCapture = true;
            }
        }
        else
        {
            Capture.Play();
        }

        // Move current piece to the new square (unless it's a gunner)
        if (!gunnerCapture)
        {
            MovePiece(currentPiece, moveCoordinates.x, moveCoordinates.y);
        }
        else
        {
            AnimGun(currentPiece, moveCoordinates.x, moveCoordinates.y);
        }
        CurrentSquare.SquareHasBeenClicked = false;
        targetSquare.FlashMaterial(targetSquare.clickedBoardMaterial, 2);

        // Clean up board now that move has completed
        ResetBoardMaterials();

        // Ore needs to be reset before the turn ends
        if (resetOre)
        {
            if (currentPiece.type == TTPieceType.Gunner || tacticianInheritSelected)
            {
                tileSelected = storedTileSelected;
            }
            else
            {
                storedTileSelected = tileSelected;
            }
            targetSquare.tag = "CaptureSquare";
            DetectLegalMoves(storedTileSelected, currentPiece);

            // Update UI
            boardUI.UpdateGoal(!navyTurn, false);
        }
        // The orebearer just captured and gets to take a second turn
        if (currentPiece.hasOre && !orebearerSecondMove)
        {
            orebearerSecondMove = true;
            turnOver = false;
            if (currentPiece.type == TTPieceType.Gunner)
            {
                CurrentSquare.SquareHasBeenClicked = true;
                currentPiece.type = TTPieceType.Mate;
            }
            else
            {
                storedTileSelected = tileSelected;
                targetSquare.SquareHasBeenClicked = true;
            }

            DetectLegalMoves(storedTileSelected, currentPiece);
        }
        // The orebearer has just taken a second turn
        else if (currentPiece.hasOre && orebearerSecondMove)
        {
            orebearerSecondMove = false;
        }

        // Turn is now over
        if (turnOver)
        {
            squareSelected = false;
            tileSelected = null;
            storedTileSelected = null;
            ResetBoardMaterials();
            string debug = "GameplayCapturePiece turn";
            NextTurn(debug);
        }
    }

    private void GameplayCannonCapture(TTSquare CurrentSquare, TTSquare targetSquare, TTPiece currentPiece, TTPiece capturedPiece, Vector2Int moveCoordinates, bool turnOver = true)
    {
        // A piece is being captured
        if (capturedPiece != null)
        {
            // Check if the captured piece is the ore
            if (capturedPiece.type == TTPieceType.Ore)
            {
                currentPiece.hasOre = true;
                currentPiece.type = TTPieceType.Mate; // Gets rid of any fancy moves at their disposal

                // Update UI
                boardUI.UpdateGoal(navyTurn, true);
            }

            // If the orebearer is being captured, the ore needs to be reset
            if (capturedPiece.hasOre)
            {
                resetOre = true;
                turnOver = false;
            }
            

            // Blanks out the captured piece's square
            GameObject captureTile = FindThisBoardSquare(capturedPiece.currentX + 1, capturedPiece.currentY + 1);
            TTSquare captureSquare = captureTile.GetComponent<TTSquare>();
            captureSquare.currentPiece = null;

            // Capture that piece
            jail.InsertAPiece(capturedPiece);
            capturedPiece.destroyPiece();
            currentPiece.hasCaptured = true;
        }

        // Move current piece to that square
        MovePiece(currentPiece, moveCoordinates.x, moveCoordinates.y);
        CurrentSquare.SquareHasBeenClicked = false;
        targetSquare.FlashMaterial(targetSquare.clickedBoardMaterial, 2);

        // Clean up board now that move has completed
        ResetBoardMaterials();

        // Ore needs to be reset before the turn ends
        if (resetOre)
        {
            storedTileSelected = tileSelected;
            targetSquare.tag = "CaptureSquare";
            DetectLegalMoves(storedTileSelected, currentPiece);
        }
        // The orebearer just captured and gets to take a second turn
        if (currentPiece.hasOre && !orebearerSecondMove)
        {
            orebearerSecondMove = true;
            turnOver = false;
            storedTileSelected = tileSelected;
            targetSquare.SquareHasBeenClicked = true;

            DetectLegalMoves(storedTileSelected, currentPiece);
        }
        // The orebearer has just taken a second turn
        else if (currentPiece.hasOre && orebearerSecondMove)
        {
            orebearerSecondMove = false;
        }
        
        // Turn is now over
        if (turnOver)
        {
            squareSelected = false;
            tileSelected = null;
            storedTileSelected = null;
            ResetBoardMaterials();
            string debug = "GameplayCannonCapture turn";
            NextTurn(debug);
        }
    }

    private void GameplayDeployPiece(TTPiece currentPiece, Vector2Int deployCoordinates, int jailIndex, int deployPieceType)
    {
        Debug.Log("Redeploying Piece");

        int spawnIndex = FindFirstOpenTeamSlot(currentPiece.isNavy);

        if (currentPiece.isNavy)
        {
            jail.navyJailedPieces[jailIndex].GetComponent<TTPiece>().destroyPiece();
            jail.PirateJailCells[jailIndex].GetComponent<TTJailCell>().resetCell();

            // if(PieceManager.instance.onlineMultiplayer && !playerIsNavy)
            // {
            if (deployPieceType == 0)
                NavyPieces[spawnIndex] = SpawnPiece(TTPieceType.LandMine, true, deployCoordinates.x, deployCoordinates.y, true);
            else
                NavyPieces[spawnIndex] = SpawnPiece(TTPieceType.Ore, true, deployCoordinates.x, deployCoordinates.y, true);
            // }
            // else
            // {
            //     if (deployPieceType == 0)
            //         NavyPieces[spawnIndex] = SpawnPiece(TTPieceType.LandMine, true, deployCoordinates.x, deployCoordinates.y);
            //     else
            //         NavyPieces[spawnIndex] = SpawnPiece(TTPieceType.Ore, true, deployCoordinates.x, deployCoordinates.y);
            // }

        }
        else
        {
            jail.pirateJailedPieces[jailIndex].GetComponent<TTPiece>().destroyPiece();
            jail.NavyJailCells[jailIndex].GetComponent<TTJailCell>().resetCell();

            if (!playerIsNavy)
            {
                if (deployPieceType == 0)
                    PiratePieces[spawnIndex] = SpawnPiece(TTPieceType.LandMine, false, deployCoordinates.x, deployCoordinates.y, true);
                else
                    PiratePieces[spawnIndex] = SpawnPiece(TTPieceType.Ore, false, deployCoordinates.x, deployCoordinates.y, true);
            }
            else
            {
                if (deployPieceType == 0)
                    PiratePieces[spawnIndex] = SpawnPiece(TTPieceType.LandMine, false, deployCoordinates.x, deployCoordinates.y);
                else
                    PiratePieces[spawnIndex] = SpawnPiece(TTPieceType.Ore, false, deployCoordinates.x, deployCoordinates.y);
            }

        }

        bool navyMove;

        if(deployPieceType == 0)
        {
            navyMove = !currentPiece.isNavy;
        }
        else
        {
            navyMove = currentPiece.isNavy;
        }

        Debug.Log(navyMove);

        // Clean up now that piece has been redeployed
        tileSelected.GetComponent<TTSquare>().SquareHasBeenClicked = false;

        tileSelected = null;
        bomberSelected = false;
        landMineSelected = false;
        resetOre = false;
        ResetBoardMaterials();


        // Turn is now over (unless the orebearer still needs to make another move)
        if(deployPieceType <= 1)
        {
            storedTileSelected.GetComponent<TTSquare>().SquareHasBeenClicked = false;

            squareSelected = false;
            storedTileSelected = null;
            string debug = "GameplayDeployPiece turn";
            NextTurn(debug);
        }
    }

    private void IdentifyBoardSquares()
    {
        for (int x = 1; x <= TILE_COUNT_X; x++)
        {
            for (int y = 1; y <= TILE_COUNT_Y; y++)
            {
                GameObject boardSquare = FindThisBoardSquare(x, y);

                boardSquare.tag = "GameSquare";
                
                tiles[x - 1, y - 1] = boardSquare;
            }
        }
    }

    private GameObject FindThisBoardSquare(int x, int y)
    {
        string piecename = "(" + x + "," + y + ")";
        GameObject boardSquare = GameObject.Find(piecename);

        return boardSquare;
    }

    private void ResetBoardMaterials(bool resetJail = true)
    {
        if (resetJail)
        {
            // Resets materials for any jail cells that were interacted with
            jail.resetMaterials();
            cellToHighlight = -2;
        }

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                tiles[x, y].tag = "GameSquare";
                TTSquare square = tiles[x, y].GetComponent<TTSquare>();
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

    private void SetCurrentPiece(TTPiece piece, int x, int y)
    {
        tiles[x, y].GetComponent<TTSquare>().currentPiece = piece;
    }

    private void NullCurrentPiece(int x, int y)
    {
        tiles[x, y].GetComponent<TTSquare>().currentPiece = null;
    }

    public TTPiece SpawnPiece(TTPieceType type, bool isNavy, int startingX = -1, int startingY = -1, bool pirateRotate = false)
    {
        TTPiece cp;

        if (!isNavy)
        {
            cp = Instantiate(PiecePrefabs[(int)type + PIECES_ADDED], this.transform).GetComponent<TTPiece>();
        }
        else
        {
            cp = Instantiate(PiecePrefabs[(int)type], this.transform).GetComponent<TTPiece>();
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

    public void MovePiece(TTPiece piece, int x, int y, bool lerpMove = true)
    {
        pieceMoving = true;

        Vector3 targetPosition = tiles[x, y].transform.position;

        // Removes piece from original square and puts it in new square
        if(piece.currentX != -1 && piece.currentY != -1)
        {
            NullCurrentPiece(piece.currentX, piece.currentY);
        }
        tiles[x, y].GetComponent<TTSquare>().currentPiece = piece;
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
            pieceMoving = false;
        }
    }

    public void AnimGun(TTPiece piece, int x, int y)
    {
        Vector3 startPosition = piece.transform.position;
        Vector3 targetPosition = tiles[x, y].transform.position;
        bool isNavy = piece.isNavy;
        StartCoroutine(boardUI.AnimGunner(startPosition, targetPosition, isNavy));
    }

    IEnumerator LerpPosition(TTPiece piece, Vector3 targetPosition, float duration = 0.1f)
    {
        pieceMoving = true;
        float time = 0;
        Vector3 startPosition = piece.transform.position;
        while (time < duration)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = targetPosition;

        pieceMoving = false;
    }

    private void DetectLegalMoves(GameObject current, TTPiece piece)
    {
        TTSquare current_square;
        int current_x = IdentifyThisBoardSquare(current).x;
        int current_y = IdentifyThisBoardSquare(current).y;

        bool invalidPiece = false;
        bool moveAble = false;
        bool captureAble = false;
        bool gunnerAble = false;
        bool cannonTarget = false;
        bool cannonJump = false;
        bool mineDeploy = false;
        bool oreDeploy = false;
        bool corsairJump = false;

        moveAssessment = new int[TILE_COUNT_X, TILE_COUNT_Y];

        current_square = tiles[current_x, current_y].GetComponent<TTSquare>();

        if (piece.hasOre && !resetOre)
        {
            moveAssessment = piece.GetComponent<TTPiece>().GetValidMovesOre(tiles);
        }
        else if (resetOre)
        {
            moveAssessment = piece.GetComponent<TTPiece>().GetValidOreReset(tiles);
        }
        else
        {
            switch (piece.type)
            {
                case TTPieceType.Ore:
                    boardUI.DisplayTempText("The Ore can't move, click on a different piece!", 1.5f);
                    Debug.Log("The Ore doesn't move!");
                    invalidPiece = true;
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<TTSquare>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case TTPieceType.LandMine:
                    boardUI.DisplayTempText("Land Mines can't move, click on a different piece!", 1.5f);
                    Debug.Log("The Land Mine doesn't move!");
                    invalidPiece = true;
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<TTSquare>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case TTPieceType.Mate:
                    moveAssessment = piece.GetComponent<TuMate>().GetValidMoves(tiles);
                    break;
                case TTPieceType.Bomber:
                    if (landMineSelected)
                    {
                        moveAssessment = piece.GetComponent<TuBomber>().DetectBombDeploy(tiles);
                    }
                    else
                    {
                        moveAssessment = piece.GetComponent<TuBomber>().GetValidMoves(tiles, landMineInJail);
                    }
                    break;
                case TTPieceType.Vanguard:
                    moveAssessment = piece.GetComponent<TuVanguard>().GetValidMoves(tiles);
                    break;
                case TTPieceType.Navigator:
                    moveAssessment = piece.GetComponent<TuNavigator>().GetValidMoves(tiles);
                    break;
                case TTPieceType.Gunner:
                    moveAssessment = piece.GetComponent<TuGunner>().GetValidMoves(tiles);
                    break;
                case TTPieceType.Cannon:
                    moveAssessment = piece.GetComponent<TuCannon>().GetValidMoves(tiles);
                    break;
                case TTPieceType.Quartermaster:
                    moveAssessment = piece.GetComponent<TuQuartermaster>().GetValidMoves(tiles);
                    break;
                case TTPieceType.Royal2:
                    if (piece.isNavy && !tacticianInheritSelected)
                    {
                        moveAssessment = piece.GetComponent<TuTactician>().GetValidMoves(tiles);
                    }
                    else
                    {
                        if(jumpCooldown > 0 && !tacticianSelected)
                        {
                            moveAssessment = piece.GetComponent<TuCorsair>().GetValidMoves(tiles, false);
                        }
                        else if(tacticianCorsairJump > 0 && tacticianSelected)
                        {
                            moveAssessment = piece.GetComponent<TuCorsair>().GetValidMoves(tiles, false);
                        }
                        else
                        {
                            moveAssessment = piece.GetComponent<TuCorsair>().GetValidMoves(tiles);
                        }
                    }
                    break;
                case TTPieceType.Royal1:
                    if (piece.isNavy && !tacticianInheritSelected)
                    {
                        moveAssessment = piece.GetComponent<TuAdmiral>().GetValidMoves(tiles);
                    }
                    else
                    {
                        moveAssessment = piece.GetComponent<TuCaptain>().GetValidMoves(tiles);
                    }
                    break;
                default:
                    Debug.Log("Cannot determine piece moveset");
                    break;
            }
        }
        
        // Ensures that an enemy is not moving to a landmine or allied occupied square
        for (int x = 0; x < TILE_COUNT_X && !piece.hasOre; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(moveAssessment[x,y] == 1)
                {
                    TTPiece possiblePiece = tiles[x, y].GetComponent<TTSquare>().currentPiece;

                    // There is a piece in that possible move square
                    if(possiblePiece != null)
                    {
                        // That piece is on the same team (can't move there)
                        if(piece.isNavy == possiblePiece.isNavy)
                        {
                            moveAssessment[x, y] = -1;
                        }
                        // That piece is a land mine (can't move there)
                        else if(possiblePiece.type == TTPieceType.LandMine)
                        {
                            if(piece.type == TTPieceType.Bomber)
                            {
                                moveAssessment[x, y] = 2;
                            }
                            else
                            {
                                moveAssessment[x, y] = -1;
                            }
                        }
                        // That piece is an enemy piece that can be captured
                        else
                        {
                            // The current Piece is a Gunner or Cannon that can't capture regularly
                            if (piece.type == TTPieceType.Gunner || piece.type == TTPieceType.Cannon)
                            {
                                moveAssessment[x, y] = -1;
                            }
                            // That piece can be captured
                            else
                            {
                                moveAssessment[x, y] = 2;
                            }
                        }
                    }
                }
            }
        }

        // Establishes squares that can be moved to or captured in
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                // Square can be moved to
                if(moveAssessment[x,y] == 1)
                {
                    tiles[x, y].tag = "MoveableSquare";
                    TTSquare activeSquare = tiles[x,y].GetComponent<TTSquare>();
                    activeSquare.SetMaterial(activeSquare.moveableBoardMaterial);
                    moveAble = true;
                }
                // Square contains a capturable piece by replacement
                else if (moveAssessment[x,y] == 2)
                {
                    tiles[x, y].tag = "CaptureSquare";
                    TTSquare activeSquare = tiles[x, y].GetComponent<TTSquare>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    captureAble = true;
                }
                // Square contains a capturable piece by shooting
                else if(moveAssessment[x,y] == 3)
                {
                    if((tacticianSelected && !tacticianGunnerCapture) || !tacticianSelected)
                    {
                        tiles[x, y].tag = "GunnerTarget";
                        TTSquare activeSquare = tiles[x, y].GetComponent<TTSquare>();
                        activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                        gunnerAble = true;
                    }
                }
                // Square contains a capturable peice by jumping
                else if(moveAssessment[x,y] == 4)
                {
                    // Cannon can jump a Land Mine but not capture it
                    if (tiles[x, y].GetComponent<TTSquare>().currentPiece.type != TTPieceType.LandMine)
                    {
                        tiles[x, y].tag = "CannonTarget";
                        TTSquare activeSquare = tiles[x, y].GetComponent<TTSquare>();
                        activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                        cannonTarget = true;
                    }
                }
                // Square can be moved to for cannon jumping capture
                else if (moveAssessment[x, y] == 5)
                {
                    tiles[x, y].tag = "CannonDestination";
                    cannonJump = true;
                }
                // A Land Mine can be deployed here
                else if (moveAssessment[x,y] == 6)
                {
                    tiles[x, y].tag = "MineDeploy";
                    TTSquare activeSquare = tiles[x, y].GetComponent<TTSquare>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    mineDeploy = true;
                }
                // The ore can be deployed here
                else if (moveAssessment[x,y] == 7)
                {
                    tiles[x, y].tag = "OreDeploy";
                    TTSquare activeSquare = tiles[x, y].GetComponent<TTSquare>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    oreDeploy = true;
                }

                // Corsair can jump here
                else if (moveAssessment[x, y] == 8)
                {
                    tiles[x, y].tag = "CorsairJump";
                    corsairJump = true;
                }
            }
        }

        // Displays current possible game actions in goal text
        if (!invalidPiece && !moveAble && !captureAble && !gunnerAble && !cannonJump && !mineDeploy && !oreDeploy && !corsairJump)
        {
            // A land mine has been selected but it can't go anywhere
            if (landMineSelected)
            {
                boardUI.GoalText("There are no valid spots for the Land Mine to go.");
                boardUI.GoalText("Click on it again to cancel.", true);
            }
            else
            {
                // No move is available with this piece
                boardUI.GoalText("This piece has no valid moves. Click on it again to cancel.");
            }
        }
        else if (!invalidPiece)
        {
            boardUI.GoalText("");
            // The orebearer can move again
            if (orebearerSecondMove)
            {
                boardUI.GoalText("The orebearer has just captured, and can make a second move.", true);
            }

            // A move is available with this piece
            if (moveAble)
            {
                boardUI.GoalText("- Click on a green square to move there", true);
            }
            // A capture is available with this piece
            if (captureAble)
            {
                boardUI.GoalText("- Click on a red square to capture that piece", true);
            }
            // The tactician can inherit a piece
            if (tacticianSelected && !tacticianInheritSelected)
            {
                if(jail.tacticianMimicPieces[0] != null)
                    boardUI.GoalText("- Click on a flashing piece to mimic that piece's moves for a turn", true);
            }
            // A mine can be redeployed
            if(bomberSelected && !landMineSelected && landMineInJail)
            {
                boardUI.GoalText("- Click on a flashing Land Mine to redeploy it to the board", true);
            }
            // A gunner can capture by shooting
            if (gunnerAble)
            {
                boardUI.GoalText("- Click on a red square to shoot that piece from a distance", true);
            }
            // The cannon can capture a piece
            if (cannonTarget)
            {
                boardUI.GoalText("- Click on a flashing green square to capture the red highlighted piece", true);
            }
            // The cannon can jump to a square
            else if (cannonJump)
            {
                boardUI.GoalText("- Click on a flashing green square to jump there", true);
            }
            // A mine is being redeployed
            if (mineDeploy)
            {
                boardUI.GoalText("- Click on a red square to redeploy the bomb there", true);
                boardUI.GoalText("- Click on that Land Mine again to cancel the redeploy", true);
            }
            // The ore is being redeployed
            if (oreDeploy)
            {
                boardUI.GoalText("- Click on a red square to redeploy the ore there", true);
            }
            // The corsair can jump to a square
            if (corsairJump)
            {
                boardUI.GoalText("- Click on a flashing green square to jump there", true);
            }
            // The tactician is inheriting a piece
            if(tacticianSelected && tacticianInheritSelected)
            {
                boardUI.GoalText("- Click on the mimicked piece again to return to a normal move", true);
            }
            // The corsair needs to cool down after a jump
            if(jumpCooldown > 0 && piece.type == TTPieceType.Royal2 && !piece.isNavy)
            {
                boardUI.GoalText("- Make sure to move between jumps", true);
            }

            // Default message
            boardUI.GoalText("- Click on that piece again to cancel the move", true);
        }
    }

    private void SpawnAllPieces()
    {
        int navyPiecesAdded = 0;
        int piratePiecesAdded = 0;

        Scene currentScene = SceneManager.GetActiveScene();

        if(StoryUI.tutorialToLoad != 0)
        {
            if (StoryUI.tutorialToLoad == 1)
            {
                // mate tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Mate, true, 5, 5);
                PiratePieces[0] = SpawnPiece(TTPieceType.Mate, false, 3, 7);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 6, 3);

            }
            else if(StoryUI.tutorialToLoad == 2)
            {
                // Quartermaster tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Quartermaster, true, 5, 5);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 6, 6);
                NavyPieces[2] = SpawnPiece(TTPieceType.LandMine, true, 7, 6);
                PiratePieces[0] = SpawnPiece(TTPieceType.LandMine, false, 5, 6);
                PiratePieces[1] = SpawnPiece(TTPieceType.LandMine, false, 4, 6);
                PiratePieces[2] = SpawnPiece(TTPieceType.Mate, false, 6, 9);

            }
            else if(StoryUI.tutorialToLoad == 3)
            {
                // Cannon tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Cannon, true, 5, 5);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 5, 7);
                PiratePieces[0] = SpawnPiece(TTPieceType.Mate, false, 5, 8);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 6, 8);
            }
            else if(StoryUI.tutorialToLoad == 4)
            {
                // Engineer tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Bomber, true, 5, 5);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 5, 4);
                PiratePieces[0] = SpawnPiece(TTPieceType.LandMine, false, 6, 6);
                PiratePieces[1] = SpawnPiece(TTPieceType.LandMine, false, 6, 5);
                PiratePieces[2] = SpawnPiece(TTPieceType.Mate, false, 5, 9);
            }
            else if(StoryUI.tutorialToLoad == 5)
            {
                // Vanguard tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Vanguard, true, 5, 5);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 3, 5);
                PiratePieces[0] = SpawnPiece(TTPieceType.Mate, false, 5, 8);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 7, 6);
            }
            else if(StoryUI.tutorialToLoad == 6)
            {
                // Navigator tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Navigator, true, 5, 5);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 5, 3);
                PiratePieces[0] = SpawnPiece(TTPieceType.Mate, false, 8, 5);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 6, 7);
            }
            else if(StoryUI.tutorialToLoad == 7)
            {
                // Gunner tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Gunner, true, 5, 5);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 5, 7);
                PiratePieces[0] = SpawnPiece(TTPieceType.Mate, false, 5, 8);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 6, 8);
                PiratePieces[2] = SpawnPiece(TTPieceType.Mate, false, 5, 2);
                PiratePieces[3] = SpawnPiece(TTPieceType.Mate, false, 9, 5);
            }
            else if(StoryUI.tutorialToLoad == 8)
            {
                // Admiral tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Royal1, true, 5, 5);
                PiratePieces[0] = SpawnPiece(TTPieceType.Mate, false, 0, 0);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 9, 9);
                PiratePieces[2] = SpawnPiece(TTPieceType.Mate, false, 0, 9);
                PiratePieces[3] = SpawnPiece(TTPieceType.Mate, false, 9, 0);
            }
            else if(StoryUI.tutorialToLoad == 9)
            {
                // Tactician tutorial
                NavyPieces[0] = SpawnPiece(TTPieceType.Royal2, true, 5, 4);
                PiratePieces[0] = SpawnPiece(TTPieceType.Gunner, false, 8, 6);
                PiratePieces[1] = SpawnPiece(TTPieceType.Mate, false, 5, 7);
                PiratePieces[2] = SpawnPiece(TTPieceType.Royal2, false, 2, 5);
                PiratePieces[3] = SpawnPiece(TTPieceType.Vanguard, false, 3, 4);

            }
            else if(StoryUI.tutorialToLoad == 10)
            {
                // Captain tutorial
                PiratePieces[0] = SpawnPiece(TTPieceType.Royal1, false, 5, 5);
                PiratePieces[1] = SpawnPiece(TTPieceType.LandMine, false, 4, 5);
                NavyPieces[0] = SpawnPiece(TTPieceType.Mate, true, 5, 7);
                NavyPieces[1] = SpawnPiece(TTPieceType.LandMine, true, 5, 6);
                NavyPieces[2] = SpawnPiece(TTPieceType.LandMine, true, 5, 4);
            }
            else if(StoryUI.tutorialToLoad == 11)
            {
                // Corsair tutorial
                PiratePieces[0] = SpawnPiece(TTPieceType.Royal2, false, 4, 5);
                PiratePieces[1] = SpawnPiece(TTPieceType.LandMine, false, 0, 6);
                PiratePieces[2] = SpawnPiece(TTPieceType.LandMine, false, 1, 6);
                PiratePieces[3] = SpawnPiece(TTPieceType.LandMine, false, 2, 6);
                PiratePieces[4] = SpawnPiece(TTPieceType.LandMine, false, 3, 6);
                PiratePieces[5] = SpawnPiece(TTPieceType.LandMine, false, 4, 6);
                PiratePieces[6] = SpawnPiece(TTPieceType.LandMine, false, 5, 6);
                PiratePieces[7] = SpawnPiece(TTPieceType.LandMine, false, 6, 6);
                PiratePieces[8] = SpawnPiece(TTPieceType.LandMine, false, 7, 6);
                PiratePieces[9] = SpawnPiece(TTPieceType.LandMine, false, 8, 6);
                PiratePieces[10] = SpawnPiece(TTPieceType.LandMine, false, 9, 6);
                NavyPieces[0] = SpawnPiece(TTPieceType.Mate, true, 2, 0);
                NavyPieces[1] = SpawnPiece(TTPieceType.Mate, true, 6, 7);
                NavyPieces[2] = SpawnPiece(TTPieceType.LandMine, true, 0, 3);
                NavyPieces[3] = SpawnPiece(TTPieceType.LandMine, true, 1, 3);
                NavyPieces[4] = SpawnPiece(TTPieceType.LandMine, true, 2, 3);
                NavyPieces[5] = SpawnPiece(TTPieceType.LandMine, true, 3, 3);
                NavyPieces[6] = SpawnPiece(TTPieceType.LandMine, true, 4, 3);
                NavyPieces[7] = SpawnPiece(TTPieceType.LandMine, true, 5, 3);
                NavyPieces[8] = SpawnPiece(TTPieceType.LandMine, true, 6, 3);
                NavyPieces[9] = SpawnPiece(TTPieceType.LandMine, true, 7, 3);
                NavyPieces[10] = SpawnPiece(TTPieceType.LandMine, true, 8, 3);
                NavyPieces[11] = SpawnPiece(TTPieceType.LandMine, true, 9, 3);
                
                
            }
            
        }
        else if(PieceManager.instance == null)
        {
            Debug.Log("No pieces available, using default spawn");

            //Decent board starting positions for a sample game

            NavyPieces[0] = SpawnPiece(TTPieceType.Ore, true, 1, 0);
            NavyPieces[1] = SpawnPiece(TTPieceType.Royal1, true, 3, 0);
            NavyPieces[2] = SpawnPiece(TTPieceType.Mate, true, 9, 0);
            NavyPieces[3] = SpawnPiece(TTPieceType.Cannon, true, 0, 1);
            NavyPieces[4] = SpawnPiece(TTPieceType.Mate, true, 1, 1);
            NavyPieces[5] = SpawnPiece(TTPieceType.Vanguard, true, 3, 1);
            NavyPieces[6] = SpawnPiece(TTPieceType.Cannon, true, 4, 1);
            NavyPieces[7] = SpawnPiece(TTPieceType.Mate, true, 6, 1);
            NavyPieces[8] = SpawnPiece(TTPieceType.Vanguard, true, 7, 1);
            NavyPieces[9] = SpawnPiece(TTPieceType.Navigator, true, 0, 2);
            NavyPieces[10] = SpawnPiece(TTPieceType.Bomber, true, 1, 2);
            NavyPieces[11] = SpawnPiece(TTPieceType.Quartermaster, true, 2, 2);
            NavyPieces[12] = SpawnPiece(TTPieceType.Gunner, true, 3, 2);
            NavyPieces[13] = SpawnPiece(TTPieceType.Mate, true, 5, 2);
            NavyPieces[14] = SpawnPiece(TTPieceType.Gunner, true, 6, 2);
            NavyPieces[15] = SpawnPiece(TTPieceType.Navigator, true, 7, 2);
            NavyPieces[16] = SpawnPiece(TTPieceType.Bomber, true, 8, 2);
            NavyPieces[17] = SpawnPiece(TTPieceType.Royal2, true, 9, 2);

            NavyPieces[18] = SpawnPiece(TTPieceType.LandMine, true, 3, 6);
            NavyPieces[19] = SpawnPiece(TTPieceType.LandMine, true, 5, 5);
            NavyPieces[20] = SpawnPiece(TTPieceType.LandMine, true, 8, 5);
            NavyPieces[21] = SpawnPiece(TTPieceType.LandMine, true, 9, 6);
            PiratePieces[0] = SpawnPiece(TTPieceType.LandMine, false, 3, 3);
            PiratePieces[1] = SpawnPiece(TTPieceType.LandMine, false, 3, 9);
            PiratePieces[2] = SpawnPiece(TTPieceType.LandMine, false, 4, 6);
            PiratePieces[3] = SpawnPiece(TTPieceType.LandMine, false, 1, 4);

            PiratePieces[4] = SpawnPiece(TTPieceType.Ore, false, 6, 9);
            PiratePieces[5] = SpawnPiece(TTPieceType.Bomber, false, 0, 7);
            PiratePieces[6] = SpawnPiece(TTPieceType.Navigator, false, 1, 7);
            PiratePieces[7] = SpawnPiece(TTPieceType.Mate, false, 2, 7);
            PiratePieces[8] = SpawnPiece(TTPieceType.Gunner, false, 3, 7);
            PiratePieces[9] = SpawnPiece(TTPieceType.Mate, false, 4, 7);
            PiratePieces[10] = SpawnPiece(TTPieceType.Quartermaster, false, 5, 7);
            PiratePieces[11] = SpawnPiece(TTPieceType.Bomber, false, 6, 7);
            PiratePieces[12] = SpawnPiece(TTPieceType.Mate, false, 7, 7);
            PiratePieces[13] = SpawnPiece(TTPieceType.Navigator, false, 8, 7);
            PiratePieces[14] = SpawnPiece(TTPieceType.Gunner, false, 9, 7);
            PiratePieces[15] = SpawnPiece(TTPieceType.Cannon, false, 0, 8);
            PiratePieces[16] = SpawnPiece(TTPieceType.Mate, false, 4, 8);
            PiratePieces[17] = SpawnPiece(TTPieceType.Cannon, false, 6, 8);
            PiratePieces[18] = SpawnPiece(TTPieceType.Vanguard, false, 8, 8);
            PiratePieces[19] = SpawnPiece(TTPieceType.Royal2, false, 9, 8);
            PiratePieces[20] = SpawnPiece(TTPieceType.Royal1, false, 2, 9);
            PiratePieces[21] = SpawnPiece(TTPieceType.Vanguard, false, 9, 9);
        }

        boardUI.PlayTurnAnim(navyTurn);
    }

    public void GameOver(bool teamWon, bool stalemate = false)
    {
        gameOver = true;
        gameWon = true;
        // gameTimer.pauseTimer();
        boardUI.GameWon(teamWon, stalemate);
    }

    // Changes the turn from one player to the next
    public void NextTurn(string debug)
    {
        ResetBoardMaterials(true);
        ResetBoardMaterials(true);  // Just in case

        if (tacticianCorsairJump != 0)
        {
            tacticianCorsairJump--;
        }

        if (jumpCooldown != 0)
        {
            jumpCooldown--;
        }

        orebearerSecondMove = false;
        if (!resetOre)
        {
            if (navyTurn)
            {
                navyTurn = false;
                playerIsNavy = false;
            }
            else if(!navyTurn)
            {
                navyTurn = true;
                playerIsNavy = true;
            }
            boardUI.PlayTurnAnim(navyTurn);
        }

        // gameTimer.ResetBar();

        Debug.Log(debug);
    }

    #region Events

    // Server
    private void OnMovePieceServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnCapturePieceServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnCannonCaptureServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnRespawnServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    private void OnGameWonServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }

    private void OnCapturePieceClient(NetMessage msg)
    {
        NetCapturePiece cp = msg as NetCapturePiece;

        bool pieceIsNavy = cp.teamID == 0;

        if(pieceIsNavy != playerIsNavy)
        {
            Debug.Log($"{cp.originalX},{cp.originalY} captures a piece on {cp.targetX},{cp.targetY}");
            if(cp.gunnerCapture == 1)
            {
                Debug.Log("A gunner is capturing");
            }
            else
            {
                Debug.Log("Regular capture");
            }

            if(cp.turnOver == 0)
            {
                Debug.Log("The turn isn't over yet, more stuff needs to happen first");
            }



            GameObject thisTile = FindThisBoardSquare(cp.originalX + 1, cp.originalY + 1);
            TTSquare thisSquare = thisTile.GetComponent<TTSquare>();
            TTPiece thisPiece = thisSquare.currentPiece;

            GameObject targetTile = FindThisBoardSquare(cp.targetX + 1, cp.targetY + 1);
            TTSquare targetSquare = targetTile.GetComponent<TTSquare>();
            TTPiece targetPiece = targetSquare.currentPiece;

            Vector2Int originalCoords = new Vector2Int(cp.originalX, cp.originalY);
            Vector2Int moveCoords = new Vector2Int(cp.targetX, cp.targetY);

            GameplayCapturePiece(thisSquare, targetSquare, thisPiece, targetPiece, moveCoords, cp.gunnerCapture == 1, cp.turnOver == 1);
        }
    }

    private void OnCannonCaptureClient(NetMessage msg)
    {
        NetCannonCapture cc = msg as NetCannonCapture;

        bool pieceIsNavy = cc.teamID == 0;
        
        if(pieceIsNavy != playerIsNavy)
        {
            Debug.Log($"{cc.originalX},{cc.originalY} is moving");

            GameObject thisTile = FindThisBoardSquare(cc.originalX + 1, cc.originalY + 1);
            TTSquare thisSquare = thisTile.GetComponent<TTSquare>();
            TTPiece thisPiece = thisSquare.currentPiece;

            GameObject targetTile = FindThisBoardSquare(cc.targetX + 1, cc.targetY + 1);
            TTSquare targetSquare = targetTile.GetComponent<TTSquare>();

            Vector2Int originalCoords = new Vector2Int(cc.originalX, cc.originalY);
            Vector2Int moveCoords = new Vector2Int(cc.targetX, cc.targetY);

            GameObject captureTile = null;
            TTSquare captureSquare = null;
            TTPiece capturePiece = null;

            switch (cc.captureDir)
            {
                case 0: 
                    Debug.Log("Capturing Up");
                    captureTile = FindThisBoardSquare(cc.targetX + 1, cc.targetY + 2);
                    captureSquare = captureTile.GetComponent<TTSquare>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                case 1:
                    Debug.Log("Capturing Right");
                    captureTile = FindThisBoardSquare(cc.targetX + 2, cc.targetY + 1);
                    captureSquare = captureTile.GetComponent<TTSquare>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                case 2:
                    Debug.Log("Capturing Down");
                    captureTile = FindThisBoardSquare(cc.targetX + 1, cc.targetY);
                    captureSquare = captureTile.GetComponent<TTSquare>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                case 3:
                    Debug.Log("Capturing Left");
                    captureTile = FindThisBoardSquare(cc.targetX, cc.targetY + 1);
                    captureSquare = captureTile.GetComponent<TTSquare>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                default:
                    Debug.Log("Not capturing a piece");
                    break;
            }

            GameplayCannonCapture(thisSquare, targetSquare, thisPiece, capturePiece, moveCoords, cc.turnOver == 1);
        }
    }

    private void OnGameWonClient(NetMessage msg)
    {
        NetGameWon gw = msg as NetGameWon;

        GameOver(gw.teamID == 0);
    }

    #endregion
}