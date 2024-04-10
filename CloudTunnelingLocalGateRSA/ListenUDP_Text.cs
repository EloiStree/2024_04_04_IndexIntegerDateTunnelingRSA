using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingLocalGateRSA
{
    internal class ListenUDP_Text
    {
        //Create a Task thread that listen to udp port 5000
        public static async Task ListenUDP(int port=5001)
        {
            using (System.Net.Sockets.UdpClient listener = new System.Net.Sockets.UdpClient(port))
            {
                System.Net.IPEndPoint groupEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
                Task task = Task.Run(() =>
                {
                    while (true)
                    {
                        byte[] bytes = listener.Receive(ref groupEP);
                        string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                        Console.WriteLine("Received{0}: {1}", bytes.Length, BitConverter.ToString(bytes));
                        WebSocketClientToServerRSA.SentToServerAsUTF8(message);
                    }
                });
                await task;
            }
        }
    }
}
