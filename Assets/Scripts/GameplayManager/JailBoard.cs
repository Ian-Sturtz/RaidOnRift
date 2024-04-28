using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailBoard : MonoBehaviour
{
    public GameObject gameBoard;
    public GameBoard board;
    public GameObject[] NavyJailCells;      // Cells in the Navy Jail (where Pirates go)
    public GameObject[] PirateJailCells;    // Cells in the Pirate Jail (where Navy go)
    public GameObject[] TacticianMimicCells;// Buttons for the tactician to mimic

    public Piece[] navyJailedPieces;        // Captured Navy Pieces
    public Piece[] pirateJailedPieces;      // Captured Pirate Pieces
    public Piece[] tacticianMimicPieces;    // Pieces tactician can inherit
    private float jail_square_size = .5f;

    private void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        board = gameBoard.GetComponent<GameBoard>();

        NavyJailCells = new GameObject[board.teamSize];
        PirateJailCells = new GameObject[board.teamSize];
        
        navyJailedPieces = new Piece[board.teamSize];
        pirateJailedPieces = new Piece[board.teamSize];

        TacticianMimicCells = new GameObject[9];
        tacticianMimicPieces = new Piece[9];

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

    public void InsertAPiece(Piece piece, bool tactician = false)
    {
        int cellToPlaceIn;
        int pieceIndex;
        JailCell cell;

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
            TacticianMimicCells[cellToPlaceIn].GetComponent<JailCell>().hasPiece = true;
            cell = TacticianMimicCells[cellToPlaceIn].GetComponent<JailCell>();
        }
        else
        {
            if (piece.isNavy)
            {
                cellToPlaceIn = FindFirstOpen(PirateJailCells);
                pieceIndex = FindNextSlot(navyJailedPieces);
                navyJailedPieces[pieceIndex] = SpawnPiece(piece.type, true, cellToPlaceIn);
                PirateJailCells[cellToPlaceIn].GetComponent<JailCell>().hasPiece = true;
                cell = PirateJailCells[cellToPlaceIn].GetComponent<JailCell>();
            }
            else
            {
                cellToPlaceIn = FindFirstOpen(NavyJailCells);
                pieceIndex = FindNextSlot(pirateJailedPieces);
                pirateJailedPieces[pieceIndex] = SpawnPiece(piece.type, false, cellToPlaceIn);
                cell = NavyJailCells[cellToPlaceIn].GetComponent<JailCell>();
            }
        }

        cell.hasPiece = true;

        if (piece.isNavy && !tactician)
        {
            PirateJailCells[cellToPlaceIn].GetComponent<JailCell>().currentPiece = navyJailedPieces[cellToPlaceIn].GetComponent<Piece>();
        }
        else if (!piece.isNavy && !tactician)
        {
            NavyJailCells[cellToPlaceIn].GetComponent<JailCell>().currentPiece = pirateJailedPieces[cellToPlaceIn].GetComponent<Piece>();
        }
        else
        {
            TacticianMimicCells[cellToPlaceIn].GetComponent<JailCell>().currentPiece = tacticianMimicPieces[cellToPlaceIn].GetComponent<Piece>();
        }

        if(!tactician)
            cell.FlashMaterial(cell.clickedJailMaterial, 3);
    }

    public int FindFirstOpen(GameObject[] teamJailCell, bool tactician = false)
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

            if (!cellBuffer.GetComponent<JailCell>().hasPiece)
            {
                return i;
            }
        }
        Debug.Log("No room in the jail cell!");
        return -1;
    }

    protected int FindNextSlot(Piece[] jailedPieces, bool tactician = false)
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

    public Piece SpawnPiece(PieceType type, bool isNavy, int cellToPlaceIn, bool tactician = false)
    {
        Piece cp;
        Vector3 targetPosition;

        if (tactician)
        {
            cp = Instantiate(board.PiecePrefabs[(int)type + board.PIECES_ADDED], this.transform).GetComponent<Piece>();
            targetPosition = TacticianMimicCells[cellToPlaceIn].transform.position;
        }
        else
        {
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
            NavyJailCells[i].GetComponent<JailCell>().interactable = false;
            NavyJailCells[i].GetComponent<JailCell>().clicked = false;

            PirateJailCells[i].GetComponent<JailCell>().interactable = false;
            PirateJailCells[i].GetComponent<JailCell>().clicked = false;
        }

        for (int i = 0; i < 9; i++)
        {
            if (TacticianMimicCells[i].GetComponent<JailCell>().hasPiece)
            {
                TacticianMimicCells[i].GetComponent<JailCell>().currentPiece.destroyPiece();
            }

            TacticianMimicCells[i].GetComponent<JailCell>().interactable = false;
            TacticianMimicCells[i].GetComponent<JailCell>().clicked = false;
            TacticianMimicCells[i].GetComponent<JailCell>().resetCell();
            TacticianMimicCells[i].GetComponent<JailCell>().stopFlashing();

            if (tacticianMimicPieces[i] != null)
                tacticianMimicPieces[i] = null;
        }
    }
}
