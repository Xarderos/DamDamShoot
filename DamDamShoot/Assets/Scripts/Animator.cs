using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator; 
    private bool isRunning;    

    void Start()
    {
        
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            isRunning = true; 
        }
        else
        {
            isRunning = false; 
        }

        animator.SetBool("isRunning", isRunning);
    }
}
