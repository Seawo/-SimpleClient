using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static PlayerAnimator;

public class PlayerAnimator : MonoBehaviour
{
    public enum CurrentAni
    {
        Idle,
        Run,
        Jump,
        Attack
    }


    private Animator p_Animator;
    //public Collider[] skillImpactColiider;

    public GameObject swordCollider2;
    public ParticleSystem swordParticleSystem;

    int hashAttackCount = Animator.StringToHash("attackCount");

    NetWork_manager network_manager; // 서버로 현재 상태를 보내면서 다른 유저가 나의 상태를 볼 수 있게 한다

    public CurrentAni currentAni;

    private void Awake()
    {
        TryGetComponent(out p_Animator);
        //p_Animator = GetComponent<Animator>();

        network_manager = GameObject.FindObjectOfType<NetWork_manager>();
    }

    private void AniSand(CurrentAni anistate)
    {
        AniStatePacket aniStatePacket = new AniStatePacket();
        aniStatePacket.id = network_manager.m_myID;
        aniStatePacket.checkAniNum = (int)anistate;
        network_manager.SendData(aniStatePacket);
    }


    public void OnMovement(float horizontal, float vertical)
    {
        p_Animator.SetFloat("horizontal", horizontal);
        p_Animator.SetFloat("vertical", vertical);

       if (horizontal == 0 && vertical == 0 && currentAni != CurrentAni.Idle && currentAni != CurrentAni.Jump && currentAni != CurrentAni.Jump) 
       {
            AniSand(CurrentAni.Idle);
            currentAni = CurrentAni.Idle;
       }
       else if (vertical != 0 && currentAni != CurrentAni.Run && currentAni != CurrentAni.Jump)
       {
            AniSand(CurrentAni.Run);
            currentAni = CurrentAni.Run;
       }
    }

    public void OnJump()
    {
        // 점프 애니메이션
        p_Animator.SetTrigger("onjump");

        AniSand(CurrentAni.Jump);
        currentAni = CurrentAni.Jump;
    }

    public void Attack()
    {
        p_Animator.SetTrigger("attack");
        AttackCount = 0;

        // 공격 신호를 보낸다
        AniSand(CurrentAni.Attack);
        currentAni = CurrentAni.Attack;
    }

    public int AttackCount
    {
        get => p_Animator.GetInteger(hashAttackCount);
        set => p_Animator.SetInteger(hashAttackCount, value);
    }

    // sword 충돌박스 생성
    public void AttackCollision()
    {
        //Debug.Log("attack");
        StartCoroutine(DisableCollider2(swordCollider2));
    }

    IEnumerator DisableCollider2(GameObject sword)
    {
        //yield return new WaitForSeconds(0.1f);
        sword.active = true;
        yield return new WaitForSeconds(0.3f);
        sword.active = false;
    }
}
