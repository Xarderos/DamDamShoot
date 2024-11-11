using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    public float speed = 5f; // Velocidad del jugador
    private Rigidbody rb;

    void Start()
    {
        // Obtiene el Rigidbody adjunto y desactiva la gravedad
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Desactiva la gravedad
    }

    void FixedUpdate()
    {
        // Captura la entrada del teclado WASD
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Define el vector de movimiento en el plano XZ
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed;

        // Asigna el vector de movimiento a la velocidad del Rigidbody
        rb.velocity = movement;
    }
}
