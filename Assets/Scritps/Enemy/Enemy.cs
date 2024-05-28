using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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

    public Slider m_bosshpslider;
    public Slider m_bosssBackhpslider;

    public GameObject DamageUI;
    public DamageText c_DamageUI;

    [SerializeField]
    Camera m_cam;

    [SerializeField]
    private float maxHealth = 3000.0f;  // 최대체력
    public float curHealth = 0.0f;  // 현재체력

    public float bossTermTime = 0.1f;
    public float bossAniTime = 3.0f;

    public  SkinnedMeshRenderer meshRenderer;
    private Rigidbody rigid;
    private BoxCollider bossCollider;
    private Animator animator;
    public Transform headPos;

    public int aniStateValue = 0;


    [SerializeField]
    NavMeshAgent m_TargetPlayer = null;

    [SerializeField] Transform[] m_tWayPoints = null;
    int m_count = 0;

    float m_bossPatrolSpeed = 2.5f;
    float m_bosschaseSpeed = 5.0f;
    Transform m_target = null;

    private bool isState = true; // Anistate
    public bool isDead = false; // 죽음
    public bool isPatrol = false; // 순찰
    public bool isBattle = false; // 전투 

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bossCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        curHealth = maxHealth;

        m_TargetPlayer = GetComponent<NavMeshAgent>();

        animator.SetInteger("AniState", (int)AniState.Sleep);

        InvokeRepeating("MoveToNextWayPoint", 0f, 2f);
    }

    private void Update()
    {
        m_bosshpslider.value = Mathf.Lerp(m_bosshpslider.value, curHealth/maxHealth, Time.deltaTime*5f);
        m_bosssBackhpslider.value = Mathf.Lerp(m_bosssBackhpslider.value, m_bosshpslider.value, Time.deltaTime * 2f);


        if (m_target != null && isBattle == false)
        {
            //m_enemy.SetDestination(m_target.position);

            if (Vector3.Distance(m_target.position, this.transform.position) <= 6.0f)
            {
                m_bosshpslider.gameObject.SetActive(true);
                m_bosssBackhpslider.gameObject.SetActive(true);

                m_TargetPlayer.acceleration = 0f;
                m_TargetPlayer.velocity = Vector3.zero;
               
                isBattle = true;
                isPatrol = false;
                animator.SetInteger("AniState", (int)AniState.IdleCombat);
            }
            else
            {
                m_bosshpslider.gameObject.SetActive(false);
                m_bosssBackhpslider.gameObject.SetActive(false);


                m_TargetPlayer.SetDestination(m_target.position);
                isBattle = false;
                
            }
        }
        else if (m_TargetPlayer.velocity.sqrMagnitude >= 0.2f * 0.2f && m_TargetPlayer.remainingDistance <= 0.5f)
        {
            animator.SetInteger("AniState", (int)AniState.IdleCombat);
            //Debug.LogWarning("도착");
        }
    }

    public void SetTarget(Transform p_target)
    {
        CancelInvoke();
        isPatrol = true;
        m_target = p_target;
        animator.SetInteger("AniState", (int)AniState.Run);
        m_TargetPlayer.speed = m_bosschaseSpeed;
    }

    public void RemoveTarget()
    {
        m_target = null;
        isBattle = false;
        m_TargetPlayer.SetDestination(m_tWayPoints[m_count].position);
        InvokeRepeating("MoveToNextWayPoint", 0f, 2f); // 0초후, 1.5초마다 실행
        Debug.Log("patrol");    

    }

    void MoveToNextWayPoint()
    {
        if (m_target == null)
        {
            isPatrol = true;
            m_TargetPlayer.speed = m_bossPatrolSpeed;


            // 도착 했다면 가만히 서 있는다
            if (m_TargetPlayer.velocity.sqrMagnitude >= 0.2f * 0.2f && m_TargetPlayer.remainingDistance <= 0.5f)
            {
                animator.SetInteger("AniState", (int)AniState.IdleCombat);
                //Debug.LogWarning("도착");
            }

            //도착한다면 다음 waypoint로 넘어간다
            if (m_TargetPlayer.velocity == Vector3.zero)
            {
                //animator.SetInteger("AniState", (int)AniState.IdleCombat);

                Debug.Log("현재 목적지는 " + m_tWayPoints[m_count].name + " 입니다");
                m_TargetPlayer.SetDestination(m_tWayPoints[m_count++].position);

                // 다시 움직인다
                animator.SetInteger("AniState", (int)AniState.Walk);

                // 카운터가 최대가 된다면 다시 처음위치에서 움직이게 사이클
                if (m_count >= m_tWayPoints.Length)
                    m_count = 0;
            }
            else
            {
                // 다음 목적지로 향하게 이동 한다
                //m_TargetPlayer.SetDestination(m_tWayPoints[m_count].position);
                animator.SetInteger("AniState", (int)AniState.Walk);

            }
        }
    }

    // 현재 상태 걷기, 뛰기, 전투 모드 등등 지속적인 상태 구분
    IEnumerator SetAnimationState(AniState p_state)
    {
        // AniState 열거형 멤버를 정수 값으로 변환하여 전달
        isState = false;
        aniStateValue = (int)p_state;
        animator.SetInteger("AniState", aniStateValue);
        yield return new WaitForSeconds(0.3f);
        isState = true;
    }

    // 공격 패턴 애니메이션 다시 이전 애니메이션으로 돌아가는 부분 예를 들어 공격하고 전투모드로
    IEnumerator SetAnimationTrigger(AniState p_state)
    {
        isState = true;
        // AniState 열거형 멤버를 정수 값으로 변환하여 전달
        //Debug.Log(state.ToString());
        animator.SetTrigger("do" + p_state.ToString());
        yield return new WaitForSeconds(1f);
        isState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(curHealth == maxHealth)
        {
            AttackPattern();
        }
        //StartCoroutine(AttackPattern());

        Debug.Log("hit target : "+other.name);
        if (other.name == "SwordCollier" && isDead == false)
        {
            //TakeDamage();
            StartCoroutine(OnDamage());
            if (isState != true)
            {
                // 공격 상태일때 공격 받으면 히트 애니 재생 x
                StartCoroutine(SetAnimationTrigger(AniState.GetHit));
            }
        }
    }

    // test용
    void TakeDamage()
    {
        int ranDamage = Random.Range(100, 500);
        m_cam = Camera.main;
        Instantiate(DamageUI);
        c_DamageUI = DamageUI.GetComponentInChildren<DamageText>();
        c_DamageUI.m_text.text = ranDamage.ToString();
        c_DamageUI.m_text.transform.position = m_cam.WorldToScreenPoint(headPos.transform.position);
        //DamageUI.transform.position = headPos.transform.position;

        curHealth -= ranDamage;

        Debug.Log("ȭ�鿡 �������� ������ : " + c_DamageUI.m_text.text);
        Debug.Log("���� ������ : " + ranDamage);
        Debug.Log("���� ü�� : " + curHealth.ToString());
        meshRenderer.material.color = Color.red;

        if (curHealth > 0)
        {
            meshRenderer.material.color = Color.white;
        }
        else if (curHealth <= 0)
        {
            // �׾�����.
            StartCoroutine(Die());
            //Debug.Log("�׾���");
        }
    }

    IEnumerator OnDamage()
    {
        //int ranDamage = Random.Range(100, 500);
        int ranDamage = 300;
        m_cam = Camera.main;
        c_DamageUI = DamageUI.GetComponentInChildren<DamageText>();
        c_DamageUI.m_text.text = ranDamage.ToString();
   
        // 위치를 정해주고, 특정 오브젝트 자식으로 생성한 후, 위치값을 초기화
        GameObject obj = Instantiate(DamageUI, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(transform.Find("DamageUiPos"), false);
       
        curHealth -= ranDamage;

        Debug.Log("화면에 표시되는 데미지 : " + c_DamageUI.m_text.text);
        Debug.Log("실제 데미지 : " + ranDamage);
        Debug.Log("현재 체력 : " + curHealth.ToString());
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
            //Debug.Log("�׾���");
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        bossCollider.enabled = false;
        meshRenderer.material.color = Color.black;
        StartCoroutine(SetAnimationState(AniState.Death));
        yield return new WaitForSeconds(6f);
        Destroy(m_bosshpslider.gameObject);
        Destroy(this.gameObject);
    }

    private void AttackPattern()
    {
        // 기본 Idle 상태에서 
        if (isDead == false && isPatrol == false && isBattle == true)
        {

            StartCoroutine(SetAnimationState(AniState.IdleCombat));
            //yield return new WaitForSeconds(bossAniTime); // �⺻ �� �ð�

            int ranAction = Random.Range(0, 1);
            //Debug.LogWarning(ranAction);
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
        //else if (isDead == true && isPatrol == false)
        //{

        //    StopCoroutine(SetAnimationTrigger(AniState.Attack1));
        //    StopCoroutine(SetAnimationTrigger(AniState.Attack2));
        //    StopCoroutine(SetAnimationTrigger(AniState.Buff));
        //    StopCoroutine(Attack1());
        //    StopCoroutine(Attack2());
        //    StopCoroutine(Buff());
        //}
    }


    IEnumerator Attack1()
    {
        // RightAttack
        isState = true;
        StartCoroutine(SetAnimationTrigger(AniState.Attack1));
        yield return new WaitForSeconds(bossAniTime);
        isState = false;
        AttackPattern();
        //StartCoroutine(AttackPattern());
    }


    IEnumerator Attack2()
    {
        // leftAttack
        isState = false;
        StartCoroutine(SetAnimationTrigger(AniState.Attack2));
        yield return new WaitForSeconds(bossAniTime);
        isState = true;
        AttackPattern();
        //StartCoroutine(AttackPattern());
    }


    IEnumerator Buff()
    {
        // 피가 절반 때부터 발동이 가능한 상태
        if (curHealth <= maxHealth / 2)
        {
            isState = false;
            StartCoroutine(SetAnimationTrigger(AniState.Buff));
            yield return new WaitForSeconds(bossAniTime);
            isState = true;
        }
        AttackPattern();
        //StartCoroutine(AttackPattern());
    }

}
