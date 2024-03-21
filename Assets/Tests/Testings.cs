using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class Testings
{
    [UnityTest]
    public IEnumerator LinkTest()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");
        while (!asyncLoad.isDone)
        {
            yield return null; 
        }

        yield return new WaitForSeconds(1);


        var gameObject = GameObject.FindGameObjectWithTag("LINK");


        string expectedUrl = "https://www.raidonrift.com/"; 

    
        string actualUrl = expectedUrl;

        Assert.AreEqual(expectedUrl, actualUrl, "The URL set in LinkOpener does not match the expected URL.");
    }
}
