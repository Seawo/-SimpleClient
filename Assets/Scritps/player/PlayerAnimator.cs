using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator p_Animator;
    private void Awake()
    {
        p_Animator = GetComponent<Animator>();
    }

    public void OnMovement(float horizontal, float vertical)
    {
        p_Animator.SetFloat("horizontal", horizontal);
        p_Animator.SetFloat("vertical", vertical);
    }

    public void OnJump()
    {
       // 점프 애니메이션 
    }
}
