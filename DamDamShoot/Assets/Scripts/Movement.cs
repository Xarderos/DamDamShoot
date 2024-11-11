using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;    // Speed of movement
    private Rigidbody rb;

    // Identifies if this capsule is Player 1 or Player 2
    public bool isPlayerOne;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // Check if the player should have control based on role
        if ((isPlayerOne && gameManager.isServer) || (!isPlayerOne && gameManager.isClient))
        {
            HandleMovement();
        }
        else
        {
            // If not allowed to move, stop movement
            StopMovement();
        }
    }

    void HandleMovement()
    {
        // Capture movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            // Calculate and apply movement velocity based on input
            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized * moveSpeed;
            Vector3 velocity = transform.TransformDirection(movement);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }
        else
        {
            // No input, stop horizontal movement
            StopMovement();
        }
    }

    // Method to immediately stop horizontal movement
    private void StopMovement()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
}
