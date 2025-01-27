using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bitmask enum representing the four cardinal directions.  We'll be using this to identify in which
/// directions tiles have connections to other tiles (e.g. for road tiles).
/// </summary>
[Flags]
public enum Directions
{
	North = 1,
	East = 2,
	South = 4,
	West = 8,
}

public enum Direction
{
	North = 1,
	East = 2,
	South = 4,
	West = 8,
}

[Serializable]
public class TilePath
{
	public Direction start;
	public Direction end;
	public GameObject path;
}

/// <summary>
/// Represents a single tile that's used for generating the world.  The main information we store 
/// here is a reference to the prefab to instantiate, but we also store other information that's
/// needed in the generation process, for example in which directions the tile has connections.
/// </summary>
[Serializable]
public class Tile : MonoBehaviour
{
	/// <summary>
	/// The cardinal directions that this tile has connections with, which will be used in some 
	/// generation algorithms (e.g. for connecting roads together).
	/// </summary>
	public Directions connections;

	public TilePath[] paths;

	public Chunk chunk;
	public Vector3Int coords = new Vector3Int();
	public Vector3Int worldCoords = new Vector3Int();
	public Transform pickupPointsParent;

	public TilePath RandomTilePath()
	{
		if(paths.Length > 0)
			return paths[ UnityEngine.Random.Range( 0, paths.Length ) ];

		return null;
	}

	public TilePath RandomTilePath( Direction direction )
	{
		if(paths.Length > 0)
		{
			// Get all the paths that have a start matching the specified direction.
			List<TilePath> directionPaths = new List<TilePath>();
			foreach(TilePath path in paths)
				if(path.start == direction)
					directionPaths.Add( path );

			if(directionPaths.Count > 0)
				return directionPaths[ UnityEngine.Random.Range( 0, directionPaths.Count ) ];
		}

		return null;
	}

	public Transform[] GetTilePickupPoints()
	{
		if (pickupPointsParent == null || pickupPointsParent.childCount <= 0)
			return null;

		Transform[] children = new Transform[pickupPointsParent.childCount];
        for (int i = 0; i < children.Length; i++)
			children[i] = pickupPointsParent.GetChild(i);

		return children;
	}
}
