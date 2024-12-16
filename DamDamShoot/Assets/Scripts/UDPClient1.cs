using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ClientUDP1 : MonoBehaviour
{
    Socket socket;
    string serverIP;
    public InputField ipInputField;
    public GameObject player1;
    public GameObject player2;

    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float bulletTime = 2f;

    private Vector3 receivedPositionP1;
    private Vector3 playerPosition;
    public static ClientUDP1 Instance { get; private set; }

    private readonly Queue<System.Action> mainThreadActions = new Queue<System.Action>();


    bool positionUpdatedP1;
    bool isRunning = true;


    HostJoinManager script;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Inicialización adicional si es necesario
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
        if (!string.IsNullOrEmpty(script.ip))
        {
            serverIP = script.ip;
        }

        receivedPositionP1 = new Vector3(0, 0, 0);
        playerPosition = player2.transform.position;

        StartClient();
    }

    void Update()
    {
        playerPosition = player2.transform.position;

        if (positionUpdatedP1)
        {
            player1.transform.position = receivedPositionP1;
            positionUpdatedP1 = false;
        }

        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }
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

            while (isRunning)
            {
                SendPosition(ipep);

                ReceivePosition();
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

    void ReceivePosition()
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
            else if (parts[0] == "SHOT" && parts.Length == 6)
            {
                if (float.TryParse(parts[1], out float px) &&
                    float.TryParse(parts[2], out float py) &&
                    float.TryParse(parts[3], out float pz) &&
                    float.TryParse(parts[4], out float dx) &&
                    float.TryParse(parts[5], out float dz))
                {
                    HandleShot(px, py, pz, dx, dz);
                }
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
    public void HandleShot(float px, float py, float pz, float dx, float dz)
    {
        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(() =>
            {
                Vector3 position = new Vector3(px, py, pz);
                Vector3 direction = new Vector3(dx, 0, dz);

                GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = direction.normalized * projectileSpeed;
                }
                Destroy(projectile, bulletTime);
            });
        }
    }

    public void SendShot(float px, float py, float pz, float dx, float dz)
    {
        Debug.Log("SendShot");

        string message = $"SHOT|{px}|{py}|{pz}|{dx}|{dz}";
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
                socket.Shutdown(SocketShutdown.Both); // Apagar el envío y la recepción
            }
            catch (SocketException)
            {
                // Ignorar si el socket ya está cerrado
            }
            socket.Close(); // Cerrar el socket
            socket = null;
        }

        Debug.Log("Client stopped and socket closed.");
    }
}
