using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isServer = false;
    public bool isClient = false;

    public GameObject playerOne;
    public GameObject playerTwo;

    public GameObject Server;
    public GameObject Client;



    ////Reload Zone
    //public GameObject ReloadZoneP1;
    //public GameObject ReloadZoneP2;

    HostJoinManager script;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        isServer = false;
        isClient = false;

        SetPlayerMovement(false, false);

        script = GameObject.Find("DontDestroy").GetComponent<HostJoinManager>();
        if (script)
        {
            if (script.isServer)
            {
                setServer();
            }
            if (script.isClient)
            {
                setClient();
            }
        }
    }

    public void setServer()
    {
        isServer = true;
        isClient = false;
        Server.SetActive(true);
        SetPlayerMovement(true, false);
    }

    public void setClient()
    {
        isClient = true;
        isServer = false;
        Client.SetActive(true);
        SetPlayerMovement(false, true);
    }


    private void SetPlayerMovement(bool enablePlayerOne, bool enablePlayerTwo)
    {
        if (playerOne != null)
            playerOne.GetComponent<CapsuleMovement>().enabled = enablePlayerOne;

        if (playerTwo != null)
            playerTwo.GetComponent<CapsuleMovement>().enabled = enablePlayerTwo;
    }
}
