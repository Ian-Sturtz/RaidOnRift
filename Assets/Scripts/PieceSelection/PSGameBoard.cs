using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSGameBoard : MonoBehaviour
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

    public int teamSize = 30;

    #region BoardInfo
    // Board Information
    public float tile_size = 1f;
    public float tile_size_margins = 1.05f;
    public float game_board_size = 10.55f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    private GameObject gameBoard;   // Reference to gameBoard object

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

        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();

        SpawnAllPieces();

    }

    private void Update()
    {
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
                if (tileSelected.tag == "JailCell")
                {
                    // Do nothing
                }

                // No square had previously been clicked
                else if (!squareSelected)
                {
                    PSSquare current_square = tileSelected.GetComponent<PSSquare>();
                    if (current_square.currentPiece != null)
                    {
                        // The wrong team is trying to move
                        if (current_square.currentPiece.isNavy != navyTurn)
                        {
                            Debug.Log("It's not your turn!");
                            PSSquare selectedTile = tileSelected.GetComponent<PSSquare>();
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
                        PSSquare currentSquare = storedTileSelected.GetComponent<PSSquare>();

                        // Checks if the piece is a Tactician that has moved since mimicking a Gunner
                        Piece currentPiece = currentSquare.currentPiece;
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        if (currentSquare.currentPiece.type == PieceType.Gunner)
                        {
                            currentSquare.currentPiece.hasCaptured = false;
                        }
                        MovePiece(currentSquare.currentPiece, moveCoordinates.x, moveCoordinates.y);

                        ResetBoardMaterials();
                        PSSquare selectedTile = tileSelected.GetComponent<PSSquare>();
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
                        PSSquare currentSquare = storedTileSelected.GetComponent<PSSquare>();

                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        MovePiece(currentSquare.currentPiece, moveCoordinates.x, moveCoordinates.y);
                        ResetBoardMaterials();
                        NextTurn();
                        PSSquare selectedTile = tileSelected.GetComponent<PSSquare>();
                        selectedTile.FlashMaterial(selectedTile.moveableBoardMaterial, 2);
                        squareSelected = false;
                        currentSquare.SquareHasBeenClicked = false;
                        tileSelected = null;
                        storedTileSelected = null;
                    }

                    else if (tileSelected.tag == "CaptureSquare" || tileSelected.tag == "GunnerTarget")
                    {
                        PSSquare CurrentSquare = storedTileSelected.GetComponent<PSSquare>();
                        PSSquare targetSquare = tileSelected.GetComponent<PSSquare>();
                        Piece currentPiece = CurrentSquare.currentPiece;
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);
                        Piece capturedPiece = targetSquare.currentPiece;

                        // Capture that Piece
                        capturedPiece.destroyPiece();
                        currentPiece.hasCaptured = true;

                        // Move current piece to the new square (unless it's a gunner)
                        if (tileSelected.tag != "GunnerTarget")
                        {
                            MovePiece(currentPiece, moveCoordinates.x, moveCoordinates.y);
                        }
                        CurrentSquare.SquareHasBeenClicked = false;
                        targetSquare.FlashMaterial(targetSquare.clickedBoardMaterial, 2);

                        // Clean up board now that move has completed
                        ResetBoardMaterials();

                        // Turn is now over
                        if (true)
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
                        PSSquare CurrentSquare = storedTileSelected.GetComponent<PSSquare>();
                        PSSquare targetSquare = tileSelected.GetComponent<PSSquare>();
                        Piece currentPiece = CurrentSquare.currentPiece;
                        Vector2Int moveCoordinates = IdentifyThisBoardSquare(tileSelected);

                        // Find which piece is being captured
                        PSSquare captureSquare;
                        Piece capturedPiece = null;

                        // Checks the square to the right of move square
                        if (moveCoordinates.x + 1 < 10)
                        {
                            Debug.Log("Checked Right");
                            if (tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<PSSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Right");
                                captureSquare = tiles[moveCoordinates.x + 1, moveCoordinates.y].GetComponent<PSSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureSquare.currentPiece = null;
                            }
                        }
                        // Checks the square to the left of move square
                        if (moveCoordinates.x - 1 >= 0 && capturedPiece == null)
                        {
                            Debug.Log("Checked Left");
                            if (tiles[moveCoordinates.x - 1, moveCoordinates.y].GetComponent<PSSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Left");
                                captureSquare = tiles[moveCoordinates.x - 1, moveCoordinates.y].GetComponent<PSSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureSquare.currentPiece = null;
                            }
                        }
                        // Checks the square above the move square
                        if (moveCoordinates.y + 1 < 10 && capturedPiece == null)
                        {
                            Debug.Log("Checked Up");
                            if (tiles[moveCoordinates.x, moveCoordinates.y + 1].GetComponent<PSSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Up");
                                captureSquare = tiles[moveCoordinates.x, moveCoordinates.y + 1].GetComponent<PSSquare>();
                                capturedPiece = captureSquare.currentPiece;
                                captureSquare.currentPiece = null;
                            }
                        }
                        // Checks the square below the move square
                        if (moveCoordinates.y - 1 >= 0 && capturedPiece == null)
                        {
                            Debug.Log("Checked Down");
                            if (tiles[moveCoordinates.x, moveCoordinates.y - 1].GetComponent<PSSquare>().tag == "CannonTarget")
                            {
                                Debug.Log("Found Down");
                                captureSquare = tiles[moveCoordinates.x, moveCoordinates.y - 1].GetComponent<PSSquare>();
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

                            // Capture that piece
                            capturedPiece.destroyPiece();
                            currentPiece.hasCaptured = true;
                        }

                        // Move current piece to that square
                        MovePiece(currentPiece, moveCoordinates.x, moveCoordinates.y);
                        CurrentSquare.SquareHasBeenClicked = false;
                        targetSquare.FlashMaterial(targetSquare.clickedBoardMaterial, 2);

                        // Clean up board now that move has completed
                        ResetBoardMaterials();
                    }
                   
                    // The same square was clicked twice (cancel the square selection)
                    else if (tileSelected.GetComponent<PSSquare>().SquareHasBeenClicked)
                    {
                        ResetBoardMaterials(true);


                        squareSelected = false;
                        tileSelected.GetComponent<PSSquare>().SquareHasBeenClicked = false;
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

    private void ResetBoardMaterials(bool resetJail = true)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                tiles[x, y].tag = "GameSquare";
                PSSquare square = tiles[x, y].GetComponent<PSSquare>();
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
                if (NavyPieces[index] == null)
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
                if (tiles[x, y] == square)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);

    }

    private void SetCurrentPiece(Piece piece, int x, int y)
    {
        tiles[x, y].GetComponent<PSSquare>().currentPiece = piece;
    }

    private void NullCurrentPiece(int x, int y)
    {
        tiles[x, y].GetComponent<PSSquare>().currentPiece = null;
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

        if (startingX != -1 && startingY != -1)
        {
            MovePiece(cp, startingX, startingY, false);
        }

        return cp;
    }

    public void MovePiece(Piece piece, int x, int y, bool lerpMove = true)
    {
        Vector3 targetPosition = tiles[x, y].transform.position;

        // Removes piece from original square and puts it in new square
        if (piece.currentX != -1 && piece.currentY != -1)
        {
            NullCurrentPiece(piece.currentX, piece.currentY);
        }
        tiles[x, y].GetComponent<PSSquare>().currentPiece = piece;
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
        PSSquare current_square;
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

        current_square = tiles[current_x, current_y].GetComponent<PSSquare>();

            switch (piece.type)
            {
                case PieceType.Ore:
                    Debug.Log("The Ore doesn't move!");
                    invalidPiece = true;
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<PSSquare>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case PieceType.LandMine:
                    Debug.Log("The Land Mine doesn't move!");
                    invalidPiece = true;
                    squareSelected = false;
                    current_square.SquareHasBeenClicked = false;
                    current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<PSSquare>().clickedBoardMaterial, 3);
                    tileSelected = null;
                    break;
                case PieceType.Mate:
                    moveAssessment = piece.GetComponent<Mate>().GetValidMoves(tiles);
                    break;
                case PieceType.Bomber:
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
                    if (piece.isNavy)
                    {
                        moveAssessment = piece.GetComponent<Tactician>().GetValidMoves(tiles);
                    }
                    else
                    {
                        if (false)
                        {
                            Debug.Log("Corsair can't jump yet, move someone else!");
                            squareSelected = false;
                            current_square.SquareHasBeenClicked = false;
                            current_square.FlashMaterial(tiles[current_x, current_y].GetComponent<PSSquare>().clickedBoardMaterial, 3);
                            tileSelected = null;
                        }
                        else
                        {
                            moveAssessment = piece.GetComponent<Corsair>().GetValidMoves(tiles);
                        }
                    }
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
        

        // Ensures that an enemy is not moving to a landmine or allied occupied square
        for (int x = 0; x < TILE_COUNT_X && !piece.hasOre; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    Piece possiblePiece = tiles[x, y].GetComponent<PSSquare>().currentPiece;

                    // There is a piece in that possible move square
                    if (possiblePiece != null)
                    {
                        // That piece is on the same team (can't move there)
                        if (piece.isNavy == possiblePiece.isNavy)
                        {
                            moveAssessment[x, y] = -1;
                        }
                        // That piece is a land mine (can't move there)
                        else if (possiblePiece.type == PieceType.LandMine)
                        {
                            if (piece.type == PieceType.Bomber)
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
                if (moveAssessment[x, y] == 1)
                {
                    tiles[x, y].tag = "MoveableSquare";
                    PSSquare activeSquare = tiles[x, y].GetComponent<PSSquare>();
                    activeSquare.SetMaterial(activeSquare.moveableBoardMaterial);
                    moveAble = true;
                }
                // Square contains a capturable piece by replacement
                else if (moveAssessment[x, y] == 2)
                {
                    tiles[x, y].tag = "CaptureSquare";
                    PSSquare activeSquare = tiles[x, y].GetComponent<PSSquare>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    captureAble = true;
                }
                // Square contains a capturable piece by shooting
                else if (moveAssessment[x, y] == 3)
                {
                    if (true) // check this later
                    {
                        tiles[x, y].tag = "GunnerTarget";
                        PSSquare activeSquare = tiles[x, y].GetComponent<PSSquare>();
                        activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                        gunnerAble = true;
                    }
                }
                // Square contains a capturable peice by jumping
                else if (moveAssessment[x, y] == 4)
                {
                    // Cannon can jump a Land Mine but not capture it
                    if (tiles[x, y].GetComponent<PSSquare>().currentPiece.type != PieceType.LandMine)
                    {
                        tiles[x, y].tag = "CannonTarget";
                        PSSquare activeSquare = tiles[x, y].GetComponent<PSSquare>();
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
                else if (moveAssessment[x, y] == 6)
                {
                    tiles[x, y].tag = "MineDeploy";
                    PSSquare activeSquare = tiles[x, y].GetComponent<PSSquare>();
                    activeSquare.SetMaterial(activeSquare.enemyBoardMaterial);
                    mineDeploy = true;
                }
                // The ore can be deployed here
                else if (moveAssessment[x, y] == 7)
                {
                    tiles[x, y].tag = "OreDeploy";
                    PSSquare activeSquare = tiles[x, y].GetComponent<PSSquare>();
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
                if (NavyPieces[i] != null)
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

    private void SpawnAllPieces()
    {
        int navyPiecesAdded = 0;
        int piratePiecesAdded = 0;

        if (PieceManager.instance == null)
        {
            Debug.Log("No pieces available, using default spawn");

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
                    NavyPieces[navyPiecesAdded] = SpawnPiece(PieceManager.instance.pieceTypes[i], true, PieceManager.instance.pieceCoords[i, 0], PieceManager.instance.pieceCoords[i, 1]);
                    navyPiecesAdded++;
                }
                else
                {
                    PiratePieces[piratePiecesAdded] = SpawnPiece(PieceManager.instance.pieceTypes[i], false, PieceManager.instance.pieceCoords[i, 0], PieceManager.instance.pieceCoords[i, 1]);
                    piratePiecesAdded++;
                }
            }
        }
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
    }
}
