using UnityEngine;

public class AmmoUIManager : MonoBehaviour
{
    public GameObject bulletIconPrefab; 
    public Transform p1BulletContainer; 
    public Transform p2BulletContainer; 

    private CapsuleMovement player1;
    private CapsuleMovement player2;

    void Start()
    {
       
        CapsuleMovement[] players = FindObjectsOfType<CapsuleMovement>();
        foreach (CapsuleMovement player in players)
        {
            if (player.isP1) player1 = player;
            else player2 = player;
        }
    }

    void Update()
    {
        
        if (player1 != null)
        {
            UpdateBullets(p1BulletContainer, player1.ammoCount);
        }

     
        if (player2 != null)
        {
            UpdateBullets(p2BulletContainer, player2.ammoCount);
        }
    }

    void UpdateBullets(Transform container, int ammoCount)
    {
      
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

      
        for (int i = 0; i < ammoCount; i++)
        {
            Instantiate(bulletIconPrefab, container);
        }
    }
}


