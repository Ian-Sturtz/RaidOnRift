using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditsScript : MonoBehaviour
{
    private float creditDuration = 10f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {
        while(creditDuration > 0)
        {
            creditDuration -= 1f;
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("Board");
    }
}
