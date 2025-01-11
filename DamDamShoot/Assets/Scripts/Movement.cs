using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    public bool canMove = false;

    public float speed = 5f;
    public GameObject projectilePrefab;
    public GameObject powerfulProjectilePrefab;
    public float projectileSpeed = 10f;
    public float reloadTime = 2f;
    public float reloadSpeedMultiplier = 0.5f;
    public float dashSpeedMultiplier = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    public float bulletTime = 2f;
    public bool isP1 = true;

    private Rigidbody rb;
    private Camera mainCamera;

    public int ammoCount = 1;
    private bool isReloading = false;
    private float originalSpeed;
    private bool canDash = true;
    private Coroutine reloadCoroutine = null;

    // Escudo
    public float shieldDuration = 0.75f;
    public float shieldCooldown = 4f;
    private bool canUseShield = true;
    private bool isShieldActive = false;
    public GameObject shieldVisual;

    //Zona de recarga
    private bool isInReloadZone = false;

    //Parry
    public GameObject parryVisual; 
    public float parryDuration = 0.2f; 
    public LayerMask bulletLayerMask;
    public float parryDirectionX = 0;
    public float parryDirectionZ = 0;

    private bool isParrying = false;


    public ReloadUIController reloadUIController;

    void Start()
    {
        ammoCount = 1;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        mainCamera = Camera.main;
        originalSpeed = speed;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        if (parryVisual != null)
            parryVisual.SetActive(false);

    }

    void FixedUpdate()
    {

        if (!canMove)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed;
        rb.velocity = movement;
    }

    void Update()
    {
        if (!canMove) return;

        if (Input.GetMouseButtonDown(0) && ammoCount > 0)
        {
            ShootProjectile();
        }

        if (Input.GetMouseButtonDown(1) && ammoCount >= 2)
        {
            ShootPowerfulProjectile();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isInReloadZone)
            {
                StartReload();
            }
            else
            {
                Debug.LogWarning("No puedes recargar fuera de una zona de recarga.");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.Q) && canUseShield)
        {
            ActivateShield();
            if (GameManager.instance != null)
            {
                if (GameManager.instance.isServer && ServerUDP.Instance != null)
                {
                    ServerUDP.Instance.SendShield();
                }
                else if (GameManager.instance.isClient && ClientUDP1.Instance != null)
                {
                    ClientUDP1.Instance.SendShield();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isParrying && ammoCount>0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 direction = (hitInfo.point - transform.position).normalized;

                if (GameManager.instance != null)
                {
                    if (GameManager.instance.isServer && ServerUDP.Instance != null)
                    {
                        ServerUDP.Instance.BroadcastParry(direction.x, direction.z);
                    }
                    else if (GameManager.instance.isClient && ClientUDP1.Instance != null)
                    {
                        ClientUDP1.Instance.SendParry(direction.x, direction.z);
                    }
                }
                StartParry(direction.x, direction.z);

            }
        }


    }
    public void StartParry(float X, float Y)
    {
        StartCoroutine(PerformParry(X, Y));
    }
    void ShootProjectile()
    {
        if (ammoCount <= 0)
        {
            Debug.LogWarning("No ammo left.");
            return;
        }

        ammoCount--;
        Debug.Log("Disparo realizado. Balas restantes: " + ammoCount);

        CreateProjectile(projectilePrefab, false);
        AudioManager.Instance.PlayAudio("Shot");


    }

    void ShootPowerfulProjectile()
    {
        if (ammoCount < 2)
        {
            Debug.LogWarning("No tienes suficiente munición para disparar la bala potente.");
            return;
        }

        ammoCount -= 2;
        Debug.Log("Disparo potente realizado. Balas restantes: " + ammoCount);

        CreateProjectile(powerfulProjectilePrefab, true);
        AudioManager.Instance.PlayAudio("StrongShot");

    }

    void CreateProjectile(GameObject prefab, bool isPowerful)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 direction = (hitInfo.point - transform.position).normalized;
            Vector3 newBulletPosition = transform.position + direction;
            GameObject projectile = Instantiate(prefab, newBulletPosition, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = direction * projectileSpeed;
            }

            if (GameManager.instance != null)
            {
                if (GameManager.instance.isServer && ServerUDP.Instance != null)
                {
                    ServerUDP.Instance.BroadcastShot(newBulletPosition.x, newBulletPosition.y, newBulletPosition.z, direction.x, direction.z, isPowerful);
                }
                else if (GameManager.instance.isClient && ClientUDP1.Instance != null)
                {
                    ClientUDP1.Instance.SendShot(newBulletPosition.x, newBulletPosition.y, newBulletPosition.z, direction.x, direction.z, isPowerful);
                }
            }
            Destroy(projectile, bulletTime);
        }
    }

    public void ActivateShield()
    {
        Debug.Log("Escudo Activado");
        isShieldActive = true;
        canUseShield = false;
        AudioManager.Instance.PlayAudio("Shield");

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        StartCoroutine(ShieldDurationCoroutine());
        StartCoroutine(ShieldCooldownCoroutine());
    }

    IEnumerator ShieldDurationCoroutine()
    {
        yield return new WaitForSeconds(shieldDuration);
        isShieldActive = false;
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
        Debug.Log("Escudo Desactivado");
    }

    IEnumerator ShieldCooldownCoroutine()
    {
        yield return new WaitForSeconds(shieldCooldown);
        canUseShield = true;
        Debug.Log("Escudo Listo");
    }

    void StartReload()
    {
        if (isReloading) return;
        if (reloadUIController != null)
        {
            reloadUIController.StartReloadAnimation(isP1);
        }
        StartCoroutine(ReloadCoroutine());
        AudioManager.Instance.PlayAudio("Reload");

    }

    IEnumerator ReloadCoroutine()
    {
        Debug.Log("Recargando...");
        isReloading = true;
        speed *= reloadSpeedMultiplier;
        yield return new WaitForSeconds(reloadTime);
        speed = originalSpeed;
        ammoCount++;
        Debug.Log("Recarga completa. Balas disponibles: " + ammoCount);
        isReloading = false;
    }

    void Dash()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            speed = originalSpeed;
            isReloading = false;
        }
        StartCoroutine(DashCoroutine());
        AudioManager.Instance.PlayAudio("Dash");

    }

    IEnumerator DashCoroutine()
    {
        Debug.Log("Dash iniciado");
        canDash = false;
        speed *= dashSpeedMultiplier;
        yield return new WaitForSeconds(dashDuration);
        speed = originalSpeed;
        Debug.Log("Dash terminado");
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    IEnumerator PerformParry(float X, float Z)
    {

        parryDirectionX = X;
        parryDirectionZ = Z;
  
        isParrying = true;
        ammoCount--;
        Debug.Log("Parry activado. Munición restante: " + ammoCount);


        if (parryVisual != null)
            parryVisual.SetActive(true);


        yield return new WaitForSeconds(parryDuration);

        if (parryVisual != null)
            parryVisual.SetActive(false);

        isParrying = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isParrying && collision.gameObject.layer == LayerMask.NameToLayer("Bullets"))
        {
            
            Debug.Log("Bala reflejada con Parry");
            Destroy(collision.gameObject);
            Vector3 direction = new Vector3(parryDirectionX, 0, parryDirectionZ);
            Vector3 newBulletPosition = transform.position + direction;
            GameObject projectile = Instantiate(powerfulProjectilePrefab, newBulletPosition, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction.normalized * projectileSpeed;
            }
            Destroy(projectile, bulletTime);
            isParrying = false;

        }
        else if (isP1 && collision.transform.CompareTag("PowerfulBulletP2"))
        {
            SceneManager.LoadScene("P2Win");
        }
        else if (!isP1 && collision.transform.CompareTag("PowerfulBulletP1"))
        {
            SceneManager.LoadScene("P1Win");
        }
        else if (isShieldActive && (collision.transform.CompareTag("BulletP2") || collision.transform.CompareTag("BulletP1")))
        {
            Debug.Log("Bala destruida por el escudo");
            collision.gameObject.SetActive(false);
        }
        else if (isP1 && collision.transform.CompareTag("BulletP2"))
        {
            SceneManager.LoadScene("P2Win");
        }
        else if (!isP1 && collision.transform.CompareTag("BulletP1"))
        {
            SceneManager.LoadScene("P1Win");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReloadZone"))
        {
            isInReloadZone = true;
            Debug.Log("Entraste en la zona de recarga.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReloadZone"))
        {
            isInReloadZone = false;
            Debug.Log("Saliste de la zona de recarga.");
        }
    }

}
