using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isServer = false;
    public bool isClient = false;
    public Canvas canvas;

    public GameObject playerOne;
    public GameObject playerTwo;

    public GameObject Server;
    public GameObject Client;

    void Awake()
    {
        // Verifica si ya existe una instancia, si no, asigna esta.
        if (instance == null)
        {
            instance = this;
            // Aseg�rate de que el objeto GameManager persista entre escenas si es necesario
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe una instancia, destruye el nuevo objeto.
            Destroy(gameObject);
        }
    }
    void Start()
    {
        isServer = false;
        isClient = false;

        SetPlayerMovement(false, false);
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

    public void closeCanvas()
    {
        if (canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void SetPlayerMovement(bool enablePlayerOne, bool enablePlayerTwo)
    {
        if (playerOne != null)
            playerOne.GetComponent<CapsuleMovement>().enabled = enablePlayerOne;

        if (playerTwo != null)
            playerTwo.GetComponent<CapsuleMovement>().enabled = enablePlayerTwo;
    }
}
