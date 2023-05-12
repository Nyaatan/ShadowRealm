using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System;

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
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, tcpPort);
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

    public async Task<bool> SendWorldData(int data)
    {
        Debug.Log("SEND");
        Debug.Log(data);
        NetworkStream stream = tcpClient.GetStream();
        byte[] message = new byte[6];
        int[] seed = new int[1] { data };
        Buffer.BlockCopy(seed, 0, message, 2, 4);
        message[0] = (byte)MessageType.MESSAGE;
        message[1] = (byte)ActionType.INFO;
        stream.Write(message, 0, 6);
        return true;
    }

    public async Task<int> ReceiveWorldData()
    {
        try
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[6];
            stream.Read(buffer, 0, 6);
            if(buffer[0] == (byte)MessageType.MESSAGE && buffer[1] == (byte)ActionType.INFO)
            {
                int[] seed = new int[1];
                Buffer.BlockCopy(buffer, 2, seed, 0, 4);
                Debug.Log("REC");
                Debug.Log(seed[0]);
                return seed[0];
            }
            else
            {
                throw new SocketException();
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
            return -2137;
        }

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
        INFO = 0x69,
        NONE = 0xFF
    }
}
