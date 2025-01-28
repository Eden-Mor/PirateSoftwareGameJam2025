using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Represents a chunk within a wider world and is composed of a grid of Tiles, which are selected 
/// from the passed Tile Sets.
/// </summary>
public class Chunk : MonoBehaviour
{
	/// <summary>
	/// A list of tile sets from which to pick parts to randomly place in the gaps between roads.
	/// </summary>
	public TileSet[] tilesets;

	/// <summary>
	/// The Tile Set used to generate the roads.  Roads likely use a different generation algorithm.
	/// </summary>
	public TileSet roadsTileSet;

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
	/// Reference to the world as we need to get some settings from it.
	/// </summary>
	public World world;

	/// <summary>
	/// Data structure that holds a representation of the chunk (from which the scene can be built),
	/// queried, and modified.
	/// </summary>
	public GameObject[,] tiles;

	public List<GameObject> roadTiles = new List<GameObject>();

	// TODO: Do this in a better way.
	public Tile[,] instances;

	public Vector3Int coords;

	/// <summary>
	/// Initialises the tiles array then sets off the Generation and Building process.
	/// </summary>
	public void Start()
	{
		// TODO: Create seperate model and instance datastructures, using composition to reduce duplication.
		tiles = new GameObject[ size, size ];
		instances = new Tile[ size, size ];
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
		GeneratePointsOfInterest();
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

				if(tiles[ x, z ] != null)
					roadTiles.Add( tiles[ x, z ] );
			}
		}
	}

	public void GeneratePointsOfInterest()
	{
		// TODO: Randomly determine if this chunk has a PoI (ideally every chunk does, but if we don't
		// have enough PoI for the world size then some chunks won't.
		if(world.poiTilePool.Count > 0)
		{
			var poiTile = world.TakeRandomPoi();
			Debug.Log( $"Chunk > GeneratePointsOfInterest > poiTile.name: {poiTile.name}" );

			// Clone our list of road tiles and then start randomly searching for a neighbour that will
			// fit our poi.
			var remaining = new List<GameObject>( roadTiles );
			while(remaining.Count > 0)
			{
				var roadObject = remaining[ UnityEngine.Random.Range( 0, remaining.Count ) ];
				var road = roadObject.GetComponent<Tile>();
				remaining.Remove( roadObject );

				// Try each neighour in-turn to see if we can fit our poi.
				var neighbours = GetEmptyNeighbours( road.coords );
				if(neighbours.Count > 0)
				{
					foreach(var neighbour in neighbours)
					{
						// TODO: Take into account poi connections (so we'd need to record which direction each
						// neighbour is from the road tile we're checking).
						var fits = true;
						for(var z = neighbour.z; z < neighbour.z + poiTile.size.z; z++)
						{
							for(var x = neighbour.x; x < neighbour.x + poiTile.size.x; x++)
							{
								if(z > 0 && z < size && x > 0 && x < size)
								{
									if(tiles[ x, z ] != null)
									{
										fits = false;
										break;
									}
								}
							}
						}

						if(fits)
						{
							Debug.Log( $"Out poi tile fits here!" );
						}

					}
				}
			}
		}
		else
			Debug.Log( "poiTilePool empty!" );
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
		UnityEngine.Random.InitState( seed );

		for(int z = 0; z < size; z++)
		{
			for(int x = 0; x < size; x++)
			{
				if(tiles[ x, z ] == null)
					tiles[ x, z ] = SelectRandomTile( x, z );
			}
		}
	}

	/// <summary>
	/// Select a random tile from the lists of tiles in tilesets.  Each tileset has a weight curve
	/// which is used to determine the chances of that tileset being picked based on the distance to
	/// the centre of the World.
	/// </summary>
	/// <param name="x">x-coordinate of the tile within the chunk.  Not currently used, but will be soon.</param>
	/// <param name="z">z-coordinate of the tile within the chunk.  Not currently used, but will be soon.</param>
	/// <returns></returns>
	GameObject SelectRandomTile( int x, int z )
	{
		// Determine how far this tile is from the world origin as a fraction of the total radius of the city.
		Vector2 tilePosition = new Vector2( transform.position.x + (x * tileSize), transform.position.z + (z * tileSize) );
		Vector2 worldOrigin = new Vector2( world.transform.position.x + world.WorldRadius() - (world.chunkSize * tileSize), world.transform.position.z + world.WorldRadius() - (world.chunkSize * tileSize) );
		float distanceFromOrigin = Vector2.Distance( worldOrigin, tilePosition );
		float normalisedDistanceFromOrigin = Mathf.Clamp( distanceFromOrigin / world.WorldRadius(), 0f, 1f );

		// Total all the weights of the tilesets.
		// TODO: Do this on start so we're not calculating it every tile.
		float totalWeight = 0f;
		foreach(TileSet t in tilesets)
			totalWeight += t.weight.Evaluate( normalisedDistanceFromOrigin );

		// Generate a random number between 0 and totalWeight, then loop through each tileset to
		// determine which one is chosen.  Therefore, tilesets with more weight will be more likely to 
		// be selected.  We default to the first tileset 
		float target = UnityEngine.Random.Range( 0f, totalWeight );
		float current = 0f;

		TileSet tileset = tilesets[ 0 ];
		foreach(TileSet t in tilesets)
		{
			float weight = t.weight.Evaluate( normalisedDistanceFromOrigin );
			if(weight > 0f)
			{
				if(target >= current && target <= current + weight)
				{
					tileset = t;
					break;
				}
				else
				{
					current += weight;
				}
			}
		}

		// Now we're going to randomly select a tile within the chosen tileset.
		int tileIndex = UnityEngine.Random.Range( 0, tileset.tiles.Length );
		return tileset.tiles[ tileIndex ];
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
					BuildTile( x, z );
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
		GameObject tileObject = Instantiate( tiles[ x, z ] );
		tileObject.transform.SetParent( tilesParent, false );
		tileObject.transform.localPosition = new Vector3( x * tileSize, 0.0f, z * tileSize );

		Debug.Log( "Chunk > Building Tile: " + x + "," + z + " (" + tileObject.name + ")" );

		Tile tile = tileObject.GetComponent<Tile>();
		tile.chunk = this;
		tile.coords = new Vector3Int( x, 0, z );
		tile.worldCoords = world.WorldCoords( this.coords, tile.coords );

		tile.transform.name = tile.worldCoords.x + "," + tile.worldCoords.z;

		instances[ x, z ] = tile;

		// Add all our road tile instances to the world roadTiles variable so spawners can use them.
		if(tiles[ x, z ].name.StartsWith( "road" ))
			world.roadTiles.Add( tile );
	}

	string TileKey( int x, int z )
	{
		return x + "," + z;
	}

	public bool CoordsAreWithinBounds( Vector3Int coords )
	{
		return coords.x > 0 && coords.x < size && coords.z > 0 && coords.z < size;
	}

	public List<Vector3Int> GetNeighbours( Vector3Int coords )
	{
		var neighbours = new List<Vector3Int>();

		Vector3Int[] offsets =
		{
			new Vector3Int(1, 0, 0),
			new Vector3Int(-1, 0, 0),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1),
		};

		foreach(var offset in offsets)
			if(CoordsAreWithinBounds( coords + offset ))
				neighbours.Add( coords + offset );

		return neighbours;
	}

	public List<Vector3Int> GetEmptyNeighbours( Vector3Int coords )
	{
		var neighbours = GetNeighbours( coords );

		foreach(var neighbour in neighbours)
			if(tiles[ neighbour.x, neighbour.z ] != null)
				neighbours.Remove( neighbour );

		return neighbours;
	}
}


