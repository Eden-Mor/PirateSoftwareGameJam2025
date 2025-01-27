using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleSwapper : MonoBehaviour
{
	public GameObject replacement;
	bool destroyed = false;

	void OnCollisionEnter( Collision collision )
	{
		if(!destroyed && collision.gameObject.tag == "Player")
		{
			GameObject swap = Instantiate( replacement );
			swap.transform.SetParent( transform.parent, false );
			swap.transform.position = transform.position;
			swap.transform.rotation = transform.rotation;
			swap.transform.localScale = transform.localScale;

			var swapCleaner = swap.GetComponent<Cleaner>();
			if(swapCleaner != null)
				swapCleaner.Activate();

			destroyed = true;
			Destroy( gameObject );
		}
	}
}
