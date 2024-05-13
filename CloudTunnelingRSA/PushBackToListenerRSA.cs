using CloudTunnelingRSA.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebSocketServer;

namespace CloudTunnelingRSA
{
    public class PushBackToListenerRSA
    {
        public static PushBackToListenerRSA Instance = new PushBackToListenerRSA();



        public void PushBackBytes(RsaPublicKeyRef rsa, byte[] pushBack) {

            if (rsa == null)
                return;
            if(pushBack == null)
                return;
            if (pushBack.Length > 20) 
                return;
            
            if (!AppConfig.Configuration.m_allowToListenAtChanged)
                return;

            DicoOwnerListener.
                Instance.GetListeners(rsa.GetObjectMemoryId(),
                out List<OwnerListener> listeners);

            
            if(listeners==null)
                return;
            foreach (var item in listeners)
            {
                if (item != null && item.m_connectionForCallback != null
                    && item.m_connectionForCallback.WebSocket != null &&
                    item.m_connectionForCallback.WebSocket.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    item.PushBack(pushBack);
                }            
            }


        }
    }
}
