using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ServerUDP : MonoBehaviour
{
    Socket socket;
    public GameObject player1;
    public GameObject player2;
    string serverText;

    void Start()
    {
        startServer();
    }

    public void startServer()
    {
        serverText = "Starting UDP Server...";

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket.Bind(ipep);

        Thread receiveThread = new Thread(Receive);
        receiveThread.Start();
    }

    void Update()
    {
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;

        serverText = serverText + "\nWaiting for new Client...";

        while (true)
        {
            int recv = socket.ReceiveFrom(data, ref Remote);
            string message = Encoding.ASCII.GetString(data, 0, recv);

            serverText = serverText + $"\nMessage received from {Remote.ToString()}: {message}";

            Thread sendThread = new Thread(() => Send(Remote));
            sendThread.Start();
        }
    }

    void Send(EndPoint Remote)
    {
        //Sending player1 position to client
        Vector3 playerPosition = player1.transform.position;
        string message = $"{playerPosition.x}|{playerPosition.y}|{playerPosition.z}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        socket.SendTo(data, Remote);
    }
}
