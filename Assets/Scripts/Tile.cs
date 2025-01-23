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
}
