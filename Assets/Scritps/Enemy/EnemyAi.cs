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
        // �����Ѵٸ� ���� waypoint�� �Ѿ��
        m_enemy.SetDestination(m_tWayPoints[m_count++].position);

        // ī���Ͱ� �ִ밡 �ȴٸ� �ٽ� ó����ġ���� �����̰� ����Ŭ
        if (m_count >= m_tWayPoints.Length)
            m_count = 0;
    }
}
