using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class VehiclePathController : MonoBehaviour
{
	[Header("Vehicle Properties")]
	public float speed = 0.2f;

	[Header( "Internals" )]
	public bool active = true;
	public float progress = 0f;
	public TilePath tilePath;

	public void Start()
	{
		// TODO: Cache distance calculation(s) for the path.
		UpdateTransform();
	}

	public void Update()
	{
		// TODO: Have speed represent distance travelled over time, rather than percentage of path travelled over time.
		progress += speed * Time.deltaTime;

		if (progress >= 1f)
		{
			// TODO: Get the next path to move along.
			progress -= 1f;
		}

		UpdateTransform();
	}


	public void UpdateTransform()
	{
		if (active)
		{
			var path = GetPath();

			float3 position;
			float3 tangent;
			float3 up;

			path.Evaluate( Mathf.Clamp(progress, 0f, 1f), out position, out tangent, out up );

			Debug.Log( "VehiclePathController > UpdateTransform > position: " + position );

			transform.position = position;
		}
	}

	SplineContainer GetPath()
	{
		GameObject pathObject = tilePath.path;
		return pathObject.GetComponent<SplineContainer>();
	}
}
