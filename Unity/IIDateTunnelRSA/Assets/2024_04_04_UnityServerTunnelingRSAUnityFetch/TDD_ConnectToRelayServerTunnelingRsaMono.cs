using System.Collections;
using UnityEngine;

public class TDD_ConnectToRelayServerTunnelingRsaMono : MonoBehaviour
{
    public ConnectToRelayServerTunnelingRsaMono m_connection;

    public string m_time = "";
    void Start()
    {
        StartCoroutine(ConnectAndRun());
    }

    // Update is called once per frame
    IEnumerator ConnectAndRun()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            m_time= System.DateTime.UtcNow.ToString();
            if (m_connection.m_connectionEstablishedAndVerified) {

                m_connection.PushMessageText("Time Client: " + System.DateTime.UtcNow.ToString());
                yield return new WaitForSeconds(5);
                //generate 32 bytes of random data
                byte[] randomData = new byte[32];
                for (int i = 0; i < randomData.Length; i++)
                {
                    randomData[i] = (byte) Random.Range(0, 255);
                }


                m_connection.PushMessageBytes(randomData);
                yield return new WaitForSeconds(5);

            }
            yield return new WaitForSeconds(1);
        }
        
    }
}
