using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TTGameBoard : MonoBehaviour
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
    public TTJailBoard jail;
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

    private void Start()
    {
        PIECES_ADDED = Enum.GetValues(typeof(PieceType)).Length;

        //Initialize the game board and all variables
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        game_board_size = gameBoard.transform.localScale.x;
        tile_size = gameBoard.transform.localScale.x / game_board_size;
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        teamSize = 30;

        boardUI = FindObjectOfType<BoardUI>();
        boardUI.GoalText(defaultText);

        JailCells = GameObject.FindGameObjectWithTag("JailBoard");
        jail = JailCells.GetComponent<TTJailBoard>();
        NavyPieces = new Piece[teamSize];
        PiratePieces = new Piece[teamSize];
        IdentifyBoardSquares();


        Debug.Log("Testing mate");
        StartCoroutine(TestMate());
    }

    IEnumerator TestMate()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Mate, true, 5, 5); // Main Character
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 2, 2);
        NavyPieces[2] = SpawnPiece(PieceType.Mate, true, 8, 2);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 8, 5);
        NavyPieces[4] = SpawnPiece(PieceType.Ore, true, 6, 0);

        PiratePieces[0] = SpawnPiece(PieceType.Ore, false, 0, 9);
        PiratePieces[1] = SpawnPiece(PieceType.LandMine, false, 7, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Mate, false, 2, 7);
        PiratePieces[3] = SpawnPiece(PieceType.Mate, false, 6, 8);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Mate";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Mate.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the mate! The mate is one of the fundamental crewmates to have on your team.");
        boardUI.PieceDisplayDescription("\nHe's inexpensive to clone more of for your team and can be very helpful both for defensive and offensive strategies.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Mate to see how he can move.", true);

        TTSquare currentSquare = tiles[5, 5].GetComponent<TTSquare>();
        tiles[5, 5].tag = "InteractablePiece";

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                        break;
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        ResetBoardMaterials();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Mate>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if(moveAssessment[x,y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    tiles[x, y].tag = "MoveableSquare";
                    moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                }
            }
        }

        tiles[6, 6].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("The mate can move one square in any direction, but with some restrictions.");
        boardUI.PieceDisplayDescription("\nHe can't move backwards until after he's captured a piece. Notice how he can't move backwards yet?", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Mate there.", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        ResetBoardMaterials();
                        MovePiece(NavyPieces[0], 6, 6);
                        MovePiece(PiratePieces[3], 6, 7);
                        ResetBoardMaterials();
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare = tiles[6, 6].GetComponent<TTSquare>();
        tiles[6, 6].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("Good job! You've moved the mate. In response, the Pirate Mate has moved too.");
        boardUI.PieceDisplayDescription("\nThe Pirate Mate is facing the opposite direction, and so moves the opposite direction too.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Mate again to continue moving him.", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        break;
                    }
                }
            }

            yield return null;
        }
        yield return new WaitForEndOfFrame();
        ResetBoardMaterials();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Mate>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    tiles[x, y].tag = "MoveableSquare";
                    moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                }
            }
        }

        TTSquare captureSquare = tiles[6, 7].GetComponent<TTSquare>();
        captureSquare.tag = "CaptureSquare";
        captureSquare.SetMaterial(captureSquare.enemyBoardMaterial);

        // Land Mine
        tiles[7, 7].GetComponent<TTSquare>().SetMaterial(captureSquare.defaultBoardMaterial);

        boardUI.PieceDisplayDescription("The Mate captures by landing on another piece.");
        boardUI.PieceDisplayDescription("\nSince the Pirate Mate is in range, you can capture him! Notice that the Mate can't capture Energy Shields, though.", true);
        boardUI.PieceDisplayDescription("\nClick on the Red Square to capture the Pirate Mate.", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "CaptureSquare")
                    {
                        ResetBoardMaterials();
                        jail.InsertAPiece(PiratePieces[3]);
                        PiratePieces[3].destroyPiece();
                        NavyPieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 6, 7);
                        MovePiece(PiratePieces[2], 3, 6);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nice job capturing a piece!");
        boardUI.PieceDisplayDescription("\nIn response to your move, the other Pirate Mate moved, too.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Mate again to continue moving him.", true);

        currentSquare = tiles[6, 7].GetComponent<TTSquare>();
        currentSquare.tag = "InteractablePiece";

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        ResetBoardMaterials();
                        break;
                    }
                }
            }

            yield return null;
        }

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Mate>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    tiles[x, y].tag = "MoveableSquare";
                    moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                }
            }
        }
        // Land Mine
        tiles[7, 7].GetComponent<TTSquare>().SetMaterial(captureSquare.defaultBoardMaterial);
        tiles[6, 8].tag = "InteractablePiece";

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Now that you've captured a piece, the Mate can move backwards.");
        boardUI.PieceDisplayDescription("\nNotice how you can now move in all directions?", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to continue moving the Mate forwards.", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        ResetBoardMaterials();
                        MovePiece(NavyPieces[0], 6, 8);
                        MovePiece(PiratePieces[2], 4, 5);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Look at you go!");
        boardUI.PieceDisplayDescription("\nYou've moved the Mate almost all the way across the board!.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Mate again to continue moving him.", true);

        currentSquare = tiles[6, 8].GetComponent<TTSquare>();
        currentSquare.tag = "InteractablePiece";

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        ResetBoardMaterials();
                        break;
                    }
                }
            }

            yield return null;
        }

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Mate>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    tiles[x, y].tag = "MoveableSquare";
                    moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                }
            }
        }
        
        tiles[6, 9].tag = "InteractablePiece";

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("You're almost there!");
        boardUI.PieceDisplayDescription("\nThe Mate gets stronger when he reaches the back row of the board.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move him into the final board row.", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        ResetBoardMaterials();
                        MovePiece(NavyPieces[0], 6, 9);
                        MovePiece(PiratePieces[2], 5, 4);
                        break;
                    }
                }
            }

            yield return null;
        }

        currentSquare = tiles[6, 9].GetComponent<TTSquare>();
        currentSquare.tag = "InteractablePiece";

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("You made it!");
        boardUI.PieceDisplayDescription("\nNow that the Mate has made it to the back row of the board, he has gained some extra abilities.", true);
        boardUI.PieceDisplayDescription("\nClick on him again to see his new moves!", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "InteractablePiece")
                    {
                        ResetBoardMaterials();
                        break;
                    }
                }
            }

            yield return null;
        }

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Mate>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if(moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[0,9].tag = "CaptureSquare";
        tiles[0, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Woah! The Mate is really strong now.");
        boardUI.PieceDisplayDescription("\nAfter getting special abilities, the Mate can now move any distance in any direction! The Pirate Ore is now in range!", true);
        boardUI.PieceDisplayDescription("\nClick on the red Pirate Ore to capture it and finish this tutorial!", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // A square has been clicked
                if (hit.collider != null)
                {
                    tileSelected = GameObject.Find(hit.collider.name);

                    if (tileSelected.tag == "CaptureSquare")
                    {
                        ResetBoardMaterials();
                        jail.InsertAPiece(PiratePieces[0]);
                        PiratePieces[0].destroyPiece();
                        MovePiece(NavyPieces[0], 0, 9);
                        NavyPieces[0].hasOre = true;
                        MovePiece(PiratePieces[2], 5, 3);
                        break;
                    }
                }
            }

            yield return null;
        }
        
        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Main Menu");
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

    private TTSquare IdentifySquareByPiece(Piece piece)
    {
        TTSquare currentSquare;

        string squareName = "{" + (piece.currentX + 1) + "," + (piece.currentY + 1) + "}";

        Debug.Log(squareName);

        GameObject square = GameObject.Find(squareName);

        Debug.Log(square);

        currentSquare = square.GetComponent<TTSquare>();

        return currentSquare;
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
                TTSquare square = tiles[x, y].GetComponent<TTSquare>();
                square.SetMaterial(square.defaultBoardMaterial);
                square.SquareHasBeenClicked = false;
            }
        }
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
        tiles[x, y].GetComponent<TTSquare>().currentPiece = piece;
    }

    private void NullCurrentPiece(int x, int y)
    {
        tiles[x, y].GetComponent<TTSquare>().currentPiece = null;
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
            MovePiece(cp, startingX, startingY);
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
}
