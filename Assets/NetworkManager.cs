using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour
{
    private TcpClient tcpClient;
    private TcpListener tcpListener;
    private UdpClient udpClient;
    public int udpPort = 2137;
    public int tcpPort = 2138;

    public bool inSession = false;

    public async Task<bool> Host()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 13);
        tcpListener = new TcpListener(ipEndPoint);
        try
        {
            tcpListener.Start();
            Debug.Log("Hosting");
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            Debug.Log("Connection request received");
            if (!inSession)
            {
                return await SetSession();
            }
            else
            {
                return await RejectSession();
            }
        }
        catch (SocketException e)
        {
            tcpClient = null;
            inSession = false;
            Host();
        }
        return false;
    }

    private async Task<bool> SetSession()
    {
        Debug.Log("Connecting");
        NetworkStream stream = tcpClient.GetStream();
        byte[] mess = new byte[1] { (byte)MessageType.ACCEPT };
        stream.Write(mess, 0, 1);
        inSession = true;
        Debug.Log("Session established");
        return true;
    }

    private async Task<bool> RejectSession()
    {
        Debug.Log("Rejecting");
        NetworkStream stream = tcpClient.GetStream();
        byte[] mess = new byte[1] { (byte)MessageType.REJECT };
        stream.Write(mess, 0, 1);
        tcpClient.Close();
        tcpClient = null;
        Debug.Log("Session rejected");
        return false;
    }

    public async Task<bool> Connect(IPEndPoint server)
    {
        try
        {
            tcpClient = new TcpClient();
            Debug.Log("Connecting to " + server.ToString());
            tcpClient.Connect(server);
            Debug.Log("Connected");
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            if (buffer[0] == (byte)MessageType.ACCEPT) return true;
        }

        catch (SocketException e)
        {
            Debug.Log(e.ToString());
            tcpClient = null;
            inSession = false;
        }
        Debug.Log("Connection to " + server.ToString() + " failed");
        return false;
    }

    enum MessageType : byte
    { 
        REQUEST = 0x00,
        ACCEPT = 0x01,
        REJECT = 0x02,
        MESSAGE = 0x03
    }

    enum ActionType : byte
    {
        MOVE = 0x00,
        ACTION = 0x01,
        HIT = 0x02,
        NONE = 0xFF
    }
}