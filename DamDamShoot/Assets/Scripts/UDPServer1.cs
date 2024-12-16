using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

public class ServerUDP : MonoBehaviour
{

    Socket socket;
    public GameObject player1;
    public GameObject player2;

    Vector3 playerPosition;
    private Vector3 receivedPositionP2;
    private EndPoint clientEndpoint;
    private bool isSending;
    private Thread sendThread;
    bool positionUpdatedP2;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float bulletTime = 2f;
    public static ServerUDP Instance { get; private set; }
    private readonly Queue<System.Action> mainThreadActions = new Queue<System.Action>();

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
        playerPosition = player1.transform.position;
        startServer();
    }
    public void startServer()
    {
        Debug.Log("Starting UDP Server...");

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket.Bind(ipep);

        Thread receiveThread = new Thread(Receive);
        receiveThread.Start();

        isSending = true;
        sendThread = new Thread(SendContinuously);
        sendThread.Start();
    }

    void Update()
    {
        playerPosition = player1.transform.position;
        if (positionUpdatedP2)
        {
            player2.transform.position = receivedPositionP2;
            positionUpdatedP2 = false;
        }
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }
    }

    void Receive()
    {

        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)sender;

        Debug.Log("Waiting for new Client...");

        while (true)
        {
            try
            {
                int recv = socket.ReceiveFrom(data, ref remote);
                string message = Encoding.ASCII.GetString(data, 0, recv);

                string[] positionData = message.Split('|');

                if (positionData.Length == 3 &&
                    float.TryParse(positionData[0], out float x) &&
                    float.TryParse(positionData[1], out float y) &&
                    float.TryParse(positionData[2], out float z))
                {
                    receivedPositionP2 = new Vector3(x, y, z);
                    positionUpdatedP2 = true;
                }
                else if (positionData[0] == "SHOT" && positionData.Length == 6)
                {
                    Debug.Log("Receive");
                    if (float.TryParse(positionData[1], out float px) &&
                        float.TryParse(positionData[2], out float py) &&
                        float.TryParse(positionData[3], out float pz) &&
                        float.TryParse(positionData[4], out float dx) &&
                        float.TryParse(positionData[5], out float dz))
                    {

                        HandleShotOnServer(px, py, pz, dx, dz);

                    }
                }
                lock (this)
                {
                    clientEndpoint = remote;
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Debug.LogWarning("La conexión se ha cerrado por el cliente.");
                }
                else
                {
                    Debug.LogError("Error de socket: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Excepción general: " + ex.Message);
            }
        }
    }

    void SendContinuously()
    {
        while (isSending)
        {
            if (clientEndpoint != null)
            {
                lock (this)
                {
                    Send(clientEndpoint);
                }
            }
            Thread.Sleep(10);
        }
    }


    void Send(EndPoint remote)
    {
        string message = $"{playerPosition.x}|{playerPosition.y}|{playerPosition.z}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        try
        {
            socket.SendTo(data, remote);
        }
        catch (SocketException ex)
        {
            Debug.LogError("SocketException during Send: " + ex.Message);
        }
    }
    void HandleShotOnServer(float px, float py, float pz, float dx, float dz)
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
    public void BroadcastShot(float px, float py, float pz, float dx, float dz)
    {
        Debug.Log("Broadcast");
        string message = $"SHOT|{px}|{py}|{pz}|{dx}|{dz}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        if (clientEndpoint != null)
        {
            socket.SendTo(data, clientEndpoint);
        }
    }

    void OnDestroy()
    {
        StopServer();
    }

    public void StopServer()
    {
        isSending = false;

        // Cerrar el hilo de envío si está activo
        if (sendThread != null && sendThread.IsAlive)
        {
            sendThread.Abort();
            sendThread = null;
        }

        // Cerrar el socket si está activo
        if (socket != null)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both); // Apagar la comunicación
            }
            catch (SocketException)
            {
                // Ignorar si el socket ya está cerrado
            }
            socket.Close(); // Cerrar el socket
            socket = null;
        }

        Debug.Log("Server stopped and socket closed.");
    }

}
