// Copyright © bl4ck & XDev, 2022-2024
// oh boy why does this script exist

using DG.Tweening;
using EZCameraShake;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] public float m_JumpForce = 400f;                 
	[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   
	[SerializeField] private bool m_AirControl = false;                
	[SerializeField] private Transform m_GroundCheck;                    
	[SerializeField] private Transform m_CeilingCheck;
	[SerializeField] private Collider2D m_CrouchDisableCollider; // THERE'S NO FUCKING CROUCHING

	public MainEditorComponent m;
	public int checkpointPriority;

	public Portal portal;

	public bool isEditor;
	public MainEditorComponent editor;

	public LayerMask lm;

	public bool m_Grounded;//, box;
	public Rigidbody m_Rigidbody2D;
	private Vector3 velocity = Vector3.zero;
	public Transform particles;
	public ParticleSystem slideParticlesLeft, slideParticlesRight;

	public bool airborneBetweenCollisions = false;
	public LayerMask mask;
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody>();
    }
	
	
	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Ground") || col.gameObject.layer == lm && !m_Grounded)
		{
			if (airborneBetweenCollisions) CameraShaker.Instance.ShakeOnce(0.5f, 0, 0.25f, 0.25f);
			airborneBetweenCollisions = false;
			m_Grounded = true;
			
		}

		if (col.gameObject.CompareTag("button") && !col.gameObject.GetComponent<Button>().isEnabled)
		{
			col.gameObject.GetComponent<Button>().Activate();
		}
            

        if (col.gameObject.CompareTag("button") && col.gameObject.GetComponent<Button>().turnBackOff && col.gameObject.GetComponent<Button>().isEnabled)
            col.gameObject.GetComponent<Button>().Deactivate();
    }

	private void Update()
	{
		
		if (Physics.CheckBox(transform.position, Vector3.one * 0.8f, transform.rotation, mask))
		{
			//box = true;
			
			
		}
		else
		{
			
		}
	}

	IEnumerator Coyote()
	{
		yield return new WaitForSeconds(0.05f);
		
		if (!Physics.CheckBox(transform.position, Vector3.one * 0.8f, transform.rotation, mask))
		{
			//box = false;
			m_Grounded = false;
		}
	}
	
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawWireCube(transform.position, Vector3.one * 1.7f);
	}

	private void OnCollisionStay(Collision col)
	{
        if (col.gameObject.CompareTag("Ground") || col.gameObject.layer == 3)
            m_Grounded = true;
    }
	private void OnCollisionExit(Collision col)
	{
		if ((col.gameObject.CompareTag("Ground") || col.gameObject.layer == 3)) //&& !box)
		{
			StartCoroutine(Coyote());
		}
            
    }
	private Vector3 targetVelocity;

	public void Move(float move, bool jump)
	{
		if (m_Grounded || m_AirControl)
		{
			targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			slideParticlesLeft.transform.position = transform.position - Vector3.up / 2;
			slideParticlesRight.transform.position = transform.position - Vector3.up / 2;
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
		}

		if (jump && (m_Grounded)) //|| box))
		{
			m_Grounded = false;
			airborneBetweenCollisions = true;
            m_Rigidbody2D.AddForce(new Vector3(0f, m_JumpForce));	
		}
	}
}