using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameManager();
        }
        return instance;
    }

    Scene currentScene;

    NetWork_manager network_manager;

    public GameObject playerPrefab;
    GameObject m_myObject;

    PosPacket pos_packet;

    // Start is called before the first frame update
    void Start()
    {
        network_manager = GameObject.FindObjectOfType<NetWork_manager>();

        // ingame 시작시 오브젝트를 리스트에 넣고 생성한다
        // 현재 활성화된 씬의 정보 가져오기
        currentScene = SceneManager.GetActiveScene();

        pos_packet = new PosPacket();

        var player = Instantiate(playerPrefab);
        
        m_myObject = player.transform.GetChild(0).gameObject;

        pos_packet.id = network_manager.m_myID;
        print("packet id : " + pos_packet.id);
        network_manager.m_playerList.Add(pos_packet.id, player);
    }

    float tick = 0;
    // Update is called once per frame
    void Update()
    {
        tick -= Time.deltaTime;

        if (tick <= 0 && SceneManager.GetActiveScene().name == "Environment_Free" && pos_packet.id != 0)
        {

            PosPacket pos_packet = new PosPacket();
            pos_packet.id = network_manager.m_myID;

            pos_packet.pos = m_myObject.transform.position;
            pos_packet.rot = m_myObject.transform.rotation;

            network_manager.SendData(pos_packet);
            tick = 0.1f;
        }
    }
}
