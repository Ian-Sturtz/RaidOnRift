using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piece : MonoBehaviour
{
    public GameObject controller;
    public GameObject gameBoard;
    public GameObject movePlate;

    public const int startingX = 1;
    public const int startingY = 1;

    // Position on the board
    private int xBoard = -1;
    private int yBoard = -1;

    // Team is either "navy" or "pirate"
    private string team;

    // Navy piece references
    public Sprite navy_mate;
    public Sprite navy_flag;

    // Pirate piece references
    public Sprite pirate_mate;
    public Sprite pirate_flag;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");

        xBoard = startingX;
        yBoard = startingY;

        SetCoords();

        // Assigns a piece to its corresponding sprite
        switch (this.name)
        {
            case "navy_mate": this.GetComponent<SpriteRenderer>().sprite = navy_mate;
                break;

            case "navy_flag":
                this.GetComponent<SpriteRenderer>().sprite = navy_flag;
                break;

            case "pirate_mate":
                this.GetComponent<SpriteRenderer>().sprite = pirate_mate;
                break;

            case "pirate_flag":
                this.GetComponent<SpriteRenderer>().sprite = pirate_flag;
                break;
        }
    }

    // Adjusts board position to match Unity's position vector
    public void SetCoords()
    {
        

        float x = xBoard;
        float y = yBoard;
    }
}
