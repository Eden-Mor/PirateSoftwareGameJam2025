using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleSpawner : MonoBehaviour
{
	/// <summary>
	/// The number of vehicles to maintain in the world.
	/// </summary>
	public int numberOfVehicles = 10;

	/// <summary>
	/// The vehicle prefabs to spawn, they will be randomly picked.
	/// </summary>
	public GameObject[] vehiclePrefabs;

	/// <summary>
	/// Transform to put all our instantiated vehicles as children of.
	/// </summary>
	public Transform vehiclesParent;

	/// <summary>
	/// The World.
	/// </summary>
	public World world;

	/// <summary>
	/// Used to determine if the initial spawn has occurred, as we need to wait for the world to be 
	/// created before spawning vehicles within it.
	/// </summary>
	bool spawned = false;

	public void Update()
	{
		// There's a timing issue with having a vehicle spawn in Start, as the World isn't always there yet.
		// TODO: A much nicer solution than this!
		if(!spawned && world.chunks.Count > 0)
		{
			for(int i = 0; i < numberOfVehicles; i++)
				SpawnVehicle();

			spawned = true;
		}
	}

	/// <summary>
	/// Spawns a random vehicle on a random road tile.
	/// TODO: Have this the behaviour of the initial spawn, though eliminating tiles each time so we don't collide.
	/// </summary>
	public void SpawnVehicle()
	{
		// TODO: Take into account player view frustrum, where we've recently spawned vehicles, etc.
		var tile = world.GetRandomRoadTile();

		// Get a Tile Path from the tile, which we'll have the vehicle follow.
		TilePath tilePath = tile.RandomTilePath();

		Debug.Log( "VehicleSpawner > SpawnVehicle > tile.transform.position: " + tile.transform.position );

		if(tilePath != null)
		{
			// Spawn the Vehicle and set it on the tile path.
			// TODO: Weights.
			GameObject vehiclePrefab = vehiclePrefabs[ UnityEngine.Random.Range( 0, vehiclePrefabs.Length ) ];
			GameObject vehicleObject = Instantiate( vehiclePrefab );
			vehicleObject.transform.SetParent( vehiclesParent.transform, false );

			VehiclePathController vehicle = vehicleObject.GetComponent<VehiclePathController>();
			vehicle.tile = tile;
			vehicle.tilePath = tilePath;
			vehicle.spawner = this;
		}
	}

	/// <summary>
	/// Despawns the passed vehicle then randomly spawns another.
	/// </summary>
	/// <param name="vehicleObject">The game object of the vehicle to despawn.</param>
	public void DespawnVehicle( GameObject vehicleObject )
	{
		Destroy( vehicleObject );
		SpawnVehicle();
	}
}
