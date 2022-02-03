using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class YesNoClient : Singleton<YesNoClient>
{
    int PORT = 1235;

    TcpClient tcpClient;
    NetworkStream networkStream;
    StreamWriter streamWriter;
    StreamReader streamReader;
    bool socketReady = false;

    byte[] sendByte;

    public void Open ()
    {
        try
        {
            tcpClient = new TcpClient(REEL.D2E.D2EConstants.SERVER_IP, PORT);
            networkStream = tcpClient.GetStream();
            streamWriter = new StreamWriter(networkStream);
            streamReader = new StreamReader(networkStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    public void Close ()
    {
        if (!socketReady)
            return;
        streamWriter.Close();
        streamReader.Close();
        tcpClient.Close();
        socketReady = false;
    }

    public void Write (string theLine)
    {
        if (!socketReady)
            return;
        String tmpString = theLine + "\r\n";
        streamWriter.Write(tmpString);
        streamWriter.Flush();
    }

    public string Read ()
    {
        if (!socketReady)
            return "";
        if (networkStream.DataAvailable)
            return streamReader.ReadLine();
        return "";
    }
}
