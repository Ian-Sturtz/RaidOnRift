using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Ore = 0,
    LandMine = 1,
    Mate = 2,
    Bomber = 3,
    Vanguard = 4,
    Navigator = 5,
    Gunner = 6,
    Cannon = 7,
    Quartermaster = 8,
    Royal2 = 9,
    Royal1 = 10,
}

public class Piece : MonoBehaviour
{
    public Material PiratePiece;
    public Material NavyPiece;
    public Material NavyOre;
    public Material PirateOre;

    public PieceType type;
    public bool isNavy;
    public bool hasCaptured;
    public bool hasOre;
    public int currentX = -1;
    public int currentY = -1;

    [SerializeField] private float flashDelay = 1f;

    private bool continualFlash = false;

    private void Update()
    {
        if (isNavy)
        {
            if(flashDelay != (1f - .1f * (9 - currentY)))
            {
                flashDelay = 1f - .1f * (9 - currentY);
            }
        }
        else
        {
            if(flashDelay != 1f - .1f * (currentY))
            {
                flashDelay = 1f - .1f * (currentY);
            }
        }

        if (hasOre && !continualFlash)
        {
            if (isNavy)
            {
                continualFlash = true;
                StartCoroutine(ContinualFlash(NavyPiece, NavyOre));
            }
            else
            {
                continualFlash = true;
                StartCoroutine(ContinualFlash(PiratePiece, PirateOre));
            }
        }
    }

    public void SetMaterial(Material newMaterial)
    {
        if (tag != "GameController")
            GetComponent<SpriteRenderer>().material = newMaterial;
    }

    public void destroyPiece()
    {
        Destroy(gameObject);
    }

    protected bool IsSquareOnBoard(int x, int y)
    {
        return (x >= 0 && x < 10 && y >= 0 && y < 10) ;
    }

    public int[,] GetValidMovesOre(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        // For all squares +/- 1 away from current position
        for (int x_change = -1; x_change < 2; x_change++)
        {
            for (int y_change = -1; y_change < 2; y_change++)
            {
                if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                {
                    Square possibleSquare = tiles[currentX + x_change, currentY + y_change].GetComponent<Square>();
                    if (possibleSquare.currentPiece == null)
                        moveAssessment[currentX + x_change, currentY + y_change] = 1;
                    else if (isNavy != possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != PieceType.LandMine)
                        moveAssessment[currentX + x_change, currentY + y_change] = 2;
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }

    public int[,] GetValidOreReset(GameObject[,] tiles)
    {
        int[,] moveAssessment;
        int squareX = -1;
        int squareY = -1;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                moveAssessment[x, y] = -1;
                if(tiles[x,y].tag == "CaptureSquare")
                {
                    squareX = x;
                    squareY = y;
                }
            }
        }

        // For all squares +/- 1 away from current position
        for (int x_change = -1; x_change < 2; x_change++)
        {
            for (int y_change = -1; y_change < 2; y_change++)
            {
                if (IsSquareOnBoard(squareX + x_change, squareY + y_change))
                    if(tiles[squareX + x_change, squareY + y_change].GetComponent<Square>().currentPiece == null)
                        moveAssessment[squareX + x_change, squareY + y_change] = 7;
            }
        }

        moveAssessment[squareX, squareY] = 0;

        return moveAssessment;
    }

    IEnumerator ContinualFlash(Material StartingMaterial, Material TargetMaterial)
    {
        while (continualFlash)
        {
            SetMaterial(TargetMaterial);
            yield return new WaitForSeconds(flashDelay);
            SetMaterial(StartingMaterial);
            yield return new WaitForSeconds(flashDelay);
        }
    }
}
