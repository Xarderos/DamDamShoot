using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateSun : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Start()
    {
    }


    void Update()
    {
        // Rota el objeto alrededor de su eje Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}

