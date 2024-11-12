using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    public GameObject player2; // El objeto del jugador P2 (cliente)
    public GameObject player1; // El objeto del jugador P1 (host)

    public int port = 7777;   // Puerto para enviar/recibir datos
    public string serverIP = "192.168.1.136"; // IP del servidor (P1)
    private UdpClient udpClient;
    private Vector3 receivedPositionP1; // Almacena la posición de P1 recibida
    private bool positionUpdatedP1 = false; // Marca cuando llega una nueva posición de P1

    void Start()
    {
        receivedPositionP1 = new Vector3(0, 0, 0);
        udpClient = new UdpClient();
        udpClient.Connect(serverIP, port); // Conectar al servidor (P1)
        udpClient.BeginReceive(ReceiveData, null); // Comienza a recibir datos
    }

    void Update()
    {
        // Enviar la posición de P2 (cliente) al servidor (P1)
        SendPositionP2();

        // El cliente actualiza la posición de P1 con los datos recibidos
        if (positionUpdatedP1)
        {
            player1.transform.position = receivedPositionP1;
            positionUpdatedP1 = false;
        }
    }

    // Método para enviar datos desde el cliente (P2) al servidor (P1)
    private void SendPositionP2()
    {
        Vector3 playerPositionP2 = player2.transform.position;
        string message = $"{playerPositionP2.x}|{playerPositionP2.y}|{playerPositionP2.z}";

        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length); // Enviar la posición al servidor (P1)
    }

    // Método para recibir datos en el cliente (P2) desde el servidor (P1)
    private void ReceiveData(IAsyncResult result)
    {
        // Obtiene el mensaje recibido
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        byte[] receivedData = udpClient.EndReceive(result, ref remoteEndPoint);

        // Convertir de bytes a string y luego a posición
        string message = Encoding.UTF8.GetString(receivedData);
        string[] positionData = message.Split('|'); // Usamos "|" como delimitador

        // Parsear la posición
        if (positionData.Length == 3 &&
            float.TryParse(positionData[0], out float x) &&
            float.TryParse(positionData[1], out float y) &&
            float.TryParse(positionData[2], out float z))
        {
            receivedPositionP1 = new Vector3(x, y, z);
            positionUpdatedP1 = true; // Marca que hemos recibido una nueva posición
        }

        // Continuar recibiendo datos
        udpClient.BeginReceive(ReceiveData, null);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
