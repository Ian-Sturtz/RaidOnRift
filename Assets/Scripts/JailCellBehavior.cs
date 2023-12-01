using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailCell : MonoBehaviour
{
    [Header("Board Materials")]
    public Material inactiveJailMaterial;
    public Material activeJailMaterial;
    public Material clickedJailMaterial;

    public Piece currentPiece;
    public bool hasPiece = false;

    private void Start()
    {

    }

    private void Update()
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

    public void SetMaterial(Material newMaterial)
    {
        if (this.tag != "GameController")
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
}
