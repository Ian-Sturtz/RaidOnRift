using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private int PIECES_ADDED = 2;    //Increase this number for each new piece added
    [SerializeField] private GameObject board;
    
    
    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] PiecePrefabs;

    private void Awake()
    {
        board = GameObject.FindGameObjectWithTag("GameBoard");
    }

    private void Update()
    {
        
    }

    

}
