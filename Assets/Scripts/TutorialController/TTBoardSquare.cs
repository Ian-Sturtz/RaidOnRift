using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTSquare : MonoBehaviour
{
    [SerializeField] private GameObject gameBoard;
    private TTGameBoard board;

    [Header("Board Materials")]
    public Material defaultBoardMaterial;
    public Material hoveredBoardMaterial;
    public Material clickedBoardMaterial;
    public Material moveableBoardMaterial;
    public Material enemyBoardMaterial;
    public Piece currentPiece;

    public bool SquareHasBeenClicked = false;
    private bool flashing = false;
    private bool continualFlash = false;

    private void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        board = gameBoard.GetComponent<TTGameBoard>();
        SetMaterial(defaultBoardMaterial);
    }

    private void Update()
    {
        if (!flashing)
        {
            if (SquareHasBeenClicked)
            {
                SetMaterial(clickedBoardMaterial);
            }
        }

        if (tag == "CannonDestination")
        {
            if (!continualFlash)
            {
                continualFlash = true;
                StartCoroutine(ContinualFlash(defaultBoardMaterial, moveableBoardMaterial));
            }
        }
        else if(tag == "CannonTarget")
        {
            if (!continualFlash)
            {
                continualFlash = true;
                StartCoroutine(ContinualFlash(defaultBoardMaterial, enemyBoardMaterial));
            }
        }
        else if (tag == "CorsairJump")
        {
            if (!continualFlash)
            {
                continualFlash = true;
                StartCoroutine(ContinualFlash(defaultBoardMaterial, hoveredBoardMaterial, .6f));
            }
        }
        else if (tag == "InteractablePiece")
        {
            if(!continualFlash)
            {
                continualFlash = true;
                StartCoroutine(ContinualFlash(defaultBoardMaterial, moveableBoardMaterial));
            }
        }
        else
        {
            if (continualFlash)
            {
                continualFlash = false;
                flashing = false;
                StopAllCoroutines();
            }
        }
    }

    private void OnMouseOver()
    {
        if (!board.squareSelected && !flashing && tag == "GameSquare")
            SetMaterial(hoveredBoardMaterial);
    }

    private void OnMouseExit()
    {
        if (!board.squareSelected && tag == "GameSquare")
            SetMaterial(defaultBoardMaterial);
    }

    public void SetMaterial(Material newMaterial)
    {
        GetComponent<SpriteRenderer>().material = newMaterial;
    }

    public void FlashMaterial(Material flashingMaterial, int flashCount)
    {
        StartCoroutine(MaterialFlash(defaultBoardMaterial, flashingMaterial, flashCount));
        SetMaterial(defaultBoardMaterial);
    }

    IEnumerator MaterialFlash(Material StartingMaterial, Material TargetMaterial, int flashCount)
    {
        flashing = true;
        float delay = .07f;
        for (int i = 0; i < flashCount; i++)
        {
            SetMaterial(TargetMaterial);
            yield return new WaitForSeconds(delay);
            SetMaterial(StartingMaterial);
            yield return new WaitForSeconds(delay);
        }
        flashing = false;
    }

    IEnumerator ContinualFlash(Material StartingMaterial, Material TargetMaterial, float delay = .35f)
    {
        while (continualFlash)
        {
            SetMaterial(TargetMaterial);
            yield return new WaitForSeconds(delay);
            SetMaterial(StartingMaterial);
            if (delay != .35f)
            {
                yield return new WaitForSeconds(.1f);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
