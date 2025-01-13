using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScoreText : MonoBehaviour
{
    // Start is called before the first frame update
    public Text wins1;
    public Text wins2;
    HostJoinManager script;

    void Start()
    {
        script = GameObject.Find("DontDestroy").GetComponent<HostJoinManager>();

        if (script)
        {

            wins1.text = "P1 Wins: " + script.p1Victories;
            wins2.text = "P2 Wins: " + script.p2Victories;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
