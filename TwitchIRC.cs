using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Diagnostics;
using System;
using UnityEngine;

namespace TwitchIRC
{
    public delegate void MessageReceivedEvent(ChatMessage message);
    public class TwitchIRC : MonoBehaviour
    {
        public event MessageReceivedEvent OnMessageReceived;
        private string oauthToken;
        private string nickName;
        private string channelName;
        private string serverIP = "irc.chat.twitch.tv";
        private int serverPort = 6667;
        public void SetCredentials(string oauthToken, string nickName, string channelName)
        {
            this.oauthToken = oauthToken;
            this.nickName = nickName;
            this.channelName = channelName;
        }
        private string buffer = string.Empty;
        private Queue<string> outputQueue = new Queue<string>();
        private List<ChatMessage> chatBuffer = new List<ChatMessage>();
        private Thread inputThread;
        private Thread outputThread;
        public bool InitializeIRC()
        {
            enabled = true;
            TcpClient client = new TcpClient();
            client.Connect(serverIP, serverPort);
            if (!client.Connected)
            {
                Plugin.Log("Connection failed!");
                enabled = false;
                return false;
            }
            NetworkStream networkStream = client.GetStream();
            StreamReader inputStream = new StreamReader(networkStream);
            StreamWriter outputStream = new StreamWriter(networkStream);
            if(oauthToken != string.Empty)
                outputStream.WriteLine("PASS " + (oauthToken.ToLower().StartsWith("oauth:") ? oauthToken : "oauth:" + oauthToken));
            outputStream.WriteLine("NICK " + (oauthToken != string.Empty ? nickName : "justinfan" + UnityEngine.Random.Range(0, 99999).ToString().PadLeft(5, '0')));
            outputStream.Flush();
            outputThread = new Thread(() => OutputThread(outputStream));
            outputThread.Start();
            inputThread = new Thread(() => InputThread(inputStream, networkStream));
            inputThread.Start();
            return true;
        }
        private void OutputThread(TextWriter outputStream)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while(enabled)
            {
                lock(outputQueue)
                {
                    if(outputQueue.Count > 0)
                    {
                        if(stopWatch.ElapsedMilliseconds > 1750)
                        {
                            outputStream.WriteLine(outputQueue.Peek());
                            outputStream.Flush();
                            outputQueue.Dequeue();
                            stopWatch.Restart();
                        }
                    }
                }
            }
        }
        private void InputThread(TextReader inputStream, NetworkStream networkStream)
        {
            while (enabled)
            {
                if (!networkStream.DataAvailable)
                {
                    continue;
                }
                buffer = inputStream.ReadLine();
                if (buffer.Contains("PRIVMSG #"))
                {
                    lock (chatBuffer)
                    {
                        string username = buffer.Split('!')[1].Split('@')[0];
                        string message = buffer.Substring(buffer.IndexOf("PRIVMSG #") + "PRIVMSG #".Length + channelName.Length + 2);
                        string color = "#" + new System.Random(username.GetHashCode()).Next((int)Math.Pow(256, 3) - 1).ToString("X6");
                        chatBuffer.Add(new ChatMessage(username, message, color));
                    }
                }
                if (buffer.StartsWith("PING "))
                {
                    SendTwitchCommand(buffer.Replace("PING", "PONG"));
                }
                if(buffer.Split(' ')[1] == "001")
                {
                    SendTwitchCommand("JOIN #" + channelName);
                }
            }
        }
        public void SendTwitchMessage(string message)
        {
            outputQueue.Enqueue("PRIVMSG #" + channelName + " :" + message);
            OnMessageReceived?.Invoke(new ChatMessage("Sent Chat Message", message, "#E51212"));
        }
        public void SendTwitchCommand(string message)
        {
            outputQueue.Enqueue(message);
        }
        public void Update()
        {
            lock (chatBuffer)
            {
                if(chatBuffer.Count > 0)
                {
                    chatBuffer.ForEach((chatMessage) =>
                    {
                        OnMessageReceived?.Invoke(chatMessage);
                    });
                    chatBuffer.Clear();
                }
            }
        }
    }
}
