using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public PieceType originalType;
    public PieceType type;
    public bool isNavy;
    public bool hasCaptured;
    public bool hasOre;
    public bool gameWon = false;
    public int currentX = -1;
    public int currentY = -1;

    [SerializeField] private float flashDelay = 1f;
    private bool continualFlash = false;

    private void Awake()
    {
        originalType = type;
    }

    protected virtual void Update()
    {
        if (!gameWon)
        {
            if (isNavy)
            {
                if (flashDelay != (1f - .1f * (9 - currentY)))
                {
                    flashDelay = 1f - .1f * (9 - currentY);
                }
            }
            else
            {
                if (flashDelay != 1f - .1f * (currentY))
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
        else
        {
            continualFlash = false;
            StopAllCoroutines();

            if (isNavy)
            {
                SetMaterial(NavyOre);
            }
            else
            {
                SetMaterial(PirateOre);
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

    public void SetCoordinates(int x, int y)
    {
        currentX = x;
        currentY = y;
    }

    public int[,] GetValidMovesOre(GameObject[,] tiles)
    {
        int[,] moveAssessment;

        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        if (SceneManager.GetActiveScene().name == "Board")
        {
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
        }
        else
        {
            // For all squares +/- 1 away from current position
            for (int x_change = -1; x_change < 2; x_change++)
            {
                for (int y_change = -1; y_change < 2; y_change++)
                {
                    if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                    {
                        TTSquare possibleSquare = tiles[currentX + x_change, currentY + y_change].GetComponent<TTSquare>();
                        if (possibleSquare.currentPiece == null)
                            moveAssessment[currentX + x_change, currentY + y_change] = 1;
                        else if (isNavy != possibleSquare.currentPiece.isNavy && possibleSquare.currentPiece.type != PieceType.LandMine)
                            moveAssessment[currentX + x_change, currentY + y_change] = 2;
                    }
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
        if(SceneManager.GetActiveScene().name == "Board")
        {
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
        }
        else
        {
            // For all squares +/- 1 away from current position
            for (int x_change = -1; x_change < 2; x_change++)
            {
                for (int y_change = -1; y_change < 2; y_change++)
                {
                    if (IsSquareOnBoard(squareX + x_change, squareY + y_change))
                        if (tiles[squareX + x_change, squareY + y_change].GetComponent<TTSquare>().currentPiece == null)
                            moveAssessment[squareX + x_change, squareY + y_change] = 7;
                }
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
