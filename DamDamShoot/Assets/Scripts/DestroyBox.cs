using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    public AudioClip breakSound;           
    public GameObject particlePrefab;      

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");

       
        if (collision.gameObject.CompareTag("BulletP1") || collision.gameObject.CompareTag("PowerfulBulletP1"))
        {
            if (CompareTag("Box2"))
            {
                HandleDestruction(collision.gameObject); 
            }
            else if (CompareTag("Box1"))
            {
                DestroyBulletOnly(collision.gameObject); 
            }
        }
        
        else if (collision.gameObject.CompareTag("BulletP2") || collision.gameObject.CompareTag("PowerfulBulletP2"))
        {
            if (CompareTag("Box1"))
            {
                HandleDestruction(collision.gameObject);
            }
            else if (CompareTag("Box2"))
            {
                DestroyBulletOnly(collision.gameObject); 
            }
        }
    }

    private void HandleDestruction(GameObject bullet)
    {
        Debug.Log("Destruction conditions met. Destroying box...");

      
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        if (particlePrefab != null)
        {
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }

      
        Destroy(gameObject);

       
        Destroy(bullet);
    }

    private void DestroyBulletOnly(GameObject bullet)
    {
        Debug.Log("Bullet hit a non-destroyable box. Destroying bullet only.");
        Destroy(bullet);
    }
}
