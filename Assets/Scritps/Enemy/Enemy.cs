using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer.Internal;
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

        animator.SetInteger("AniState", (int)AniState.Sleep);
        //StartCoroutine(Think());
    }

    private void Update()
    {

    }

    // ���� ���� �ȱ�, �ٱ�, ���� ��� ��� �������� ���� ����
    IEnumerator SetAnimationState(AniState state)
    {
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        isState = false;
        aniStateValue = (int)state;
        animator.SetInteger("AniState", aniStateValue);
        yield return new WaitForSeconds(0.3f);
        isState = true;
    }

    // ���� ���� �ִϸ��̼� �ٽ� ���� �ִϸ��̼����� ���ư��� �κ� ���� ��� �����ϰ� ��������
    IEnumerator SetAnimationTrigger(AniState state)
    {
        isState = true;
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        //Debug.Log(state.ToString());
        animator.SetTrigger("do" + state.ToString());
        yield return new WaitForSeconds(1f);
        isState = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(curHealth == maxHealth)
        {
            StartCoroutine(Think());
        }

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

    IEnumerator Think()
    {
        // �⺻ Idle ���¿��� 
        if ( isDead != true )
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
            StopCoroutine(Think());
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
        // �ǰ� ���� ������ �ߵ��� ������ ����
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
