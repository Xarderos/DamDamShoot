using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using UnityEngine.UI;

public class ClientUDP1 : MonoBehaviour
{
    Socket socket;
    string clientText;
    string serverIP;

    public GameObject player1;
    public GameObject player2;
    private Vector3 receivedPositionP1; //Saves the recieved p1 position
    Vector3 playerPosition; //Saves the p2 position

    bool positionUpdatedP1;

    void Start()
    {

        serverIP = "192.168.56.1";
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
    public void StartClient()
    {


        if (!string.IsNullOrEmpty(serverIP))
        {
            Thread mainThread = new Thread(Send);
            mainThread.Start();
        }
        else
        {
            clientText = "Please enter a valid IP address!";
        }
    }

    void Send()
    { 
        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string message = $"{playerPosition.x}|{playerPosition.y}|{playerPosition.z}";
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.SendTo(data, ipep);

            Thread receive = new Thread(Receive);
            receive.Start();
        }
        catch (SocketException e)
        {
            clientText = "Error: " + e.Message;
        }
    }

    void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);
        byte[] data = new byte[1024];

        int recv = socket.ReceiveFrom(data, ref Remote);
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
}
