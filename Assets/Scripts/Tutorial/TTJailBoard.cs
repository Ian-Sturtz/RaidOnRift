using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTJailBoard : MonoBehaviour
{
    public GameObject gameBoard;
    public TTGameBoard board;
    public GameObject[] NavyJailCells;      // Cells in the Navy Jail (where Pirates go)
    public GameObject[] PirateJailCells;    // Cells in the Pirate Jail (where Navy go)
    public GameObject[] TacticianMimicCells;// Buttons for the tactician to mimic

    public TTPiece[] navyJailedPieces;        // Captured Navy Pieces
    public TTPiece[] pirateJailedPieces;      // Captured Pirate Pieces
    public TTPiece[] tacticianMimicPieces;    // Pieces tactician can inherit
    private float jail_square_size = .5f;

    private void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        board = gameBoard.GetComponent<TTGameBoard>();

        NavyJailCells = new GameObject[board.teamSize];
        PirateJailCells = new GameObject[board.teamSize];
        
        navyJailedPieces = new TTPiece[board.teamSize];
        pirateJailedPieces = new TTPiece[board.teamSize];

        TacticianMimicCells = new GameObject[9];
        tacticianMimicPieces = new TTPiece[9];

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

        for (int i = 1; i <= 9; i++)
        {
            cellname = "TacticianTarget" + i;

            GameObject tacticianCell = GameObject.Find(cellname);
            tacticianCell.tag = "JailCell";
            TacticianMimicCells[i - 1] = tacticianCell;
        }
    }

    public void InsertAPiece(TTPiece piece, bool tactician = false)
    {
        int cellToPlaceIn;
        int pieceIndex;
        TTJailCell cell;

        if (tactician)
        {
            cellToPlaceIn = FindFirstOpen(TacticianMimicCells, true);
            pieceIndex = FindNextSlot(tacticianMimicPieces, true);

            // This piece is an orebearer, so it's type is a Mate regardless of what piece it actually is
            if (piece.hasOre)
            {
                // Resets their type to match what it was originally
                piece.type = piece.originalType;
            }

            tacticianMimicPieces[pieceIndex] = SpawnPiece(piece.type, true, cellToPlaceIn, true);
            TacticianMimicCells[cellToPlaceIn].GetComponent<TTJailCell>().hasPiece = true;
            cell = TacticianMimicCells[cellToPlaceIn].GetComponent<TTJailCell>();
        }
        else
        {
            if (piece.isNavy)
            {
                cellToPlaceIn = FindFirstOpen(PirateJailCells);
                pieceIndex = FindNextSlot(navyJailedPieces);
                navyJailedPieces[pieceIndex] = SpawnPiece(piece.type, true, cellToPlaceIn);
                PirateJailCells[cellToPlaceIn].GetComponent<TTJailCell>().hasPiece = true;
                cell = PirateJailCells[cellToPlaceIn].GetComponent<TTJailCell>();
            }
            else
            {
                cellToPlaceIn = FindFirstOpen(NavyJailCells);
                pieceIndex = FindNextSlot(pirateJailedPieces);
                pirateJailedPieces[pieceIndex] = SpawnPiece(piece.type, false, cellToPlaceIn);
                cell = NavyJailCells[cellToPlaceIn].GetComponent<TTJailCell>();
            }
        }

        cell.hasPiece = true;

        if (piece.isNavy && !tactician)
        {
            PirateJailCells[cellToPlaceIn].GetComponent<TTJailCell>().currentPiece = navyJailedPieces[cellToPlaceIn].GetComponent<TTPiece>();
        }
        else if (!piece.isNavy && !tactician)
        {
            NavyJailCells[cellToPlaceIn].GetComponent<TTJailCell>().currentPiece = pirateJailedPieces[cellToPlaceIn].GetComponent<TTPiece>();
        }
        else
        {
            TacticianMimicCells[cellToPlaceIn].GetComponent<TTJailCell>().currentPiece = tacticianMimicPieces[cellToPlaceIn].GetComponent<TTPiece>();
        }

        if(!tactician)
            cell.FlashMaterial(cell.clickedJailMaterial, 3);
    }

    protected int FindFirstOpen(GameObject[] teamJailCell, bool tactician = false)
    {
        GameObject cellBuffer;

        int maxSize;

        if (tactician)
        {
            maxSize = 9;
        }
        else
        {
            maxSize = board.teamSize;
        }

        for (int i = 0; i < maxSize; i++)
        {
            cellBuffer = teamJailCell[i];

            if (!cellBuffer.GetComponent<TTJailCell>().hasPiece)
            {
                return i;
            }
        }
        Debug.Log("No room in the jail cell!");
        return -1;
    }

    protected int FindNextSlot(TTPiece[] jailedPieces, bool tactician = false)
    {
        int maxSize;

        if (tactician)
        {
            maxSize = 9;
        }
        else
        {
            maxSize = board.teamSize;
        }

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

    public int FindLastSlot(bool findNavyPiece)
    {
        if (findNavyPiece)
        {
            for (int i = board.teamSize - 1; i >= 0; i--)
            {
                if(navyJailedPieces[i] != null)
                {
                    return i;
                }
            }
        }
        else
        {
            for (int i = board.teamSize - 1; i >= 0; i--)
            {
                if (pirateJailedPieces[i] != null)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public TTPiece SpawnPiece(TTPieceType type, bool isNavy, int cellToPlaceIn, bool tactician = false)
    {
        TTPiece cp;
        Vector3 targetPosition;

        if (tactician)
        {
            cp = Instantiate(board.PiecePrefabs[(int)type + board.PIECES_ADDED], this.transform).GetComponent<TTPiece>();
            targetPosition = TacticianMimicCells[cellToPlaceIn].transform.position;
        }
        else
        {
            if (!isNavy)
            {
                cp = Instantiate(board.PiecePrefabs[(int)type + board.PIECES_ADDED], this.transform).GetComponent<TTPiece>();
                targetPosition = NavyJailCells[cellToPlaceIn].transform.position;
            }
            else
            {
                cp = Instantiate(board.PiecePrefabs[(int)type], this.transform).GetComponent<TTPiece>();
                targetPosition = PirateJailCells[cellToPlaceIn].transform.position;
            }
        }

        cp.transform.localScale *= jail_square_size;

        cp.type = type;
        cp.isNavy = isNavy;

        cp.transform.position = targetPosition;

        return cp;
    }

    // Finds a given piece within the specified Jail
    public int FindPiece(TTPieceType type, bool pieceIsNavy)
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
            NavyJailCells[i].GetComponent<TTJailCell>().interactable = false;
            NavyJailCells[i].GetComponent<TTJailCell>().clicked = false;

            PirateJailCells[i].GetComponent<TTJailCell>().interactable = false;
            PirateJailCells[i].GetComponent<TTJailCell>().clicked = false;
        }

        for (int i = 0; i < 9; i++)
        {
            if (TacticianMimicCells[i].GetComponent<TTJailCell>().hasPiece)
            {
                TacticianMimicCells[i].GetComponent<TTJailCell>().currentPiece.destroyPiece();
            }

            TacticianMimicCells[i].GetComponent<TTJailCell>().interactable = false;
            TacticianMimicCells[i].GetComponent<TTJailCell>().clicked = false;
            TacticianMimicCells[i].GetComponent<TTJailCell>().resetCell();
            TacticianMimicCells[i].GetComponent<TTJailCell>().stopFlashing();

            if (tacticianMimicPieces[i] != null)
                tacticianMimicPieces[i] = null;
        }
    }
}
