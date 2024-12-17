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
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ip = ipInputField.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClient() {

        isClient = true;

    }
    public void SetServer()
    {

        isServer = true;
    }

}
