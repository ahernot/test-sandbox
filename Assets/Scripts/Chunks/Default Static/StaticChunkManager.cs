/*
 Copyright Anatole Hernot, 2021
 All rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticChunkManager : MonoBehaviour
{

    [Tooltip("Player (chunk loader)")]
    public GameObject player;
    [Tooltip("Player camera")]
    public Camera viewCamera;

    [Tooltip("Name of the chunks")]
    public string chunkName = "StaticChunk";
    public int layer;

    [Tooltip("Chunk material")]
    public Material material;

    [Tooltip("Add mesh colliders")]
    public bool meshCollider = false;

    // List of chunks
    [HideInInspector]
    public GameObject[] chunks;

    [Header("Generation Settings")]
    // Number of chunks
    [Tooltip("Half number of chunks on the x-axis")]
    public int xHalfNbChunks = 64;
    [Tooltip("Half number of chunks on the z-axis")]
    public int zHalfNbChunks = 64;

    // Chunks to load (square of side 2x+1)
    [Tooltip("Number of chunks to load in each direction around the player")]
    public int loadHighRadius = 8;
    public int loadRadius = 12;


    [Header("Chunk Settings")]
    // Size of a chunk (affects texture size)
    [Tooltip("x-axis size of the chunk (in world units)")]
    public int xChunkSize = 16;
    [Tooltip("z-axis size of the chunk (in world units)")]
    public int zChunkSize = 16;

    // Chunk mesh poly count (fineness)
    [Tooltip("Number of polygons per chunk along the x-axis")]
    public int xNbPolygons = 32;
    [Tooltip("Number of polygons per chunk along the z-axis")]
    public int zNbPolygons = 32;

    [Header("Mesh Resolution")]
    [Tooltip("Mesh reduction ratio along the x-axis")]
    [Range(1, 128)]
    public int xReductionRatio = 4;
    [Tooltip("Mesh reduction ratio along the z-axis")]
    [Range(1, 128)]
    public int zReductionRatio = 4;


    // Player chunk position (updated at runtime)
    int xChunkPlayer;
    int zChunkPlayer;

    [Space(30)]
    [Tooltip("Optimize chunk loading by hiding chunks behind player")]
    public bool optimizeLoading = false;





    void Start ()
    {
        this.DestroyChunks();
        this.GenerateChunks();
    }

    /**
    * Regenerate the chunks (with their noise maps)
    **/
    public void GenerateChunks ()
    {

        // Initialise chunks array
        this.chunks = new GameObject[this.xHalfNbChunks * this.zHalfNbChunks * 4];

        int i = 0;
        for (int xChunkId = -1 * this.xHalfNbChunks; xChunkId < this.xHalfNbChunks; xChunkId++)
        {
            for (int zChunkId = -1 * this.zHalfNbChunks; zChunkId < this.zHalfNbChunks; zChunkId++)
            {
                // Initialise empty GameObject
                this.chunks[i] = new GameObject();
                this.chunks[i] .name = this.chunkName + "_" + xChunkId.ToString() + "_" + zChunkId.ToString();
                this.chunks[i] .transform.parent = gameObject.transform; // set parent
                this.chunks[i] .layer = this.layer;

                // Update position and rotation
                this.chunks[i] .transform.position = new Vector3(xChunkId * this.xChunkSize, gameObject.transform.position.y, zChunkId * this.zChunkSize);
                this.chunks[i] .transform.rotation = Quaternion.identity;

                // Add necessary components
                this.chunks[i] .AddComponent<MeshFilter>();
                this.chunks[i] .AddComponent<MeshRenderer>();

                MeshRenderer meshRenderer = this.chunks[i] .GetComponent<MeshRenderer>();
                meshRenderer.material = (Material)Instantiate(this.material);

                // Create StaticChunkMesh component
                StaticChunkMesh staticChunkMesh = this.chunks[i] .AddComponent<StaticChunkMesh>();

                // Set StaticChunkMesh parameters
                staticChunkMesh.xChunk = xChunkId;
                staticChunkMesh.zChunk = zChunkId;
                staticChunkMesh.xChunkSize = this.xChunkSize;
                staticChunkMesh.zChunkSize = this.zChunkSize;
                staticChunkMesh.xNbPolygons = this.xNbPolygons;
                staticChunkMesh.zNbPolygons = this.zNbPolygons;
                staticChunkMesh.xReductionRatio = this.xReductionRatio;
                staticChunkMesh.zReductionRatio = this.zReductionRatio;
                staticChunkMesh.meshCollider = this.meshCollider;

                i++;
            }
        }
    }

    public void DestroyChunks ()
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void Update ()
    {
        this.GetPlayerChunk();
        this.UpdateResolutions();
    }

    void GetPlayerChunk ()
    {
        float x = this.player .transform.position.x;
        float z = this.player .transform.position.z;

        this.xChunkPlayer = (int) Mathf.Floor((float)x / this.xChunkSize);
        this.zChunkPlayer = (int) Mathf.Floor((float)z / this.zChunkSize);
    }

    void UpdateResolutions ()
    {

        // Get camera direction
        Vector2 cameraForward2D = new Vector2 ();  // blank vector to prevent errors
        if (this.optimizeLoading == true) {
            Vector3 cameraForward = this.viewCamera .transform.forward;
            cameraForward2D = new Vector2 (cameraForward.x, cameraForward.z);    
        }

        int i = 0;
        for (int xChunkId = -1 * this.xHalfNbChunks; xChunkId < this.xHalfNbChunks; xChunkId++)
        {
            for (int zChunkId = -1 * this.zHalfNbChunks; zChunkId < this.zHalfNbChunks; zChunkId++)
            {
                // Continue if chunk out of bounds (nb of chunks was increased but not yet generated)
                if (i >= this.chunks.Length) { continue; }

                // Get chunk's mesh script
                StaticChunkMesh staticChunkMesh = this.chunks[i] .GetComponent<StaticChunkMesh>();

                // Calculate horizontal distance to chunk
                float playerToChunkDist = Mathf.Sqrt ( Mathf.Pow (xChunkId - this.xChunkPlayer, 2) + Mathf.Pow(zChunkId - this.zChunkPlayer, 2) );

                // Update resolutions based on distance
                if (playerToChunkDist > this.loadRadius) {
                    staticChunkMesh.meshResolution = StaticChunkMesh.MeshResolution.Low;
                    this.chunks[i] .SetActive (false);
                } else if (playerToChunkDist > this.loadHighRadius) {
                    this.chunks[i] .SetActive (true);
                    staticChunkMesh.meshResolution = StaticChunkMesh.MeshResolution.Medium;
                } else {
                    this.chunks[i] .SetActive (true);
                    staticChunkMesh.meshResolution = StaticChunkMesh.MeshResolution.High;
                }

                // Loading optimisation (overwrites previously activated chunks)
                if (this.optimizeLoading == true) {
                    Vector2 playerToChunk = new Vector2 (xChunkId - this.xChunkPlayer, zChunkId - this.zChunkPlayer);
                    float distanceToChunk = playerToChunk.magnitude;
                    if ((Vector2.Dot (cameraForward2D, playerToChunk) / distanceToChunk < -0.2f) && (distanceToChunk > 2f))  // would work better with 3D dot product?
                    {
                        this.chunks[i] .SetActive (false);
                    }
                }

                i ++;
            }
        }

    }

}
