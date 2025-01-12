using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;


public class ClientUDP1 : MonoBehaviour
{   
    public GameObject waitingCanvas;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI waitingText;
    private bool gameStarted = false;

    Socket socket;
    string serverIP;
    public GameObject player1;
    public GameObject player2;

    public GameObject projectilePrefab;
    public GameObject powProjectilePrefab;

    public float projectileSpeed = 12f;
    public float bulletLifeTime = 2f;

    private Vector3 receivedPositionP1;
    private Vector3 playerPosition;
    public static ClientUDP1 Instance { get; private set; }

    private readonly Queue<System.Action> mainThreadActions = new Queue<System.Action>();

    bool positionUpdatedP1;
    bool isRunning = true;
    bool activateshield = false;

    float parryX = 0;
    float parryZ = 0;
    bool isparrying = false;
    HostJoinManager script;

    float time = 0;

    //ReloadZone
    public GameObject reloadZoneP2;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        serverIP = "127.0.0.1";

        script = GameObject.Find("DontDestroy").GetComponent<HostJoinManager>();
        if (script.ip != "")
        {
            serverIP = script.ip;
        }
        waitingText.gameObject.SetActive(false);
        receivedPositionP1 = new Vector3(0, 0, 0);
        playerPosition = player2.transform.position;
        projectileSpeed = player2.GetComponent<CapsuleMovement>().projectileSpeed;
        StartClient();
    }

    void Update()
    {
        if (!gameStarted)
        {
            player2.GetComponent<CapsuleMovement>().canMove = false;
        }

        playerPosition = player2.transform.position;

        if (positionUpdatedP1)
        {
            player1.transform.position = Vector3.Lerp(player1.transform.position, receivedPositionP1, Time.deltaTime * 50);
            positionUpdatedP1 = false;
        }

        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }

        if (activateshield)
        {
            activateshield = false;
            ActivateShield();
        }
        if (isparrying)
        {
            ActivateParry(parryX, parryZ);
            isparrying = false;
        }
        time = Time.time;
    }

    void StartClient()
    {
        if (!string.IsNullOrEmpty(serverIP))
        {
            Thread clientThread = new Thread(ClientLoop);
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        else
        {
            Debug.LogError("Please enter a valid IP address!");
        }
    }

    void ClientLoop()
    {
        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            lock (mainThreadActions)
            {
                mainThreadActions.Enqueue(() => StartCountdown());
            }

            float lastSendTime = 0f;
            float sendInterval = 0.02f;

            while (isRunning)
            {
                if (time - lastSendTime >= sendInterval)
                {
                    SendPosition(ipep);
                    lastSendTime = time;
                }

                ReceiveData();
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket Error: " + e.Message);
        }
        finally
        {
            socket?.Close();
        }
    }

    void SendPosition(IPEndPoint ipep)
    {
        try
        {
            string message = $"{playerPosition.x}|{playerPosition.y}|{playerPosition.z}";
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.SendTo(data, ipep);
        }
        catch (SocketException e)
        {
            Debug.LogError("Error sending data: " + e.Message);
        }
    }

    void ReceiveData()
    {
        try
        {
            byte[] data = new byte[1024];
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            int recv = socket.ReceiveFrom(data, ref remote);
            string message = Encoding.ASCII.GetString(data, 0, recv);

            string[] parts = message.Split('|');

            if (parts.Length == 3 &&
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y) &&
                float.TryParse(parts[2], out float z))
            {
                receivedPositionP1 = new Vector3(x, y, z);
                positionUpdatedP1 = true;
            }
            else if (parts[0] == "SHOT" && parts.Length == 7)
            {
                Debug.Log("Receive");
                if (float.TryParse(parts[1], out float px) &&
                    float.TryParse(parts[2], out float py) &&
                    float.TryParse(parts[3], out float pz) &&
                    float.TryParse(parts[4], out float dx) &&
                    float.TryParse(parts[5], out float dz))
                {
                    bool isPowerful = false;
                    if (parts[6] == "POWERFUL")
                    {
                        isPowerful = true;
                    }
                    HandleShot(px, py, pz, dx, dz, isPowerful);

                }
            }
            else if (parts[0] == "PARRY")
            {
                if (float.TryParse(parts[1], out float X) &&
                    float.TryParse(parts[2], out float Z))
                {
                    isparrying = true;
                    parryX = X;
                    parryZ = Z;

                }
            }
            else if (message == "SHIELD")
            {
                activateshield = true;
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false; 
    }
    public void HandleShot(float px, float py, float pz, float dx, float dz, bool isPowerful)
    {
        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(() =>
            {
                Vector3 position = new Vector3(px, py, pz);
                Vector3 direction = new Vector3(dx, 0, dz);
                GameObject prefab = isPowerful ? powProjectilePrefab : projectilePrefab;
                GameObject projectile = Instantiate(prefab, position, Quaternion.identity);
                if (isPowerful)
                {
                    AudioManager.Instance.PlayAudio("StrongShot");
                }
                else 
                {
                    AudioManager.Instance.PlayAudio("Shot");
                }

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = direction.normalized * projectileSpeed;
                }
                Destroy(projectile, bulletLifeTime);
            });
        }
    }

    public void SendShot(float px, float py, float pz, float dx, float dz, bool isPowerful)
    {
        Debug.Log("SendShot");

        string message = $"SHOT|{px}|{py}|{pz}|{dx}|{dz}|{(isPowerful ? "POWERFUL" : "NORMAL")}";
        byte[] data = Encoding.ASCII.GetBytes(message);

        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
            socket.SendTo(data, ipep);
        }
        catch (SocketException e)
        {
            Debug.LogError("Error sending shot data: " + e.Message);
        }
    }

    public void SendParry(float x, float z)
    {
        Debug.Log("SendShot");

        string message = $"PARRY|{x}|{z}";
        byte[] data = Encoding.ASCII.GetBytes(message);

        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
            socket.SendTo(data, ipep);
        }
        catch (SocketException e)
        {
            Debug.LogError("Error sending shot data: " + e.Message);
        }
    }
    public void SendShield()
    {
        Debug.Log("SendShield");

        string message = $"SHIELD";
        byte[] data = Encoding.ASCII.GetBytes(message);

        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
            socket.SendTo(data, ipep);
        }
        catch (SocketException e)
        {
            Debug.LogError("Error sending shot data: " + e.Message);
        }
    }

    void OnDestroy()
    {
        StopClient();
    }

    void StopClient()
    {
        isRunning = false;

        if (socket != null)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
            }
            socket.Close();
            socket = null;
        }

        Debug.Log("Client stopped and socket closed.");
    }

    void ActivateShield()
    {
        player1.GetComponent<CapsuleMovement>().ActivateShield();
    }
    void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }
    void ActivateParry(float x, float z)
    {
        player1.GetComponent<CapsuleMovement>().StartParry(x, z);
    }
    IEnumerator CountdownCoroutine()
    {

        waitingCanvas.SetActive(true);
        int countdown = 3;

        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }

        waitingCanvas.SetActive(false);
        player2.GetComponent<CapsuleMovement>().canMove = true;
        gameStarted = true;

        //ReloadZone
        if (reloadZoneP2 != null)
        {
            reloadZoneP2.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el objeto ReloadZone en el inspector.");
        }
    }
}
