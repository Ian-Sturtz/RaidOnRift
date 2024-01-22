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

    // Jail State Information
    public GameObject JailCells;
    public JailBoard jail;
    public int teamSize = 30;

    // Movement Information
    private int[,] moveAssessment;  // All legal moves of a clicked-on piece
    public bool squareSelected = false;
    public GameObject tileSelected;
    public GameObject storedTileSelected;

    // For use with bomber interactions
    [SerializeField] private bool bomberSelected = false;
    [SerializeField] private bool landMineSelected = false;
    private int cellToHighlight = -2;

    // Ore and Orebearer mechanics
    [SerializeField] private bool resetOre = false;
    [SerializeField] private bool orebearerSecondMove = false;

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

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<JailBoard>();
        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();

        // Test spawns one piece of each type in a random spot on the board
        NavyPieces[0] = SpawnPiece(PieceType.Ore, true, 3, 0);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 1, 2);
        NavyPieces[2] = SpawnPiece(PieceType.LandMine, true, 7, 6);
        NavyPieces[3] = SpawnPiece(PieceType.Royal1, true, 0, 3);
        NavyPieces[4] = SpawnPiece(PieceType.Vanguard, true, 1, 3);
        NavyPieces[5] = SpawnPiece(PieceType.Navigator, true, 2, 4);
        NavyPieces[6] = SpawnPiece(PieceType.Gunner, true, 4, 6);
        NavyPieces[7] = SpawnPiece(PieceType.Cannon, true, 3, 3);
        NavyPieces[8] = SpawnPiece(PieceType.Bomber, true, 5, 4);
        NavyPieces[9] = SpawnPiece(PieceType.LandMine, true, 6, 6);
        NavyPieces[10] = SpawnPiece(PieceType.Quartermaster, true, 5, 6);

        PiratePieces[0] = SpawnPiece(PieceType.Ore, false, 7, 9);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 9, 9);
        PiratePieces[2] = SpawnPiece(PieceType.LandMine, false, 5, 5);
        PiratePieces[3] = SpawnPiece(PieceType.Royal1, false, 8, 6);
        PiratePieces[4] = SpawnPiece(PieceType.Vanguard, false, 3, 7);
        PiratePieces[5] = SpawnPiece(PieceType.Navigator, false, 1, 8);
        PiratePieces[6] = SpawnPiece(PieceType.Gunner, false, 6, 9);
        PiratePieces[7] = SpawnPiece(PieceType.Cannon, false, 7, 7);
        PiratePieces[8] = SpawnPiece(PieceType.Bomber, false, 7, 5);
        PiratePieces[9] = SpawnPiece(PieceType.Royal1, false, 4, 0);
        PiratePieces[10] = SpawnPiece(PieceType.Quartermaster, false, 6, 4);
    }

    private void Update()
    {
        // Quits the game if user hits ESC (temporary prototype feature)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Check for a game win
        for (int i = 0; i < TILE_COUNT_X && !gameWon; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Piece checkPiece = tiles[i, j].GetComponent<Square>().currentPiece;

                if(checkPiece != null)
                {
                    if (checkPiece.hasOre && checkPiece.isNavy)
                    {
                        Debug.Log("The Navy Won!");
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
                        Debug.Log("The Pirates Won!");
                        gameWon = true;
                    }
                }
            }
        }

        // The active player has selected a bomber to use this turn
        if (storedTileSelected != null)
        {
            if (storedTileSelected.GetComponent<Square>().currentPiece.type == PieceType.Bomber) {
                bomberSelected = true;
            }
        }
        else
        {
            bomberSelected = false;
        }

        // Highlights a captured enemy land mines so the bomber can deploy them
        if (bomberSelected && cellToHighlight == -2)
        {
            // The selected bomber is Navy and can deploy Pirate Bombs
            if (tileSelected.GetComponent<Square>().currentPiece.isNavy)
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
                            Debug.Log("It's not your turn!");
                            Square selectedTile = tileSelected.GetComponent<Square>();
                            selectedTile.FlashMaterial(selectedTile.clickedBoardMaterial, 3);
                        }
                        // The right team is trying to move
                        else
                        {
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
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        if (currentSquare.currentPiece.type == PieceType.Gunner)
                        {
                            currentSquare.currentPiece.hasCaptured = false;
                        }
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

                        // Move current piece to the new square (unless it's a gunner)
                        if (currentPiece.type != PieceType.Gunner)
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
                            storedTileSelected = tileSelected;
                            targetSquare.tag = "CaptureSquare";
                            DetectLegalMoves(storedTileSelected, currentPiece);
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

                    // A landmine has been selected from jail
                    else if (tileSelected.tag == "InteractablePiece")
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
                        ResetBoardMaterials();
                        
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
                    Debug.Log("The Ore doesn't move!");
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<Square>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case PieceType.LandMine:
                    Debug.Log("The Land Mine doesn't move!");
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
        }
        
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
                }
                // Square contains a capturable piece by replacement
                else if (moveAssessment[x,y] == 2)
                {
                    tiles[x, y].tag = "CaptureSquare";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                }
                // Square contains a capturable piece by shooting
                else if(moveAssessment[x,y] == 3)
                {
                    tiles[x, y].tag = "GunnerTarget";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
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
                    }
                }
                // Square can be moved to for jumping capture
                else if (moveAssessment[x, y] == 5)
                {
                    tiles[x, y].tag = "CannonDestination";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                }
                // A Land Mine can be deployed here
                else if (moveAssessment[x,y] == 6)
                {
                    tiles[x, y].tag = "MineDeploy";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                }
                // The ore can be deployed here
                else if (moveAssessment[x,y] == 7)
                {
                    tiles[x, y].tag = "OreDeploy";
                    Square activeSquare = tiles[x, y].GetComponent<Square>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                }
            }
        }
    }

    // Changes the turn from one player to the next
    private void NextTurn()
    {
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
    }
}
