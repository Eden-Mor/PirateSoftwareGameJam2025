using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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
	[Header( "Chunks" )]
	/// <summary>
	/// The size of the world in chunks (width and length).
	/// </summary>
	public int size = 10;

	/// <summary>
	/// The size of the chunks to build the world from (width and length).
	/// </summary>
	public int chunkSize = 5;

	/// <summary>
	/// The prefab to use for creating chunks.
	/// </summary>
	public GameObject chunkPrefab;

	/// <summary>
	/// The Transform to use as a parent for all the chunk prefabs that are instantiated.
	/// </summary>
	public Transform chunksParent;

	[Space( 10 )]
	[Header( "Tiles" )]

	/// <summary>
	/// The size of the tile models (width and length).
	/// </summary>
	public float tileSize = 1.0f;

	/// <summary>
	/// References to all currently active chunks, such that we can easily remove them and identify if
	/// we need to build a chunk in this location or not.
	/// </summary>
	public Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();

	void Start()
	{
		int worldRadius = size / 2;
		// DEBUG: Just creating something rather than dynamically creating it based on player/camera location.
		for(int z = -worldRadius; z < worldRadius; z++)
		{
			for(int x = -worldRadius; x < worldRadius; x++)
			{
				BuildChunk( x, z );
			}
		}
	}

	public float WorldRadius()
	{
		// TODO: Change this to have the radius to the corners of the city by using the diagonals?
		return (size * chunkSize * tileSize * transform.localScale.x) / 2;
	}

	/// <summary>
	/// Builds the chunk at the specified coordinates and stores a reference to it in chunks.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	void BuildChunk( int x, int z )
	{
		float chunkWorldSize = chunkSize * tileSize;

		GameObject chunkGameObject = Instantiate( chunkPrefab );
		chunkGameObject.transform.SetParent( chunksParent, false );
		chunkGameObject.transform.localPosition = new Vector3( x * chunkWorldSize, 0.0f, z * chunkWorldSize );
		chunkGameObject.name = ChunkKey( x, z );

		Chunk chunk = chunkGameObject.GetComponent<Chunk>();
		chunk.world = this;
		chunk.size = chunkSize;
		chunk.tileSize = tileSize;
		chunk.seed = ChunkSeed( x, z );
		chunk.coords = new Vector3Int( x, 0, z );
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

	public Vector3Int WorldCoords( Vector3Int chunkCoords, Vector3Int tileCoords )
	{
		int x = chunkCoords.x * chunkSize + tileCoords.x;
		int z = chunkCoords.z * chunkSize + tileCoords.z;

		return new Vector3Int(x, 0, z);
	}

	public GameObject GetTileObject( Vector3Int worldCoords )
	{
		int chunkX = worldCoords.x / chunkSize;
		int chunkZ = worldCoords.z / chunkSize;
		int tileX = worldCoords.x % chunkSize;
		int tileZ = worldCoords.z % chunkSize;

		GameObject chunkObject = chunks[ ChunkKey( chunkX, chunkZ) ];
		Chunk chunk = chunkObject.GetComponent<Chunk>();

		return chunk.instances[ tileX, tileZ ];
	}
}
