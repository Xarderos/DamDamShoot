using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Animation


public class PlayerAnimController : MonoBehaviour
{
    public Animator playeranimator;
    Vector3 oldposition = Vector3.zero;
    float timer = 0;
    public float AnimOFFSET = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        oldposition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float posTesterX = oldposition.x - transform.position.x;
        float posTesterZ = oldposition.z - transform.position.z;
       
        if ((posTesterX > AnimOFFSET || posTesterZ > AnimOFFSET) || (posTesterX < -AnimOFFSET || posTesterZ < -AnimOFFSET))
        {
            playeranimator.SetBool("isRunning", true);
        }
        else
        {
            playeranimator.SetBool("isRunning", false);
        }

        if (timer < 0)
        {
            oldposition = transform.position;
            timer = 0.1f;
        }
        timer-=Time.deltaTime;
    }
}
