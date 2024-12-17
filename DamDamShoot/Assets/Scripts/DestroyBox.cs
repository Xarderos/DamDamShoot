using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    public AudioClip breakSound;           // Assign the sound in the Inspector
    public GameObject particlePrefab;      // Assign the particle prefab in the Inspector

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("BulletP1") || collision.gameObject.CompareTag("BulletP2"))
        {
            Debug.Log("Collision with a bullet! Destroying box...");

            // Play the breaking sound
            if (breakSound != null)
            {
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }

            // Spawn particle effect
            SpawnParticles();

            // Destroy the box
            Destroy(gameObject);

            // Destroy the bullet
            Destroy(collision.gameObject);
        }
    }

    private void SpawnParticles()
    {
        if (particlePrefab != null)
        {
            // Instantiate the particle system at the box's position
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Particle prefab is not assigned!");
        }
    }
}
