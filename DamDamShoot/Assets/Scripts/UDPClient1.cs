using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class ClientUDP1 : MonoBehaviour
{
    Socket socket;
    string serverIP;
    public GameObject player1;
    public GameObject player2;

    private Vector3 receivedPositionP1;
    private Vector3 playerPosition;

    bool positionUpdatedP1;
    bool isRunning = true;

    void Start()
    {
        serverIP = "192.168.1.137";
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

            string[] positionData = message.Split('|');

            if (positionData.Length == 3 &&
                float.TryParse(positionData[0], out float x) &&
                float.TryParse(positionData[1], out float y) &&
                float.TryParse(positionData[2], out float z))
            {
                receivedPositionP1 = new Vector3(x, y, z);
                positionUpdatedP1 = true;
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
}
