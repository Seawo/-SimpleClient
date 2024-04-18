using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerAnimator : MonoBehaviour
{
    [HideInInspector]
    public enum AniState
    {
        // otherPlayer state
        Idle,
        Run,

        // otherPlayer trigger
        Attack,
        Jump,
    }


    public int aniStateValue = 0;

    private bool isState = true;
    private bool isDead = false;

    private Animator op_Animator; // otherplayer
    //public Collider[] skillImpactColiider;

    public GameObject swordCollider2;
    public ParticleSystem swordParticleSystem;

    int hashAttackCount = Animator.StringToHash("attackCount");
    private void Awake()
    {
        TryGetComponent(out op_Animator);
        //p_Animator = GetComponent<Animator>();

        //op_Animator.SetInteger("anistate", (int)AniState.Idle);
    }

    private void Update()
    {
        // �ִϸ��̼� �׽�Ʈ������ �ڿ������� �ִϸ��̼� ������ ���� ����
        if (!isDead)
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                StartCoroutine(SetAnimationState(AniState.Idle));
            }
            else if (Input.GetKeyUp(KeyCode.F2))
            {
                StartCoroutine(SetAnimationState(AniState.Run));
            }
            else if (Input.GetKeyUp(KeyCode.F3))
            {
                StartCoroutine(SetAnimationTrigger(AniState.Attack));
            }
            else if (Input.GetKeyUp(KeyCode.F4))
            {
                StartCoroutine(SetAnimationTrigger(AniState.Jump));
            }
        }
    }

    public IEnumerator SetAnimationState(AniState state)
    {
        isState = false;
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        aniStateValue = (int)state;
        op_Animator.SetInteger("anistate", aniStateValue);
        yield return new WaitForSeconds(0.3f);

        isState = true;
    }
    public IEnumerator SetAnimationTrigger(AniState state)
    {
        isState = false;
        // AniState ������ ����� ���� ������ ��ȯ�Ͽ� ����
        //Debug.Log(state.ToString());
        op_Animator.SetTrigger("on" + state.ToString());
        yield return new WaitForSeconds(0.3f);
        isState = true;
    }

    public int AttackCount
    {
        get => op_Animator.GetInteger(hashAttackCount);
        set => op_Animator.SetInteger(hashAttackCount, value);
    }

    // sword �浹�ڽ� ����
    public void AttackCollision()
    {
        Debug.Log("attack");
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
