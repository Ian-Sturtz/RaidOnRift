using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float SpinSpeed = 10f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, SpinSpeed * Time.deltaTime, Space.Self);
    }
}
