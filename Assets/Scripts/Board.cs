using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public float tile_size = 1.15f;
    private const int TILE_COUNT_X = 10;
    private const int TILE_COUNT_Y = 10;
    private GameObject[,] tiles;
    private GameObject gameBoard;

    private Camera currentCamera;


    // Start is called before the first frame update
    void Start()
    {
        GenerateAllTiles(tile_size, TILE_COUNT_X, TILE_COUNT_Y);        
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.current;
            return;
        }
    }

    //Generates the board and sets the board in the center of the camera
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        tiles = new GameObject[tileCountX, tileCountY];


        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);



        centerBoardInCamera();
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, y * tileSize, 0);
        vertices[1] = new Vector3(x * tileSize, (y+1) * tileSize, 0);
        vertices[2] = new Vector3((x+1) * tileSize, y * tileSize, 0);
        vertices[3] = new Vector3((x+1) * tileSize, (y+1) * tileSize, 0);

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    private void centerBoardInCamera()
    {
        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");

        float x_pos;
        float y_pos;

        x_pos = gameBoard.transform.position.x;
        y_pos = gameBoard.transform.position.y;

        x_pos -= tile_size * (TILE_COUNT_X / 2);
        y_pos -= tile_size * (TILE_COUNT_Y / 2);

        Debug.Log("xpos = " + x_pos);
        Debug.Log("ypos = " + y_pos);

        gameBoard.transform.position = new Vector3 (x_pos, y_pos, -9);
    }
}
