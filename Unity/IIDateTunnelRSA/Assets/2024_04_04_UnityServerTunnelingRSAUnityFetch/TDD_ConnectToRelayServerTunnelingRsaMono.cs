using System;
using System.Collections;
using UnityEngine;

public class TDD_ConnectToRelayServerTunnelingRsaMono : MonoBehaviour
{
    public ConnectToRelayServerTunnelingRsaMono m_connection;

    public string m_time = "";

    public int m_intIndex = 0;
    public int [] m_intValue = new int[] { 0,42,69,2501,314,31418};
    public ulong m_utcDateMilliseconds = 0;

    void Start()
    {
        StartCoroutine(ConnectAndRun());
    }

    IEnumerator ConnectAndRun()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            m_time= System.DateTime.UtcNow.ToString();
            if (m_connection.m_connectionEstablishedAndVerified) {

                m_connection.PushMessageText("Time Client: " + System.DateTime.UtcNow.ToString());
                yield return new WaitForSeconds(1);
                //generate 32 bytes of random data
                byte[] randomData = new byte[16];
                for (int i = 0; i < randomData.Length; i++)
                {
                    m_utcDateMilliseconds = (ulong) DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                    BitConverter.GetBytes(m_intIndex).CopyTo(randomData, 0);
                    BitConverter.GetBytes(GetRandomInteger()).CopyTo(randomData, 4);
                    BitConverter.GetBytes(m_utcDateMilliseconds).CopyTo(randomData, 8);
                }


                m_connection.PushMessageBytes(randomData);
                yield return new WaitForSeconds(1);

            }
            yield return new WaitForSeconds(1);
        }
        
    }

    private int GetRandomInteger()
    {
        if(m_intValue.Length > 0)
            return m_intValue[UnityEngine.Random.Range(0, m_intValue.Length)];
        return 0;
    }
}
