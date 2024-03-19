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
    Text LoginID_Text; // 입력된 ID 부분
    [SerializeField]
    InputField LoginPW_Text; // 입력된 PW 부분
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
        m_tcpClient.BeginConnect(m_ipAddr, m_port, requestCall, null); // 비동기식 대기 상태
    }

    private void requestCall(System.IAsyncResult ar)
    {
        print("연결");
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

                print("Encoding.UTF8.GetString(data) 전 : " + Encoding.UTF8.GetString(data));
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
            // 씬 불러오기
            LoadingScenesController.LoadScene("Environment_Free");
        }
    }

    public void Click_Logine()
    {
        print("입력한 login_ID : " + LoginID_Text.text);

        if (LoginID_Text.text.Equals("test") || LoginID_Text.text.Equals("test2") || LoginID_Text.text.Equals( "test3") || LoginID_Text.text.Equals("test4") || LoginID_Text.text.Equals("chlgusdyd") || LoginID_Text.text.Equals("wjswnsah") || LoginID_Text.text.Equals("tjdk"))
        {
            // 보내기
            // 서버의 전송하기
            var data = Encoding.UTF8.GetBytes(LoginID_Text.text);

            if (m_tcpClient != null && m_tcpClient.Connected)
                m_tcpClient.GetStream().Write(data, 0, data.Length);

            // 내용 비우기
            LoginID_Text.text = "";
            LoginPW_Text.text = "";

            return;
        }
        else
        {
            print("ID or PW 제대로 입력해 주세요");

            LoginID_Text.text = "";
            LoginPW_Text.text = "";

            StartCoroutine(errorMsg());
            // 오류 메세지

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
