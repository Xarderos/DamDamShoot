using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private bool isGrounded;

    // Indicate if this capsule is Player 1 or Player 2
    public bool isPlayerOne;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Locate GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // Allow movement if GameManager confirms the correct role
        if ((isPlayerOne && gameManager.isServer) || (!isPlayerOne && gameManager.isClient))
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized * moveSpeed;

        rb.MovePosition(transform.position + transform.TransformDirection(movement) * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
