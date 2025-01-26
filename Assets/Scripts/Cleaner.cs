using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaner : MonoBehaviour
{
	public float delay = 5f;
	public float current = 0f;

	private bool active = false;

	// Update is called once per frame
	void Update()
	{
		if(active)
		{
			current += Time.deltaTime;
			if(current >= delay)
				Destroy( gameObject );
		}
	}

	public void Activate()
	{
		active = true;
	}
}
