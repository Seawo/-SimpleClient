using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    NavMeshAgent m_enemy = null;

    [SerializeField] Transform[] m_tWayPoints = null;
    int m_count = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_enemy = GetComponent<NavMeshAgent>();
        InvokeRepeating("MoveToNextWayPoint", 0f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveToNextWayPoint()
    {
        // 도착한다면 다음 waypoint로 넘어간다
        m_enemy.SetDestination(m_tWayPoints[m_count++].position);

        // 카운터가 최대가 된다면 다시 처음위치에서 움직이게 사이클
        if (m_count >= m_tWayPoints.Length)
            m_count = 0;
    }
}
