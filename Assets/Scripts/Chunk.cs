using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a chunk within a wider world and is composed of a grid of Tiles, which are selected 
/// from the passed Tile Sets.
/// </summary>
public class Chunk : MonoBehaviour
{
	/// <summary>
	/// The Tile Set used to generate the roads.  Roads likely use a different generation algorithm.
	/// </summary>
	public TileSet roadsTileSet;

	/// <summary>
	/// The Tile Set used for all the other tiles.
	/// TODO: Break this up into different sets as our generation functions demand.
	/// </summary>
	public TileSet othersTileSet;

	/// <summary>
	/// This is the width and height of the chunk; we're only allowing square chunks.
	/// </summary>
	public int size;

	/// <summary>
	/// The size of the tile objects themselves (e.g. how far apart to place them from each other so
	/// they neatly tile).
	/// </summary>
	public float tileSize = 1.0f;

	/// <summary>
	/// The seed used in the random number generator.  Set this to provide some determinism,
	/// otherwise a random seed will be used.
	/// </summary>
	public int seed;

	/// <summary>
	/// The object (empty) with which to parent all generated tiles.  This then lets us manage those 
	/// with affecting other children of this object.  Should just be set to an empty in the chunk's
	/// prefab.
	/// </summary>
	public Transform tilesParent;

	/// <summary>
	/// Data structure that holds a representation of the chunk (from which the scene can be built),
	/// queried, and modified.
	/// </summary>
	Tile[,] tiles;

	/// <summary>
	/// Initialises the tiles array then sets off the Generation and Building process.
	/// </summary>
	public void Start()
	{
		tiles = new Tile[ size, size ];
		Generate();
		Build();
	}

	/// <summary>
	/// Generates the chunk's data-structure, which is later used by Build to realise that data 
	/// structure in the scene.
	/// </summary>
	public void Generate()
	{
		GenerateRoads();
		GenerateOthers();
	}

	/// <summary>
	/// Generate the roads.  At the moment this is super simple and just creates a cross shape.
	/// 
	/// TODO: Make this far, far better!  L-systems look interesting.
	/// TODO: Have tile names and a means of getting them by name.
	/// </summary>
	public void GenerateRoads()
	{
		for(int z = 0; z < size; z++)
		{
			for(int x = 0; x < size; x++)
			{
				bool xCentre = (x == (size - 1) / 2);
				bool zCentre = (z == (size - 1) / 2);

				if(xCentre && zCentre)
				{
					tiles[ x, z ] = roadsTileSet.tiles[ 4 ]; // Crossroads
				}
				else
				{
					if(xCentre) tiles[ x, z ] = roadsTileSet.tiles[ 10 ]; // Straight (North/South)
					if(zCentre) tiles[ x, z ] = roadsTileSet.tiles[ 9 ]; // Straight (East/West)
				}
			}
		}
	}

	/// <summary>
	/// Generates all the non-road tiles.  Currently this just randomly picks one from the Others 
	/// Tile Set.
	/// 
	/// TODO: Make this far, far better:
	///   - Wave Function Collapse looks interesting.
	///   - Tile weights would be really useful and simple to do.
	///     - Could then use weight maps to add more interest to generation.
	/// </summary>
	public void GenerateOthers()
	{
		Random.InitState( seed );

		for(int z = 0; z < size; z++)
		{
			for(int x = 0; x < size; x++)
			{
				if(tiles[x,z] == null)
				{
					int tileSetIndex = Random.Range( 0, othersTileSet.tiles.Length );
					tiles[x,z] = othersTileSet.tiles[ tileSetIndex ];
				}
			}
		}
	}

	/// <summary>
	/// Builds the chunk in the scene by iterating over the generated tiles data structre and 
	/// instantiating each tiles prefab in the desired location.
	/// </summary>
	public void Build()
	{
		for(int z = 0; z < size; z++)
		{
			for(int x = 0; x < size; x++)
			{
				if(tiles[ x, z ] != null)
				{
					BuildTile( x, z );
				}
			}
		}
	}

	/// <summary>
	/// Instantiates the prefab of the tile at the specified location.  The tile is positioned 
	/// relative to the tilesParent transform.
	/// </summary>
	/// <param name="x">x-coordinate of the tile.</param>
	/// <param name="z">z-coordinate of the tile.</param>
	void BuildTile( int x, int z )
	{
		GameObject tile = Instantiate( tiles[ x, z ].prefab );
		tile.transform.SetParent( tilesParent, false );
		tile.transform.localPosition = new Vector3( x * tileSize, 0.0f, z * tileSize );
	}
}


