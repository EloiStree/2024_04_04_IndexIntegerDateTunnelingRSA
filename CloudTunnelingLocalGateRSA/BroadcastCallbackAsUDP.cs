using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingLocalGateRSA
{

    public class BroadcastCallbackAsUDP
    {
        public static BroadcastCallbackAsUDP Instance = new BroadcastCallbackAsUDP();
        public Queue<byte[]> m_broadcastBytes = new Queue<byte[]>();
        public Queue<string> m_broadcastText = new Queue<string>();

        public List<TargetIp> m_ipPortUDP = new List<TargetIp>();

        public void AddUdpPort(int port)
        {
            AddIpTarget("127.0.0.1", port);
        }
        public void AddIpTarget(string ip, int port)
        {
            m_ipPortUDP.Add(new TargetIp(ip, port));
        }
        public void AddIpTarget(string ip)
        {
            m_ipPortUDP.Add(new TargetIp(ip));
        }


        public void Enqueue(byte[] bytes)
        {
            m_broadcastBytes.Enqueue(bytes);
        }
        public void Enqueue(string text)
        {
            m_broadcastText.Enqueue(text);
        }


        public class TargetIp
        {

            public IPEndPoint m_ipEndPoint;
            public TargetIp(string ip, int port)
            {
                m_ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }

            public TargetIp(string ip)
            {
                string[] t = ip.Split(':');
                if (t.Length == 2)
                {
                    m_ipEndPoint = new IPEndPoint(IPAddress.Parse(t[0]), int.Parse(t[1]));
                }

            }

        }
        public Task Start()
        {

            string ip127 = "127.0.0.1";
            while (true)
            {


                string text;
                byte[] bytes;

                while (m_broadcastBytes.Count > 0)
                {
                        bytes = m_broadcastBytes.Dequeue();
                    foreach (var ipPort in m_ipPortUDP)
                    {
                        Console.WriteLine($"Broadcasting {m_ipPortUDP.Count} to {ipPort.m_ipEndPoint.Address}:{ipPort.m_ipEndPoint.Port}");
                        SendUdpByte(bytes, in ipPort.m_ipEndPoint);

                    }
                }
                while (m_broadcastText.Count > 0)
                {
                        text = m_broadcastText.Dequeue();
                    foreach (var ipPort in m_ipPortUDP)
                    {
                        //SendUdpText(text, in ipPort.m_ipEndPoint);
                    }

                }
                Thread.Sleep(1);
            }
        }


        private void SendUdpByte(byte[] bytes, in IPEndPoint endPoint)
        {
            UdpClient udpClient = new UdpClient();
            try
            {
                udpClient.Send(bytes, bytes.Length, endPoint);
                Console.Write($"Sent bytes {endPoint.Address}:{endPoint.Port} | {bytes.Length.ToString()}");

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while sending the message: " + e.Message);
            }
            finally
            {
                udpClient.Close();
            }
        }
        private void SendUdpText(string bytes, in IPEndPoint endPoint)
        {

            byte[] sendBytes = Encoding.ASCII.GetBytes(bytes);
            SendUdpByte(sendBytes, endPoint);
        }

        public void TryToAddByLines(string[] lines)
        {
            foreach (string line in lines)
            {
                if (int.TryParse(line, out int port))
                {
                    AddUdpPort(port);
                }
                else
                {
                    string[] t = line.Split(':');
                    if (t.Length >= 2 && int.TryParse(t[1], out int port2))
                    {
                        Console.WriteLine("ADD LINE: " + line);
                        Console.WriteLine($"ADD ({t[0]})  ({t[1]}) ({port2}) ");
                        try
                        {
                            AddIpTarget(t[0], port2);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);

                        }
                    }
                }

            }
        }
    }
}
