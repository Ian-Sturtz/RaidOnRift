using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    void OnEnable()
    {
        if (!StaticTutorialControl.cameFromStoryScene)
        {
            StaticTutorialControl.piece = PieceType.Mate;
            StaticTutorialControl.cameFromStoryScene = false;
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
    }
}
