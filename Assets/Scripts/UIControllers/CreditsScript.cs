using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class CreditsScript : MonoBehaviour
{
    [SerializeField] private float creditDuration = 5f;
    [SerializeField] private float sceneDuration = 1f;
    [SerializeField] private int sceneCount = 1;
    [SerializeField] private GameObject AudioSource;
    [SerializeField] private float audioLength;
    [SerializeField] private int currentScene = 0;
    [SerializeField] private string[] credits = new string[10];
    [SerializeField] private TMP_Text creditText;
    [SerializeField] private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        audioLength = AudioSource.GetComponent<AudioSource>().clip.length;
        creditDuration = audioLength;
        sceneDuration = creditDuration / sceneCount;
        StartCoroutine(RollCredits());
    }

    private void Update()
    {
        // Skips the credits if any key is pressed
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    private void nextScene()
    {
        if (currentScene < sceneCount)
        {
            currentScene += 1;

            creditText.SetText(credits[currentScene - 1]);
        }
    }

    IEnumerator RollCredits()
    {
        float startTime = Time.time;
        float sceneTime = 0f;
        nextScene();

        while (creditDuration > 0)
        {
            elapsedTime = 0f;
            elapsedTime = Time.time - startTime;
            startTime = elapsedTime;
            creditDuration = audioLength - elapsedTime;
            sceneTime += elapsedTime;

            
            // Scene has not gone on for long enough
            if(sceneTime < sceneDuration)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                nextScene();
            }
        }

        SceneManager.LoadScene("Main Menu");
    }
}
