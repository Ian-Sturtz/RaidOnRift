using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PieceMoveTest
{
    [UnityTest]
    public IEnumerator CaptainMovesetChecker()
    {
        SceneManager.LoadScene("Board");

        yield return new WaitForSeconds(0.1f);

        // This is what the move array should look like according to the Captain's rules
        // Assume the Captain starts at square {5,6} or x = 4, y = 5 when indexed at 0
        // index = 1: piece can move there
        // index = 0: piece is already there,
        // index =-1: piece can't move there
        int[,] correctMoveArray =
        {
            {-1, -1, -1, -1, 1, -1, 1, -1, -1, -1},
            {-1, -1, -1, 1, -1, 1, -1, 1, -1, -1},
            {-1, -1, 1, -1, 1, -1, 1, -1, 1, -1},
            {-1, 1, -1, 1, -1, 1, -1, 1, -1, 1},
            {1, -1, 1, -1, 1, 0, 1, -1, 1, -1},
            {-1, 1, -1, 1, -1, 1, -1, 1, -1, 1},
            {-1, -1, 1, -1, 1, -1, 1, -1, 1, -1},
            {-1, -1, -1, 1, -1, 1, -1, 1, -1, -1},
            {-1, -1, -1, -1, 1, -1, 1, -1, -1, -1},
            {-1, -1, -1, -1, -1, 1, -1, -1, -1, -1}
        };

        var gameBoard = GameObject.Find("Board");

        var captainObject = new GameObject();
        var captain = captainObject.AddComponent<Captain>();

        // Captain starts at position {5,6}
        captain.currentX = 4;
        captain.currentY = 5;

        int[,] moveAssessment = new int[10, 10];

        moveAssessment = captain.GetValidMoves(gameBoard.GetComponent<GameBoard>().tiles);

        Assert.AreEqual(correctMoveArray, moveAssessment);
    }
}
