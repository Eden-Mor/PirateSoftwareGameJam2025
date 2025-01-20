using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a world composed of chunks arranged on a grid, which are in-turn composed of tiles
/// arranged on a grid.  There should probably be only one World in your world.
/// 
/// Currently it's static, but has been designed to make being dynamically generated at runtime 
/// reasonably easy to implement.
/// </summary>
public class World : MonoBehaviour
{
	/// <summary>
	/// The size of the world in chunks (width and length).
	/// </summary>
	public int worldSize = 10;

	/// <summary>
	/// The size of the chunks to build the world from (width and length).
	/// </summary>
	public int chunkSize = 5;

	/// <summary>
	/// The size of the tile models (width and length).
	/// </summary>
	public float tileSize = 1.0f;

	/// <summary>
	/// The prefab to use for creating chunks.
	/// </summary>
	public GameObject chunkPrefab;

	/// <summary>
	/// The Transform to use as a parent for all the chunk prefabs that are instantiated.
	/// </summary>
	public Transform chunksParent;

	/// <summary>
	/// References to all currently active chunks, such that we can easily remove them and identify if
	/// we need to build a chunk in this location or not.
	/// </summary>
	Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();

	void Start()
	{
		int worldRadius = worldSize / 2;
		// DEBUG: Just creating something rather than dynamically creating it based on player/camera location.
		for (int z = -worldRadius; z < worldRadius; z++)
		{
			for (int  x = -worldRadius; x < worldRadius; x++)
			{
				BuildChunk( x, z );
			}
		}
	}

	/// <summary>
	/// Builds the chunk at the specified coordinates and stores a reference to it in chunks.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	void BuildChunk( int x, int z )
	{
		float chunkWorldSize = chunkSize * tileSize;
		Vector3 chunkLocation = new Vector3( x * chunkWorldSize, 0.0f, z * chunkWorldSize );
		//GameObject chunkGameObject = Instantiate( chunkPrefab, chunkLocation, Quaternion.identity, chunksParent );
		GameObject chunkGameObject = Instantiate( chunkPrefab );
		chunkGameObject.transform.SetParent( chunksParent, false );
		chunkGameObject.transform.localPosition = chunkLocation;

		Chunk chunk = chunkGameObject.GetComponent<Chunk>();
		chunk.size = chunkSize;
		chunk.tileSize = tileSize;
		chunk.seed = ChunkSeed( x, z );

		chunks[ ChunkKey( x, z ) ] = chunkGameObject;
	}

	/// <summary>
	/// Returns a unique string key for the chunk at the specified coordinates.  Currently this is
	/// just by concatenating "x,z".
	/// </summary>
	/// <param name="x">The x-coordinate of the chunk.</param>
	/// <param name="z">The y-coordinate of the chunk.</param>
	/// <returns></returns>
	string ChunkKey( int x, int z )
	{
		return x + "," + z;
	}

	/// <summary>
	/// Generates a 'unique' integer for the specified chunk.
	/// 
	/// NOTE: It's not actually unique, it'll start generating clashes after an x or z value >= 100,000.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	int ChunkSeed( int x, int z )
	{
		return (z * 100000) + x;
	}
}
