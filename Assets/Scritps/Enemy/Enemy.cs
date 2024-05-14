using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    enum AniState
    { 
        Idle,
        Sleep,
        Walk,
        Run,
        IdleCombat,
        Death,
        
        GetHit,
        Attack1,
        Attack2,
        Buff,
    }

    public float maxHealth = 100.0f;  // �ִ� ü�� 
    public float curHealth = 0.0f;  // ���� ü��

    public float bossTermTime = 0.1f;
    public float bossAniTime = 3.0f;

    public  SkinnedMeshRenderer meshRenderer;
    private Rigidbody rigid;
    private BoxCollider bossCollider;
    private Animator animator;

    public int aniStateValue = 0;

    private bool isState = true; // Anistate
    private bool isDead = false; // ����
    private bool isPatrol = false; // ����
    private bool isBattle = false; // ��Ʋ


    NavMeshAgent m_enemy = null;

    [SerializeField] Transform[] m_tWayPoints = null;
    int m_count = 0;

    float m_bossPatrolSpeed = 2.5f;
    float m_bosschaseSpeed = 5.0f;
    Transform m_target = null;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bossCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        curHealth = maxHealth;

        m_enemy = GetComponent<NavMeshAgent>();

        animator.SetInteger("AniState", (int)AniState.Sleep);

        InvokeRepeating("MoveToNextWayPoint", 0f, 1.5f);
    }

    private void Update()
    {
        if (m_target != null && isBattle == false)
        {
            //m_enemy.SetDestination(m_target.position);

            if (Vector3.Distance(m_target.position, this.transform.position) <= 8.0f)
            {
                m_enemy.acceleration = 0f;
                m_enemy.velocity = Vector3.zero;
                Debug.Log("�տ��ִ�");
                isBattle = true;
                isPatrol = false;
                animator.SetInteger("AniState", (int)AniState.IdleCombat);
            }
            else
            {
                m_enemy.SetDestination(m_target.position);
                isBattle = false;
                Debug.Log("�޸���");
            }
        }
    }

    public void SetTarget(Transform p_target)
    {
        CancelInvoke();
        isPatrol = true;
        m_target = p_target;
        animator.SetInteger("AniState", (int)AniState.Run);
        m_enemy.speed = m_bosschaseSpeed;
    }

    public void RemoveTarget()
    {
        m_target = null;
        isBattle = false;
        InvokeRepeating("MoveToNextWayPoint", 0f, 1.5f); // 0����, 1.5�ʸ��� ����
        Debug.Log("patrol");
    }

    void MoveToNextWayPoint()
    {
        if (m_target == null)
        {
            isPatrol = true;
            m_enemy.speed = m_bossPatrolSpeed;

            // ���� �ߴٸ� ������ �� �ִ´�
            if (m_enemy.velocity.sqrMagnitude >= 0.2f * 0.2f && m_enemy.remainingDistance <= 0.5f)
            {
                animator.SetInteger("AniState", (int)AniState.IdleCombat);
                Debug.LogWarning("����");
            }

            //�����Ѵٸ� ���� waypoint�� �Ѿ��
            if (m_enemy.velocity == Vector3.zero)
            {
                //animator.SetInteger("AniState", (int)AniState.IdleCombat);

                Debug.Log("���� �������� " + m_tWayPoints[m_count].name + " �Դϴ�");
                m_enemy.SetDestination(m_tWayPoints[m_count++].position);

                // �ٽ� �����δ�
                animator.SetInteger("AniState", (int)AniState.Walk);

                // ī���Ͱ� �ִ밡 �ȴٸ� �ٽ� ó����ġ���� �����̰� ����Ŭ
                if (m_count >= m_tWayPoints.Length)
                    m_count = 0;
            }
            else
            {
                // ���� �������� ���ϰ� �̵� �Ѵ�
                animator.SetInteger("AniState", (int)AniState.Walk);
            }
        }
    }

    // ���� ���� �ȱ�, �ٱ�, ���� ��� ��� �������� ���� ����
    IEnumerator SetAnimationState(AniState p_state)
    {
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        isState = false;
        aniStateValue = (int)p_state;
        animator.SetInteger("AniState", aniStateValue);
        yield return new WaitForSeconds(0.3f);
        isState = true;
    }

    // ���� ���� �ִϸ��̼� �ٽ� ���� �ִϸ��̼����� ���ư��� �κ� ���� ��� �����ϰ� ��������
    IEnumerator SetAnimationTrigger(AniState p_state)
    {
        isState = true;
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        //Debug.Log(state.ToString());
        animator.SetTrigger("do" + p_state.ToString());
        yield return new WaitForSeconds(1f);
        isState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(curHealth == maxHealth)
        {
        }
        StartCoroutine(AttackPattern());

        Debug.Log("hit target : "+other.name);
        if (other.name == "SwordCollier" && !isDead)
        {
            StartCoroutine(OnDamage());
            if(isState != true)
            {
                // ���� �����϶� ���� ������ ��Ʈ �ִ� ��� x
                StartCoroutine(SetAnimationTrigger(AniState.GetHit));
            }
        }
    }

    IEnumerator OnDamage()
    {
        curHealth -= 10.0f;
        meshRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            meshRenderer.material.color = Color.white;
        }
        else if (curHealth <= 0)
        {
            // �׾�����.
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        bossCollider.enabled = false;
        meshRenderer.material.color = Color.black;
        StartCoroutine(SetAnimationState(AniState.Death));
        yield return new WaitForSeconds(6f);
        Destroy(this.gameObject);
    }

    IEnumerator AttackPattern()
    {
        // �⺻ Idle ���¿��� 
        if (isDead != true && isPatrol == false && isBattle == true)
        {
            StartCoroutine(SetAnimationState(AniState.IdleCombat));
            yield return new WaitForSeconds(bossAniTime); // �⺻ �� �ð�

            int ranAction = Random.Range(0, 2);
            switch (ranAction)
            {
                case 0:
                case 1:
                    StartCoroutine(Attack1());
                    break;
                case 2:
                case 3:
                    StartCoroutine(Attack2());
                    break;
                case 4:
                    StartCoroutine(Buff());
                    break;
            }
        }
        else
        {
            StopCoroutine(AttackPattern());
            StopCoroutine(Attack1());
            StopCoroutine(Attack2());
            StopCoroutine(Buff());
        }
        
    }

    IEnumerator Attack1()
    {
        // RightAttack
        isState = true;
        StartCoroutine(SetAnimationTrigger(AniState.Attack1));
        yield return new WaitForSeconds(bossAniTime);
        isState = false;

        StartCoroutine(AttackPattern());
    }

    IEnumerator Attack2()
    {
        // leftAttack
        isState = false;
        StartCoroutine(SetAnimationTrigger(AniState.Attack2));
        yield return new WaitForSeconds(bossAniTime);
        isState = true;

        StartCoroutine(AttackPattern());
    }

    IEnumerator Buff()
    {
        // �ǰ� ���� ������ �ߵ��� ������ ����
        if(curHealth <= maxHealth/2)
        {
            isState = false;
            StartCoroutine(SetAnimationTrigger(AniState.Buff));
            yield return new WaitForSeconds(bossAniTime);
            isState = true;
        }

        StartCoroutine(AttackPattern());
    }

}
