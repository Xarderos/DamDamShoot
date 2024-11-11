using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    public GameObject player; // El objeto P1 (host)
    public int port = 7777;   // Puerto para enviar datos (asegúrate que el Cliente esté escuchando en este puerto)
    private UdpClient udpServer;

    void Start()
    {
        udpServer = new UdpClient();
    }

    void Update()
    {
        // Obtener la posición del jugador y convertirla en un string (ej. "x,y,z")
        Vector3 playerPosition = player.transform.position;
        string message = $"{playerPosition.x}|{playerPosition.y}|{playerPosition.z}";

        // Convertir el mensaje en bytes y enviarlo al cliente
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpServer.Send(data, data.Length, "192.168.1.100", port); // Cambia la IP si el cliente está en otro dispositivo
    }

    void OnApplicationQuit()
    {
        udpServer.Close();
    }
}
