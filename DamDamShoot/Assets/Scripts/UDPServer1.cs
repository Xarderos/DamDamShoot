using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;

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

    public static ServerUDP Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Inicializaci�n adicional si es necesario
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
                lock (this)
                {
                    clientEndpoint = remote;
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)  // Verificamos si fue una desconexi�n
                {
                    Debug.LogWarning("La conexi�n se ha cerrado por el cliente.");
                }
                else
                {
                    Debug.LogError("Error de socket: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Excepci�n general: " + ex.Message);
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

    public void BroadcastShot(float px, float py, float pz, float dx, float dz)
    {
        string message = $"SHOT|{px}|{py}|{pz}|{dx}|{dz}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        if (clientEndpoint != null)
        {
            socket.SendTo(data, clientEndpoint);
        }
    }

}
