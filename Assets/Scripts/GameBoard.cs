using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    #region GameInfo
    // How many pieces have been added to the game so far
    public int PIECES_ADDED;

    // Game State Information
    public bool gameWon = false;
    public bool stalemate = false;
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

    BoardUI boardUI;

    #endregion

    #region JailInfo

    // Jail State Information
    public GameObject JailCells;
    public JailBoard jail;
    public int teamSize = 30;

    #endregion

    #region MovementInfo
    // Movement Information
    private int[,] moveAssessment;  // All legal moves of a clicked-on piece
    public bool squareSelected = false;
    public GameObject tileSelected;
    public GameObject storedTileSelected;

    #endregion

    #region PieceInteractions
    // For use with bomber interactions
    [SerializeField] private bool bomberSelected = false;
    [SerializeField] private bool landMineSelected = false;
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
    [SerializeField] private int jumpCooldown = 0;

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
        boardUI.GoalText("Click on a piece to move it!");

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<JailBoard>();
        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();

        // Decent board starting positions for a sample game
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
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 1, 5);

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

        // Test spawns one piece of each type in a random spot on the board
        //NavyPieces[0] = SpawnPiece(PieceType.Ore, true, 3, 0);
        //NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 1, 2);
        //NavyPieces[2] = SpawnPiece(PieceType.LandMine, true, 7, 6);
        //NavyPieces[3] = SpawnPiece(PieceType.Royal1, true, 0, 3);
        //NavyPieces[4] = SpawnPiece(PieceType.Vanguard, true, 1, 3);
        //NavyPieces[5] = SpawnPiece(PieceType.Navigator, true, 2, 4);
        //NavyPieces[6] = SpawnPiece(PieceType.Gunner, true, 4, 6);
        //NavyPieces[7] = SpawnPiece(PieceType.Cannon, true, 3, 3);
        //NavyPieces[8] = SpawnPiece(PieceType.Bomber, true, 5, 4);
        //NavyPieces[9] = SpawnPiece(PieceType.LandMine, true, 6, 6);
        //NavyPieces[10] = SpawnPiece(PieceType.Quartermaster, true, 5, 6);
        //NavyPieces[11] = SpawnPiece(PieceType.Royal2, true, 8, 9);

        //PiratePieces[0] = SpawnPiece(PieceType.Ore, false, 7, 9);
        //PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 9, 9);
        //PiratePieces[2] = SpawnPiece(PieceType.LandMine, false, 5, 5);
        //PiratePieces[3] = SpawnPiece(PieceType.Royal1, false, 8, 6);
        //PiratePieces[4] = SpawnPiece(PieceType.Vanguard, false, 3, 7);
        //PiratePieces[5] = SpawnPiece(PieceType.Navigator, false, 1, 8);
        //PiratePieces[6] = SpawnPiece(PieceType.Gunner, false, 6, 9);
        //PiratePieces[7] = SpawnPiece(PieceType.Cannon, false, 7, 7);
        //PiratePieces[8] = SpawnPiece(PieceType.Bomber, false, 7, 5);
        //PiratePieces[9] = SpawnPiece(PieceType.Royal1, false, 4, 0);
        //PiratePieces[10] = SpawnPiece(PieceType.Quartermaster, false, 6, 4);
        //PiratePieces[11] = SpawnPiece(PieceType.Royal2, false, 9, 4);
    }

    private void Update()
    {
        // Check for a game win
        for (int i = 0; i < TILE_COUNT_X && !gameWon; i++)
        {
            Piece checkPiece = tiles[i, 0].GetComponent<Square>().currentPiece;

            if (checkPiece != null)
            {
                if (checkPiece.hasOre && checkPiece.isNavy)
                {
                    gameWon = true;
                    checkPiece.gameWon = true;

                    // Update UI
                    boardUI.GameWon(true);
                }
            }

            checkPiece = tiles[i, 9].GetComponent<Square>().currentPiece;

            if (checkPiece != null)
            {
                if (checkPiece.hasOre && !checkPiece.isNavy)
                {
                    gameWon = true;
                    checkPiece.gameWon = true;

                    // Update UI
                    boardUI.GameWon(false);
                }
            }
        }

        // The active player has selected a bomber or tactician to use this turn
        if (storedTileSelected != null)
        {
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

                    if(jail.tacticianMimicPieces[i] != null)
                        jail.tacticianMimicPieces[i].GetComponent<Piece>().destroyPiece();
                    jail.TacticianMimicCells[i].GetComponent<JailCell>().resetCell();
                    ResetBoardMaterials(true);
                }
            }
        }

        // Highlights a captured enemy land mines so the bomber can deploy them
        if (bomberSelected && cellToHighlight == -2)
        {
            // Tactician is mimicking a bomber
            if (tacticianInheritSelected)
            {
                cellToHighlight = jail.FindPiece(PieceType.LandMine, false);
                if(cellToHighlight >= 0)
                {
                    jail.NavyJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
                    // A bomb is being highlighted right now in a jail cell, it can potentially be replaced on the board
                }
            }
            // The selected bomber is Navy and can deploy Pirate Bombs
            else if (tileSelected.GetComponent<Square>().currentPiece.isNavy)
            {
                cellToHighlight = jail.FindPiece(PieceType.LandMine, false);
                if (cellToHighlight >= 0)
                {
                    jail.NavyJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
                }
            }
            // The selected bomber is Pirate and can deploy Navy Bombs
            else
            {
                cellToHighlight = jail.FindPiece(PieceType.LandMine, true);
                if (cellToHighlight >= 0)
                {
                    jail.PirateJailCells[cellToHighlight].GetComponent<JailCell>().interactable = true;
                }
            }
        }

        // Highlights all possible pieces that tactician can inherit
        if(tacticianSelected)
        {
            for (int i = 0; i < 9; i++)
            {
                if(jail.TacticianMimicCells[i].GetComponent<JailCell>().hasPiece == true) {
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
        if (Input.GetMouseButtonDown(0) && !gameWon && !stalemate)
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
                    // Do nothing
                }

                // No square had previously been clicked
                else if (!squareSelected)
                {
                    Square current_square = tileSelected.GetComponent<Square>();
                    if (current_square.currentPiece != null)
                    {
                        // The wrong team is trying to move
                        if(current_square.currentPiece.isNavy != navyTurn)
                        {
                            boardUI.DisplayTempText("It's not your turn yet!", 1.5f);
                            Debug.Log("It's not your turn!");
                            Square selectedTile = tileSelected.GetComponent<Square>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);
                        }
                        // The right team is trying to move
                        else
                        {
                            if(current_square.currentPiece.type == PieceType.Royal2 && current_square.currentPiece.isNavy)
                            {
                                tacticianSelected = true;
                            }
                            if(current_square.currentPiece.type == PieceType.Bomber)
                            {
                                bomberSelected = true;
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
                    // A moveable square has been clicked
                    if (tileSelected.tag == "MoveableSquare")
                    {
                        Square currentSquare = storedTileSelected.GetComponent<Square>();

                        // Checks if the piece is a Tactician that has moved since mimicking a Gunner
                        Piece currentPiece = currentSquare.currentPiece;
                        if (currentPiece.type == PieceType.Royal2 && currentPiece.isNavy)
                        {
                            tacticianGunnerCapture = false;
                        }

                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        if (currentSquare.currentPiece.type == PieceType.Gunner)
                        {
                            currentSquare.currentPiece.hasCaptured = false;
                        }
                        MovePiece(currentSquare.currentPiece, moveCoordinates.x, moveCoordinates.y);

                        ResetBoardMaterials();
                        Square selectedTile = tileSelected.GetComponent<Square>();
                        selectedTile.FlashMaterial(selectedTile.moveableBoardMaterial, 2);
                        squareSelected = false;
                        currentSquare.SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                        NextTurn();
                    }

                    // The Corsair has jumped to an open board space
                    else if (tileSelected.tag == "CorsairJump")
                    {
                        Square currentSquare = storedTileSelected.GetComponent<Square>();

                        // Corsair jumping requires 2 turns of cooldown before the next jump
                        if (currentSquare.currentPiece.type == PieceType.Royal2 && !currentSquare.currentPiece.isNavy)
                            jumpCooldown = 3;
                        // Tactician inherits a corsair and now has a cooldown
                        else
                        {
                            tacticianCorsairJump = 3;
                        }
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

                    else if (tileSelected.tag == "CaptureSquare" || tileSelected.tag == "GunnerTarget")
                    {
                        Square CurrentSquare = storedTileSelected.GetComponent<Square>();
                        Square targetSquare = tileSelected.GetComponent<Square>();
                        Piece currentPiece = CurrentSquare.currentPiece;
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        Piece capturedPiece = targetSquare.currentPiece;

                        // Check if capture target is the ore
                        if (capturedPiece.type == PieceType.Ore)
                        {
                            currentPiece.hasOre = true;

                            // Update UI
                            boardUI.UpdateGoal(navyTurn, true);
                        }

                        // If the orebearer is being captured, the ore needs to be reset
                        if (capturedPiece.hasOre)
                        {
                            resetOre = true;
                        }

                        // Capture that Piece
                        jail.InsertAPiece(capturedPiece);
                        capturedPiece.destroyPiece();
                        currentPiece.hasCaptured = true;

                        // Prevents the Tactician from mimicking a Gunner and capturing twice
                        if (tileSelected.tag == "GunnerTarget")
                        {
                            if (currentPiece.type == PieceType.Royal2 && currentPiece.isNavy)
                            {
                                tacticianGunnerCapture = true;
                            }
                        }

                        // Move current piece to the new square (unless it's a gunner)
                        if (tileSelected.tag != "GunnerTarget")
                        {
                            MovePiece(currentPiece, moveCoordinates.x, moveCoordinates.y);
                        }
                        CurrentSquare.SquareHasBeenClicked = false;
                        targetSquare.FlashMaterial(targetSquare.clickedBoardMaterial, 2);

                        // Clean up board now that move has completed
                        ResetBoardMaterials();

                        // Ore needs to be reset before the turn ends
                        if (resetOre)
                        {
                            if(currentPiece.type == PieceType.Gunner)
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

                        // Turn is now over
                        if (!resetOre && !orebearerSecondMove)
                        {
                            squareSelected = false;
                            tileSelected = null;
                            storedTileSelected = null;
                            ResetBoardMaterials();
                            NextTurn();
                        }
                    }

                    // Cannon is capturing a piece by jumping
                    else if (tileSelected.tag == "CannonDestination")
                    {
                        Square CurrentSquare = storedTileSelected.GetComponent<Square>();
                        Square targetSquare = tileSelected.GetComponent<Square>();
                        Piece currentPiece = CurrentSquare.currentPiece;
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        // Find which piece is being captured
                        Square captureSquare;
                        Piece capturedPiece = null;

                        // Checks the square to the right of move square
                        if (moveCoordinates.x + 1 < 10)
                        {
                            Debug.Log("Checked Right");
                            if (tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<Square>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Right");
                                captureSquare = tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<Square>();
                                capturedPiece = captureSquare.currentPiece;
                                captureSquare.currentPiece = null;
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
                                captureSquare.currentPiece = null;
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
                                captureSquare.currentPiece = null;
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
                                captureSquare.currentPiece = null;
                            }
                        }

                        // A piece is being captured
                        if (capturedPiece != null)
                        {
                            // Check if the captured piece is the ore
                            if (capturedPiece.type == PieceType.Ore)
                            {
                                currentPiece.hasOre = true;

                                // Update UI
                                boardUI.UpdateGoal(navyTurn, true);
                            }

                            // If the orebearer is being captured, the ore needs to be reset
                            if (capturedPiece.hasOre)
                            {
                                resetOre = true;
                            }

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
                        if (!resetOre && !orebearerSecondMove)
                        {
                            squareSelected = false;
                            tileSelected = null;
                            storedTileSelected = null;
                            ResetBoardMaterials();
                            NextTurn();
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
                                    if(jail.TacticianMimicCells[i].GetComponent<JailCell>().hasPiece)
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
                                Debug.Log(currentPosition.x + ", " + currentPosition.y);

                                DetectLegalMoves(storedTileSelected, inheritingPiece);

                                tileSelected = storedTileSelected;
                            }
                        }
                    }

                    // A Land Mine is being redeployed to the board
                    else if (tileSelected.tag == "MineDeploy")
                    {
                        Debug.Log("Deploying mine");
                        Square bomberSquare = storedTileSelected.GetComponent<Square>();
                        Vector2Int deployCoordinates = IdentifyThisBoardSquare(tileSelected);
                        int spawnIndex = FindFirstOpenTeamSlot(!bomberSquare.currentPiece.isNavy);

                        if (bomberSquare.currentPiece.isNavy)
                        {
                            jail.pirateJailedPieces[cellToHighlight].GetComponent<Piece>().destroyPiece();
                            jail.NavyJailCells[cellToHighlight].GetComponent<JailCell>().resetCell();
                            PiratePieces[spawnIndex] = SpawnPiece(PieceType.LandMine, false, deployCoordinates.x, deployCoordinates.y);
                        }
                        else
                        {
                            jail.navyJailedPieces[cellToHighlight].GetComponent<Piece>().destroyPiece();
                            jail.PirateJailCells[cellToHighlight].GetComponent<JailCell>().resetCell();
                            NavyPieces[spawnIndex] = SpawnPiece(PieceType.LandMine, true, deployCoordinates.x, deployCoordinates.y);
                        }

                        // Clean up now that mine has been deployed
                        ResetBoardMaterials();
                        squareSelected = false;
                        tileSelected.GetComponent<Square>().SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected.GetComponent<Square>().SquareHasBeenClicked = false;
                        storedTileSelected = null;
                        bomberSelected = false;
                        landMineSelected = false;
                        NextTurn();
                    }

                    // The ore is being redeployed to the board
                    else if (tileSelected.tag == "OreDeploy")
                    {
                        Debug.Log("Redeploying Ore");
                        Square pieceSquare = storedTileSelected.GetComponent<Square>();
                        Vector2Int deployCoordinates = IdentifyThisBoardSquare(tileSelected);
                        int spawnIndex = FindFirstOpenTeamSlot(pieceSquare.currentPiece.isNavy);

                        cellToHighlight = jail.FindPiece(PieceType.Ore, pieceSquare.currentPiece.isNavy);

                        Debug.Log(cellToHighlight);

                        if (pieceSquare.currentPiece.isNavy)
                        {
                            jail.navyJailedPieces[cellToHighlight].GetComponent<Piece>().destroyPiece();
                            jail.PirateJailCells[cellToHighlight].GetComponent<JailCell>().resetCell();
                            NavyPieces[spawnIndex] = SpawnPiece(PieceType.Ore, true, deployCoordinates.x, deployCoordinates.y);
                        }
                        else
                        {
                            jail.pirateJailedPieces[cellToHighlight].GetComponent<Piece>().destroyPiece();
                            jail.NavyJailCells[cellToHighlight].GetComponent<JailCell>().resetCell();
                            PiratePieces[spawnIndex] = SpawnPiece(PieceType.Ore, false, deployCoordinates.x, deployCoordinates.y);
                        }

                        // Clean up now that ore has been redeployed
                        ResetBoardMaterials();
                        resetOre = false;
                        tileSelected.GetComponent<Square>().SquareHasBeenClicked = false;
                        tileSelected = null;
                        bomberSelected = false;
                        landMineSelected = false;

                        // Turn is now over
                        if (!orebearerSecondMove)
                        {
                            squareSelected = false;
                            storedTileSelected.GetComponent<Square>().SquareHasBeenClicked = false;
                            storedTileSelected = null;
                            NextTurn();
                        }
                        // Return control to the orebearer for a second turn
                        else
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

                        // Ends turn if orebearer decides not to move a second time
                        if (tileSelected.GetComponent<Square>().currentPiece.hasOre && orebearerSecondMove)
                        {
                            orebearerSecondMove = false;
                            NextTurn();
                        }

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
                        moveAssessment = piece.GetComponent<Bomber>().GetValidMoves(tiles);
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
                    }
                    else
                    {
                        if(jumpCooldown > 0 && !tacticianSelected)
                        {
                            boardUI.DisplayTempText("The Corsair needs to recharge before jumping again. Click on a different piece to move it!");
                            Debug.Log("Corsair can't jump yet, move someone else!");
                            squareSelected = false;
                            current_square.SquareHasBeenClicked = false;
                            current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<Square>().clickedBoardMaterial, 3);
                            tileSelected = null;
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

        //bool moveAble = false;
        //bool captureAble = false;
        //bool gunnerAble = false;
        //bool cannonTarget = false;
        //bool cannonJump = false;
        //bool mineDeploy = false;
        //bool oreDeploy = false;
        //bool corsairJump = false;

        // Displays current possible game actions in goal text
        if (!invalidPiece && !moveAble && !captureAble && !gunnerAble && !cannonJump && !mineDeploy && !oreDeploy && !corsairJump)
        {
            // No move is available with this piece
            boardUI.GoalText("This piece has no valid moves. Click on it again to cancel.");
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
            if(bomberSelected && !landMineSelected)
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
            // Default message
            boardUI.GoalText("- Click on that piece again to cancel the move", true);
        }
    }

    // Checks if a team has no valid moves left to trigger a stalemate
    private void CheckForStalemate(bool checkingNavy)
    {
        bool noLegalMoves = false;

        moveAssessment = new int[TILE_COUNT_X, TILE_COUNT_Y];

        // Checks all pieces left on the Navy's board to ensure there are legal moves open
        if (checkingNavy)
        {
            for (int i = 0; i < teamSize; i++)
            {
                if(NavyPieces[i] != null)
                {
                    Piece possiblePiece = NavyPieces[i];

                    switch (possiblePiece.type)
                    {
                        case PieceType.Mate:
                            moveAssessment = possiblePiece.GetComponent<Mate>().GetValidMoves(tiles);
                            break;
                        case PieceType.Bomber:
                            moveAssessment = possiblePiece.GetComponent<Bomber>().GetValidMoves(tiles);
                            break;
                        case PieceType.Vanguard:
                            moveAssessment = possiblePiece.GetComponent<Vanguard>().GetValidMoves(tiles);
                            break;
                        case PieceType.Navigator:
                            moveAssessment = possiblePiece.GetComponent<Navigator>().GetValidMoves(tiles);
                            break;
                        case PieceType.Gunner:
                            moveAssessment = possiblePiece.GetComponent<Gunner>().GetValidMoves(tiles);
                            break;
                        case PieceType.Cannon:
                            moveAssessment = possiblePiece.GetComponent<Cannon>().GetValidMoves(tiles);
                            break;
                        case PieceType.Quartermaster:
                            moveAssessment = possiblePiece.GetComponent<Quartermaster>().GetValidMoves(tiles);
                            break;
                        case PieceType.Royal2:
                            moveAssessment = possiblePiece.GetComponent<Tactician>().GetValidMoves(tiles);
                            break;
                        case PieceType.Royal1:
                            moveAssessment = possiblePiece.GetComponent<Admiral>().GetValidMoves(tiles);
                            break;
                    }

                    for (int x = 0; x < TILE_COUNT_X; x++)
                    {
                        for (int y = 0; y < TILE_COUNT_Y; y++)
                        {

                        }
                    }
                }
            }
        }
    }

    // Changes the turn from one player to the next
    private void NextTurn()
    {
        ResetBoardMaterials(true);

        if(tacticianCorsairJump != 0)
        {
            tacticianCorsairJump--;
        }

        if(jumpCooldown != 0)
        {
            jumpCooldown--;
        }

        orebearerSecondMove = false;
        if (!resetOre)
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

        // Update UI
        boardUI.UpdateTurn(navyTurn);
        boardUI.GoalText("Click on a piece to move it!");
    }
}
