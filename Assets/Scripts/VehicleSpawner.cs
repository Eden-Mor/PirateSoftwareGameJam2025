using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleSpawner : MonoBehaviour
{
	public int numberOfVehicles = 10;
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
			for (int i = 0; i < numberOfVehicles; i++)
				SpawnVehicle();

			spawned = true;
		}
	}

	public void SpawnVehicle()
	{
		// TODO: Take into account player view frustrum, where we've recently spawned vehicles, etc.
		var tileObject = world.GetRandomRoadTile();

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
	}

	public void DespawnVehicle(GameObject vehicleObject )
	{
		Destroy( vehicleObject );
		SpawnVehicle();
	}
}
