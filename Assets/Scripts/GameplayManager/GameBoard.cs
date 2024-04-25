using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class GameBoard : MonoBehaviour
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
    public Piece[] NavyPieces;      // All Navy game pieces
    public Piece[] PiratePieces;    // All Pirate game

    public Bar gameTimer;

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
    public JailBoard jail;
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

    [SerializeField] private GameObject gunnerLine;
    [SerializeField] private GameObject gunnerParticles;

    #endregion

    #region GameAudio

    [Header("Game Audio clips")]
    [SerializeField] AudioSource movementAudio;
    [SerializeField] AudioSource gunnerAudio;
    [SerializeField] AudioSource gunnerRecharge;
    [SerializeField] AudioSource Capture;
    [SerializeField] AudioSource corsairMove;
    [SerializeField] AudioSource tacticianMove;


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
        if (PieceManager.instance.onlineMultiplayer)
            MultiplayerController.Instance.gameWon = -1;
    }

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
        

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<JailBoard>();
        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();

        SpawnAllPieces();

        // Identify Player ID in multiplayer
        if (PieceManager.instance != null)
            if (PieceManager.instance.onlineMultiplayer)
                playerIsNavy = MultiplayerController.Instance.currentTeam == 0;

        if (PieceManager.instance.navyFirst)
        {
            navyTurn = true;
        }
        else
        {
            navyTurn = false;
        }

        if (!PieceManager.instance.onlineMultiplayer && !navyTurn)
            StartCoroutine(RotateBoard(false));
        else if (PieceManager.instance.onlineMultiplayer && !playerIsNavy)
            StartCoroutine(RotateBoard(false));

        if(!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == navyTurn))
            boardUI.GoalText("Click on a piece to move it!");
        else
            boardUI.GoalText("Waiting for opponent to move.");

        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void Update()
    {
        // Check for a game win
        if (gameTimer.timeOver && !gameWon)
        {
            GameOver(!navyTurn);
        }

        // Check for connection drop
        if (PieceManager.instance.onlineMultiplayer)
        {
            if (MultiplayerController.Instance.gameWon != -1 && boardUI.gameOver == false)
            {
                if (MultiplayerController.Instance.gameWon == 0)
                {
                    GameOver(!playerIsNavy);
                }
                else
                {
                    GameOver(playerIsNavy);
                }
            }
        }


        for (int i = 0; i < TILE_COUNT_X && !gameWon; i++)
        {
            Piece checkPiece = tiles[i, 0].GetComponent<Square>().currentPiece;

            if (checkPiece != null)
            {
                if (checkPiece.hasOre && checkPiece.isNavy)
                {
                    checkPiece.gameWon = true;
                    GameOver(true);
                }
            }

            checkPiece = tiles[i, 9].GetComponent<Square>().currentPiece;

            if (checkPiece != null)
            {
                if (checkPiece.hasOre && !checkPiece.isNavy)
                {
                    checkPiece.gameWon = true;
                    GameOver(false);
                }
            }
        }

        boardUI.UpdateTurn(navyTurn);

        // The active player has selected a bomber or tactician to use this turn
        if (storedTileSelected != null)
        {
            boardUI.UpdateSelectedPiece(storedTileSelected.GetComponent<Square>().currentPiece.type, storedTileSelected.GetComponent<Square>().currentPiece.isNavy, storedTileSelected.GetComponent<Square>().currentPiece.hasOre);

            // The active player has selected a bomber to use this turn
            if (storedTileSelected.GetComponent<Square>().currentPiece.type == PieceType.Bomber) {
                bomberSelected = true;
            }

            // The active player has selected a tactician to use this turn
            else if (storedTileSelected.GetComponent<Square>().currentPiece.type == PieceType.Royal2)
            {
                if (storedTileSelected.GetComponent<Square>().currentPiece.isNavy)
                {
                    tacticianSelected = true;
                }
            }
        }
        // The active player hasn't selected a tactician or bomber to use this turn
        else
        {
            bomberSelected = false;

            // Despawns all pieces out of Tactician inherit cells (if applicable)
            if (tacticianSelected)
            {
                tacticianSelected = false;
                tacticianInheritSelected = false;

                for (int i = 0; i < 9; i++)
                {

                    if (jail.tacticianMimicPieces[i] != null)
                        jail.tacticianMimicPieces[i].GetComponent<Piece>().destroyPiece();
                    jail.TacticianMimicCells[i].GetComponent<JailCell>().resetCell();
                    ResetBoardMaterials(true);
                }
            }
        }

        // Highlights a captured enemy land mines so the bomber can deploy them
        if (bomberSelected && cellToHighlight == -2)
        {
            // The selected bomber is Navy and can deploy Pirate Bombs
            if (tileSelected.GetComponent<Square>().currentPiece.isNavy)
            {
                // The bomber hasn't captured a bomb yet
                if (tileSelected.GetComponent<Square>().currentPiece.GetComponent<Bomber>().capturedBomb == null)
                {
                    Debug.Log("No bombs captured, searching for one");
                    cellToHighlight = jail.FindPiece(PieceType.LandMine, false);
                }
                else
                {
                    Debug.Log("Bomb captured, locating its position");
                    // Searches jail for the corresponding captured bomb
                    for (int i = 0; i < teamSize; i++)
                    {
                        if (jail.NavyJailCells[i].GetComponent<JailCell>().currentPiece == tileSelected.GetComponent<Square>().currentPiece.GetComponent<Bomber>().capturedBomb)
                        {
                            cellToHighlight = i;
                        }
                    }
                }

                if (cellToHighlight >= 0)
                {
                    jail.NavyJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
                }

            }
            // The selected bomber is Pirate and can deploy Navy Bombs
            else
            {
                // The bomber hasn't captured a bomb yet
                if (tileSelected.GetComponent<Square>().currentPiece.GetComponent<Bomber>().capturedBomb == null)
                {
                    Debug.Log("No bombs captured, searching for one");
                    cellToHighlight = jail.FindPiece(PieceType.LandMine, true);
                }
                else
                {
                    Debug.Log("Bomb captured, locating its position");
                    // Searches jail for the corresponding captured bomb
                    for (int i = 0; i < teamSize; i++)
                    {
                        if (jail.PirateJailCells[i].GetComponent<JailCell>().currentPiece == tileSelected.GetComponent<Square>().currentPiece.GetComponent<Bomber>().capturedBomb)
                        {
                            cellToHighlight = i;
                        }
                    }
                }

                if (cellToHighlight >= 0)
                {
                    jail.PirateJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
                }
            }
        }

        // Highlights all possible pieces that tactician can inherit
        if (tacticianSelected)
        {
            for (int i = 0; i < 9; i++)
            {
                if (jail.TacticianMimicCells[i].GetComponent<JailCell>().hasPiece == true) {
                    // Tactician has inherited a piece
                    if (tacticianInheritSelected)
                    {
                        // Jail cell in question was not selected (stop flashing)
                        if (!jail.TacticianMimicCells[i].GetComponent<JailCell>().clicked)
                        {
                            jail.TacticianMimicCells[i].GetComponent<JailCell>().interactable = false;
                        }
                    }
                    // Tactician has not yet inherited a piece (start flashing)
                    else if (jail.TacticianMimicCells[i].GetComponent<JailCell>().interactable == false)
                    {
                        JailCell tacticianCell = jail.TacticianMimicCells[i].GetComponent<JailCell>();
                        tacticianCell.interactable = true;
                    }
                }
            }
        }

        // Highlights the captured ore that needs redeployment
        if (resetOre && cellToHighlight == -2)
        {
            // Navy Ore needs redeployment
            if (!tileSelected.GetComponent<Square>().currentPiece.isNavy)
            {
                cellToHighlight = jail.FindPiece(PieceType.Ore, false);
                Debug.Log(cellToHighlight);
                if (cellToHighlight >= 0)
                {
                    jail.NavyJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
                }
            }
            // Pirate Ore needs redeployment
            else
            {
                cellToHighlight = jail.FindPiece(PieceType.Ore, true);
                Debug.Log(cellToHighlight);
                if (cellToHighlight >= 0)
                {
                    jail.PirateJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
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
                    Square current_square = tileSelected.GetComponent<Square>();
                    if (current_square.currentPiece != null)
                    {
                        // It's not your turn yet!
                        if (PieceManager.instance.onlineMultiplayer && navyTurn != playerIsNavy)
                        {
                            boardUI.DisplayTempText("It's not your turn yet!", 1.5f);
                            Debug.Log("It's not your turn!");
                            Square selectedTile = tileSelected.GetComponent<Square>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);

                            boardUI.UpdateSelectedPiece(current_square.currentPiece.type, current_square.currentPiece.isNavy, current_square.currentPiece.hasOre);
                        }
                        else if (PieceManager.instance.onlineMultiplayer && current_square.currentPiece.isNavy != navyTurn)
                        {
                            boardUI.DisplayTempText("That's not your piece!", 1.5f);
                            Debug.Log("Wrong piece selected!");
                            Square selectedTile = tileSelected.GetComponent<Square>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);

                            boardUI.UpdateSelectedPiece(current_square.currentPiece.type, current_square.currentPiece.isNavy, current_square.currentPiece.hasOre);
                        }
                        // The wrong team is trying to move
                        else if (current_square.currentPiece.isNavy != navyTurn)
                        {
                            boardUI.DisplayTempText("It's not your turn yet!", 1.5f);
                            Debug.Log("It's not your turn!");
                            Square selectedTile = tileSelected.GetComponent<Square>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);

                            boardUI.UpdateSelectedPiece(current_square.currentPiece.type, current_square.currentPiece.isNavy, current_square.currentPiece.hasOre);
                        }
                        // The right team is trying to move
                        else
                        {
                            if (current_square.currentPiece.type == PieceType.Royal2 && current_square.currentPiece.isNavy)
                            {
                                tacticianSelected = true;
                            }
                            if (current_square.currentPiece.type == PieceType.Bomber)
                            {
                                bomberSelected = true;
                            }

                            // Finds enemy bombs in jail cells
                            if (bomberSelected)
                            {
                                // Tactician is mimicking a bomber
                                if (tacticianInheritSelected)
                                {
                                    landMineInJail = jail.FindPiece(PieceType.LandMine, false) >= 0;
                                }
                                // The selected bomber is Navy and can deploy Pirate Bombs
                                else if (tileSelected.GetComponent<Square>().currentPiece.isNavy)
                                {
                                    landMineInJail = jail.FindPiece(PieceType.LandMine, false) >= 0;
                                }
                                // The selected bomber is Pirate and can deploy Navy Bombs
                                else
                                {
                                    landMineInJail = jail.FindPiece(PieceType.LandMine, true) >= 0;
                                }
                            }

                            storedTileSelected = tileSelected;
                            squareSelected = true;
                            current_square.SquareHasBeenClicked = true;
                            DetectLegalMoves(tileSelected, current_square.currentPiece);

                            //Fixing edge case for selection lol
                            if (storedTileSelected.GetComponent<Square>().currentPiece.type == PieceType.LandMine || storedTileSelected.GetComponent<Square>().currentPiece.type == PieceType.Ore)
                                boardUI.UpdateSelectedPiece(storedTileSelected.GetComponent<Square>().currentPiece.type, storedTileSelected.GetComponent<Square>().currentPiece.isNavy, false, true);
                        }

                    }
                }
                // A square had been previously clicked
                else
                {
                    // A moveable square has been clicked (or the corsair has jumped to an open space)
                    if (tileSelected.tag == "MoveableSquare" || tileSelected.tag == "CorsairJump")
                    {
                        Square currentSquare = storedTileSelected.GetComponent<Square>();
                        Piece currentPiece = currentSquare.currentPiece;
                        Square targetSquare = tileSelected.GetComponent<Square>();

                        Vector2Int currentSquareCoords = IdentifyThisBoardSquare(storedTileSelected);
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        boardUI.HideSelectedPiece();



                        // Sends move data if online
                        if (PieceManager.instance.onlineMultiplayer)
                        {
                            Debug.Log("Sending move to server");
                            NetMovePiece mp = new NetMovePiece();

                            mp.teamID = currentPiece.isNavy ? 0 : 1;
                            mp.originalX = currentSquareCoords.x;
                            mp.originalY = currentSquareCoords.y;
                            mp.targetX = moveCoordinates.x;
                            mp.targetY = moveCoordinates.y;
                            mp.corsairJump = tileSelected.tag == "CorsairJump" ? 1 : 0;

                            Debug.Log($"{mp.teamID}: {mp.originalX}{mp.originalY} to {mp.targetX}{mp.targetY}");
                            if (mp.corsairJump == 1)
                            {
                                Debug.Log("Corsair is jumping");
                            }

                            Client.Instance.SendToServer(mp);
                        }

                        GameplayMovePiece(currentSquare, targetSquare, currentPiece, currentSquareCoords, moveCoordinates, tileSelected.tag == "CorsairJump");
                    }

                    // A capturable piece has been clicked (regular pieces or gunners)
                    else if (tileSelected.tag == "CaptureSquare" || tileSelected.tag == "GunnerTarget")
                    {
                        Square CurrentSquare = storedTileSelected.GetComponent<Square>();
                        Square targetSquare = tileSelected.GetComponent<Square>();
                        Piece currentPiece = CurrentSquare.currentPiece;
                        Piece capturedPiece = targetSquare.currentPiece;
                        Vector2Int currentCoordinates = IdentifyThisBoardSquare(storedTileSelected);
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        NetCapturePiece cp = new NetCapturePiece();

                        if (PieceManager.instance.onlineMultiplayer)
                        {
                            Debug.Log("Sending capture to server");

                            if (currentPiece.isNavy)
                                cp.teamID = 0;
                            else
                                cp.teamID = 1;
                            cp.teamID = currentPiece.isNavy ? 0 : 1;
                            cp.originalX = currentCoordinates.x;
                            cp.originalY = currentCoordinates.y;
                            cp.targetX = moveCoordinates.x;
                            cp.targetY = moveCoordinates.y;
                            cp.gunnerCapture = tileSelected.tag == "GunnerTarget" ? 1 : 0;


                            Debug.Log($"{cp.teamID}: {cp.originalX},{cp.originalY} moving to {cp.targetX},{cp.targetY}");
                            if (cp.gunnerCapture == 1)
                                Debug.Log("Gunner is capturing");
                            else
                                Debug.Log("Gunner is not capturing " + cp.gunnerCapture);
                        }

                        GameplayCapturePiece(CurrentSquare, targetSquare, currentPiece, capturedPiece, moveCoordinates, tileSelected.tag == "GunnerTarget");
                        
                        if(PieceManager.instance.onlineMultiplayer)
                        {
                            cp.turnOver = (resetOre || orebearerSecondMove) ? 0 : 1;
                            Client.Instance.SendToServer(cp);
                        }
                    }

                    // Cannon is capturing a piece by jumping
                    else if (tileSelected.tag == "CannonDestination")
                    {
                        Square CurrentSquare = storedTileSelected.GetComponent<Square>();
                        Square targetSquare = tileSelected.GetComponent<Square>();
                        Piece currentPiece = CurrentSquare.currentPiece;
                        Vector2Int currentCoordinates = IdentifyThisBoardSquare(storedTileSelected);
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        // Find which piece is being captured
                        Square captureSquare;
                        Piece capturedPiece = null;

                        int captureDirection = -1;

                        // Checks the square to the right of move square
                        if (moveCoordinates.x + 1 < 10)
                        {
                            Debug.Log("Checked Right");
                            if (tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<Square>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Right");
                                captureSquare = tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<Square>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 1;
                            }
                        }
                        // Checks the square to the left of move square
                        if (moveCoordinates.x - 1 >= 0 && capturedPiece == null)
                        {
                            Debug.Log("Checked Left");
                            if (tiles[moveCoordinates.x - 1, moveCoordinates.y].GetComponent<Square>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Left");
                                captureSquare = tiles[moveCoordinates.x - 1, moveCoordinates.y].GetComponent<Square>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 3;
                            }
                        }
                        // Checks the square above the move square
                        if (moveCoordinates.y + 1 < 10 && capturedPiece == null)
                        {
                            Debug.Log("Checked Up");
                            if (tiles[moveCoordinates.x, moveCoordinates.y + 1].GetComponent<Square>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Up");
                                captureSquare = tiles[moveCoordinates.x, moveCoordinates.y + 1].GetComponent<Square>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 0;
                            }
                        }
                        // Checks the square below the move square
                        if (moveCoordinates.y - 1 >= 0 && capturedPiece == null)
                        {
                            Debug.Log("Checked Down");
                            if (tiles[moveCoordinates.x, moveCoordinates.y - 1].GetComponent<Square>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Down");
                                captureSquare = tiles[moveCoordinates.x, moveCoordinates.y - 1].GetComponent<Square>();
                                capturedPiece = captureSquare.currentPiece;
                                captureDirection = 2;
                            }
                        }

                        NetCannonCapture cc = new NetCannonCapture();

                        if (PieceManager.instance.onlineMultiplayer)
                        {
                            Debug.Log("Sending cannon capture to server");

                            cc.teamID = currentPiece.isNavy ? 0 : 1;
                            cc.originalX = currentCoordinates.x;
                            cc.originalY = currentCoordinates.y;
                            cc.targetX = moveCoordinates.x;
                            cc.targetY = moveCoordinates.y;
                            cc.captureDir = captureDirection;
                        }

                        GameplayCannonCapture(CurrentSquare, targetSquare, currentPiece, capturedPiece, moveCoordinates);

                        if (PieceManager.instance.onlineMultiplayer)
                        {
                            cc.turnOver = (resetOre || orebearerSecondMove) ? 0 : 1;
                            Client.Instance.SendToServer(cc);
                        }
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
                                DetectLegalMoves(tileSelected, tileSelected.GetComponent<Square>().currentPiece);
                            }
                            else
                            {
                                landMineSelected = true;
                                tileSelected.GetComponent<JailCell>().clicked = true;
                                ResetBoardMaterials(false);
                                DetectLegalMoves(storedTileSelected, storedTileSelected.GetComponent<Square>().currentPiece);
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
                                    if (jail.TacticianMimicCells[i].GetComponent<JailCell>().hasPiece)
                                        jail.TacticianMimicCells[i].GetComponent<JailCell>().interactable = true;
                                }

                                tacticianInheritSelected = false;
                                cellToHighlight = -2;
                                tileSelected = storedTileSelected;
                                ResetBoardMaterials(true);
                                ResetBoardMaterials(true);
                                DetectLegalMoves(tileSelected, storedTileSelected.GetComponent<Square>().currentPiece);
                            }
                            // The user clicked on a piece to inherit
                            else
                            {
                                tacticianInheritSelected = true;

                                for (int i = 0; i < 9; i++)
                                {
                                    jail.TacticianMimicCells[i].GetComponent<JailCell>().interactable = false;
                                }

                                tileSelected.GetComponent<JailCell>().clicked = true;
                                ResetBoardMaterials(false);

                                Square currentSquare = storedTileSelected.GetComponent<Square>();
                                Piece inheritingPiece = tileSelected.GetComponent<JailCell>().currentPiece;

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
                        Square mineSquare = storedTileSelected.GetComponent<Square>();
                        Vector2Int deployCoordinates = IdentifyThisBoardSquare(tileSelected);

                        Piece shieldPiece;
                        if (!mineSquare.currentPiece.isNavy)
                            shieldPiece = jail.navyJailedPieces[cellToHighlight].GetComponent<Piece>();
                        else
                            shieldPiece = jail.pirateJailedPieces[cellToHighlight].GetComponent<Piece>();

                        if (PieceManager.instance.onlineMultiplayer)
                        {
                            NetRespawn rs = new NetRespawn();

                            rs.teamID = shieldPiece.isNavy ? 0 : 1;
                            rs.jailIndex = cellToHighlight;
                            rs.targetX = deployCoordinates.x;
                            rs.targetY = deployCoordinates.y;
                            rs.specialPiece = 0;

                            Client.Instance.SendToServer(rs);
                        }

                        GameplayDeployPiece(shieldPiece, deployCoordinates, cellToHighlight, 0);
                    }

                    // The ore is being redeployed to the board
                    else if (tileSelected.tag == "OreDeploy")
                    {
                        Debug.Log("Redeploying Ore");
                        Square pieceSquare = storedTileSelected.GetComponent<Square>();
                        Vector2Int deployCoordinates = IdentifyThisBoardSquare(tileSelected);

                        cellToHighlight = jail.FindPiece(PieceType.Ore, pieceSquare.currentPiece.isNavy);

                        Piece orePiece;
                        if (pieceSquare.currentPiece.isNavy || tacticianSelected)
                            orePiece = jail.navyJailedPieces[cellToHighlight].GetComponent<Piece>();
                        else
                            orePiece = jail.pirateJailedPieces[cellToHighlight].GetComponent<Piece>();

                        Debug.Log(cellToHighlight);

                        int turnOver = orebearerSecondMove ? 2 : 1;

                        if (PieceManager.instance.onlineMultiplayer)
                        {
                            NetRespawn rs = new NetRespawn();

                            rs.teamID = orePiece.isNavy ? 0 : 1;
                            rs.jailIndex = cellToHighlight;
                            rs.targetX = deployCoordinates.x;
                            rs.targetY = deployCoordinates.y;
                            rs.specialPiece = turnOver;

                            Client.Instance.SendToServer(rs);
                        }

                        GameplayDeployPiece(orePiece, deployCoordinates, cellToHighlight, turnOver);

                        // Return control to the orebearer for a second turn
                        if(orebearerSecondMove)
                        {
                            pieceSquare.SquareHasBeenClicked = true;
                            DetectLegalMoves(storedTileSelected, pieceSquare.currentPiece);
                        }
                    }

                    // The same square was clicked twice (cancel the square selection)
                    else if (tileSelected.GetComponent<Square>().SquareHasBeenClicked)
                    {
                        ResetBoardMaterials(true);
                        boardUI.GoalText("Click on a piece to move it!");
                        boardUI.HideSelectedPiece();

                        // Ends turn if orebearer decides not to move a second time
                        if (tileSelected.GetComponent<Square>().currentPiece.hasOre && orebearerSecondMove)
                        {
                            // Send a blank move message to the opponent so they can end the turn
                            if (PieceManager.instance.onlineMultiplayer)
                            {
                                Square currentSquare = tileSelected.GetComponent<Square>();
                                Piece currentPiece = currentSquare.currentPiece;

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
                            NextTurn();
                        }

                        landMineInJail = false;
                        squareSelected = false;
                        tileSelected.GetComponent<Square>().SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                        bomberSelected = false;
                        landMineSelected = false;
                    }
                }
            }
        }
    }

    private void GameplayMovePiece(Square currentSquare, Square targetSquare, Piece currentPiece, Vector2Int currentSquareCoords, Vector2Int moveCoordinates, bool corsairJump = false)
    {
        movementAudio.Play();

        // Corsair jumping (or tactician imitating) requires a cooldown
        if (corsairJump)
        {
            // Corsair jumping requires 2 turns of cooldown before the next jump
            if (currentPiece.type == PieceType.Royal2 && !currentPiece.isNavy)
            {
                jumpCooldown = 3;
                corsairMove.Play();
            }
            else
            {
                tacticianCorsairJump = 3;
            }
        }
        else
        {
            // Checks if the piece is a Tactician that has moved since mimicking a Gunner
            if (currentPiece.type == PieceType.Royal2 && currentPiece.isNavy)
            {
                tacticianGunnerCapture = false;
            }

            if (currentPiece.type == PieceType.Gunner && currentPiece.hasCaptured)
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
        NextTurn();
    }

    private void GameplayCapturePiece(Square CurrentSquare, Square targetSquare, Piece currentPiece, Piece capturedPiece, Vector2Int moveCoordinates, bool gunnerCapture = false, bool turnOver = true)
    {
        // Check if capture target is the ore
        if (capturedPiece.type == PieceType.Ore)
        {
            currentPiece.hasOre = true;
            currentPiece.type = PieceType.Mate; // Gets rid of any fancy moves at their disposal

            // Update UI
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == currentPiece.isNavy))
                boardUI.UpdateGoal(navyTurn, true);
        }

        // If the orebearer is being captured, the ore needs to be reset (handled by opponent in multiplayer)
        if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == currentPiece.isNavy))
        {
            if (capturedPiece.hasOre)
            {
                resetOre = true;
                turnOver = false;
            }
        }

        // Capture that Piece
        jail.InsertAPiece(capturedPiece);
        capturedPiece.destroyPiece();
        currentPiece.hasCaptured = true;

        // Links the engineer with his captured shield
        if (capturedPiece.type == PieceType.LandMine && currentPiece.type == PieceType.Bomber)
        {
            int bombJailIndex = jail.FindLastSlot(!currentPiece.isNavy);
            Debug.Log(bombJailIndex);
            if (currentPiece.isNavy)
            {
                currentPiece.GetComponent<Bomber>().capturedBomb = jail.pirateJailedPieces[bombJailIndex];
            }
            else
            {
                currentPiece.GetComponent<Bomber>().capturedBomb = jail.navyJailedPieces[bombJailIndex];
            }
        }

        // Prevents the Tactician from mimicking a Gunner and capturing twice
        if (gunnerCapture)
        {
            gunnerAudio.Play();

            if (currentPiece.type == PieceType.Royal2 && currentPiece.isNavy)
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

        if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == currentPiece.isNavy))
        {
            // Ore needs to be reset before the turn ends
            if (resetOre)
            {
                if (currentPiece.type == PieceType.Gunner || tacticianInheritSelected)
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
                if (currentPiece.type == PieceType.Gunner)
                {
                    CurrentSquare.SquareHasBeenClicked = true;
                    currentPiece.type = PieceType.Mate;
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
        }

        // Turn is now over
        if (turnOver)
        {
            squareSelected = false;
            tileSelected = null;
            storedTileSelected = null;
            ResetBoardMaterials();
            NextTurn();
        }
    }

    private void GameplayCannonCapture(Square CurrentSquare, Square targetSquare, Piece currentPiece, Piece capturedPiece, Vector2Int moveCoordinates, bool turnOver = true)
    {
        // A piece is being captured
        if (capturedPiece != null)
        {
            // Check if the captured piece is the ore
            if (capturedPiece.type == PieceType.Ore)
            {
                currentPiece.hasOre = true;
                currentPiece.type = PieceType.Mate; // Gets rid of any fancy moves at their disposal

                // Update UI
                boardUI.UpdateGoal(navyTurn, true);
            }

            // If the orebearer is being captured, the ore needs to be reset (handled by opponent in multiplayer)
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == currentPiece.isNavy))
            {
                if (capturedPiece.hasOre)
                {
                    resetOre = true;
                    turnOver = false;
                }
            }

            // Blanks out the captured piece's square
            GameObject captureTile = FindThisBoardSquare(capturedPiece.currentX + 1, capturedPiece.currentY + 1);
            Square captureSquare = captureTile.GetComponent<Square>();
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

        if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == currentPiece.isNavy))
        {
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
        }

        // Turn is now over
        if (turnOver)
        {
            squareSelected = false;
            tileSelected = null;
            storedTileSelected = null;
            ResetBoardMaterials();
            NextTurn();
        }
    }

    private void GameplayDeployPiece(Piece currentPiece, Vector2Int deployCoordinates, int jailIndex, int deployPieceType)
    {
        Debug.Log("Redeploying Piece");

        int spawnIndex = FindFirstOpenTeamSlot(currentPiece.isNavy);

        if (currentPiece.isNavy)
        {
            jail.navyJailedPieces[jailIndex].GetComponent<Piece>().destroyPiece();
            jail.PirateJailCells[jailIndex].GetComponent<JailCell>().resetCell();

            if(PieceManager.instance.onlineMultiplayer && !playerIsNavy)
            {
                if (deployPieceType == 0)
                    NavyPieces[spawnIndex] = SpawnPiece(PieceType.LandMine, true, deployCoordinates.x, deployCoordinates.y, true);
                else
                    NavyPieces[spawnIndex] = SpawnPiece(PieceType.Ore, true, deployCoordinates.x, deployCoordinates.y, true);
            }
            else
            {
                if (deployPieceType == 0)
                    NavyPieces[spawnIndex] = SpawnPiece(PieceType.LandMine, true, deployCoordinates.x, deployCoordinates.y);
                else
                    NavyPieces[spawnIndex] = SpawnPiece(PieceType.Ore, true, deployCoordinates.x, deployCoordinates.y);
            }

        }
        else
        {
            jail.pirateJailedPieces[jailIndex].GetComponent<Piece>().destroyPiece();
            jail.NavyJailCells[jailIndex].GetComponent<JailCell>().resetCell();

            if (PieceManager.instance.onlineMultiplayer && !playerIsNavy)
            {
                if (deployPieceType == 0)
                    PiratePieces[spawnIndex] = SpawnPiece(PieceType.LandMine, false, deployCoordinates.x, deployCoordinates.y, true);
                else
                    PiratePieces[spawnIndex] = SpawnPiece(PieceType.Ore, false, deployCoordinates.x, deployCoordinates.y, true);
            }
            else
            {
                if (deployPieceType == 0)
                    PiratePieces[spawnIndex] = SpawnPiece(PieceType.LandMine, false, deployCoordinates.x, deployCoordinates.y);
                else
                    PiratePieces[spawnIndex] = SpawnPiece(PieceType.Ore, false, deployCoordinates.x, deployCoordinates.y);
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
        if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == navyMove))
            tileSelected.GetComponent<Square>().SquareHasBeenClicked = false;

        tileSelected = null;
        bomberSelected = false;
        landMineSelected = false;
        resetOre = false;
        ResetBoardMaterials();


        // Turn is now over (unless the orebearer still needs to make another move)
        if(deployPieceType <= 1)
        {
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == navyMove))
                storedTileSelected.GetComponent<Square>().SquareHasBeenClicked = false;

            squareSelected = false;
            storedTileSelected = null;
            NextTurn();
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
        string piecename = "{" + x + "," + y + "}";
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

    public Piece SpawnPiece(PieceType type, bool isNavy, int startingX = -1, int startingY = -1, bool pirateRotate = false)
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

        if (pirateRotate)
        {
            cp.transform.Rotate(0, 0, 180f, Space.Self);
        }

        if(startingX != -1 && startingY != -1)
        {
            MovePiece(cp, startingX, startingY, false);
        }

        return cp;
    }

    public void MovePiece(Piece piece, int x, int y, bool lerpMove = true)
    {
        pieceMoving = true;

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
            pieceMoving = false;
        }
    }

    public void AnimGun(Piece piece, int x, int y)
    {
        Vector3 startPosition = piece.transform.position;
        Vector3 targetPosition = tiles[x, y].transform.position;
        bool isNavy = piece.isNavy;
        StartCoroutine(boardUI.AnimGunner(startPosition, targetPosition, isNavy));
    }

    IEnumerator LerpPosition(Piece piece, Vector3 targetPosition, float duration = 0.1f)
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

    private void DetectLegalMoves(GameObject current, Piece piece)
    {
        Square current_square;
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

        current_square = tiles[current_x, current_y].GetComponent<Square>();

        if (piece.hasOre && !resetOre)
        {
            moveAssessment = piece.GetComponent<Piece>().GetValidMovesOre(tiles);
        }
        else if (resetOre)
        {
            moveAssessment = piece.GetComponent<Piece>().GetValidOreReset(tiles);
        }
        else
        {
            switch (piece.type)
            {
                case PieceType.Ore:
                    boardUI.DisplayTempText("The Ore can't move, click on a different piece!", 1.5f);
                    Debug.Log("The Ore doesn't move!");
                    invalidPiece = true;
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<Square>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case PieceType.LandMine:
                    boardUI.DisplayTempText("Land Mines can't move, click on a different piece!", 1.5f);
                    Debug.Log("The Land Mine doesn't move!");
                    invalidPiece = true;
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<Square>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case PieceType.Mate:
                    moveAssessment = piece.GetComponent<Mate>().GetValidMoves(tiles);
                    break;
                case PieceType.Bomber:
                    if (landMineSelected)
                    {
                        moveAssessment = piece.GetComponent<Bomber>().DetectBombDeploy(tiles);
                    }
                    else
                    {
                        moveAssessment = piece.GetComponent<Bomber>().GetValidMoves(tiles, landMineInJail);
                    }
                    break;
                case PieceType.Vanguard:
                    moveAssessment = piece.GetComponent<Vanguard>().GetValidMoves(tiles);
                    break;
                case PieceType.Navigator:
                    moveAssessment = piece.GetComponent<Navigator>().GetValidMoves(tiles);
                    break;
                case PieceType.Gunner:
                    moveAssessment = piece.GetComponent<Gunner>().GetValidMoves(tiles);
                    break;
                case PieceType.Cannon:
                    moveAssessment = piece.GetComponent<Cannon>().GetValidMoves(tiles);
                    break;
                case PieceType.Quartermaster:
                    moveAssessment = piece.GetComponent<Quartermaster>().GetValidMoves(tiles);
                    break;
                case PieceType.Royal2:
                    if (piece.isNavy && !tacticianInheritSelected)
                    {
                        moveAssessment = piece.GetComponent<Tactician>().GetValidMoves(tiles);
                        tacticianMove.Play();
                        //play tact sound
                    }
                    else
                    {
                        if(jumpCooldown > 0 && !tacticianSelected)
                        {
                            moveAssessment = piece.GetComponent<Corsair>().GetValidMoves(tiles, false);
                        }
                        else if(tacticianCorsairJump > 0 && tacticianSelected)
                        {
                            moveAssessment = piece.GetComponent<Corsair>().GetValidMoves(tiles, false);
                        }
                        else
                        {
                            moveAssessment = piece.GetComponent<Corsair>().GetValidMoves(tiles);
                        }
                    }
                    break;
                case PieceType.Royal1:
                    if (piece.isNavy && !tacticianInheritSelected)
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
        }
        
        // Ensures that an enemy is not moving to a landmine or allied occupied square
        for (int x = 0; x < TILE_COUNT_X && !piece.hasOre; x++)
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
                            if(piece.type == PieceType.Bomber)
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
                            if (piece.type == PieceType.Gunner || piece.type == PieceType.Cannon)
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
                    Square activeSquare = tiles[x,y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.moveableBoardMaterial);
                    moveAble = true;
                }
                // Square contains a capturable piece by replacement
                else if (moveAssessment[x,y] == 2)
                {
                    tiles[x, y].tag = "CaptureSquare";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    captureAble = true;
                }
                // Square contains a capturable piece by shooting
                else if(moveAssessment[x,y] == 3)
                {
                    if((tacticianSelected && !tacticianGunnerCapture) || !tacticianSelected)
                    {
                        tiles[x, y].tag = "GunnerTarget";
                        Square activeSquare = tiles[x, y].GetComponent<Square>();
                        activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                        gunnerAble = true;
                    }
                }
                // Square contains a capturable peice by jumping
                else if(moveAssessment[x,y] == 4)
                {
                    // Cannon can jump a Land Mine but not capture it
                    if (tiles[x, y].GetComponent<Square>().currentPiece.type != PieceType.LandMine)
                    {
                        tiles[x, y].tag = "CannonTarget";
                        Square activeSquare = tiles[x, y].GetComponent<Square>();
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
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    mineDeploy = true;
                }
                // The ore can be deployed here
                else if (moveAssessment[x,y] == 7)
                {
                    tiles[x, y].tag = "OreDeploy";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
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
            if(jumpCooldown > 0 && piece.type == PieceType.Royal2 && !piece.isNavy)
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

        if(PieceManager.instance == null)
        {
            Debug.Log("No pieces available, using default spawn");

            //Decent board starting positions for a sample game

            NavyPieces[0] = SpawnPiece(PieceType.Ore, true, 1, 0);
            NavyPieces[1] = SpawnPiece(PieceType.Royal1, true, 3, 0);
            NavyPieces[2] = SpawnPiece(PieceType.Mate, true, 9, 0);
            NavyPieces[3] = SpawnPiece(PieceType.Cannon, true, 0, 1);
            NavyPieces[4] = SpawnPiece(PieceType.Mate, true, 1, 1);
            NavyPieces[5] = SpawnPiece(PieceType.Vanguard, true, 3, 1);
            NavyPieces[6] = SpawnPiece(PieceType.Cannon, true, 4, 1);
            NavyPieces[7] = SpawnPiece(PieceType.Mate, true, 6, 1);
            NavyPieces[8] = SpawnPiece(PieceType.Vanguard, true, 7, 1);
            NavyPieces[9] = SpawnPiece(PieceType.Navigator, true, 0, 2);
            NavyPieces[10] = SpawnPiece(PieceType.Bomber, true, 1, 2);
            NavyPieces[11] = SpawnPiece(PieceType.Quartermaster, true, 2, 2);
            NavyPieces[12] = SpawnPiece(PieceType.Gunner, true, 3, 2);
            NavyPieces[13] = SpawnPiece(PieceType.Mate, true, 5, 2);
            NavyPieces[14] = SpawnPiece(PieceType.Gunner, true, 6, 2);
            NavyPieces[15] = SpawnPiece(PieceType.Navigator, true, 7, 2);
            NavyPieces[16] = SpawnPiece(PieceType.Bomber, true, 8, 2);
            NavyPieces[17] = SpawnPiece(PieceType.Royal2, true, 9, 2);

            NavyPieces[18] = SpawnPiece(PieceType.LandMine, true, 3, 6);
            NavyPieces[19] = SpawnPiece(PieceType.LandMine, true, 5, 5);
            NavyPieces[20] = SpawnPiece(PieceType.LandMine, true, 8, 5);
            NavyPieces[21] = SpawnPiece(PieceType.LandMine, true, 9, 6);
            PiratePieces[0] = SpawnPiece(PieceType.LandMine, false, 3, 3);
            PiratePieces[1] = SpawnPiece(PieceType.LandMine, false, 3, 9);
            PiratePieces[2] = SpawnPiece(PieceType.LandMine, false, 4, 6);
            PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 1, 4);

            PiratePieces[4] = SpawnPiece(PieceType.Ore, false, 6, 9);
            PiratePieces[5] = SpawnPiece(PieceType.Bomber, false, 0, 7);
            PiratePieces[6] = SpawnPiece(PieceType.Navigator, false, 1, 7);
            PiratePieces[7] = SpawnPiece(PieceType.Mate, false, 2, 7);
            PiratePieces[8] = SpawnPiece(PieceType.Gunner, false, 3, 7);
            PiratePieces[9] = SpawnPiece(PieceType.Mate, false, 4, 7);
            PiratePieces[10] = SpawnPiece(PieceType.Quartermaster, false, 5, 7);
            PiratePieces[11] = SpawnPiece(PieceType.Bomber, false, 6, 7);
            PiratePieces[12] = SpawnPiece(PieceType.Mate, false, 7, 7);
            PiratePieces[13] = SpawnPiece(PieceType.Navigator, false, 8, 7);
            PiratePieces[14] = SpawnPiece(PieceType.Gunner, false, 9, 7);
            PiratePieces[15] = SpawnPiece(PieceType.Cannon, false, 0, 8);
            PiratePieces[16] = SpawnPiece(PieceType.Mate, false, 4, 8);
            PiratePieces[17] = SpawnPiece(PieceType.Cannon, false, 6, 8);
            PiratePieces[18] = SpawnPiece(PieceType.Vanguard, false, 8, 8);
            PiratePieces[19] = SpawnPiece(PieceType.Royal2, false, 9, 8);
            PiratePieces[20] = SpawnPiece(PieceType.Royal1, false, 2, 9);
            PiratePieces[21] = SpawnPiece(PieceType.Vanguard, false, 9, 9);
        }

        else
        {
            navyTurn = PieceManager.instance.navyFirst;
            for (int i = 0; i < PieceManager.instance.totalPieces; i++)
            {
                if (PieceManager.instance.factions[i])
                {
                    NavyPieces[navyPiecesAdded] = SpawnPiece(PieceManager.instance.pieceTypes[i], true, PieceManager.instance.pieceCoords[i,0], PieceManager.instance.pieceCoords[i,1]);
                    navyPiecesAdded++;
                }
                else
                {
                    PiratePieces[piratePiecesAdded] = SpawnPiece(PieceManager.instance.pieceTypes[i], false, PieceManager.instance.pieceCoords[i, 0], PieceManager.instance.pieceCoords[i, 1]);
                    piratePiecesAdded++;
                }
            }
        }

        boardUI.PlayTurnAnim(navyTurn);
    }

    public void GameOver(bool teamWon, bool stalemate = false)
    {
        gameOver = true;
        gameWon = true;
        gameTimer.pauseTimer();
        boardUI.GameWon(teamWon, stalemate);
    }

    public void ForfeitGame()
    {
        if (PieceManager.instance != null)
        {
            if (PieceManager.instance.onlineMultiplayer)
            {
                NetGameWon gw = new NetGameWon();
                gw.teamID = playerIsNavy ? 1 : 0;

                Client.Instance.SendToServer(gw);
            }
        }
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

                // Make sure gunner animates correctly
                gunnerLine.transform.Rotate(0f, 0f, -180f, Space.Self);
                Vector3 targetPos = gunnerLine.GetComponent<LineRenderer>().GetPosition(1) * -1;
                gunnerParticles.GetComponent<Transform>().transform.position = targetPos;

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

                yield return new WaitForSeconds(0.5f);
                gunnerLine.transform.Rotate(0f, 0f, 180f, Space.Self);

            }
        }
        else
        {
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && !playerIsNavy))
            {
                Debug.Log("Putting the pirates at the bottom of the screen");
                gameBoard.transform.Rotate(0f, 0f, 180f, Space.Self);

                // Make sure gunner animates correctly
                gunnerLine.transform.Rotate(0f, 0f, 180f, Space.Self);
                Vector3 targetPos = gunnerLine.GetComponent<LineRenderer>().GetPosition(1) * -1;
                gunnerParticles.GetComponent<Transform>().transform.position = targetPos;

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

                yield return new WaitForSeconds(0.5f);
                gunnerLine.transform.Rotate(0f, 0f, -180f, Space.Self);
            }
        }

        yield return null;
    }

    // Changes the turn from one player to the next
    public void NextTurn()
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
                if(!PieceManager.instance.onlineMultiplayer)
                    StartCoroutine(RotateBoard(navyTurn));
            }
            else
            {
                navyTurn = true;
                if(!PieceManager.instance.onlineMultiplayer)
                    StartCoroutine(RotateBoard(navyTurn));
            }
            boardUI.PlayTurnAnim(navyTurn);
        }

        gameTimer.ResetBar();

        //stalemate = CheckForStalemate(navyTurn);

        // Update UI if no stalemate
        if (!stalemate) 
        {
            if (!PieceManager.instance.onlineMultiplayer || (PieceManager.instance.onlineMultiplayer && playerIsNavy == navyTurn))
                boardUI.GoalText("Click on a piece to move it!");
            else
                boardUI.GoalText("Waiting for opponent to move.");
        }
    }

    #region Events

    private void RegisterEvents()
    {
        NetUtility.S_MOVE_PIECE += OnMovePieceServer;
        NetUtility.S_CAPTURE_PIECE += OnCapturePieceServer;
        NetUtility.S_CANNON_CAPTURE += OnCannonCaptureServer;
        NetUtility.S_RESPAWN += OnRespawnServer;
        NetUtility.S_GAME_WON += OnGameWonServer;

        NetUtility.C_MOVE_PIECE += OnMovePieceClient;
        NetUtility.C_CAPTURE_PIECE += OnCapturePieceClient;
        NetUtility.C_CANNON_CAPTURE += OnCannonCaptureClient;
        NetUtility.C_RESPAWN += OnRespawnClient;
        NetUtility.C_GAME_WON += OnGameWonClient;
    }

    private void UnRegisterEvents()
    { 
        NetUtility.S_MOVE_PIECE -= OnMovePieceServer;
        NetUtility.S_CAPTURE_PIECE -= OnCapturePieceServer;
        NetUtility.S_CANNON_CAPTURE -= OnCannonCaptureServer;
        NetUtility.S_RESPAWN -= OnRespawnServer;
        NetUtility.S_GAME_WON -= OnGameWonServer;


        NetUtility.C_MOVE_PIECE -= OnMovePieceClient;
        NetUtility.C_CAPTURE_PIECE -= OnCapturePieceClient;
        NetUtility.C_CANNON_CAPTURE -= OnCannonCaptureClient;
        NetUtility.C_RESPAWN -= OnRespawnClient;
        NetUtility.C_GAME_WON -= OnGameWonClient;
    }

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

    // Client
    private void OnMovePieceClient(NetMessage msg)
    {
        NetMovePiece mp = msg as NetMovePiece;

        if(mp.teamID == 0 && !playerIsNavy || mp.teamID == 1 && playerIsNavy)
        {
            Debug.Log($"{mp.originalX}{mp.originalY} moving to {mp.targetX}{mp.targetY}");
            if (mp.corsairJump == 1)
                Debug.Log("Corsair is jumping");
            GameObject thisTile = FindThisBoardSquare(mp.originalX + 1, mp.originalY + 1);
            Square thisSquare = thisTile.GetComponent<Square>();
            Piece thisPiece = thisSquare.currentPiece;

            GameObject targetTile = FindThisBoardSquare(mp.targetX + 1, mp.targetY + 1);
            Square targetSquare = targetTile.GetComponent<Square>();
            
            Debug.Log("Current Square: " + thisSquare + "\nCurrent Piece: " + thisPiece);
            Vector2Int originalCoords = new Vector2Int(mp.originalX, mp.originalY);
            Vector2Int moveCoords = new Vector2Int(mp.targetX, mp.targetY);

            GameplayMovePiece(thisSquare, targetSquare, thisPiece, originalCoords, moveCoords, mp.corsairJump == 1);
        }
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
            Square thisSquare = thisTile.GetComponent<Square>();
            Piece thisPiece = thisSquare.currentPiece;

            GameObject targetTile = FindThisBoardSquare(cp.targetX + 1, cp.targetY + 1);
            Square targetSquare = targetTile.GetComponent<Square>();
            Piece targetPiece = targetSquare.currentPiece;

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
            Square thisSquare = thisTile.GetComponent<Square>();
            Piece thisPiece = thisSquare.currentPiece;

            GameObject targetTile = FindThisBoardSquare(cc.targetX + 1, cc.targetY + 1);
            Square targetSquare = targetTile.GetComponent<Square>();

            Vector2Int originalCoords = new Vector2Int(cc.originalX, cc.originalY);
            Vector2Int moveCoords = new Vector2Int(cc.targetX, cc.targetY);

            GameObject captureTile = null;
            Square captureSquare = null;
            Piece capturePiece = null;

            switch (cc.captureDir)
            {
                case 0: 
                    Debug.Log("Capturing Up");
                    captureTile = FindThisBoardSquare(cc.targetX + 1, cc.targetY + 2);
                    captureSquare = captureTile.GetComponent<Square>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                case 1:
                    Debug.Log("Capturing Right");
                    captureTile = FindThisBoardSquare(cc.targetX + 2, cc.targetY + 1);
                    captureSquare = captureTile.GetComponent<Square>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                case 2:
                    Debug.Log("Capturing Down");
                    captureTile = FindThisBoardSquare(cc.targetX + 1, cc.targetY);
                    captureSquare = captureTile.GetComponent<Square>();
                    capturePiece = captureSquare.currentPiece;
                    break;
                case 3:
                    Debug.Log("Capturing Left");
                    captureTile = FindThisBoardSquare(cc.targetX, cc.targetY + 1);
                    captureSquare = captureTile.GetComponent<Square>();
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

    private void OnRespawnClient(NetMessage msg)
    {
        NetRespawn rs = msg as NetRespawn;

        bool navyMove;

        Debug.Log($"{rs.teamID}: deploying piece type {rs.specialPiece} from {rs.jailIndex} to {rs.targetX},{rs.targetY}");

        if(rs.specialPiece == 0)
            navyMove = rs.teamID == 1;
        else
            navyMove = rs.teamID == 0;

        if (navyMove != playerIsNavy)
        {
            Debug.Log("Received a move from opponent!");

            Piece deployPiece;

            if(rs.specialPiece == 0)
            {
                if (rs.teamID == 0)
                    deployPiece = jail.navyJailedPieces[rs.jailIndex].GetComponent<Piece>();
                else
                    deployPiece = jail.pirateJailedPieces[rs.jailIndex].GetComponent<Piece>();
            }
            else
            {
                if(rs.teamID == 0)
                    deployPiece = jail.navyJailedPieces[rs.jailIndex].GetComponent<Piece>();
                else
                    deployPiece = jail.pirateJailedPieces[rs.jailIndex].GetComponent<Piece>();
            }

            Vector2Int deployCoordinates = new Vector2Int(rs.targetX, rs.targetY);

            GameplayDeployPiece(deployPiece, deployCoordinates, rs.jailIndex, rs.specialPiece);
        }
    }

    #endregion
}
