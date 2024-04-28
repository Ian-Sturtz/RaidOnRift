using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPJailBoard : MonoBehaviour
{
    public GameObject gameBoard;
    public PPGameBoard board;

    public GameObject[] NavyJailCells;      // Cells in the Navy Jail (where Pirates go)
    public GameObject[] PirateJailCells;    // Cells in the Pirate Jail (where Navy go)

    public Piece[] navyJailedPieces;        // Captured Navy Pieces
    public Piece[] pirateJailedPieces;      // Captured Pirate Pieces
    private float jail_square_size = .6f;

    private void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        board = gameBoard.GetComponent<PPGameBoard>();

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
        PPJailCell cell;
        
        if (piece.isNavy)
        {
            cellToPlaceIn = FindFirstOpen(PirateJailCells);
            pieceIndex = FindNextSlot(navyJailedPieces);
            navyJailedPieces[pieceIndex] = SpawnPiece(piece.type, true, cellToPlaceIn);
            PirateJailCells[cellToPlaceIn].GetComponent<PPJailCell>().hasPiece = true;
            cell = PirateJailCells[cellToPlaceIn].GetComponent<PPJailCell>();
        }
        else
        {
            cellToPlaceIn = FindFirstOpen(NavyJailCells);
            pieceIndex = FindNextSlot(pirateJailedPieces);
            pirateJailedPieces[pieceIndex] = SpawnPiece(piece.type, false, cellToPlaceIn);
            cell = NavyJailCells[cellToPlaceIn].GetComponent<PPJailCell>();
        }

        cell.hasPiece = true;

        if (piece.isNavy)
        {
            PirateJailCells[cellToPlaceIn].GetComponent<PPJailCell>().currentPiece = navyJailedPieces[cellToPlaceIn].GetComponent<Piece>();
        }
        else if (!piece.isNavy)
        {
            NavyJailCells[cellToPlaceIn].GetComponent<PPJailCell>().currentPiece = pirateJailedPieces[cellToPlaceIn].GetComponent<Piece>();
        }
    }

    protected int FindFirstOpen(GameObject[] teamJailCell)
    {
        GameObject cellBuffer;

        int maxSize = board.teamSize;

        for (int i = 0; i < maxSize; i++)
        {
            cellBuffer = teamJailCell[i];

            if (!cellBuffer.GetComponent<PPJailCell>().hasPiece)
            {
                return i;
            }
        }
        Debug.Log("No room in the jail cell!");
        return -1;
    }

    protected int FindNextSlot(Piece[] jailedPieces)
    {
        int maxSize = board.teamSize;

        for (int i = 0; i < maxSize; i++)
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

    // Finds a given piece within the specified Jail
    public int FindPiece(PieceType type, bool pieceIsNavy)
    {
        int index;

        // Searches the Pirate Jail for a Navy piece
        if (pieceIsNavy)
        {
            for (index = 0; index < 30; index++)
            {
                if (navyJailedPieces[index] != null)
                {
                    if (navyJailedPieces[index].type == type)
                    {
                        return index;
                    }
                }
            }
        }
        else
        {
            for (index = 0; index < 30; index++)
            {
                if (pirateJailedPieces[index] != null)
                {
                    if (pirateJailedPieces[index].type == type)
                    {
                        return index;
                    }
                }
            }
        }

        return -1;
    }

    public void resetMaterials()
    {
        for(int i = 0; i < 30; i++)
        {
            NavyJailCells[i].GetComponent<PPJailCell>().interactable = false;
            NavyJailCells[i].GetComponent<PPJailCell>().clicked = false;

            PirateJailCells[i].GetComponent<PPJailCell>().interactable = false;
            PirateJailCells[i].GetComponent<PPJailCell>().clicked = false;
        }
    }
}
