using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float reloadTime = 2f;
    public float reloadSpeedMultiplier = 0.5f;
    public float dashSpeedMultiplier = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    public float bulletTime = 2f;

    private Rigidbody rb;
    private Camera mainCamera;

    private int ammoCount = 1;
    private bool isReloading = false;
    private float originalSpeed;
    private bool canDash = true;
    private Coroutine reloadCoroutine = null;

    void Start()
    {
        ammoCount = 1;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        mainCamera = Camera.main;

        originalSpeed = speed;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed;

        rb.velocity = movement;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ammoCount > 0)
        {
            ShootProjectile();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            Dash();
        }
    }

    void ShootProjectile()
    {
        ammoCount--;
        Debug.Log("Disparo realizado. Balas restantes: " + ammoCount);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 direction = (hitInfo.point - transform.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * projectileSpeed;
            }

            Destroy(projectile, bulletTime);
        }
    }

    void StartReload()
    {
        if (reloadCoroutine != null) return;

        reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    System.Collections.IEnumerator ReloadCoroutine()
    {
        Debug.Log("Recargando...");
        isReloading = true;

        speed *= reloadSpeedMultiplier;

        yield return new WaitForSeconds(reloadTime);

        speed = originalSpeed;

        ammoCount++;
        Debug.Log("Recarga completa. Balas disponibles: " + ammoCount);

        isReloading = false;
        reloadCoroutine = null;
    }

    void Dash()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;

            speed = originalSpeed;
            isReloading = false;
            Debug.Log("Recarga interrumpida");
        }

        StartCoroutine(DashCoroutine());
    }

    System.Collections.IEnumerator DashCoroutine()
    {
        Debug.Log("Dash iniciado");

        canDash = false;

        speed *= dashSpeedMultiplier;

        yield return new WaitForSeconds(dashDuration);

        speed = originalSpeed;

        Debug.Log("Dash terminado");

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        Debug.Log("Dash listo para usarse nuevamente");
    }
}
