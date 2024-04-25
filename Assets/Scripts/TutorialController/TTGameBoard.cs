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

        switch (StaticTutorialControl.piece)
        {
            case PieceType.Mate:
                Debug.Log("Testing Mate");
                StartCoroutine(TestMate());
                break;
            case PieceType.Bomber:
                Debug.Log("Testing Engineer");
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
                break;
            case PieceType.Cannon:
                Debug.Log("Testing Cannon");
                break;
            case PieceType.Quartermaster:
                Debug.Log("Testing Quartermaster");
                StartCoroutine(TestQuartermaster());
                break;
            case PieceType.Royal2:
                if (StaticTutorialControl.isNavy)
                {
                    Debug.Log("Testing Tactician");
                }
                else
                {
                    Debug.Log("Testing Corsair");
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
                StartCoroutine(TestGunner());
                break;
        }
    }
    IEnumerator TestGunner()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Gunner, true, 5, 3);
        NavyPieces[1] = SpawnPiece(PieceType.Gunner, true, 6, 7);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 8, 0);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 2, 4);
        NavyPieces[4] = SpawnPiece(PieceType.LandMine, true, 8, 4);

        PiratePieces[0] = SpawnPiece(PieceType.Gunner, false, 3, 4);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 2, 3);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 1, 5);
        PiratePieces[4] = SpawnPiece(PieceType.LandMine, false, 8, 5);
        PiratePieces[5] = SpawnPiece(PieceType.Mate, false, 8, 2);
        PiratePieces[6] = SpawnPiece(PieceType.Mate, false, 2, 7);


        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Gunner";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Gunner.\n\nClick anywhere to continue.");

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;
            else
                yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("This is the Gunner! The Gunner is incredible at holding enemys off at a distance.");
        boardUI.PieceDisplayDescription("\nWith a fairly expensive cost to clone for your team, he makes a great addition to putting pressure on your opponent.", true);
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
        boardUI.PieceDisplayDescription("\nNotice the gunner cant capture the ore, but the enemy Gunner is in range of the Navy Gunner for capture.", true);
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
        boardUI.PieceDisplayDescription("\nUse the other Navy Gunner to capture the enemy mate!", true);
        boardUI.PieceDisplayDescription("\nKeep watch though, the other enemy mate has moved and captured the ore. We will need to something about that", true);

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

        boardUI.PieceDisplayDescription("Awesome! you captured the enemy Mate.");
        boardUI.PieceDisplayDescription("\nUh Oh! It looks like that pesky Mate captured the ore, and was able to make an additonal move.", true);
        boardUI.PieceDisplayDescription("\nThe Gunner gets to reload after making an move on the board.", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Gunner to move and reload the Gun.", true);

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

        boardUI.PieceDisplayDescription("Very nice! we are in a great position to capture the ore bearer and take the ore back.");
        boardUI.PieceDisplayDescription("\nClick on the Navy Gunner, then shoot the enemy ore bearer and take back whats ours!", true);

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
        SceneManager.LoadScene("Story");

    }
    IEnumerator TestQuartermaster()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Quartermaster, true, 5, 3);
        NavyPieces[1] = SpawnPiece(PieceType.Quartermaster, true, 1, 4);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 2, 0);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 1, 5);
        NavyPieces[4] = SpawnPiece(PieceType.LandMine, true, 8, 6);

        PiratePieces[0] = SpawnPiece(PieceType.Quartermaster, false, 2, 7);
        PiratePieces[1] = SpawnPiece(PieceType.Quartermaster, false, 4, 9);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 5, 4);
        PiratePieces[4] = SpawnPiece(PieceType.LandMine, false, 6, 4);

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
        SceneManager.LoadScene("Story");
    }

    IEnumerator TestCaptain()
    {
        navyTurn = false;
        boardUI.UpdateTurn(false);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");
        string tutorialHeader = "Captain - 22 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Captain.\n\nClick anywhere to continue.");

        PiratePieces[0] = SpawnPiece(PieceType.Royal1, false, 4, 5);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 8, 3);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 8, 0);
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 0, 5);
        PiratePieces[4] = SpawnPiece(PieceType.LandMine, false, 7, 6);

        NavyPieces[0] = SpawnPiece(PieceType.Mate, true, 6, 8);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 9, 8);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 7, 9);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 4, 6);
        NavyPieces[4] = SpawnPiece(PieceType.LandMine, true, 5, 6);

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
        SceneManager.LoadScene("Story");
    }

    IEnumerator TestNavigator()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Navigator, true, 1, 1);
        NavyPieces[1] = SpawnPiece(PieceType.Navigator, true, 6, 2);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 9, 0);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 2, 4);
        NavyPieces[4] = SpawnPiece(PieceType.LandMine, true, 7, 5);

        PiratePieces[0] = SpawnPiece(PieceType.Navigator, false, 2, 9);
        PiratePieces[1] = SpawnPiece(PieceType.Navigator, false, 4, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 4, 9);
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 0, 6);
        PiratePieces[4] = SpawnPiece(PieceType.LandMine, false, 5, 7);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Navigator - 5 Points";
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
        boardUI.PieceDisplayDescription("\nWith a fairly inexpensive cost to clone for your team, he makes a great addition to your ore protection plans.", true);
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
                        MovePiece(PiratePieces[1], 5, 8);
                        break;
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        boardUI.PieceDisplayDescription("Look! The enemys Navigator is able to be captured!");
        boardUI.PieceDisplayDescription("\nTake this oppurtunity to capture the piece, notice the enemys shield cant be captured. The Navigator cant capture shields!", true);
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
        boardUI.PieceDisplayDescription("\nMove the Navy Navigator up to the higlighted position to capture the enemys ore!", true);
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

        boardUI.PieceDisplayDescription("Awesome! you captured the ore! You now have the ore bearer's moveset!");
        boardUI.PieceDisplayDescription("\nThe ore bear gets to take two turns", true);
        boardUI.PieceDisplayDescription("\nClick on the Navy Navigator to use your second move to capture the Enemy Navigator", true);

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
        SceneManager.LoadScene("Story");

    }
    
    IEnumerator TestAdmiral()
    {
        boardUI.GoalText("Raid On Rift: Tutorial Mode");
        string tutorialHeader = "Admiral - 22 Points";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Admiral.\n\nClick anywhere to continue.");

        NavyPieces[0] = SpawnPiece(PieceType.Royal1, true, 5, 4);
        NavyPieces[1] = SpawnPiece(PieceType.Mate, true, 8, 2);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 8, 0);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 0, 5);
        NavyPieces[4] = SpawnPiece(PieceType.LandMine, true, 7, 5);

        PiratePieces[0] = SpawnPiece(PieceType.Mate, false, 8, 9);
        PiratePieces[1] = SpawnPiece(PieceType.Mate, false, 3, 7);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 2, 7);
        PiratePieces[4] = SpawnPiece(PieceType.LandMine, false, 8, 6);

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
                        yield return new WaitForEndOfFrame();
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

                        yield return new WaitForEndOfFrame();
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
        SceneManager.LoadScene("Story");
    }

    IEnumerator TestVanguard()
    {
        NavyPieces[0] = SpawnPiece(PieceType.Vanguard, true, 7, 2);
        NavyPieces[1] = SpawnPiece(PieceType.Vanguard, true, 0, 1);
        NavyPieces[2] = SpawnPiece(PieceType.Ore, true, 5, 0);
        NavyPieces[3] = SpawnPiece(PieceType.LandMine, true, 5, 5);
        NavyPieces[4] = SpawnPiece(PieceType.LandMine, true, 3, 3);

        PiratePieces[0] = SpawnPiece(PieceType.Vanguard, false, 4, 1);
        PiratePieces[1] = SpawnPiece(PieceType.Vanguard, false, 9, 1);
        PiratePieces[2] = SpawnPiece(PieceType.Ore, false, 6, 9);
        PiratePieces[3] = SpawnPiece(PieceType.LandMine, false, 2, 5);
        PiratePieces[4] = SpawnPiece(PieceType.LandMine, false, 8, 4);
        PiratePieces[5] = SpawnPiece(PieceType.Vanguard, false, 8, 6);

        boardUI.GoalText("Raid On Rift: Tutorial Mode");

        string tutorialHeader = "Vanguard - 6 Points";
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
        SceneManager.LoadScene("Story");
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

        string tutorialHeader = "Mate - 1 Point";
        boardUI.SetPieceDisplay(tutorialHeader, "Welcome to tutorial mode! In this tutorial, you will learn about the Mate.\n\nClick anywhere to continue.");

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
        SceneManager.LoadScene("Story");
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
