using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleToggle : MonoBehaviour
{
	public float threshold = 1750f;
	Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
	}

	void OnCollisionEnter( Collision collision )
	{
		if(rb.isKinematic && collision.impulse.magnitude > threshold)
			rb.isKinematic = false;
	}
}
