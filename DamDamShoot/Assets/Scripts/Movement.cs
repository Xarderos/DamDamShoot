using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    private Rigidbody rb;
    private Camera mainCamera;

    private bool canShoot = true;
    public float reloadTime = 2f;
    public float reloadSpeedMultiplier = 0.5f;

    private float originalSpeed;

    void Start()
    {
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
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            ShootProjectile();
            canShoot = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void ShootProjectile()
    {
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

            Destroy(projectile, 3f);
        }
    }

    void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    System.Collections.IEnumerator ReloadCoroutine()
    {
        Debug.Log("Recargando...");

  
        speed *= reloadSpeedMultiplier;

        yield return new WaitForSeconds(reloadTime); 


        speed = originalSpeed;

        canShoot = true;
        Debug.Log("Recarga completa");
    }
}
