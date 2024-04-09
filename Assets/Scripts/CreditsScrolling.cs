using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScrolling : MonoBehaviour
{
    [Range(0, 5)]
    public float scrollSpeed;

    void Update()
    {

        Vector3 pos = transform.position;

        Vector3 localVectorUp = transform.TransformDirection(0, 1, 0);
        pos += localVectorUp * scrollSpeed * Time.deltaTime; transform.position = pos;


        if (Input.anyKey)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }     
}
