using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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

    private Rigidbody rigid;
    private BoxCollider boxCollider;
    public  SkinnedMeshRenderer meshRenderer;
    private Animator animator;

    public int aniStateValue = 0;
    

    private bool isState = true;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        curHealth = maxHealth;

        animator.SetInteger("AniState", (int)AniState.Sleep);
    }

    private void Update()
    {
        


        //test animation
        if (isState == true)
        {
            
            if (Input.GetKeyUp(KeyCode.F1))
            {
                StartCoroutine(SetAnimationState(AniState.Idle));
            }
            else if (Input.GetKeyUp(KeyCode.F2))
            {
                StartCoroutine(SetAnimationState(AniState.Walk));
            }
            else if (Input.GetKeyUp(KeyCode.F3))
            {
                StartCoroutine(SetAnimationState(AniState.Run));
            }
            else if (Input.GetKeyUp(KeyCode.F4))
            {
                StartCoroutine(SetAnimationState(AniState.IdleCombat));
                
            }
            else if (Input.GetKeyUp(KeyCode.F5))
            {
                StartCoroutine(SetAnimationState(AniState.Death));
            }


            else if (Input.GetKeyUp(KeyCode.F6))
            {
                StartCoroutine(SetAnimationTrigger(AniState.GetHit));
            }
            else if (Input.GetKeyUp(KeyCode.F7))
            {
                StartCoroutine(SetAnimationTrigger(AniState.Attack1));

            }
            else if (Input.GetKeyUp(KeyCode.F8))
            {
                StartCoroutine(SetAnimationTrigger(AniState.Attack2));

            }
            else if (Input.GetKeyUp(KeyCode.F9))
            {
                StartCoroutine(SetAnimationTrigger(AniState.Buff));

            }
        }
        
    }

    IEnumerator SetAnimationState(AniState state)
    {
        isState = false;
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        aniStateValue = (int)state;
        animator.SetInteger("AniState", aniStateValue);
        yield return new WaitForSeconds(0.3f);
        
        isState = true;
    }

    IEnumerator SetAnimationTrigger(AniState state)
    {
        isState = false;
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        Debug.Log(state.ToString());
        animator.SetTrigger("do" + state.ToString());
        yield return new WaitForSeconds(0.3f);
        isState = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        StartCoroutine(OnDamage());
        if (other.tag == "Weapon")
        {
            //���� ����
        }
    }

   IEnumerator OnDamage()
   {
        curHealth -= 10.0f;

        StartCoroutine(SetAnimationState(AniState.GetHit));
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (  curHealth > 0 ) 
        {
            meshRenderer.material.color = Color.white;
        }
        else if ( curHealth <= 0 ) 
        {
            // �׾�����.
            meshRenderer.material.color = Color.black;
        }
    }
}