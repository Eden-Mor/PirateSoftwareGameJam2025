using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepyRigidBody : MonoBehaviour
{
	Cleaner cleaner;

	private void Awake()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.Sleep();

		cleaner = GetComponent<Cleaner>();
	}

	public void OnCollisionEnter( Collision collision )
	{
		if(cleaner != null)
			cleaner.Activate();
	}
}
