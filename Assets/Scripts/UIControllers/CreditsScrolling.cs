using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScrolling : MonoBehaviour
{
    [Range(0, 5)]
    public float scrollSpeed;
    [SerializeField] private int MaxCreditHeight;
    [SerializeField] private RectTransform creditPosition;
    [SerializeField] private float yPos;
    [SerializeField] private GameObject AudioSource;

    #region Old Variables
    [SerializeField] private float creditDuration = 5f;
    [SerializeField] private float sceneDuration = 1f;
    [SerializeField] private int sceneCount = 1;
    [SerializeField] private int currentScene = 0;
    [SerializeField] private string[] credits = new string[10];
    [SerializeField] private float elapsedTime;
    #endregion

    private void Awake()
    {
        creditPosition = gameObject.GetComponent<RectTransform>();
        yPos = creditPosition.anchoredPosition3D.y;
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        Vector3 localVectorUp = transform.TransformDirection(0, 1, 0);
        pos += localVectorUp * scrollSpeed * Time.deltaTime; 
        transform.position = pos;

        // Skips the credits if any key is pressed
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Main Menu");
        }

        yPos = creditPosition.anchoredPosition3D.y;

        if (yPos >= MaxCreditHeight)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    IEnumerator RollCredits()
    {
        float startTime = Time.time;
        float sceneTime = 0f;


        while (creditDuration > 0)
        {
            elapsedTime = 0f;
            elapsedTime = Time.time - startTime;
            startTime = elapsedTime;
            sceneTime += elapsedTime;


            // Scene has not gone on for long enough
            if (sceneTime < sceneDuration)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                SceneManager.LoadScene("Main Menu");
            }
            {
                
            }

        }
    } }

