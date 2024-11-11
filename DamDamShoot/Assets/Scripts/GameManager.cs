using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isServer = false;
    public bool isClient = false;
    public Canvas canvas;
    void Start()
    {
        isServer = false;
        isClient = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setServer()
    {
        isServer = true;
    }
    public void setClient()
    {
        isClient = true;
    }
    public void closeCanvas()
    {
        if (canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
