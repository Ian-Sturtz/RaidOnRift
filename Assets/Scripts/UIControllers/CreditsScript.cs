using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditsScript : MonoBehaviour
{
    [SerializeField] private float creditDuration = 5f;
    [SerializeField] private float sceneDuration = 1f;
    [SerializeField] private int sceneCount = 1;
    [SerializeField] private GameObject AudioSource;
    [SerializeField] private float audioLength;

    // Start is called before the first frame update
    void Start()
    {
        audioLength = AudioSource.GetComponent<AudioSource>().clip.length;
        creditDuration = audioLength;
        sceneDuration = creditDuration / sceneCount;
        //StartCoroutine(RollCredits());
    }

    private void Update()
    {
        // Skips the credits if any key is pressed
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    //IEnumerator RollCredits()
    //{
    //    float startTime = Time.time;
    //    float elapsedTime;
    //    float sceneTime;
    //    float sceneStart;

    //    while (creditDuration > 0)
    //    {
    //        elapsedTime = Time.time - startTime;
    //        creditDuration -= elapsedTime;

            
    //    }

    //    SceneManager.LoadScene("Main Menu");
    //}
}
