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

    public SpriteRenderer[] boardEdges = new SpriteRenderer[2];
    public Material NavyEdges;
    public Material PirateEdges;

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

    // Used for Tactician interactions
    public bool tacticianSelected = false;
    public bool tacticianInheritSelected = false;
    public bool tacticianGunnerCapture = false;
    public int tacticianCorsairJump = 0;

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

        switch (StaticTutorialControl.piece)
        {
            case PieceType.Mate:
                Debug.Log("Testing Mate");
                StartCoroutine(TestMate());
                break;
            case PieceType.Engineer:
                Debug.Log("Testing Engineer");
                StartCoroutine(TestEngineer());
                break;
            case PieceType.Vanguard:
                Debug.Log("Testing Vanguard");
                StartCoroutine(TestVanguard());
                break;
            case PieceType.Navigator:
                Debug.Log("Testing Navigator");
                StartCoroutine(TestNavigator());
                break;
            case PieceType.Gunner:
                Debug.Log("Testing Gunner");
                StartCoroutine(TestGunner());
                break;
            case PieceType.Cannon:
                Debug.Log("Testing Cannon");
                StartCoroutine(TestCannon());
                break;
            case PieceType.Quartermaster:
                Debug.Log("Testing Quartermaster");
                StartCoroutine(TestQuartermaster());
                break;
            case PieceType.Royal2:
                if (StaticTutorialControl.isNavy)
                {
                    Debug.Log("Testing Tactician");
                    StartCoroutine(TestTactician());
                }
                else
                {
                    Debug.Log("Testing Corsair");
                    StartCoroutine(TestCorsair());
                }
                break;
            case PieceType.Royal1:
                if (StaticTutorialControl.isNavy)
                {
                    Debug.Log("Testing Admiral");
                    StartCoroutine(TestAdmiral());
                }
                else
                {
                    Debug.Log("Testing Captain");
                    StartCoroutine(TestCaptain());
                }
                break;
            default:
                Debug.Log("Default test case");
                StaticTutorialControl.cameFromStoryScene = true;
                StartCoroutine(TestCannon());
                break;
        }
    }
    
    IEnumerator TestCannon()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Cannon, true, 9, 2);
        NavyPieces[1] = SpawnPiece(PieceType.Cannon, true, 0, 1);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 7, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 3, 4);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 9, 6);

        PiratePieces[0] = SpawnPiece(PieceType.Cannon, false, 1, 6);
        PiratePieces[1] = SpawnPiece(PieceType.Cannon, false, 5, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 4, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 0, 6);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 7, 6);
        PiratePieces[5] = SpawnPiece(PieceType.Mate, false, 4, 4);
        PiratePieces[6] = SpawnPiece(PieceType.Mate, false, 4, 5);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Cannon - 8 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Cannon.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Cannon! The Cannon is very fast and great at protecting or attacking from a distance.");
        boardUI.PieceDisplayDescription("\nHis average cost, combined with his unmatched range and mobility make him a strong member of most teams.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Cannon to see how he can move.", true);

        TTSquare currentSquare = tiles[9, 2].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();


        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Cannon>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if(moveAssessment[x,y] == 5)
                {
                    tiles[x, y].tag = "CannonDestination";
                }
            }
        }

        boardUI.PieceDisplayDescription("The Cannon can move to any open square next to him.");
        boardUI.PieceDisplayDescription("\nHe can also jump over other pieces in up/down or left/right directions from him, no matter how far away they are.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to jump there.", true);

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

                    if (tileSelected.tag == "CannonDestination")
                    {
                        ResetBoardMaterials();
                        MovePiece(NavyPieces[0], 9, 7);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 0, 5);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Very nice. The Cannon can jump across any open distance, making him extremely mobile.");
        boardUI.PieceDisplayDescription("\nRemember, in order for the Cannon to jump over a piece, he has to have room on the other side to land.", true);
        boardUI.PieceDisplayDescription("\nClick on the Cannon again to see what else he can do.", true);

        currentSquare = tiles[9, 7].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Cannon>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if(moveAssessment[x,y] == 4)
                {
                    TTSquare captureTarget = tiles[x, y].GetComponent<TTSquare>();
                    if (!captureTarget.currentPiece.isNavy)
                    {
                        tiles[x, y].tag = "CaptureSquare";
                        tiles[x,y].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 5)
                {
                    if(x == 9 && y == 5)
                    {
                        tiles[x, y].GetComponent<TTSquare>().SquareHasBeenClicked = true;
                    }
                    else
                    {
                        tiles[x, y].tag = "CannonDestination";
                    }
                }
            }
        }

        boardUI.PieceDisplayDescription("Look at that! That enemy Cannon is exposed! That means he can be jumped!");
        boardUI.PieceDisplayDescription("\nJumping an enemy piece will capture it. The Cannon can't capture Energy Shields, though.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to jump there.", true);

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

                    if (tileSelected.tag == "CannonDestination")
                    {
                        ResetBoardMaterials();
                        jail.InsertAPiece(PiratePieces[1]);
                        PiratePieces[1].destroyPiece();
                        NavyPieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 4, 7);

                        yield return new WaitForSeconds(.5f);

                        jail.InsertAPiece(NavyPieces[1]);
                        NavyPieces[1].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        MovePiece(PiratePieces[0], 0, 0);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("That was a nice capture!");
        boardUI.PieceDisplayDescription("\nIt looks like that other enemy Cannon is getting pretty close your Ore though, you should definitely go put a stop to that before it's too late.", true);
        boardUI.PieceDisplayDescription("\nClick on the Cannon again to keep moving him.", true);

        currentSquare = tiles[4, 7].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Cannon>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 4)
                {
                    TTSquare captureTarget = tiles[x, y].GetComponent<TTSquare>();
                    if (!captureTarget.currentPiece.isNavy)
                    {
                        tiles[x, y].tag = "CaptureSquare";
                        tiles[x, y].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 5)
                {
                    tiles[x, y].tag = "CannonDestination";
                }
            }
        }

        boardUI.PieceDisplayDescription("Remember how the Cannon has to have space on the other side of a piece in order for it to jump?");
        boardUI.PieceDisplayDescription("\nRight now, the Cannon can't jump the Ore since there's no space on the board, and he can't jump the Mate below him since there's another Mate on the other side of him.", true);
        boardUI.PieceDisplayDescription("\nInstead, click on the flashing green square to move there.", true);

        tiles[5, 6].tag = "InteractablePiece";

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
                        
                        MovePiece(NavyPieces[0], 5, 6);

                        yield return new WaitForSeconds(.5f);

                        jail.InsertAPiece(NavyPieces[2]);
                        NavyPieces[2].destroyPiece();
                        PiratePieces[0].hasOre = true;
                        MovePiece(PiratePieces[0], 8, 0);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 8, 1);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Yikes! The enemy Cannon captured the Ore!");
        boardUI.PieceDisplayDescription("\nYou need to get your Cannon over there and deal with him, immediately!", true);
        boardUI.PieceDisplayDescription("\nClick on the Cannon again to keep moving.", true);

        currentSquare = tiles[5, 6].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Cannon>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 5)
                {
                    tiles[x, y].tag = "CannonDestination";
                }
            }
        }

        boardUI.PieceDisplayDescription("Nice! The Cannon can jump over that Energy Shield to get to the other side of the board quickly.");
        boardUI.PieceDisplayDescription("\nAlso, notice how the Cannon can't touch that enemy Mate right now? Cannons can only capture by jumping over pieces.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to jump into position.", true);

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

                    if (tileSelected.tag == "CannonDestination")
                    {
                        ResetBoardMaterials();

                        MovePiece(NavyPieces[0], 8, 6);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 8, 2);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Perfect! Remember, Cannons can jump Energy Shields but they can't capture them.");
        boardUI.PieceDisplayDescription("\nNow your Cannon is in the perfect position to take that enemy Orebearer out!", true);
        boardUI.PieceDisplayDescription("\nClick on the Cannon again to line up the jump and take the Orebearer out.", true);

        currentSquare = tiles[8, 6].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Cannon>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 4)
                {
                    TTSquare captureTarget = tiles[x, y].GetComponent<TTSquare>();
                    if (captureTarget.currentPiece.type != PieceType.EnergyShield)
                    {
                        tiles[x, y].tag = "CaptureSquare";
                        tiles[x, y].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 5)
                {
                    if (x == 6 && y == 6)
                    {
                        tiles[x, y].GetComponent<TTSquare>().SquareHasBeenClicked = true;
                    }
                    else
                    {
                        tiles[x, y].tag = "CannonDestination";
                    }
                }
            }
        }

        boardUI.PieceDisplayDescription("The enemy Orebearer is in range!");
        boardUI.PieceDisplayDescription("\nQuickly, click on the flashing green square to capture the enemy Orebearer and take back your Ore!", true);

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

                    if (tileSelected.tag == "CannonDestination")
                    {
                        ResetBoardMaterials();
                        jail.InsertAPiece(PiratePieces[0]);
                        PiratePieces[0].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 8, 1);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare = tiles[8, 1].GetComponent<TTSquare>();
        currentSquare.tag = "CaptureSquare";
        moveAssessment = NavyPieces[0].GetComponent<Piece>().GetValidOreReset(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 7)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        jail.PirateJailCells[1].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Good Job! You've captured the enemy Orebearer!");
        boardUI.PieceDisplayDescription("\nYour Ore is safe for another turn, and now needs to get redeployed back to the battlefield.", true);
        boardUI.PieceDisplayDescription("\nClick on any of the red squares to redeploy your ore and finish this tutorial!", true);

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

                    if (tileSelected.tag == "MoveableSquare")
                    {
                        Vector2Int respawnCoords = IdentifyThisBoardSquare(tileSelected);
                        ResetBoardMaterials();
                        jail.navyJailedPieces[1].GetComponent<Piece>().destroyPiece();
                        jail.PirateJailCells[1].GetComponent<TTJailCell>().resetCell();
                        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, respawnCoords.x, respawnCoords.y);
                        tileSelected.GetComponent<TTSquare>().FlashMaterial(currentSquare.moveableBoardMaterial, 3);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestEngineer()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Engineer, true, 4, 4);
        NavyPieces[1] = SpawnPiece(PieceType.Engineer, true, 7, 3);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 6, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 3, 3);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 7, 5);

        PiratePieces[0] = SpawnPiece(PieceType.Engineer, false, 2, 3);
        PiratePieces[1] = SpawnPiece(PieceType.Engineer, false, 4, 8);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 5, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 4, 6);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 5, 6);
        PiratePieces[5] = SpawnPiece(PieceType.Engineer, false, 6, 4);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Engineer - 7 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Engineer.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Engineer! The Engineer is extremely useful for any team.");
        boardUI.PieceDisplayDescription("\nHe's slightly expensive to add to your team, but his unique abilities make him extremely important.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Engineer to see how he can move.", true);

        TTSquare currentSquare = tiles[4, 4].GetComponent<TTSquare>();
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
                        break;
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        ResetBoardMaterials();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Engineer>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[4, 6].tag = "CaptureSquare";
        tiles[4, 6].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Engineer can move up to 2 open squares in any direction.");
        boardUI.PieceDisplayDescription("\nHe is the only piece that can remove enemy Energy Shields from the board. Notice how he can't capture that Pirate Engineer or that Navy Energy Shield, though?", true);
        boardUI.PieceDisplayDescription("\nClick on the red square to capture the enemy Energy Shield.", true);

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
                        MovePiece(NavyPieces[0], 4, 6);
                        NavyPieces[0].hasCaptured = true;
                        NavyPieces[0].GetComponent<Engineer>().capturedBomb = jail.pirateJailedPieces[0];

                        yield return new WaitForSeconds(.5f);

                        jail.InsertAPiece(NavyPieces[4]);
                        NavyPieces[4].destroyPiece();
                        PiratePieces[5].hasCaptured = true;
                        PiratePieces[5].GetComponent<Engineer>().capturedBomb = jail.navyJailedPieces[0];
                        MovePiece(PiratePieces[5], 7, 5);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Usually, the Engineer can only capture enemy Ore, Orebearers, and Energy Shields.");
        boardUI.PieceDisplayDescription("\nBut as long as he's got an Energy Shield in tow, he gets a lot stronger.", true);
        boardUI.PieceDisplayDescription("\nClick on the Engineer again to see what he can do now.", true);

        currentSquare = tiles[4, 6].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Engineer>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                    else
                    {
                        moveSquare.tag = "CaptureSquare";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        jail.NavyJailCells[0].GetComponent<TTJailCell>().clicked = true;

        boardUI.PieceDisplayDescription("Now that the Engineer has captured a Shield, he can capture any enemy pieces in his range.");
        boardUI.PieceDisplayDescription("\nBe aware though, each Engineer can only have up to one Energy Shield captured at a time. Notice how he can't capture that other Energy Shield?", true);
        boardUI.PieceDisplayDescription("\nClick on the red square to capture the enemy Engineer.", true);

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
                        jail.InsertAPiece(PiratePieces[1]);
                        PiratePieces[1].destroyPiece();
                        MovePiece(NavyPieces[0], 4, 8);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 2, 2);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nice job, you're right on top of that enemy Ore now.");
        boardUI.PieceDisplayDescription("\nBut look, that other enemy Engineer is setting up to capture your Ore too!", true);
        boardUI.PieceDisplayDescription("\nClick on your other Engineer to start setting up defenses for your Ore.", true);

        currentSquare = tiles[7, 3].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[1].GetComponent<Engineer>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        jail.NavyJailCells[0].GetComponent<TTJailCell>().clicked = true;

        tiles[5, 1].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("This Engineer hasn't yet captured an Energy Shield, so he still can't capture regular enemies.");
        boardUI.PieceDisplayDescription("\nRegardless, Engineers are still great at fortifying defenses, so you'll want to move him closer to your Ore to better protect it.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Engineer there.", true);

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
                        MovePiece(NavyPieces[1], 5, 1);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 4, 0);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Yikes! It looks like your Engineer got here just in time.");
        boardUI.PieceDisplayDescription("\nQuickly, take this chance to set up your defenses!", true);
        boardUI.PieceDisplayDescription("\nClick on the Engineer again to set up your defense.", true);

        currentSquare = tiles[5, 1].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[1].GetComponent<Engineer>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        jail.NavyJailCells[0].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Engineers can set any captured enemy Energy Shield back on the board.");
        boardUI.PieceDisplayDescription("\nThey can even deploy Energy Shields that were captured by other Engineers!", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Energy Shield in the corner to place it back on the board.", true);

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
                        moveAssessment = NavyPieces[1].GetComponent<Engineer>().DetectBombDeploy(tiles);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 6)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "CaptureSquare";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        tiles[5, 0].tag = "CannonTarget";

        boardUI.PieceDisplayDescription("Engineers can deploy Energy Shields up to 2 open spaces away in either up/down or left/right directions.");
        boardUI.PieceDisplayDescription("\nPlacing them down will remove the strength boost from the Engineer that captured that Shield, though.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing red square to redeploy the Energy Shield there.", true);

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

                    if (tileSelected.tag == "CannonTarget")
                    {
                        ResetBoardMaterials();
                        jail.pirateJailedPieces[0].GetComponent<Piece>().destroyPiece();
                        jail.NavyJailCells[0].GetComponent<TTJailCell>().resetCell();
                        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 5, 0);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[5], 7, 3);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nicely done, your Ore is safe for now.");
        boardUI.PieceDisplayDescription("\nSince Engineers can't take their own Energy Shields and that enemy Engineer can't capture your Engineer right now, he can't get through to your Ore without going all the way around.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Engineer to keep moving.", true);

        currentSquare = tiles[4, 8].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Engineer>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                    else
                    {
                        tiles[x, y].tag = "CaptureSquare";
                        tiles[x,y].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        boardUI.PieceDisplayDescription("Now that the other Engineer redeployed the Energy Shield you captured, this Engineer doesn't have that strength boost from before.");
        boardUI.PieceDisplayDescription("\nThat doesn't matter though, because even a non-boosted Engineer can still capture Ore!", true);
        boardUI.PieceDisplayDescription("\nClick on the red Ore to capture it and finish this tutorial!", true);

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
                        jail.InsertAPiece(PiratePieces[2]);
                        PiratePieces[2].destroyPiece();
                        NavyPieces[0].hasOre = true;
                        MovePiece(NavyPieces[0], 5, 9);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestTactician()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Royal2, true, 7, 2);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 1, 1);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 3, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 0, 5);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 9, 6);

        PiratePieces[0] = SpawnPiece(PieceType.Royal1, false, 1, 8);
        PiratePieces[1] = SpawnPiece(PieceType.Royal2, false, 3, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Gunner, false, 1, 5);
        PiratePieces[3] = SpawnPiece(PieceType.Engineer, false, 8, 7);
        PiratePieces[4] = SpawnPiece(PieceType.Vanguard, false, 4, 8);
        PiratePieces[5] = SpawnPiece(PieceType.Ore, false, 9, 9);
        PiratePieces[6] = SpawnPiece(PieceType.EnergyShield, false, 3, 4);
        PiratePieces[7] = SpawnPiece(PieceType.EnergyShield, false, 6, 5);

        boardEdges[0].material = NavyEdges;
        boardEdges[1].material = PirateEdges;

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Tactician - 20 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Tactician.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Tactician! The Tactician is strategic, always evolving, and very dangerous.");
        boardUI.PieceDisplayDescription("\nAlthough it's one of the most expensive to have on your team, it's unmatched versatility makes it an unbeatable addition to your team.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Tactician to see how it can move.", true);

        TTSquare currentSquare = tiles[7, 2].GetComponent<TTSquare>();
        tiles[7, 2].tag = "InteractablePiece";

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
        moveAssessment = NavyPieces[0].GetComponent<Tactician>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[7, 4].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("The Tactician can move up to 2 open squares in any up/down or left/right direction.");
        boardUI.PieceDisplayDescription("\nIt also has some secret powers that are unlocked when the Tactician is surrounded by enemies.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Tactician into contested territory.", true);

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
                        MovePiece(NavyPieces[0], 7, 4);

                        yield return new WaitForSeconds(.5f);

                        jail.InsertAPiece(NavyPieces[4]);
                        NavyPieces[4].destroyPiece();
                        PiratePieces[3].hasCaptured = true;
                        PiratePieces[3].GetComponent<Engineer>().capturedBomb = jail.navyJailedPieces[0];
                        MovePiece(PiratePieces[3], 9, 6);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("The Tactician can copy the moves of any enemy piece in the same zone it's in.");
        boardUI.PieceDisplayDescription("\nThe board zones are the Navy Zone (first 3 rows), Neutral Zone (middle 4 rows), and Pirate Zone (last 3 rows).", true);
        boardUI.PieceDisplayDescription("\nClick on the Tactician again to see which pieces it can copy.", true);

        currentSquare = tiles[7, 4].GetComponent<TTSquare>();
        tiles[7, 4].tag = "InteractablePiece";

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Tactician>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        Piece inheritPiece = Instantiate(PiecePrefabs[14].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        inheritPiece = Instantiate(PiecePrefabs[17].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        jail.tacticianMimicPieces[0].currentX = 7;
        jail.tacticianMimicPieces[0].currentY = 4;

        jail.TacticianMimicCells[0].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Down below the game board is the list of all the pieces the Tactician can copy.");
        boardUI.PieceDisplayDescription("\nSince the Tactician is in the Neutral Zone, it can copy other pieces in the Neutral Zone, including the Gunner and the Engineer.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Engineer to copy that piece!", true);

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
                        ResetBoardMaterials(false);
                        currentSquare.SquareHasBeenClicked = true;
                        jail.TacticianMimicCells[0].GetComponent<TTJailCell>().clicked = true;

                        moveAssessment = jail.tacticianMimicPieces[0].GetComponent<Engineer>().GetValidMoves(tiles);

                        break;
                    }

                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[6, 5].tag = "CaptureSquare";
        tiles[6, 5].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("Woah! The Tactician's moves suddenly changed to copy the enemy Engineer!");
        boardUI.PieceDisplayDescription("\nNow that the Tactician is copying the enemy Engineer, it can capture enemy Energy Shields, just like Engineers can.", true);
        boardUI.PieceDisplayDescription("\nClick on the red Energy Shield to capture that piece!", true);

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
                        jail.InsertAPiece(PiratePieces[7]);
                        PiratePieces[7].destroyPiece();
                        NavyPieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 6, 5);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 6, 8);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare = tiles[6, 5].GetComponent<TTSquare>();
        currentSquare.tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("Nice job capturing that Energy Shield.");
        boardUI.PieceDisplayDescription("\nThat enemy Captain is staying just out of range for the Tactician to copy, but it doesn't matter since the Tactician has some more tricks up it's sleeve.", true);
        boardUI.PieceDisplayDescription("\nClick on the Tactician to continue moving it.", true);

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Tactician>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        inheritPiece = Instantiate(PiecePrefabs[14].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        inheritPiece = Instantiate(PiecePrefabs[17].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        jail.tacticianMimicPieces[1].currentX = 6;
        jail.tacticianMimicPieces[1].currentY = 5;

        jail.TacticianMimicCells[1].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Since the enemy Gunner is also in the Neutral Zone, it's fair game for copying!");
        boardUI.PieceDisplayDescription("\nCopying the enemy Gunner would put that Captain right in the line of fire.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Gunner to copy it!", true);

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
                        ResetBoardMaterials(false);
                        currentSquare.SquareHasBeenClicked = true;
                        jail.TacticianMimicCells[1].GetComponent<TTJailCell>().clicked = true;

                        moveAssessment = jail.tacticianMimicPieces[1].GetComponent<Gunner>().GetValidMoves(tiles);

                        break;
                    }

                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if(moveAssessment[x,y] == 3)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece != null)
                    {
                        tiles[x, y].tag = "GunnerTarget";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        tiles[6, 8].tag = "CaptureSquare";

        boardUI.PieceDisplayDescription("Look at that! Now the Tactician can copy the Gunner.");
        boardUI.PieceDisplayDescription("\nGunners can shoot up to 3 squares away, so the Captain is now in range.", true);
        boardUI.PieceDisplayDescription("\nClick on the red Captain to shoot it!", true);

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
                        tiles[6, 8].GetComponent<TTSquare>().FlashMaterial(currentSquare.moveableBoardMaterial, 3);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[1], 7, 4);
                        PiratePieces[1].GetComponent<Corsair>().canJump = false;

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare.tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("That was an incredible shot! The enemy just lost one of their most powerful forces!");
        boardUI.PieceDisplayDescription("\nNow it looks like the enemy Corsair is lining up an attack both on the Tactician and also your Ore.", true);
        boardUI.PieceDisplayDescription("\nClick on the Tactician to see what you can do for a counterattack.", true);

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Tactician>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        inheritPiece = Instantiate(PiecePrefabs[14].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        inheritPiece = Instantiate(PiecePrefabs[17].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        inheritPiece = Instantiate(PiecePrefabs[20].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        jail.tacticianMimicPieces[1].hasCaptured = true;

        jail.tacticianMimicPieces[2].currentX = 6;
        jail.tacticianMimicPieces[2].currentY = 5;

        jail.TacticianMimicCells[2].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Since you used the Gunner to capture last turn, you can't use the Gunner to capture again until you've recharged just like the Gunner has to.");
        boardUI.PieceDisplayDescription("\nIf you were to copy the Corsair though, you COULD use her own moves against her and capture her...", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Corsair to see what ELSE you could do.", true);

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
                        ResetBoardMaterials(false);
                        currentSquare.SquareHasBeenClicked = true;
                        jail.TacticianMimicCells[2].GetComponent<TTJailCell>().clicked = true;

                        moveAssessment = jail.tacticianMimicPieces[2].GetComponent<Corsair>().GetValidMoves(tiles);

                        break;
                    }

                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                    else
                    {
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 8)
                {
                    if(x != 6 || y != 9)
                        tiles[x, y].tag = "CorsairJump";
                }
            }
        }

        tiles[6, 9].tag = "CannonTarget";

        boardUI.PieceDisplayDescription("Imitating the Corsair will let you jump around the board just like she can!");
        boardUI.PieceDisplayDescription("\nWhile you definitely COULD capture her with her own moves, why waste this opportunity to jump into enemy territory and make a play for the Ore?", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing red square at the top of the board to jump there.", true);

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

                    if (tileSelected.tag == "CannonTarget")
                    {
                        ResetBoardMaterials();

                        MovePiece(NavyPieces[0], 6, 9);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[4], 3, 9);
                        PiratePieces[1].GetComponent<Corsair>().canJump = true;

                        break;
                    }

                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare = tiles[6, 9].GetComponent<TTSquare>();
        currentSquare.tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("That was an awesome leap! Remember, the Tactician still has the limits of the pieces it captures, so no making those leaps twice in a row.");
        boardUI.PieceDisplayDescription("\nThe enemy Vanguard is now moving defensively to protect the Ore.", true);
        boardUI.PieceDisplayDescription("\nClick on the Tactician to keep moving.", true);

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Tactician>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        inheritPiece = Instantiate(PiecePrefabs[15].GetComponent<Piece>());
        jail.InsertAPiece(inheritPiece, true);
        inheritPiece.destroyPiece();

        jail.tacticianMimicPieces[0].currentX = 6;
        jail.tacticianMimicPieces[0].currentY = 9;

        jail.TacticianMimicCells[0].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Now that the Tactician is in the Pirate Zone, it can only copy enemies that are also in the Pirate Zone.");
        boardUI.PieceDisplayDescription("\nThe Tactician's regular moveset isn't enough to put the Ore in range of capture, so you'll have to try something else.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Vanguard to copy it!", true);

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
                        ResetBoardMaterials(false);
                        currentSquare.SquareHasBeenClicked = true;
                        jail.TacticianMimicCells[0].GetComponent<TTJailCell>().clicked = true;

                        moveAssessment = jail.tacticianMimicPieces[0].GetComponent<Vanguard>().GetValidMoves(tiles);

                        break;
                    }

                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                    else
                    {
                        tiles[x, y].tag = "CaptureSquare";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        tiles[9, 9].tag = "CannonTarget";

        boardUI.PieceDisplayDescription("Imitating the Vanguard will give you enough range to capture the enemy Ore!");
        boardUI.PieceDisplayDescription("\nClick on the flashing red Ore to capture it and finish this tutorial!", true);

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

                    if (tileSelected.tag == "CannonTarget")
                    {
                        ResetBoardMaterials();
                        jail.InsertAPiece(PiratePieces[5]);
                        PiratePieces[5].destroyPiece();
                        MovePiece(NavyPieces[0], 9, 9);
                        NavyPieces[0].hasOre = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestCorsair()
    {
        navyTurn = false;
        boardUI.UpdateTurn(false);

        PiratePieces[0] = SpawnPiece(PieceType.Royal2, false, 2, 2);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 7, 3);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 5, 0);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 1, 5);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 6, 5);

        NavyPieces[0] = SpawnPiece(PieceType.Mate, true, 8, 7);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 2, 8);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 6, 9);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 3, 5);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 5, 4);


        PiratePieces[0].GetComponent<Corsair>().canJump = true;

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Corsair - 20 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Corsair.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();


        boardUI.PieceDisplayDescription("This is the Corsair! The Corsair is quick, nimble, and dangerously cunning.");
        boardUI.PieceDisplayDescription("\nAlthough she's one of the most expensive to have on your team, her fast-paced surgical precision makes her tough to beat.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Corsair to see how she can move.", true);

        TTSquare currentSquare = tiles[2, 2].GetComponent<TTSquare>();
        tiles[2, 2].tag = "InteractablePiece";

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
        moveAssessment = PiratePieces[0].GetComponent<Corsair>().GetValidMoves(tiles);

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

                if (moveAssessment[x, y] == 8)
                {
                    tiles[x, y].tag = "CorsairJump";
                }
            }
        }

        tiles[6, 7].tag = "CaptureSquare";
        tiles[6, 7].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Corsair can move any open distance diagonally without effort.");
        boardUI.PieceDisplayDescription("\nShe can also jump to any square on the board that doesn't have anyone else there, but this takes a lot of effort so she can't do it all the time.", true);
        boardUI.PieceDisplayDescription("\nClick on the red square in the upper corner to jump the Corsair there.", true);

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
                        MovePiece(PiratePieces[0], 6, 7);
                        PiratePieces[0].GetComponent<Corsair>().canJump = false;

                        yield return new WaitForSeconds(.5f);

                        MovePiece(NavyPieces[0], 7, 7);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nice jump! But look at how tired the Corsair is now...");
        boardUI.PieceDisplayDescription("\nThat enemy Mate moved awfully close on his turn though, so you'll still need to keep your guard up.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing Corsair again to keep moving.", true);

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = PiratePieces[0].GetComponent<Corsair>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[5, 8].tag = "InteractablePiece";
        tiles[5, 8].GetComponent<TTSquare>().SetMaterial(currentSquare.moveableBoardMaterial);

        boardUI.PieceDisplayDescription("Since the Corsair jumped around on her last turn, she has to take it a little slower this turn.");
        boardUI.PieceDisplayDescription("\nNotice how she can only move along her regular diagonal path? She can't jump around this turn.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Corsair there while her strength builds back up.", true);

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
                        MovePiece(PiratePieces[0], 5, 8);
                        PiratePieces[0].GetComponent<Corsair>().canJump = true;
                        yield return new WaitForSeconds(.5f);

                        MovePiece(NavyPieces[1], 3, 8);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nice! The Corsair is looking like she's ready to mess up her enemy's day again!");
        boardUI.PieceDisplayDescription("\nSince you jumped within an inch of the enemy Ore, why not grab it for yourself while you're here?", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing Corsair again to prepare to capture the Ore.", true);

        currentSquare = tiles[5, 8].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = PiratePieces[0].GetComponent<Corsair>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
                else if (moveAssessment[x, y] == 8)
                {
                    tiles[x, y].tag = "CorsairJump";
                }
            }
        }

        tiles[6, 9].tag = "CaptureSquare";
        tiles[6, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Corsair captures by landing on another piece while moving diagonally.");
        boardUI.PieceDisplayDescription("\nNotice how she can jump to any open square again? But since the Ore is within her range, she's in prime position to scoop that up.", true);
        boardUI.PieceDisplayDescription("\nClick on the red Ore to capture it for yourself and finish the tutorial.", true);

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
                        jail.InsertAPiece(NavyPieces[2]);
                        NavyPieces[2].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        PiratePieces[0].hasOre = true;
                        MovePiece(PiratePieces[0], 6, 9);
                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestGunner()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Gunner, true, 5, 3);
        NavyPieces[1] = SpawnPiece(PieceType.Gunner, true, 6, 7);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 8, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 2, 4);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 8, 4);

        PiratePieces[0] = SpawnPiece(PieceType.Gunner, false, 3, 4);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 2, 3);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 1, 5);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 8, 5);
        PiratePieces[5] = SpawnPiece(PieceType.Mate, false, 8, 2);
        PiratePieces[6] = SpawnPiece(PieceType.Mate, false, 2, 7);


        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Gunner - 9 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Gunner.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Gunner! The Gunner is incredible at holding enemies off at a distance.");
        boardUI.PieceDisplayDescription("\nDespite a fairly expensive cost to clone for your team, his incredible power makes him a great addition for putting pressure on your opponent.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green gunner to see what he can do.", true);

        TTSquare currentSquare = tiles[6, 7].GetComponent<TTSquare>();
        tiles[6, 7].tag = "InteractablePiece";

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

        boardUI.PieceDisplayDescription("The Gunner can capture pieces at a range of 3 spaces in any direction, or he can also move one unblocked space in any direction");
        boardUI.PieceDisplayDescription("\nNotice the gunner can't capture the ore and the enemy Mate to the left is too far away for the Navy Gunner to see, but the enemy Gunner is in range of the Navy Gunner for capture.", true);
        boardUI.PieceDisplayDescription("\nClick on the enemy Gunner to capture him!", true);



        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[1].GetComponent<Gunner>().GetValidMoves(tiles);

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

        tiles[3, 4].tag = "CaptureSquare";
        tiles[3, 4].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        NavyPieces[1].hasCaptured = true;
                        

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[5], 8, 1);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Good job! You captured the piece!");
        boardUI.PieceDisplayDescription("\nKeep watch though, the other enemy Mate is getting dangerously close to your Ore. You'll need to something about that soon.", true);
        boardUI.PieceDisplayDescription("\nFor now, use the other Navy Gunner to capture the enemy mate!", true);

        currentSquare = tiles[5, 3].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Gunner>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[2, 3].tag = "CaptureSquare";
        tiles[2, 3].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        jail.InsertAPiece(PiratePieces[1]);
                        PiratePieces[1].destroyPiece();
                        NavyPieces[0].hasCaptured = true;

                        yield return new WaitForSeconds(.5f);

                        jail.InsertAPiece(NavyPieces[2]);
                        NavyPieces[2].destroyPiece();
                        PiratePieces[5].hasCaptured= true;
                        PiratePieces[5].hasOre = true;
                        MovePiece(PiratePieces[5], 8, 0);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[5], 7, 1);


                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("\nUh Oh! It looks like while you were busy with that other Mate, that pesky first Mate captured the ore.");
        boardUI.PieceDisplayDescription("\nThe Gunner needs to reload after capturing an enemy before he can capture again, so he can't deal with that pesky Mate yet.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Gunner to quickly reload the gun.", true);

        currentSquare = tiles[5, 3].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("\nThe Gunner can't capture again until after he reloads.");
        boardUI.PieceDisplayDescription("\nHe reloads his gun by moving to any nearby open space on his turn.", true);
        boardUI.PieceDisplayDescription("\nClick the flashing green square to move the Gunner there and reload his gun.", true);

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Gunner>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        currentSquare = tiles[6, 3].GetComponent<TTSquare>();
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
                        NavyPieces[0].hasCaptured = false;
                        MovePiece(NavyPieces[0], 6, 3);


                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[5], 7, 2);


                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Very nice! We are in a great position to capture the Orebearer and take the Ore back.");
        boardUI.PieceDisplayDescription("\nClick on the Navy Gunner, then shoot the enemy Orebearer and take back what's ours!", true);

        currentSquare = tiles[6, 3].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Gunner>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[7, 2].tag = "CaptureSquare";
        tiles[7, 2].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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

                        currentSquare = tileSelected.GetComponent<TTSquare>();
                        currentSquare.tag = "CaptureSquare";

                        jail.InsertAPiece(PiratePieces[5]);
                        PiratePieces[5].destroyPiece();
                        NavyPieces[0].hasCaptured = true;

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        moveAssessment = NavyPieces[0].GetComponent<Piece>().GetValidOreReset(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 7)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        jail.PirateJailCells[0].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Good Job! You've captured the enemy Orebearer!");
        boardUI.PieceDisplayDescription("\nYour ore is safe for another turn, and now needs to get redeployed back to the battlefield.", true);
        boardUI.PieceDisplayDescription("\nClick on any of the red squares to redeploy your ore and finish this tutorial!", true);

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

                    if (tileSelected.tag == "MoveableSquare")
                    {
                        Vector2Int respawnCoords = IdentifyThisBoardSquare(tileSelected);
                        ResetBoardMaterials();
                        jail.navyJailedPieces[0].GetComponent<Piece>().destroyPiece();
                        jail.PirateJailCells[0].GetComponent<TTJailCell>().resetCell();
                        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, respawnCoords.x, respawnCoords.y);
                        tileSelected.GetComponent<TTSquare>().FlashMaterial(currentSquare.moveableBoardMaterial, 3);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();

    }

    IEnumerator TestQuartermaster()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Quartermaster, true, 5, 3);
        NavyPieces[1] = SpawnPiece(PieceType.Quartermaster, true, 1, 4);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 2, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 1, 5);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 8, 6);

        PiratePieces[0] = SpawnPiece(PieceType.Quartermaster, false, 2, 7);
        PiratePieces[1] = SpawnPiece(PieceType.Quartermaster, false, 4, 9);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 5, 4);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 6, 4);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Quartermaster - 7 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Quatermaster.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();


        boardUI.PieceDisplayDescription("This is the Quartermaster! The Quartermaster is very agile.");
        boardUI.PieceDisplayDescription("\nWith a moderate cost to clone for your team, he's versatile and great at applying pressure around the rift.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Quartermaster to see how he can move.", true);

        TTSquare currentSquare = tiles[5, 3].GetComponent<TTSquare>();
        tiles[5, 3].tag = "InteractablePiece";

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
        moveAssessment = NavyPieces[0].GetComponent<Quartermaster>().GetValidMoves(tiles);

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

        tiles[6, 5].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("The Quartermaster can move in an 'L' shape direction!");
        boardUI.PieceDisplayDescription("\nNotice he can jump over both allied and enemy Shields.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Quartermaster there.", true);

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
                        MovePiece(NavyPieces[0], 6, 5);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[1], 5, 7);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Look! The enemy's Quartermaster has moved up. You need to be careful of him!");
        boardUI.PieceDisplayDescription("\nNotice there's an allied Shield blocking the Quartermaster's path to the right. Even though he can jump over Shields, he still can't land on them.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Quartermaster to get ready to capture the enemy.", true);

        currentSquare = tiles[6, 5].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Quartermaster>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[5, 7].tag = "CaptureSquare";
        tiles[5, 7].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        jail.InsertAPiece(PiratePieces[1]);
                        PiratePieces[1].destroyPiece();
                        NavyPieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 5, 7);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 4, 8);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Good job! You captured the enemy Quartermaster!");
        boardUI.PieceDisplayDescription("\nNow the other enemy Quartermaster is setting up a defense around their Ore, let's show him his defense doesn't intimidate us!", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Quartermaster to get ready to push forward and take the ore!", true);

        currentSquare = tiles[5, 7].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Quartermaster>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[6, 9].tag = "CaptureSquare";
        tiles[6, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        jail.InsertAPiece(PiratePieces[2]);
                        PiratePieces[2].destroyPiece();
                        NavyPieces[0].hasCaptured = true;
                        NavyPieces[0].hasOre = true;
                        MovePiece(NavyPieces[0], 6, 9);


                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Awesome! You captured the Ore! Now you move like an Orebearer!");
        boardUI.PieceDisplayDescription("\nThe Orebearer gets to make an extra move after capturing an enemy piece, so it's still your turn.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Quartermaster to use your second move to move out of your enemy's trap!", true);

        currentSquare = tiles[6, 9].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Piece>().GetValidMovesOre(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }



        currentSquare = tiles[5, 8].GetComponent<TTSquare>();
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
                        NavyPieces[0].hasCaptured = true;
                        NavyPieces[0].hasOre = true;
                        MovePiece(NavyPieces[0], 5, 8);


                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestCaptain()
    {
        navyTurn = false;
        boardUI.UpdateTurn(false);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");
        string tutorialHeader = "Captain - 25 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Captain.\n\nClick anywhere to continue.");

        PiratePieces[0] = SpawnPiece(PieceType.Royal1, false, 4, 5);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 8, 3);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 8, 0);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 0, 5);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 7, 6);

        NavyPieces[0] = SpawnPiece(PieceType.Mate, true, 6, 8);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 9, 8);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 7, 9);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 4, 6);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 5, 6);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Captain! The Captain is the most powerful Pirate to ever scourge the cosmos.");
        boardUI.PieceDisplayDescription("\nWith unrivaled reach and agility, his reach is unpredictable and infinite.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Captain to see how he can move.", true);

        TTSquare currentSquare = tiles[4, 5].GetComponent<TTSquare>();
        tiles[4, 5].tag = "InteractablePiece";

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
        moveAssessment = PiratePieces[0].GetComponent<Captain>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[6, 8].tag = "CaptureSquare";
        tiles[6, 8].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Captain can move in a pattern of adjacent 5 squares.");
        boardUI.PieceDisplayDescription("\nHe can jump over blockers, but his range can be hard to predict, with his main limitation being his diagonal reach.", true);
        boardUI.PieceDisplayDescription("\nThe Navy Mate on the other side of the board is in range. Click on the red square to capture the enemy Mate.", true);

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

                        jail.InsertAPiece(NavyPieces[0]);
                        NavyPieces[0].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        MovePiece(PiratePieces[0], 6, 8);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(NavyPieces[1], 8, 8);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Good job capturing that enemy Mate.");
        boardUI.PieceDisplayDescription("\nWhile the Captain does have an extensive reach, it's also fairly selective, making it important to strategize before moving him.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing Captain again to continue moving him.", true);

        currentSquare = tiles[6, 8].GetComponent<TTSquare>();
        tiles[6, 8].tag = "InteractablePiece";

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = PiratePieces[0].GetComponent<Captain>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[5, 8].tag = "InteractablePiece";
        boardUI.PieceDisplayDescription("The Captain is just out of reach of both the enemy Ore and Mate.");
        boardUI.PieceDisplayDescription("\nAlso, notice how even though he can jump over Energy Shields, he can't land on them or capture them?", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to reposition the Captain there.", true);

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
                        MovePiece(PiratePieces[0], 5, 8);

                        yield return new WaitForSeconds(.5f);
                        MovePiece(NavyPieces[1], 7, 8);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("The enemy Mate is moving in because he thinks you're retreating.");
        boardUI.PieceDisplayDescription("\nLittle does he know that you aren't retreating, you're setting up for a bloodbath!", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing Captain again to spring him into action.", true);

        currentSquare = tiles[5, 8].GetComponent<TTSquare>();
        tiles[5, 8].tag = "InteractablePiece";

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = PiratePieces[0].GetComponent<Captain>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[7, 9].tag = "CaptureSquare";
        tiles[7, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The enemy Ore is now in range!");
        boardUI.PieceDisplayDescription("\nLeap over the enemy Mate and sieze the Ore for yourself.", true);
        boardUI.PieceDisplayDescription("\nClick on the red square to capture the enemy Ore.", true);

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

                        jail.InsertAPiece(NavyPieces[2]);
                        NavyPieces[2].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        PiratePieces[0].hasOre = true;
                        MovePiece(PiratePieces[0], 7, 9);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare = tiles[7, 9].GetComponent<TTSquare>();
        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = PiratePieces[0].GetValidMovesOre(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[7, 8].tag = "CaptureSquare";
        tiles[7, 8].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Captain now has the Ore and moves like an Orebearer!");
        boardUI.PieceDisplayDescription("\nOrebearers get an additional move if they've captured once before this turn.", true);
        boardUI.PieceDisplayDescription("\nClick on the red square to move again on your turn. Capture the Mate, leave no survivors, and finish this tutorial!", true);

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

                        jail.InsertAPiece(NavyPieces[1]);
                        NavyPieces[1].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        MovePiece(PiratePieces[0], 7, 8);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestNavigator()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Navigator, true, 1, 1);
        NavyPieces[1] = SpawnPiece(PieceType.Navigator, true, 6, 2);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 9, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 2, 4);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 7, 5);

        PiratePieces[0] = SpawnPiece(PieceType.Navigator, false, 2, 9);
        PiratePieces[1] = SpawnPiece(PieceType.Navigator, false, 4, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 4, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 0, 6);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 5, 7);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Navigator - 8 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Navigator.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Navigator! The Navigator is essential to a strong offensive strategy.");
        boardUI.PieceDisplayDescription("\nWith a fairly inexpensive cost to clone for your team, he makes a great addition to your battle siege plans.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Navigator to see how he can move.", true);

        TTSquare currentSquare = tiles[6, 2].GetComponent<TTSquare>();
        tiles[6, 2].tag = "InteractablePiece";

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
        moveAssessment = NavyPieces[1].GetComponent<Navigator>().GetValidMoves(tiles);

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
        tiles[6, 7].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("The Navigator can move any open distance forwards or backwards, making him great at attacking the enemy base.");
        boardUI.PieceDisplayDescription("\nHe can only move one space side to side, though, making him vunerable to flanks", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Navigator there.", true);

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
                        MovePiece(NavyPieces[1], 6, 7);
                        yield return new WaitForSeconds(.5f);
                        MovePiece(PiratePieces[1], 5, 8);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Look! The enemy's Navigator is right in range!");
        boardUI.PieceDisplayDescription("\nTake this oppurtunity to capture the enemy. Also notice the enemy's shield can't be captured. The Navigator can't capture shields!", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Navigator to get ready to capture, and then capture the enemy!", true);

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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[1].GetComponent<Navigator>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[5, 8].tag = "CaptureSquare";
        tiles[5, 8].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        jail.InsertAPiece(PiratePieces[1]);
                        PiratePieces[1].destroyPiece();
                        NavyPieces[1].hasCaptured = true;
                        MovePiece(NavyPieces[1], 5, 8);

                        yield return new WaitForSeconds(.5f);

                        MovePiece(PiratePieces[0], 3, 9);

                        break;
                    }
                }
            }

            yield return null;
        }


        boardUI.PieceDisplayDescription("Good job! You captured the piece!");
        boardUI.PieceDisplayDescription("\nMove the Navy Navigator up to the higlighted position to capture the enemy's ore!", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Navigator to get ready to move up, and take the ore!", true);

        currentSquare = tiles[5, 8].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[1].GetComponent<Navigator>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[4, 9].tag = "CaptureSquare";
        tiles[4, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        jail.InsertAPiece(PiratePieces[2]);
                        PiratePieces[2].destroyPiece();
                        NavyPieces[1].hasCaptured = true;
                        NavyPieces[1].hasOre = true;
                        MovePiece(NavyPieces[1], 4, 9);


                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Awesome! you captured the Ore! You now have the Orebearer's moveset!");
        boardUI.PieceDisplayDescription("\nThe Orebear gets to move an additional time in a turn after capturing an enemy piece.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Navigator and use your second move to capture the Enemy Navigator!", true);

        currentSquare = tiles[4, 9].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[1].GetComponent<Piece>().GetValidMovesOre(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[3, 9].tag = "CaptureSquare";
        tiles[3, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

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
                        NavyPieces[1].hasCaptured = true;
                        NavyPieces[1].hasOre = true;
                        MovePiece(NavyPieces[1], 3, 9);


                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();

    }
    
    IEnumerator TestAdmiral()
    {
        boardUI.GoalText("Raid On Rift: Tutorial Mode");
        string tutorialHeader = "Admiral - 25 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Admiral.\n\nClick anywhere to continue.");

        NavyPieces[0] = SpawnPiece(PieceType.Royal1, true, 5, 4);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 8, 2);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 8, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 0, 5);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 7, 5);

        PiratePieces[0] = SpawnPiece(PieceType.Mate, false, 8, 9);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 3, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 2, 7);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 8, 6);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Admiral! The Admiral is the highest ranking and most powerful Naval officer.");
        boardUI.PieceDisplayDescription("\nWith an incredible range and power at her disposal, she is an excellent addition to any team.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Admiral to see how she can move.", true);

        TTSquare currentSquare = tiles[5, 4].GetComponent<TTSquare>();
        tiles[5, 4].tag = "InteractablePiece";

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
        moveAssessment = NavyPieces[0].GetComponent<Admiral>().GetValidMoves(tiles);

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

        tiles[8, 7].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("The Admiral can move any open distance in any direction.");
        boardUI.PieceDisplayDescription("\nHer main limitation is that she cannot jump over pieces, like the Energy Shield in the corner.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Admiral there.", true);

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
                        MovePiece(NavyPieces[0], 8, 7);
                        yield return new WaitForSeconds(.5f);
                        MovePiece(PiratePieces[0], 7, 8);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nice job moving the Admiral. In response, the Pirate Mate moved to intercept you.");
        boardUI.PieceDisplayDescription("\nNow that the Pirate Mate is in range, you can capture him to clear him out of the way.", true);
        boardUI.PieceDisplayDescription("\nClick on the Admiral again to move her once more.", true);

        currentSquare = tiles[8, 7].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Admiral>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[7, 8].tag = "CaptureSquare";
        tiles[7, 8].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Admiral captures by landing on another piece.");
        boardUI.PieceDisplayDescription("\nThe enemy Mate is now in range! Capture him to clear a path to the enemy's Ore.", true);
        boardUI.PieceDisplayDescription("\nClick on the red square to capture the enemy Mate.", true);

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
                        NavyPieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 7, 8);

                        yield return new WaitForSeconds(.5f);
                        MovePiece(PiratePieces[1], 4, 6);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Nicely done! Now the enemy forces are cleared out of the way!");
        boardUI.PieceDisplayDescription("\nNothing stands between the Admiral and the Ore now.", true);
        boardUI.PieceDisplayDescription("\nClick on the Admiral again to take control of the enemy Ore.", true);

        currentSquare = tiles[7, 8].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Admiral>().GetValidMoves(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 1)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.moveableBoardMaterial);
                    }
                }
            }
        }

        tiles[6, 9].tag = "CaptureSquare";
        tiles[6, 9].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Admiral is primed to sieze the enemy Ore!");
        boardUI.PieceDisplayDescription("\nClick on the red square to capture the Ore and finish this tutorial", true);

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
                        jail.InsertAPiece(PiratePieces[2]);
                        PiratePieces[2].destroyPiece();
                        NavyPieces[0].hasCaptured = true;
                        NavyPieces[0].hasOre = true;
                        MovePiece(NavyPieces[0], 6, 9);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestVanguard()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Vanguard, true, 7, 2);
        NavyPieces[1] = SpawnPiece(PieceType.Vanguard, true, 0, 1);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 5, 0);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 5, 5);
        NavyPieces[4] = SpawnPiece(PieceType.EnergyShield, true, 3, 3);

        PiratePieces[0] = SpawnPiece(PieceType.Vanguard, false, 4, 1);
        PiratePieces[1] = SpawnPiece(PieceType.Vanguard, false, 9, 1);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.EnergyShield, false, 2, 5);
        PiratePieces[4] = SpawnPiece(PieceType.EnergyShield, false, 8, 4);
        PiratePieces[5] = SpawnPiece(PieceType.Vanguard, false, 8, 6);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Vanguard - 5 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Vanguard.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Vanguard! The Vanguard is essential to a strong defensive strategy.");
        boardUI.PieceDisplayDescription("\nWith a fairly inexpensive cost to clone for your team, he makes a great addition to your ore protection plans.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green Vanguard to see how he can move.", true);

        TTSquare currentSquare = tiles[7, 2].GetComponent<TTSquare>();
        tiles[7, 2].tag = "InteractablePiece";

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
        moveAssessment = NavyPieces[0].GetComponent<Vanguard>().GetValidMoves(tiles);

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

        tiles[4, 2].tag = "InteractablePiece";

        boardUI.PieceDisplayDescription("The Vanguard can move any open distance side to side, making him great at protecting your home rows.");
        boardUI.PieceDisplayDescription("\nHe can only move one space forwards or backwards, though, making him not as effective at getting back to your opponent's territory.", true);
        boardUI.PieceDisplayDescription("\nClick on the flashing green square to move the Vanguard there.", true);

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
                        MovePiece(NavyPieces[0], 4, 2);
                        jail.InsertAPiece(NavyPieces[2]);
                        NavyPieces[2].destroyPiece();
                        PiratePieces[0].hasCaptured = true;
                        PiratePieces[0].hasOre = true;

                        yield return new WaitForSeconds(.5f);
                        MovePiece(PiratePieces[0], 5, 0);
                        yield return new WaitForSeconds(.5f);
                        MovePiece(PiratePieces[0], 5, 1);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Uh Oh! The Pirate Vanguard has taken your ore on his turn!");
        boardUI.PieceDisplayDescription("\nYou'll lose the game if he gets back to his home row on the other side of the board!", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Vanguard again to take back your ore.", true);

        currentSquare = tiles[4, 2].GetComponent<TTSquare>();
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

        yield return new WaitForEndOfFrame();

        currentSquare.SquareHasBeenClicked = true;
        moveAssessment = NavyPieces[0].GetComponent<Vanguard>().GetValidMoves(tiles);

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

        tiles[5, 1].tag = "CaptureSquare";
        tiles[5, 1].GetComponent<TTSquare>().SetMaterial(currentSquare.enemyBoardMaterial);

        boardUI.PieceDisplayDescription("The Vanguard captures by landing on another piece.");
        boardUI.PieceDisplayDescription("\nThe enemy Orebearer is now in range! Notice how the Vanguard can't touch the Energy Shields, though?", true);
        boardUI.PieceDisplayDescription("\nClick on the red square to capture that piece.", true);

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
                        NavyPieces[0].hasCaptured = true;
                        MovePiece(NavyPieces[0], 5, 1);

                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        currentSquare = tiles[5, 1].GetComponent<TTSquare>();
        currentSquare.tag = "CaptureSquare";
        moveAssessment = NavyPieces[0].GetComponent<Piece>().GetValidOreReset(tiles);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (moveAssessment[x, y] == 7)
                {
                    TTSquare moveSquare = tiles[x, y].GetComponent<TTSquare>();
                    if (moveSquare.currentPiece == null)
                    {
                        tiles[x, y].tag = "MoveableSquare";
                        moveSquare.SetMaterial(moveSquare.enemyBoardMaterial);
                    }
                }
            }
        }

        jail.PirateJailCells[0].GetComponent<TTJailCell>().interactable = true;

        boardUI.PieceDisplayDescription("Good Job! You've captured the enemy Orebearer!");
        boardUI.PieceDisplayDescription("\nYour ore is safe for another turn, and now needs to get redeployed back to the battlefield.", true);
        boardUI.PieceDisplayDescription("\nClick on any of the red squares to redeploy your ore and finish this tutorial!", true);

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

                    if (tileSelected.tag == "MoveableSquare")
                    {
                        Vector2Int respawnCoords = IdentifyThisBoardSquare(tileSelected);
                        ResetBoardMaterials();
                        jail.navyJailedPieces[0].GetComponent<Piece>().destroyPiece();
                        jail.PirateJailCells[0].GetComponent<TTJailCell>().resetCell();
                        NavyPieces[0] = SpawnPiece(PieceType.Ore, true, respawnCoords.x, respawnCoords.y);
                        tileSelected.GetComponent<TTSquare>().FlashMaterial(currentSquare.moveableBoardMaterial, 3);

                        break;
                    }
                }
            }

            yield return null;
        }

        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
    }

    IEnumerator TestMate()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Mate, true, 5, 5); // Main Character
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 2, 2);
        NavyPieces[2] = SpawnPiece(PieceType.Mate, true, 8, 2);
        NavyPieces[3] = SpawnPiece(PieceType.EnergyShield, true, 8, 5);
        NavyPieces[4] = SpawnPiece(PieceType.Ore, true, 6, 0);

        PiratePieces[0] = SpawnPiece(PieceType.Ore, false, 0, 9);
        PiratePieces[1] = SpawnPiece(PieceType.EnergyShield, false, 7, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Mate, false, 2, 7);
        PiratePieces[3] = SpawnPiece(PieceType.Mate, false, 6, 8);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Mate - 2 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Mate.");
        if(!StaticTutorialControl.cameFromStoryScene)
            boardUI.PieceDisplayDescription("\nIf you've played before, click on the X in the corner to skip this tutorial!.", true);
        boardUI.PieceDisplayDescription("\nClick anywhere to continue.", true);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Mate! The Mate is one of the fundamental crewmates to have on your team.");
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

                        yield return new WaitForSeconds(.5f);

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

        boardUI.PieceDisplayDescription("Good job! You've moved the Mate. In response, the Pirate Mate has moved too.");
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

                        yield return new WaitForSeconds(.5f);

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

                        yield return new WaitForSeconds(.5f);

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

                        yield return new WaitForSeconds(.5f);

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
                        break;
                    }
                }
            }

            yield return null;
        }
        
        boardUI.PieceDisplayDescription("Congrats on finishing this tutorial!");
        boardUI.PieceDisplayDescription("\nGood luck!", true);

        yield return new WaitForSeconds(3f);
        ExitTutorial();
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

    public void ExitTutorial()
    {
        if (StatManager.instance != null)
        {
            StatManager.instance.FinishedTutorial();
        }
        if (StaticTutorialControl.cameFromStoryScene)
        {
            SceneManager.LoadScene("Story");
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
