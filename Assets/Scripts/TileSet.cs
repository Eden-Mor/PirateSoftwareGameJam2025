using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a set of tiles used in the world generation process.
/// </summary>
public class TileSet : MonoBehaviour
{
	/// <summary>
	/// Array of tiles that make-up this tile set.
	/// </summary>
	public Tile[] tiles;

	/// <summary>
	/// Weight curve for the tileset.  Used to sample the probability of this tileset being selected 
	/// at a specific location in the world.  White means higher probability, black no probability.
	/// 
	/// TODO: This probably shouldn't be here, but at the moment it doesn't particularly matter.
	/// </summary>
	public AnimationCurve weight;
}
