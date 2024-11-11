using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isServer = false;
    public bool isClient = false;
    public Canvas canvas;

    public GameObject playerOne;
    public GameObject playerTwo;

    void Start()
    {
        // Only one role is active at a time for clarity
        isServer = false;
        isClient = false;

        // Disable both players' movement initially
        SetPlayerMovement(false, false);
    }

    public void setServer()
    {
        isServer = true;
        isClient = false;
        SetPlayerMovement(true, false);
    }

    public void setClient()
    {
        isClient = true;
        isServer = false;
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
