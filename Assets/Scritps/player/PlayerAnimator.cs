using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator p_Animator;
    public Collider swordCollider;
    public Collider[] skillImpactColiider;

    public GameObject swordCollider2;


    int hashAttackCount = Animator.StringToHash("attackCount");
    private void Awake()
    {
        TryGetComponent(out p_Animator);
        //p_Animator = GetComponent<Animator>();
    }

    public void OnMovement(float horizontal, float vertical)
    {
        p_Animator.SetFloat("horizontal", horizontal);
        p_Animator.SetFloat("vertical", vertical);
    }

    public void OnJump()
    {
        // 점프 애니메이션
        p_Animator.SetTrigger("onjump");
    }

    public void Attack()
    {
        p_Animator.SetTrigger("attack");
        AttackCount = 0;
    }

    public int AttackCount
    {
        get => p_Animator.GetInteger(hashAttackCount);
        set => p_Animator.SetInteger(hashAttackCount, value);
    }

    public void AttackCollision()
    {
        Debug.Log("attack");
        StartCoroutine(DisableCollider2(swordCollider2));
    }

    IEnumerator DisableCollider2(GameObject collier)
    {

        //yield return new WaitForSeconds(0.1f);
        collier.active = true;
        yield return new WaitForSeconds(0.1f);
        collier.active = false;
    }
}
