// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour
{
    bool jump = false;

    public float runSpeed = 40f;
    public CharacterController2D controller;
	float horizontalMove = 0f;

	public AudioSource jumpSFX;
	public Die Die;


    void Start() 
	{ 
		Die.PDie();
	}
	void Update()
	{
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		if (horizontalMove == 0 || !controller.m_Grounded)
		{
			controller.slideParticlesRight.Stop();
			controller.slideParticlesLeft.Stop();
		}
		if (horizontalMove > 0)
		{
			if (!controller.slideParticlesRight.isPlaying)
                controller.slideParticlesRight.Play();
			controller.slideParticlesLeft.Stop();
		}
		if (horizontalMove < 0) 
		{
			
			controller.slideParticlesRight.Stop();
			if (!controller.slideParticlesLeft.isPlaying)
			controller.slideParticlesLeft.Play();
		}
		
		if (Input.GetButtonDown("Jump"))
		{
			jumpSFX.Play();
			jump = true;
		}
	}
	private void OnDisable()
	{
		
		
		if (controller.slideParticlesLeft) controller.slideParticlesLeft.Stop();
		if (controller.slideParticlesRight) controller.slideParticlesRight.Stop();
	}
	void FixedUpdate()
	{

		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}