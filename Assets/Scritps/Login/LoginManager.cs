using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.Assertions.Must;
using System;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    string m_ipAddr = "127.0.0.1";
    [SerializeField]
    int m_port = 12345;

    [SerializeField]
    Text LoginID_Text; // �Էµ� ID �κ�
    [SerializeField]
    InputField LoginPW_Text; // �Էµ� PW �κ�
    [SerializeField]
    GameObject errorImg;
   
    public instance_NickName nickname_instance;
    TcpClient m_tcpClient;
    string m_sendID;

    bool Isload = false;

    void Start()
    {
        errorImg.gameObject.SetActive(false);
        m_tcpClient = new TcpClient();
        m_tcpClient.BeginConnect(m_ipAddr, m_port, requestCall, null); // �񵿱�� ��� ����
    }

    private void requestCall(System.IAsyncResult ar)
    {
        print("����");
        byte[] buf = new byte[512];
        m_tcpClient.GetStream().BeginRead(buf, 0, buf.Length, requestCallTCP, buf);
    }

    private void requestCallTCP(System.IAsyncResult ar)
    {
        try
        {
            var byteRead = m_tcpClient.GetStream().EndRead(ar);

            if (byteRead > 0)
            {
                byte[] data = (byte[])ar.AsyncState;

                print("Encoding.UTF8.GetString(data) �� : " + Encoding.UTF8.GetString(data));
                nickname_instance.charater_nicName = Encoding.UTF8.GetString(data);
                Isload = true;

            }
            else
            {
                m_tcpClient.GetStream().Close();
                m_tcpClient.Close();
                m_tcpClient = null;
            }
        }
        catch (SocketException e)
        {
        }
    }

    void Update()
    {
        if (Isload == true)
        {
            // �� �ҷ�����
            LoadingScenesController.LoadScene("Environment_Free");
        }
    }

    public void Click_Logine()
    {
        print("�Է��� login_ID : " + LoginID_Text.text);

        if (LoginID_Text.text.Equals("test") || LoginID_Text.text.Equals("test2") || LoginID_Text.text.Equals( "test3") || LoginID_Text.text.Equals("test4") || LoginID_Text.text.Equals("chlgusdyd") || LoginID_Text.text.Equals("wjswnsah") || LoginID_Text.text.Equals("tjdk"))
        {
            // ������
            // ������ �����ϱ�
            var data = Encoding.UTF8.GetBytes(LoginID_Text.text);

            if (m_tcpClient != null && m_tcpClient.Connected)
                m_tcpClient.GetStream().Write(data, 0, data.Length);

            // ���� ����
            LoginID_Text.text = "";
            LoginPW_Text.text = "";

            return;
        }
        else
        {
            print("ID or PW ����� �Է��� �ּ���");

            LoginID_Text.text = "";
            LoginPW_Text.text = "";

            StartCoroutine(errorMsg());
            // ���� �޼���

            return;
        }
    }

    IEnumerator errorMsg()
    {
        errorImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        errorImg.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (m_tcpClient != null)
            m_tcpClient.Close();
    } 
}
