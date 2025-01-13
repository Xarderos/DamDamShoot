using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostJoinManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isClient = false;
    public bool isServer = false;

    public InputField ipInputField;
    public string ip;

    public int p1Victories = 0;
    public int p2Victories = 0;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (ipInputField)
        ip = ipInputField.text;
    }

    // Update is called once per frame
    void Update()
    {
        if(ipInputField)
        ip = ipInputField.text;
    }

    public void SetClient() {

        isClient = true;

    }
    public void SetServer()
    {

        isServer = true;
    }

}
