using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    public float speed = 5f; // Velocidad del jugador
    public GameObject projectilePrefab; // Prefab del cubo que se disparará
    public float projectileSpeed = 10f; // Velocidad del proyectil
    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        // Obtiene el Rigidbody adjunto y desactiva la gravedad
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Obtiene la cámara principal
        mainCamera = Camera.main;
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

    void Update()
    {
        // Verifica si se presionó el botón izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        // Obtiene la posición del ratón en el espacio de la pantalla
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Calcula la dirección hacia el punto de impacto
            Vector3 direction = (hitInfo.point - transform.position).normalized;

            // Instancia el proyectil en la posición del jugador
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Aplica velocidad al proyectil en la dirección calculada
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * projectileSpeed;
            }

            // Destruye el proyectil después de 3 segundos
            Destroy(projectile, 2f);
        }
    }
}
