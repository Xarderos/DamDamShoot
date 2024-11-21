using UnityEngine;

public class AmmoUIManager : MonoBehaviour
{
    public GameObject bulletIconPrefab; 
    public Transform p1BulletContainer; 
    public Transform p2BulletContainer; 

    private CapsuleMovement player1;
    private CapsuleMovement player2;
    public GameManager GM;
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
        if(GM.isServer)
        if (player1 != null)
        {
            UpdateBullets(p1BulletContainer, player1.ammoCount);
        }

        if (GM.isClient)
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


