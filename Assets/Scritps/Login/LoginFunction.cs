using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginFunction : MonoBehaviour
{
    [SerializeField]
    InputField LoginID_Text; // 입력된 PW 부분
    [SerializeField]
    InputField LoginPW_Text; // 입력된 PW 부분
    [SerializeField]
    GameObject errorImg;

    private NetWork_manager netWorkManager;

    // Start is called before the first frame update
    void Start()
    {
        netWorkManager = GameObject.FindObjectOfType<NetWork_manager>();

        errorImg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCLick_LoginButton()
    {

        if (LoginID_Text.text.Equals("test") || LoginID_Text.text.Equals("test2") || LoginID_Text.text.Equals("test3") || LoginID_Text.text.Equals("test4") || LoginID_Text.text.Equals("chlgusdyd") || LoginID_Text.text.Equals("wjswnsah") || LoginID_Text.text.Equals("tjdk"))
        {
            print("입력한 login_ID : " + LoginID_Text.text);

            //netWorkManager.Send_Test();
            
            LoginPacket Login_Packet = new LoginPacket();

            Login_Packet.id = netWorkManager.m_myID;
            //Login_Packet.userNum = LoginID_Text.text;

            netWorkManager.SendData(Login_Packet);
            //if (netWorkManager.m_tcpClient != null && netWorkManager.m_tcpClient.Connected)
            //    netWorkManager.m_tcpClient.GetStream().Write(ObjectToByte(Login_Packet));


            LoginID_Text.text = "";
            LoginPW_Text.text = "";

            return;
        }
        else
        {
            LoginID_Text.text = "";
            LoginPW_Text.text = "";

            StartCoroutine(errorMsg());
        }
    }

    IEnumerator errorMsg()
    {
        errorImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        errorImg.gameObject.SetActive(false);
    }


    public static byte[] ObjectToByte<T>(T structure)
    {
        int size = Marshal.SizeOf(structure);
        byte[] byteArray = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structure, ptr, false);
            Marshal.Copy(ptr, byteArray, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return byteArray;
    }

}
