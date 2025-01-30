using System.Collections.Generic;
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

	public Tile emptyTile;

	/// <summary>
	/// Data structure that holds a representation of the chunk (from which the scene can be built),
	/// queried, and modified.
	/// </summary>
	public GameObject[,] tiles;

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
			}
		}
	}

	public void GeneratePointsOfInterest()
	{
		// Randomly determine if this chunk has a PoI (ideally every chunk does, but if we don't
		// have enough PoI for the world size then some chunks won't.  However, we do want to place
		// all the PoI we have, so we want to increase the probability of placing a PoI in a chunk
		// with every chunk we go through until it becomes 100% chance.
		var chunkIndex = coords.z * world.size + coords.x;
		var chunksRemaining = world.size * world.size - chunkIndex;
		var poiRemaining = world.poiTilePool.Count;
		var spawnPoi = false;

		if(poiRemaining > 0)
		{
			// If we have enough poi for the remaining chunks, then we always spawn one.
			// If we don't have enough poi for every chunk, then give this chunk an equal chance of
			// spawning one to the others.
			if(poiRemaining >= chunksRemaining)
				spawnPoi = true;
			else
				spawnPoi = UnityEngine.Random.Range( 0, chunksRemaining ) == 0;
		}

		Debug.Log( $"Chunk > GeneratePointsOfInterest > spawnPoi: {spawnPoi}, chunkIndex: {chunkIndex}, chunksRemaining: {chunksRemaining}, poiRemaining: {poiRemaining}" );

		if(spawnPoi)
		{
			var poiTile = world.TakeRandomPoi();
			Debug.Log( $"Chunk > GeneratePointsOfInterest > poiTile.name: {poiTile.name}" );

			// Define the offsets that represent the space that the poi will sit in, along with the
			// offsets that represent the 'neighbours' of that square, excluding diagonals.
			// e.g. (where p is a poi offset and n is a neighbour offset)
			//  nn
			// nppn
			// nppn
			//  nn
			var poiOffsets = new List<Vector3Int>();
			var neighbourOffsets = new List<Vector3Int>();

			for(int z = 0; z < poiTile.size; z++)
			{
				neighbourOffsets.Add( new Vector3Int( -1, 0, z ) );
				neighbourOffsets.Add( new Vector3Int( poiTile.size, 0, z ) );

				for(int x = 0; x < poiTile.size; x++)
				{
					poiOffsets.Add( new Vector3Int( x, 0, z ) );
					neighbourOffsets.Add( new Vector3Int( x, 0, -1 ) );
					neighbourOffsets.Add( new Vector3Int( x, 0, poiTile.size ) );
				}
			}

			// Loop through each tile in the chunk and test to see if our poiOffsets are all null, and that
			// at least one of our neighbourOffsets is a road tile.  That would be a valid place to put the
			// poi tile, and can be added to a list to later pick from.
			var validLocations = new List<Vector3Int>();
			for(int z = 0; z < size - poiTile.size; z++)
			{
				for(int x = 0; x < size - poiTile.size; x++)
				{
					var location = new Vector3Int( x, 0, z );
					var tileFits = true;

					foreach(var offset in poiOffsets)
					{
						var footprintLocation = location + offset;
						if(tiles[ footprintLocation.x, footprintLocation.z ] != null)
						{
							tileFits = false;
							break;
						}
					}

					// If the tile fits then we need to check that there is a roadtile as one of our neighbours,
					// if so then it's a valid location.
					if(tileFits)
					{
						foreach(var offset in neighbourOffsets)
						{
							var neighbourLocation = location + offset;
							if(CoordsAreWithinBounds( neighbourLocation ))
							{
								var neighbourTile = tiles[ neighbourLocation.x, neighbourLocation.z ];
								if(neighbourTile != null && neighbourTile.name.StartsWith( "road" ))
									validLocations.Add( location );
							}
						}
					}
				}
			}

			// If there are any valid locations, then randomly pick one and place the poi tile there.
			if(validLocations.Count > 0)
			{
				var poiLocation = validLocations[ UnityEngine.Random.Range( 0, validLocations.Count ) ];

				// We need to place 'Empty' tiles at the tiles that our poi covers, such that other
				// generation steps don't place tiles within them.
				foreach(var offset in poiOffsets)
				{
					var emptyLocation = poiLocation + offset;

					Debug.Log( $"Placing Empty at {emptyLocation.x},{emptyLocation.z}" );

					if(CoordsAreWithinBounds( emptyLocation ))
						tiles[ emptyLocation.x, emptyLocation.z ] = emptyTile.gameObject;
					else
						Debug.Log( $"Could not place Empty at {emptyLocation.x},{emptyLocation.z}" );
				}

				tiles[ poiLocation.x, poiLocation.z ] = poiTile.gameObject;
			}
			else
			{
				// There were no valid locations, so lets return the poi tile to the pool.  This shouldn't
				// happen as the chunk generation is currently defined, but if we change that then it may be
				// possible.
				Debug.Log( $"No place found for poi tile: {poiTile.name} ({poiTile.size}x{poiTile.size})" );
				world.poiTilePool.Add( poiTile.gameObject );
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

		Debug.Log( "Chunk > Building Tile: " + x + "," + z + " (" + tileObject.name + ")" );

		Tile tile = tileObject.GetComponent<Tile>();
		tile.chunk = this;
		tile.coords = new Vector3Int( x, 0, z );
		tile.worldCoords = world.WorldCoords( this.coords, tile.coords );
		tile.transform.name = tile.worldCoords.x + "," + tile.worldCoords.z;

		// Calculate the position of our tile, adjusting for tiles that are larger than 1.
		// TODO: Do this in one nice calculation; it's simple but somehow my mathmatical brain is
		// letting me down today!
		var position = new Vector3( x * tileSize, 0.0f, z * tileSize );
		if(tile.size == 2)
		{
			position.x += 0.5f * tileSize;
			position.z += 0.5f * tileSize;
		}
		else if(tile.size > 2)
		{
			position.x += (tile.size / 2) * tileSize;
			position.z += (tile.size / 2) * tileSize;
		}
		tileObject.transform.localPosition = position;

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
		return coords.x >= 0 && coords.x < size && coords.z >= 0 && coords.z < size;
	}
}
