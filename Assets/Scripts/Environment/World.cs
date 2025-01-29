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

	public TileSet poiTileSet;
	public List<GameObject> poiTilePool;

	/// <summary>
	/// References to all currently active chunks, such that we can easily remove them and identify if
	/// we need to build a chunk in this location or not.
	/// </summary>
	public Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();

	/// <summary>
	/// List of all the road tiles from the chunk generator, used to be able to pick where to spawn 
	/// vehicles, pick-ups, and drop-offs.
	/// </summary>
	public List<Tile> roadTiles = new List<Tile>();

	void Start()
	{
		poiTilePool = new List<GameObject>( poiTileSet.tiles );

		// DEBUG: Just creating something rather than dynamically creating it based on player/camera location.
		for(int z = 0; z < size; z++)
		{
			for(int x = 0; x < size; x++)
			{
				BuildChunk( x, z );
			}
		}
	}

	/// <summary>
	/// The radius of the world in real units from the centre directly to one of its edges (NOT the 
	/// diagonal distance to the corner).  Takes into account world scaling (using x axis as a 
	/// reference, the world is assumed to be square.)
	/// </summary>
	/// <returns>The radius of the world in real units.</returns>
	public float WorldRadius()
	{
		return (size * chunkSize * tileSize * transform.localScale.x) / 2;
	}

	/// <summary>
	/// Builds the chunk at the specified coordinates and stores a reference to it in chunks.
	/// </summary>
	/// <param name="x">The x-coordinate of the chunk to build.</param>
	/// <param name="z">The y-coordinate of the chunk to build.</param>
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

	/// <summary>
	/// Given a set of chunk coordinates and tile coordinates within that chunk, returns the 
	/// coordinates of that tile within the world.
	/// 
	/// NOTE: This is coordinates within our data model of the world, not unity transforms.
	/// </summary>
	/// <param name="chunkCoords">The coordinates of the chunk.</param>
	/// <param name="tileCoords">The local coordinates of the tile within that chunk.</param>
	/// <returns>The world coordinates of the tile.</returns>
	public Vector3Int WorldCoords( Vector3Int chunkCoords, Vector3Int tileCoords )
	{
		int x = chunkCoords.x * chunkSize + tileCoords.x;
		int z = chunkCoords.z * chunkSize + tileCoords.z;

		return new Vector3Int( x, 0, z );
	}

	/// <summary>
	/// Get the tile at the specified world coordinates.
	/// 
	/// NOTE: The world coordinates are those within our model, not unity transforms.
	/// </summary>
	/// <param name="worldCoords">The world coordinates to get the tile from.</param>
	/// <returns>The tile's instance game object.</returns>
	public Tile GetTile( Vector3Int worldCoords )
	{
		int chunkX = worldCoords.x / chunkSize;
		int chunkZ = worldCoords.z / chunkSize;
		int tileX = worldCoords.x % chunkSize;
		int tileZ = worldCoords.z % chunkSize;

		if(tileX < 0 || tileX > chunkSize || tileZ < 0 || tileZ > chunkSize)
			return null;

		if(!chunks.ContainsKey( ChunkKey( chunkX, chunkZ ) ))
			return null;

		GameObject chunkObject = chunks[ ChunkKey( chunkX, chunkZ ) ];
		Chunk chunk = chunkObject.GetComponent<Chunk>();
		return chunk.instances[ tileX, tileZ ];
	}

	public Tile GetRandomRoadTile()
	{
		return roadTiles[ UnityEngine.Random.Range( 0, roadTiles.Count ) ];
	}

	public Tile TakeRandomPoi()
	{
		var poiObject = poiTilePool[ UnityEngine.Random.Range( 0, poiTilePool.Count ) ];
		poiTilePool.Remove( poiObject );
		return poiObject.GetComponent<Tile>();
	}
}
