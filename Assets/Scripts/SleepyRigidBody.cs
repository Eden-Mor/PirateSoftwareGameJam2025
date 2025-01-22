using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepyRigidBody : MonoBehaviour
{
	private void Awake()
	{
		Debug.Log( "Tree is awake!" );
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.Sleep();
	}
}
