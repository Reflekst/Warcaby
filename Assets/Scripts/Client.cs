using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Client : MonoBehaviour
{
    public bool isFirst = false;
    private bool socketReady;
    private TcpClient socket;
    byte[] _buffer = new byte[5];
    int BytesReceived = 0;
    private string message = "";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        this.socket = new TcpClient();
    }
    public bool ConnectToServer(string host, int port)
    {
        
        try
        {
            if (!socket.Connected)
            {
                socket.Connect(host, port);
                socket.GetStream().BeginRead(_buffer, 0, _buffer.Length, Server_MessageRecived, null);

            }
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error " + e.Message);
        }
        return socket.Connected;
    }
    //public bool ConnectToServer_old(string host, int port)
    //{
    //    if (socketReady) return false;

    //    try
    //    {
    //        if (!socket.Connected)
    //        {
    //            socket.Connect(host, port);
    //            //socket.GetStream().BeginRead(_buffer, 0, _buffer.Length, Server_MessageRecived, null);
    //            stream = socket.GetStream();
    //            writer = new StreamWriter(stream);
    //            reader = new StreamReader(stream);
    //            socketReady = true;
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Socket error " + e.Message);
    //    }
    //    return socketReady;
    //}
    private void Update()
    {
        if (socketReady)
        {
            socketReady = false;
            OnIncomingData(message);
        }
    }
    public void Send(byte[] data)
    {
        Debug.Log("Send: " + data.ToString());
        socket.GetStream().Write(data, 0, data.Length);
    }

    private void OnIncomingData(string data)
    {
        Debug.Log("Client: " + data);
        char[] aData = data.ToCharArray();

        switch (aData[0])
        {
            case 'F':
                isFirst = true;
                GameMenager.Instance.StartGame();
                break;
            case 'D':
                GameMenager.Instance.StartGame();
                break;
            case 'C':
                GameMenager.Instance.TryMove(aData);
                break;
        }

    }

    private void Server_MessageRecived(IAsyncResult ar)
    {
        if (ar.IsCompleted)
        {
            var msg = new CheesServerData();

            try
            {
                if (!socket.Connected)
                {
                    throw new Exception("Połączenie z serwerem zostało zakończone");
                }
                var bytesIN = socket.GetStream().EndRead(ar);
                if (bytesIN == 0) throw new Exception("Serwer zerwał połączenie");

                if (bytesIN > 0)
                {
                    message = DataChanger.BytesToString(_buffer, false);
                    socketReady = true;

                }

                this.BytesReceived += bytesIN; // zwiekszamy liczbe odczytanych bytow
                Array.Clear(_buffer, 0, _buffer.Length);
                socket.GetStream().BeginRead(_buffer, 0, _buffer.Length, Server_MessageRecived, null);

                if (this.BytesReceived >= DataChanger.MaxCommandLength)
                {
                    this.BytesReceived = 0;
                }//zerujemy bity
            }
            catch (Exception ex)
            {
                this.DisconnectClient();
            }
        }
    }
    private void DisconnectClient()
    {
        this.socket.Dispose();
        this.socket = new TcpClient();
    }
}