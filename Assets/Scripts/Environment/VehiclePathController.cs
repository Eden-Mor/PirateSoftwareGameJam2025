using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Splines;

public class VehiclePathController : MonoBehaviour
{
    [Header("Vehicle Properties")]

    /// <summary>
    /// The distance that the vehicle will move per-second.
    /// 
    /// TODO: Add other properties like acceleration, max speed, braking, and so on to give a more 
    /// realistic sense of movement and allow them to better interact with the world.
    /// </summary>
    public float speed = 1f;
    public float maxSpeed = 4f;
    public bool isBraking = false;
    public float brakeSensitvity = 0.1f;
    public float acceleration = 0.075f;
    public bool isPathing = true;
    [Header("Internals")]

    /// <summary>
    /// What fraction of the path has the vehicle travelled along, from 0 to 1.
    /// </summary>
    public float progress = 0f;

    /// <summary>
    /// The tile that this vehicle is currently traversing.
    /// </summary>
    public Tile tile;

    /// <summary>
    /// The tile path that this vehicle is currently traversing.
    /// </summary>
    public TilePath tilePath;

    /// <summary>
    /// Refernce to the vehicle spawner so we can request to be de-spawned.
    /// </summary>
    public VehicleSpawner spawner;
    public Rigidbody rb;

    public void Start()
    {
        UpdateTransform();
        rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false;
            isPathing = false;
        }
    }

    public void Update()
    {
        if (isPathing)
        {


            // TODO: Cache distance calculation(s) for the path.
            var path = GetPath();
            float pathLength = path.CalculateLength();
            progress += (speed / pathLength) * Time.deltaTime;

            // If we've reached the end of the path, get the next tile in our direction of travel and a path
            // from it, or despawn if there's no route.
            if (progress >= 1f)
            {
                // Maintain any excess progress into the next tile.
                // TODO: This will need to be different when using distance not percentage of path.
                progress -= 1f;

                // Get the coords of the next tile in our direction.
                Vector3Int nextTileOffset = new Vector3Int();
                if (tilePath.end == Direction.North)
                    nextTileOffset.z = 1;
                else if (tilePath.end == Direction.South)
                    nextTileOffset.z = -1;
                else if (tilePath.end == Direction.East)
                    nextTileOffset.x = 1;
                else
                    nextTileOffset.x = -1;

                Tile currentTile = tile;
                Vector3Int nextTileCoords = currentTile.worldCoords + nextTileOffset;

                // Get the next tile object or despawn if there isn't one (reached the edge of the world).
                Tile nextTile = spawner.world.GetTile(nextTileCoords);
                if (nextTile == null)
                {
                    spawner.DespawnVehicle(gameObject);
                    return;
                }

                // Get an appropriate tile path for us to follow.
                Direction invertedDirection;
                if (tilePath.end == Direction.North)
                    invertedDirection = Direction.South;
                else if (tilePath.end == Direction.South)
                    invertedDirection = Direction.North;
                else if (tilePath.end == Direction.East)
                    invertedDirection = Direction.West;
                else
                    invertedDirection = Direction.East;

                TilePath nextTilePath = nextTile.RandomTilePath(invertedDirection);

                if (nextTilePath == null)
                {
                    spawner.DespawnVehicle(gameObject);
                    return;
                }

                // Set our tile and path references to the next things and off we go.
                tile = nextTile;
                tilePath = nextTilePath;
            }

            UpdateTransform();
        }
    }

    /// <summary>
    /// Updates the position and rotation of the vehicle based on its progress along the path.
    /// </summary>
    public void UpdateTransform()
    {
        var path = GetPath();

        float3 position;
        float3 tangent;
        float3 up;

        path.Evaluate(Mathf.Clamp(progress, 0f, 1f), out position, out tangent, out up);
        Vector3 forward = Vector3.Cross(Vector3.up, tangent);

        transform.position = position;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(new Vector3(0f, -90f, 0f));
    }

    /// <summary>
    /// We often want the SplineContainer component from the tile path, this makes getting that a 
    /// fraction easier.
    /// </summary>
    /// <returns>The SplineContainer for the current tile path.</returns>
    SplineContainer GetPath()
    {
        GameObject pathObject = tilePath.path;
        return pathObject.GetComponent<SplineContainer>();
    }

    private void FixedUpdate()
    {
        if (this.isBraking == true)
        {
            if (speed > 0)
            {
                speed = Mathf.Max(speed - brakeSensitvity, 0f);
            }

        }
        else
        {
            if (speed < maxSpeed)
            {
                speed = speed + acceleration;
            }
        }
    }
}
