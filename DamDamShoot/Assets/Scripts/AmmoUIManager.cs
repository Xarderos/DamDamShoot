using UnityEngine;
using UnityEngine.UI;

public class AmmoUIManager : MonoBehaviour
{
    public GameObject bulletIconPrefab; 
    public Transform p1BulletContainer; 
    public Transform p2BulletContainer; 
    private CapsuleMovement player1;
    private CapsuleMovement player2;

    void Start()
    {
       
        player1 = FindObjectOfType<CapsuleMovement>(); 
        player2 = FindObjectOfType<CapsuleMovement>(); 
    }

    void Update()
    {
        if (player1 != null && player1.isP1)
        {
            UpdateBullets(p1BulletContainer, player1.ammoCount);
        }

        if (player2 != null && !player2.isP1)
        {
            UpdateBullets(p2BulletContainer, player2.ammoCount);
        }
    }

    void UpdateBullets(Transform container, int ammoCount)
    {
        // Clear existing icons
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Add new icons
        for (int i = 0; i < ammoCount; i++)
        {
            Instantiate(bulletIconPrefab, container);
        }
    }
}

