using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBounce : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float height = 0.2f;

    Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }
    void Update()
    {


        float newY = Mathf.Sin(Time.time * speed) * height + pos.y;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}