using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

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

        // Inicia el hilo para recibir mensajes
        Thread receiveThread = new Thread(Receive);
        receiveThread.Start();

        // Inicializa el hilo para enviar posiciones continuamente
        isSending = true;
        sendThread = new Thread(SendContinuously);
        sendThread.Start();
    }

    void Update()
    {
        // Actualiza la posición del jugador desde el motor de física de Unity
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
                // Actualiza el cliente remoto
                lock (this)
                {
                    clientEndpoint = remote;
                }
            }
            catch (SocketException ex)
            {
                Debug.Log(ex.Message);
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
            Thread.Sleep(10); // Envía cada 100 ms (ajustar según sea necesario)
        }
    }

    void Send(EndPoint remote)
    {
        // Envía la posición del jugador al cliente
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
}
