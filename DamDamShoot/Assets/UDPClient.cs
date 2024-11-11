using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    public GameObject player; // El objeto que representa a P1 en el cliente
    public int port = 7777;   // Puerto para recibir datos (debe coincidir con el puerto del servidor)
    private UdpClient udpClient;
    private Vector3 receivedPosition; // Almacena la posición recibida del servidor
    private bool positionUpdated = false; // Marca cuando llega una posición nueva

    void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.BeginReceive(ReceiveData, null); // Inicia la recepción de datos
    }

    void Update()
    {
        // Actualiza la posición solo si ha llegado una nueva
        if (positionUpdated)
        {
            player.transform.position = receivedPosition;
            positionUpdated = false;
        }
    }

    private void ReceiveData(IAsyncResult result)
    {
        // Obtiene el mensaje recibido
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        byte[] receivedData = udpClient.EndReceive(result, ref remoteEndPoint);

        // Convertir de bytes a string y luego a posición
        string message = Encoding.UTF8.GetString(receivedData);
        string[] positionData = message.Split('|');

        // Parsear la posición
        if (positionData.Length == 3 &&
            float.TryParse(positionData[0], out float x) &&
            float.TryParse(positionData[1], out float y) &&
            float.TryParse(positionData[2], out float z))
        {
            receivedPosition = new Vector3(x, y, z);
            positionUpdated = true; // Marca que hemos recibido una nueva posición
        }

        // Continuar recibiendo datos
        udpClient.BeginReceive(ReceiveData, null);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
