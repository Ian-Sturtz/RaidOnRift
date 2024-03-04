using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PSTests
{
    [UnityTest]
    public IEnumerator PieceManagerCreated()
    {
        SceneManager.LoadScene("Piece Selection");
        yield return new WaitForSeconds(0.1f);

        Assert.That(PieceManager.instance, !Is.Null);
    }

    [UnityTest]
    public IEnumerator PieceManagerDefaultData()
    {
        SceneManager.LoadScene("Piece Selection");
        yield return new WaitForSeconds(0.1f);

        var gameObject = GameObject.Find("Team Selections");
        var ps = gameObject.GetComponent<PieceSelection>();
        ps.chosenFaction(true);
        ps.confirmSelection();

        Assert.AreEqual(PieceManager.instance.navyMate, 5);
    }
}
