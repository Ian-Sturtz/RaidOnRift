using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailCell : MonoBehaviour
{
    [Header("Board Materials")]
    public Material inactiveJailMaterial;
    public Material activeJailMaterial;
    public Material interactableJailMaterial;
    public Material clickedJailMaterial;

    public Piece currentPiece;
    public bool hasPiece = false;

    public bool clicked = false;
    public bool interactable = false;
    private bool continualFlash = false;

    private void Update()
    {
        if (clicked)
        {
            SetMaterial(clickedJailMaterial);
            interactable = false;
            continualFlash = false;
        }
        else if (interactable && !continualFlash)
        {
            if(currentPiece.GetComponent<Piece>().type != PieceType.Ore)
                tag = "InteractablePiece";
            
            continualFlash = true;
            StartCoroutine(ContinualFlash(activeJailMaterial, interactableJailMaterial));
        }
        else if(!interactable && continualFlash)
        {
            tag = "JailCell";
            continualFlash = false;
        }
        else if(!interactable && !continualFlash)
        {
            if (hasPiece)
            {
                SetMaterial(activeJailMaterial);
            }
            else
            {
                SetMaterial(inactiveJailMaterial);
            }
        }
    }

    public void resetCell()
    {
        hasPiece = false;
        clicked = false;
        interactable = false;
        continualFlash = false;
        currentPiece = null;
        tag = "JailCell";
    }

    public void SetMaterial(Material newMaterial)
    {
        if (tag != "GameController")
            GetComponent<SpriteRenderer>().material = newMaterial;
    }

    public void FlashMaterial(Material flashingMaterial, int flashCount)
    {
        StartCoroutine(MaterialFlash(activeJailMaterial, flashingMaterial, flashCount));
        SetMaterial(activeJailMaterial);
    }

    IEnumerator MaterialFlash(Material StartingMaterial, Material TargetMaterial, int flashCount)
    {
        float delay = .05f;
        for (int i = 0; i < flashCount; i++)
        {
            SetMaterial(TargetMaterial);
            yield return new WaitForSeconds(delay);
            SetMaterial(StartingMaterial);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator ContinualFlash(Material StartingMaterial, Material TargetMaterial)
    {
        while (continualFlash)
        {
            float delay = .35f;

            SetMaterial(TargetMaterial);
            yield return new WaitForSeconds(delay);
            SetMaterial(StartingMaterial);
            yield return new WaitForSeconds(delay);
        }
    }
}
