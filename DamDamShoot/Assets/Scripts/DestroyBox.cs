using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    public AudioClip breakSound; // Assign the sound in the Inspector
    private AudioSource audioSource;

    private void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

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

            // Spawn particle effect (to be added below)
            SpawnParticles();

            // Destroy the box
            Destroy(gameObject);

            // Destroy the bullet as well
            Destroy(collision.gameObject);
        }
    }

    private void SpawnParticles()
    {
        // Placeholder: Add particle spawning code here
    }
}
