using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class VehiclePathController : MonoBehaviour
{
	[Header("Vehicle Properties")]
	public float speed = 1f;

	[Header( "Internals" )]
	public bool active = true;
	public float progress = 0f;
	public GameObject tileObject;
	public TilePath tilePath;
	public VehicleSpawner spawner;

	public void Start()
	{
		UpdateTransform();
	}

	public void Update()
	{
		// TODO: Cache distance calculation(s) for the path.
		var path = GetPath();
		float pathLength = path.CalculateLength();
		progress += (speed / pathLength) * Time.deltaTime;

		// If we've reached the end of the path, get the next tile in our direction of travel and a path
		// from it, or despawn if there's no route.
		if (progress >= 1f)
		{
			// Maintain any excess progress into the next tile.
			// TODO: This will need to be different when using distance not percentage of path.
			progress -= 1f;

			// Get the coords of the next tile in our direction.
			Vector3Int nextTileOffset = new Vector3Int();
			if(tilePath.end == Direction.North)
				nextTileOffset.z = 1;
			else if(tilePath.end == Direction.South)
				nextTileOffset.z = -1;
			else if(tilePath.end == Direction.East)
				nextTileOffset.x = 1;
			else
				nextTileOffset.x = -1;

			Tile currentTile = tileObject.GetComponent<Tile>();
			Vector3Int nextTileCoords = currentTile.worldCoords + nextTileOffset;

			// Get the next tile object or despawn if there isn't one (reached the edge of the world).
			GameObject nextTileObject = spawner.world.GetTileObject(nextTileCoords);
			if(nextTileObject == null)
			{
				spawner.DespawnVehicle( gameObject );
				return;
			}

			// Get an appropriate tile path for us to follow.
			Direction invertedDirection;
			if(tilePath.end == Direction.North)
				invertedDirection = Direction.South;
			else if (tilePath.end == Direction.South)
				invertedDirection = Direction.North;
			else if (tilePath.end == Direction.East)
				invertedDirection = Direction.West;
			else
				invertedDirection = Direction.East;

			Tile nextTile = nextTileObject.GetComponent<Tile>();
			TilePath nextTilePath = nextTile.RandomTilePath(invertedDirection);

			if(nextTilePath == null)
				spawner.DespawnVehicle(gameObject);

			// Set our tile and path references to the next things and off we go.
			tileObject = nextTileObject;
			tilePath = nextTilePath;
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
			Vector3 forward = Vector3.Cross( Vector3.up, tangent );

			transform.position = position;
			transform.rotation = Quaternion.LookRotation( forward, Vector3.up ) * Quaternion.Euler(new Vector3(0f, -90f, 0f));
		}
	}

	SplineContainer GetPath()
	{
		GameObject pathObject = tilePath.path;
		return pathObject.GetComponent<SplineContainer>();
	}
}
