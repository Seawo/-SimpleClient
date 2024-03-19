﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 3;
    public float gravity = -9.18f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PlayerAnimator playerAnimator;

    Vector3 velocity;
    bool isGrounded;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();

        // Plane 오브젝트 찾기
        groundCheck = GameObject.Find("Plane").transform;
    }


    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            speed = 10;
        }
        else
        {
            speed = 3;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        playerAnimator.OnMovement(x, z);

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            playerAnimator.OnJump();
        }

        velocity.y += gravity * Time.deltaTime; // 중력
        controller.Move(velocity * Time.deltaTime);

    }
}