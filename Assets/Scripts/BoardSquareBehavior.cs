using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquareBehavior : MonoBehaviour
{
    [SerializeField] private Material defaultBoardMaterial;
    [SerializeField] private Material hoveredBoardMaterial;

    void Start()
    {
        SetMaterial(defaultBoardMaterial);
    }

    private void OnMouseOver()
    {
        SetMaterial(hoveredBoardMaterial);
    }

    private void OnMouseExit()
    {
        SetMaterial(defaultBoardMaterial);
    }

    public void SetMaterial(Material newMaterial)
    {
        if (this.tag != "GameController")
            GetComponent<SpriteRenderer>().material = newMaterial;
    }
}
