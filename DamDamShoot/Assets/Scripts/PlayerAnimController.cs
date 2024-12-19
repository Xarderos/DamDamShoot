using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Animation


public class PlayerAnimController : MonoBehaviour
{
    public Animator playeranimator;
    Vector3 oldposition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        oldposition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (oldposition != transform.position)
        {
            playeranimator.SetBool("isRunning", true);
        }
        else
        {
            playeranimator.SetBool("isRunning", false);
        }
        oldposition = transform.position;
    }
}
