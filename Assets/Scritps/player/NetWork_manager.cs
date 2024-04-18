using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.VisualScripting;

//using UnityEditor.SearchService;
//using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using static OtherPlayerAnimator;


enum GAME_PACKET
{
    GET_ID,
    POS,
    EXIT_USER,
    LOGIN,
    ANISTATE,
    TEST,
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class BasePacket
{
    public ushort packet_len;
    public ushort packet_id;
};


// #pragma pack(push, 1)
// ����ü / Ŭ���� �� �е��� �����ϴ� �� ��� �˴ϴ�.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Get_IDPacket : BasePacket
{
    public Get_IDPacket()
    {
        packet_id = (ushort)GAME_PACKET.GET_ID;
        packet_len = (ushort)Marshal.SizeOf(typeof(Get_IDPacket));
    }
    public int          id;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PosPacket : BasePacket
{
    public PosPacket()
    {
        packet_id = (ushort)GAME_PACKET.POS;
        packet_len = (ushort)(Marshal.SizeOf(typeof(PosPacket)));
    }

    public int          id;
    public float        horizontal;
    public float        vertical;
    public Vector3      pos;
    public Quaternion   rot;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ExitPacket : BasePacket
{
    public ExitPacket()
    {
        packet_id = (ushort)GAME_PACKET.EXIT_USER;
        packet_len = (ushort)(Marshal.SizeOf(typeof(ExitPacket)));
    }

    public int          id = 0;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class LoginPacket : BasePacket
{
    public LoginPacket()
    {
        packet_id = (ushort)GAME_PACKET.LOGIN;
        packet_len = (ushort)Marshal.SizeOf(typeof(LoginPacket));
    }
    public int id;
    public int checkNum;
    // ���̵� �� ( ���߿� ) 
    //public char[]   userNum;
    //public string   userNum;
    // ���??
};
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class AniStatePacket : BasePacket
{
    public AniStatePacket()
    {
        packet_id = (ushort)GAME_PACKET.ANISTATE;
        packet_len = (ushort)Marshal.SizeOf(typeof(AniStatePacket));
    }
    public int id;
    public int checkAniNum;
};
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class TestPacket : BasePacket
{
    public TestPacket()
    {
        packet_id = (ushort)GAME_PACKET.TEST;
        packet_len = (ushort)(Marshal.SizeOf(typeof(TestPacket)));
    }

    public int id = 0;
    //public int test = 1004;
    public string test = "";
    //public byte[] test = null;
    //public char[] test;
};

public class NetWork_manager : MonoBehaviour
{
    private static NetWork_manager instance = null;
    public static NetWork_manager GetInstance()
    {
        if (instance == null)
        {
            instance = new NetWork_manager();
        }
        return instance;
    }

    public string m_ipAddr = "127.0.0.1";
    public int m_port = 12345;

    public GameObject playerPrefab;

    [HideInInspector]
    public TcpClient m_tcpClient;

    Queue<byte> m_streamBuffer = new Queue<byte>();
    Queue<BasePacket> m_msgQueue = new Queue<BasePacket>();

    [HideInInspector]
    public int m_myID = 0;

    [HideInInspector]
    public Dictionary<int, GameObject> m_playerList = new Dictionary<int, GameObject>();

    OtherPlayerAnimator otherPlayerAni;

    // Start is called before the first frame update
    void Start()
    {
        m_tcpClient = new TcpClient();
        m_tcpClient.BeginConnect(m_ipAddr, m_port, ConnectCallBack, null);
        // BeginConnect : �񵿱� ����Ʈ�� �񵿽����� ������ ���� �Ѵ�
        // ConnectCallBack : callback �Լ��� ����� �� �Լ��� ȣ���Ѵ�
        // null ����� ������ ������ �����ϱ� null
    }

    public void SendData<T>(T packet)
    {
        if (m_tcpClient != null && m_tcpClient.Connected)
        {
            m_tcpClient.GetStream().Write(ObjectToByte(packet));
        }
    }

    private void ConnectCallBack(IAsyncResult ar)
    // IAsyncResult��.NET �����ӿ�ũ���� �񵿱� �۾��� ���¿� ����� ǥ���ϱ� ���� �������̽�
    {
        m_tcpClient.EndConnect(ar);

        // packet send
        Get_IDPacket packet = new Get_IDPacket();
        packet.id = 9900;

        m_tcpClient.GetStream().Write(ObjectToByte(packet));
        // GetStream().Write : ( ������ ������ ) 

        byte[] buf = new byte[512];
        m_tcpClient.GetStream().BeginRead(buf, 0, buf.Length, ReadCallBack, buf);
        // ���� ���
    }

    public void ReadCallBack(IAsyncResult ar)
    {

        var byteRead = m_tcpClient.GetStream().EndRead(ar);

        if (byteRead > 0)
        {
            byte[] buf = (byte[])ar.AsyncState;

            //  ���ۿ� �ֱ�
            for (int i = 0; i < byteRead; i++)
            {
                m_streamBuffer.Enqueue(buf[i]);
            }

            //  ���ۿ� ����� ���� ���̽���Ŷ���� ũ��?
            if (m_streamBuffer.Count > Marshal.SizeOf(typeof(BasePacket)))
            {
                var basePacket = ByteToObject<BasePacket>(m_streamBuffer.ToArray());
                GAME_PACKET type = (GAME_PACKET)basePacket.packet_id;

                // ������ ��Ŷ
                if (m_streamBuffer.Count >= basePacket.packet_len)
                {
                    Queue<byte> pack = new Queue<byte>();


                    // ��Ŷ ������ �Űܴ��
                    for (int i = 0; i < basePacket.packet_len; i++)
                    {
                        pack.Enqueue(m_streamBuffer.Dequeue());
                    }

                    switch (type)
                    {
                        case GAME_PACKET.GET_ID:
                            m_msgQueue.Enqueue(ByteToObject<Get_IDPacket>(pack.ToArray()));
                            break;
                        case GAME_PACKET.EXIT_USER:
                            m_msgQueue.Enqueue(ByteToObject<ExitPacket>(pack.ToArray()));
                            break;
                        case GAME_PACKET.POS:
                            m_msgQueue.Enqueue(ByteToObject<PosPacket>(pack.ToArray()));
                            break;
                        case GAME_PACKET.LOGIN:
                            m_msgQueue.Enqueue(ByteToObject<LoginPacket>(pack.ToArray()));
                            break;
                        case GAME_PACKET.ANISTATE:
                            m_msgQueue.Enqueue(ByteToObject<AniStatePacket>(pack.ToArray()));
                            break;
                        case GAME_PACKET.TEST:
                            m_msgQueue.Enqueue(ByteToObject<TestPacket>(pack.ToArray()));
                            break;
                    }
                }
            }

            //  ���� ���
            byte[] newBuf = new byte[512];
            m_tcpClient.GetStream().BeginRead(newBuf, 0, newBuf.Length, ReadCallBack, newBuf);
        }
        else
        {
            m_tcpClient.Close();
            m_tcpClient = null;
        }
    }

    float tick = 0;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TestPacket packet = new TestPacket();
        //    packet.id = m_myID;
        //    //packet.test = "test";
        //    //packet.test = Encoding.UTF8.GetBytes("test");

        //    SendData( packet );
        //}

        if (m_msgQueue.Count > 0)
        {
            var basePack = m_msgQueue.Dequeue();

            switch ((GAME_PACKET)basePack.packet_id)
            {
                case GAME_PACKET.GET_ID:
                    {
                        var getPack = (Get_IDPacket)basePack;

                        m_myID = getPack.id;

                        print("GET_ID : " + getPack.id);
                    }
                    break;
                case GAME_PACKET.POS:
                    {
                        var getPack = (PosPacket)basePack;
                        if(SceneManager.GetActiveScene().name == "Environment_Free")
                        {
                            if (m_playerList.ContainsKey(getPack.id) == false)
                            {
                                //   ����
                                var player = Instantiate(playerPrefab);
                            
                                player.name = "Player" + getPack.id;
                                m_playerList.Add(getPack.id, player);

                                //otherPlayerAni = player.GetComponent<PlayerAnimator>();
                            }
                            if (getPack.id != m_myID)
                            {
                                m_playerList[getPack.id].transform.position = getPack.pos;
                                m_playerList[getPack.id].transform.rotation = getPack.rot;

                                // �ִϸ��̼� ���� �ִϸ��̼�
                                //otherPlayerAni.OnMovement(getPack.horizontal, getPack.vertical);
                                // ���� : update�� ��ġ�� ȸ������ �ǽð����� �����鼭 �ִϸ��̼��� ���� ������ �̷���
                                // �ذ��� : Ư�� �ִϸ��̼��� ȣ���ϴ� ������ ä��
                                // 1. Ư�� Packet���·� ���� ���� �̸ֹ��̼� ���¸� ��Ÿ���� packet�� ������ ����
                                 
                                //otherPlayerAni.OnRun();
                            }
                        }
                    }
                    break;
                case GAME_PACKET.EXIT_USER:
                    {
                        var getPack = (ExitPacket)basePack;
                        if (m_playerList.ContainsKey(getPack.id) == true)
                        {
                            Destroy(m_playerList[getPack.id]);
                            m_playerList.Remove(getPack.id);
                        }
                    }
                    break;
                case GAME_PACKET.LOGIN:
                    {
                        var getPack = (LoginPacket)basePack;
                        print("Login id : " + getPack.id);
                        print("code : " + getPack.checkNum);

                        LoadingScenesController.LoadScene("Environment_Free");
                        //m_userNameName = getPack.userNum.ToString();
                    }
                    break;
                case GAME_PACKET.ANISTATE:
                    {
                        var getPack = (AniStatePacket)basePack;

                        Debug.Log("packetId : " + getPack.id);
                        Debug.Log("Anistate : " + getPack.checkAniNum);

                        otherPlayerAni = m_playerList[(int)getPack.id].GetComponent<OtherPlayerAnimator>();
                        
                        if(otherPlayerAni != null)
                        {
                            if(getPack.checkAniNum == (int)AniState.Idle)
                            {
                                StartCoroutine(otherPlayerAni.SetAnimationState(AniState.Idle));
                            }
                            else if(getPack.checkAniNum == (int)AniState.Run)
                            {
                                StartCoroutine(otherPlayerAni.SetAnimationState(AniState.Run));
                            }
                            else if(getPack.checkAniNum == (int)AniState.Attack)
                            {
                                StartCoroutine(otherPlayerAni.SetAnimationTrigger(AniState.Attack));
                            }
                            else if(getPack.checkAniNum == (int)AniState.Jump)
                            {
                                StartCoroutine(otherPlayerAni.SetAnimationTrigger(AniState.Jump));
                            }
                        }

                        //if(getPack.checkAniNum < 3)
                        //{
                        //    StartCoroutine(otherPlayerAni.SetAnimationState(getPack.checkAniNum));
                        //}
                        //else if(getPack.checkAniNum >= 3)
                        //{
                            
                            
                        //}
                    }
                    break;
                case GAME_PACKET.TEST:
                    {
                        var getPack = (TestPacket)basePack;
                        print("Test id : " + getPack.id);
                        //print("test : " + getPack.test);
                    }
                    break;
            }
        }
    }


    // ������ ����
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
    public static T ByteToObject<T>(byte[] buffer)
    {
        T structure;

        int size = Marshal.SizeOf(typeof(T));

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(buffer, 0, ptr, size);
            structure = Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return structure;
    }

    
}

