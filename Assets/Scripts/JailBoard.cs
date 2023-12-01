using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailBoard : MonoBehaviour
{
    public GameObject gameBoard;
    public GameBoard board;
    public GameObject[] NavyJailCells;      // Cells in the Navy Jail (where Pirates go)
    public GameObject[] PirateJailCells;    // Cells in the Pirate Jail (where Navy go)
    public Piece[] navyJailedPieces;
    public Piece[] pirateJailedPieces;
    private float jail_square_size = .6f;

    private void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        board = gameBoard.GetComponent<GameBoard>();

        NavyJailCells = new GameObject[board.teamSize];
        PirateJailCells = new GameObject[board.teamSize];
        
        navyJailedPieces = new Piece[board.teamSize];
        pirateJailedPieces = new Piece[board.teamSize];

        IdentifyJailSquares();
    }

    private void IdentifyJailSquares()
    {
        string cellname;

        for (int i = 1; i <= board.teamSize; i++)
        {
            cellname = "JailBoardSquare " + i;

            GameObject navyJailCell = GameObject.Find("Navy" + cellname);
            navyJailCell.tag = "JailCell";
            NavyJailCells[i - 1] = navyJailCell;

            GameObject pirateJailCell = GameObject.Find("Pirate" + cellname);
            pirateJailCell.tag = "JailCell";
            PirateJailCells[i - 1] = pirateJailCell;
        }
    }

    public void InsertAPiece(Piece piece)
    {
        int cellToPlaceIn;
        int pieceIndex;
        JailCell cell;

        if (piece.isNavy)
        {
            cellToPlaceIn = FindFirstOpen(PirateJailCells);
            Debug.Log(cellToPlaceIn);
            pieceIndex = FindNextSlot(navyJailedPieces);
            navyJailedPieces[pieceIndex] = SpawnPiece(piece.type, true, cellToPlaceIn);
            PirateJailCells[cellToPlaceIn].GetComponent<JailCell>().hasPiece = true;
            cell = PirateJailCells[cellToPlaceIn].GetComponent<JailCell>();
        }
        else
        {
            cellToPlaceIn = FindFirstOpen(NavyJailCells);
            Debug.Log(cellToPlaceIn);
            pieceIndex = FindNextSlot(pirateJailedPieces);
            pirateJailedPieces[pieceIndex] = SpawnPiece(piece.type, false, cellToPlaceIn);
            cell = NavyJailCells[cellToPlaceIn].GetComponent<JailCell>();
        }

        cell.currentPiece = piece;
        cell.hasPiece = true;

        cell.FlashMaterial(cell.clickedJailMaterial, 3);
    }

    protected int FindFirstOpen(GameObject[] teamJailCell)
    {
        GameObject cellBuffer;

        for (int i = 0; i < board.teamSize; i++)
        {
            cellBuffer = teamJailCell[i];

            if (!cellBuffer.GetComponent<JailCell>().hasPiece)
            {
                return i;
            }
        }
        Debug.Log("No room in the jail cell!");
        return -1;
    }

    protected int FindNextSlot(Piece[] jailedPieces)
    {
        for (int i = 0; i < board.teamSize; i++)
        {
            if(jailedPieces[i] == null)
            {
                return i;
            }
        }

        Debug.Log("Too many Jailed Pieces!");
        return -1;
    }

    public Piece SpawnPiece(PieceType type, bool isNavy, int cellToPlaceIn)
    {
        Piece cp;
        Vector3 targetPosition;

        if (!isNavy)
        {
            cp = Instantiate(board.PiecePrefabs[(int)type + board.PIECES_ADDED], this.transform).GetComponent<Piece>();
            targetPosition = NavyJailCells[cellToPlaceIn].transform.position;
        }
        else
        {
            cp = Instantiate(board.PiecePrefabs[(int)type], this.transform).GetComponent<Piece>();
            targetPosition = PirateJailCells[cellToPlaceIn].transform.position;
        }

        cp.transform.localScale *= jail_square_size;

        cp.type = type;
        cp.isNavy = isNavy;

        cp.transform.position = targetPosition;

        return cp;
    }
}
