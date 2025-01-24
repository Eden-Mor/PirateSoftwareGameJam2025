using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleSpawner : MonoBehaviour
{
	public GameObject[] vehiclePrefabs;
	public GameObject worldObject;
	public Transform vehiclesParent;
	public World world;

	bool spawned = false;

	public void Start()
	{
		world = worldObject.GetComponent<World>();
	}

	public void Update()
	{
		// There's a timing issue with having a vehicle spawn in Start, as the World isn't always there yet.
		// TODO: A much nicer solution than this!
		if(!spawned && world.chunks.Count > 0)
		{
			SpawnVehicle();
		}
	}

	public void SpawnVehicle()
	{
		// Get the chunk in which we're spawning.
		// TODO: Randomise (based on player location).
		GameObject chunkObject = world.chunks[ "0,0" ];
		Chunk chunk = chunkObject.GetComponent<Chunk>();

		// Get a road tile from the chunk.
		// TODO: Randomise.
		GameObject tileObject = chunk.instances[ 2, 0 ];
		Debug.Log( "VehicleSpawner > SpawnVehicle > tileObject.name: " + tileObject.name );

		// Get a Tile Path from the tile, which we'll have the vehicle follow.
		Tile tile = tileObject.GetComponent<Tile>();
		TilePath tilePath = tile.RandomTilePath();

		Debug.Log( "VehicleSpawner > SpawnVehicle > tileObject.transform.position: " + tileObject.transform.position );

		if(tilePath != null)
		{
			// Spawn the Vehicle and set it on the tile path.
			// TODO: Weights.
			GameObject vehiclePrefab = vehiclePrefabs[ UnityEngine.Random.Range( 0, vehiclePrefabs.Length ) ];
			GameObject vehicleObject = Instantiate( vehiclePrefab );
			vehicleObject.transform.SetParent( vehiclesParent.transform, false );

			VehiclePathController vehicle = vehicleObject.GetComponent<VehiclePathController>();
			vehicle.tileObject = tileObject;
			vehicle.tilePath = tilePath;
			vehicle.spawner = this;
		}

		spawned = true;
	}

	public void DespawnVehicle(GameObject vehicleObject )
	{
		// TODO: Track spawns and despawns.
		Destroy( vehicleObject );
	}
}
