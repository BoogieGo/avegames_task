using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/// <summary>
/// Простенький менеджер, который устанавливает tcp-соединение с пользователями, обменивается сообщениями...
/// </summary>
public class NetworkManager : MonoBehaviour
{
    //синглтон
    public static NetworkManager I { get { return i; } private set { i = value; } }
    private static NetworkManager i;

    //ребята, которые наблюдают за нами
    private List<IPAddress> listeners = new List<IPAddress>();

    //сообщения, которые надо отправить
    public Queue<string> ToRemote = new Queue<string>();

    //сообщения, котоыре надо обработать
    public Queue<string> received = new Queue<string>();

    //это надо для пересылки текстур
    private int textureUpdates = 0;
    private bool needUpdateTexture = false;

    //это для прослушки
    public Socket Sock;
    public TcpListener Listener;

    void Awake()
    {
        i = this;
    }

    //пишем в консоль наш ip
    private void Start()
    {
        Debug.Log(Dns.GetHostByName(Dns.GetHostName()).AddressList[0]);
    }

    //сообщения отправляем не чаще раза в .2 секунды
    private float lastSend;
    private void Update()
    {
        if (listeners.Count == 0)
            ToRemote.Clear();
        else
        {
            if (Time.time - lastSend > .2f && ToRemote.Count > 0)
            {
                lastSend = Time.time;
                new Thread(() => SendGameMessage(ToRemote.Dequeue())).Start();
            }
        }

        if (needUpdateTexture)
            GradientGenerator.SendTextures();
        needUpdateTexture = false;

        if (received.Count > 0)
            Handler(received.Dequeue());
    }
    
    //обработчик сообщений
    void Handler(string msg)
    {
        string[] answ = msg.Split(' ');
        if (answ[0] == "create")
        {
            ViewManager.I.CreateOne(int.Parse(answ[1]), int.Parse(answ[2]), float.Parse(answ[3])
                , float.Parse(answ[4]), float.Parse(answ[5]));
        }
        else if (answ[0] == "destroy")
        {
            ViewManager.I.DestroyOne(int.Parse(answ[1]), int.Parse(answ[2]));
        }
        else if (answ[0] == "texture")
        {
            int size = int.Parse(answ[1]) < 2 ? 32 : int.Parse(answ[1]) < 4 ? 64 : int.Parse(answ[1]) < 128 ? 128 : 256;
            GradientGenerator.RemouteTextures[int.Parse(answ[1])] = GradientGenerator.Generate(size, 
                new Color(float.Parse(answ[2]), float.Parse(answ[3]), float.Parse(answ[4])), 
                new Color(float.Parse(answ[5]), float.Parse(answ[6]), float.Parse(answ[7])));
            textureUpdates++;

            if (textureUpdates %8 == 0) GC.Collect();
        }
    }
    

    public void StartTcpServer()
    {
        new Thread(() => StartListener()).Start();
    }

    //пытаемся отправить кому-то сообщение о том, что хотим понаблюдать за его игрой
    //если прокатывает - в ответ нам начнут приходить сообщения о состоянии игры
    public void TryConnect(string ip, string port)
    {
        try
        {
            TcpClient tcpclnt = new TcpClient();
            tcpclnt.Connect(IPAddress.Parse(ip), int.Parse(port));
            String str = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
            Stream stm = tcpclnt.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str);
            stm.Write(ba, 0, ba.Length);
            byte[] bb = new byte[255];
            int k = stm.Read(bb, 0, 255);

            string an = "";
            for (int i = 0; i < k; i++)
                an += Convert.ToChar(bb[i]);

            stm.Close();
            tcpclnt.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    /// <summary>
    /// Отсюда игрок рассылает всем наблюдателям сообщения о происходящем в игре
    /// </summary>
    private void SendGameMessage(string msg)
    {
        foreach (IPAddress ipAddress in listeners)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect(ipAddress, 8001);
                String str = msg;
                Stream stm = tcpclnt.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);
                stm.Write(ba, 0, ba.Length);

                byte[] bb = new byte[16000];
                int k = stm.Read(bb, 0, 16000);
                string t = "";
                for (int i = 0; i < k; i++)
                    t += Convert.ToChar(bb[i]);
                
                stm.Close();
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                listeners.Remove(ipAddress);
            }
        }
    }

    /// <summary>
    /// Запускаем прослушку
    /// </summary>
    private void StartListener()
    {
        try
        {
            IPAddress ipAd = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
            Listener = new TcpListener(ipAd, 8001);
            Listener.Start();

            while (true)
            {
                Sock = Listener.AcceptSocket();
                byte[] b = new byte[255];
                int k = Sock.Receive(b);
                string text = "";
                for (int i = 0; i < k; i++)
                    text += Convert.ToChar(b[i]);
                received.Enqueue(text);
                
                if (text.Split('.').Length == 4)
                {
                    try
                    {
                        IPAddress ip = IPAddress.Parse(text);
                            if (!listeners.Contains(ip)) listeners.Add(ip);
                        needUpdateTexture = true;
                    }
                    catch (Exception e)
                    {
                        
                    }
                }

                ASCIIEncoding asen = new ASCIIEncoding();
                Sock.Send(asen.GetBytes("ok"));
                Sock.Close();
            }
            
            Listener.Stop();
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    
    /// <summary>
    /// Если вдруг чё - закрываем соединение
    /// </summary>
    void OnDisable()
    {
        if (Sock != null && Sock.Connected) Sock.Disconnect(false);
        if (Listener != null) Listener.Stop();
    }

    void OnDestroy()
    {
        if (Sock != null && Sock.Connected) Sock.Disconnect(false);
        if (Listener != null) Listener.Stop();
    }

    void OnApplicationExit()
    {
        if (Sock != null && Sock.Connected) Sock.Disconnect(false);
        if (Listener != null) Listener.Stop();
    }
}
