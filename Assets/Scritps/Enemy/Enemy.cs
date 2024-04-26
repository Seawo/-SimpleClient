using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
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

    public float maxHealth = 100.0f;
    public float curHealth = 0.0f;
    public float bossTermTime = 0.1f;
    public float bossAniTime = 3.0f;

    public  SkinnedMeshRenderer meshRenderer;
    private Rigidbody rigid;
    private BoxCollider bossCollider;
    private Animator animator;

    public int aniStateValue = 0;
    
    private bool isState = true;
    private bool isDead = false;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bossCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        curHealth = maxHealth;

        //animator.SetInteger("AniState", (int)AniState.Sleep);
        StartCoroutine(Think());
    }

    private void Update()
    {

    }

    // 현재 상태 걷기, 뛰기, 전투 모드 등등 지속적인 상태 구분
    IEnumerator SetAnimationState(AniState state)
    {
        // AniState 열거형 멤버를 정수 값으로 변환하여 전달
        isState = false;
        aniStateValue = (int)state;
        animator.SetInteger("AniState", aniStateValue);
        yield return new WaitForSeconds(0.3f);
        isState = true;
    }

    // 공격 패턴 애니메이션 다시 이전 애니메이션으로 돌아가는 부분 예를 들어 공격하고 전투모드로
    IEnumerator SetAnimationTrigger(AniState state)
    {
        isState = true;
        // AniState 열거형 멤버를 정수 값으로 변환하여 전달
        //Debug.Log(state.ToString());
        animator.SetTrigger("do" + state.ToString());
        yield return new WaitForSeconds(1f);
        isState = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit target : "+other.name);
        if (other.name == "SwordCollier" && !isDead)
        {
            StartCoroutine(OnDamage());
            if(isState != true)
            {
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
            // 죽었을때.
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        meshRenderer.material.color = Color.black;
        StartCoroutine(SetAnimationState(AniState.Death));
        bossCollider.enabled = false;
        yield return new WaitForSeconds(6f);
        isDead = true;
        Destroy(this.gameObject);
    }

    IEnumerator Think()
    {
        // 기본 Idle 상태에서 
        StartCoroutine(SetAnimationState(AniState.IdleCombat));
        yield return new WaitForSeconds(bossAniTime); // 기본 텀 시간

        int ranAction = Random.Range(0, 4);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(Attack1());
                break;
            case 2:
            case 3:
                StartCoroutine (Attack2());
                break;
            case 4:
                StartCoroutine(Buff());
                break;
        }
    }

    IEnumerator Attack1()
    {
        // RightAttack
        isState = true;
        StartCoroutine(SetAnimationTrigger(AniState.Attack1));
        yield return new WaitForSeconds(bossAniTime);
        isState = false;

        StartCoroutine(Think());
    }

    IEnumerator Attack2()
    {
        // leftAttack
        isState = false;
        StartCoroutine(SetAnimationTrigger(AniState.Attack2));
        yield return new WaitForSeconds(bossAniTime);
        isState = true;

        StartCoroutine(Think());
    }

    IEnumerator Buff()
    {
        // 피가 절반 때부터 발동이 가능한 상태
        if(curHealth <= maxHealth/2)
        {
            isState = false;
            StartCoroutine(SetAnimationTrigger(AniState.Buff));
            yield return new WaitForSeconds(bossAniTime);
            isState = true;
        }

        StartCoroutine(Think());
    }

}
