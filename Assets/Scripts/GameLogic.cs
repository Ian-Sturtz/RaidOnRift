using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private int PIECES_ADDED = 2;    //Increase this number for each new piece added
    [SerializeField] private GameBoard board;
    
    
    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] PiecePrefabs;

    public Piece newPiece;

    private void Awake()
    {
        newPiece = SpawnPiece(PieceType.Mate, true);
    }

    private void Update()
    {
        newPiece.transform.position = new Vector3(1,1,1);
    }

    private Piece SpawnPiece(PieceType type, bool isNavy)
    {
        Piece cp;

        if (!isNavy)
        {
            cp = Instantiate(PiecePrefabs[(int)type + PIECES_ADDED], transform).GetComponent<Piece>();

            Debug.Log("Instantiated a Pirate Mate");
        }
        else
        {
            cp = Instantiate(PiecePrefabs[(int)type], transform).GetComponent<Piece>();

            Debug.Log("Instantiated a Navy Mate");
        }

        cp.type = type;
        cp.isNavy = isNavy;

        return cp;
    }

}
