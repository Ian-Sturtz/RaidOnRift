using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skip : MonoBehaviour
{

    void Update()
    {
        if (Input.anyKey)
        {
            if (!StaticTutorialControl.cameFromStoryScene && !(StatManager.instance != null && StatManager.instance.completedTutorial))
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
}
