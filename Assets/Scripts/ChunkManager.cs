using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    [Tooltip("Player (chunk loader)")]
    public GameObject player;

    [Tooltip("Chunk material")]
    public Material material;

    // List of chunks
    [HideInInspector]
    public GameObject[] chunks;

    // [Space(30)]
    

    [Header("Generation Settings")]
    // Number of chunks
    [Tooltip("Half number of chunks on the x-axis")]
    public int xHalfNbChunks = 64;
    [Tooltip("Half number of chunks on the z-axis")]
    public int zHalfNbChunks = 64;

    // Chunks to load (square of side 2x+1)
    [Tooltip("Number of chunks to load in each direction around the player")]
    public int loadHighRadius = 8;


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

    // Player chunk position
    int xChunkPlayer;
    int zChunkPlayer;

    // Noise settings
    [Header("Noise Settings")]
    public float noiseScale = 3f;
    public int noiseOctaves = 4;
    public float noiseAmplitudeMult = 2f;
    public float noiseFrequencyMult = 10f;

    public float noiseMultiplier = 1f;

    void Start ()
    {
        this.DestroyChunks();
        this.GenerateChunks();        
    }

    public void GenerateChunks ()
    {
        // Initialise chunks array
        this.chunks = new GameObject[this.xHalfNbChunks * this.zHalfNbChunks * 4];

        int i = 0;
        for (int xChunk = -1 * this.xHalfNbChunks; xChunk < this.xHalfNbChunks; xChunk++)
        {
            for (int zChunk = -1 * this.zHalfNbChunks; zChunk < this.zHalfNbChunks; zChunk++)
            {
                // Initialise empty GameObject
                this.chunks[i] = new GameObject();
                this.chunks[i] .name = "Chunk_" + xChunk.ToString() + "_" + zChunk.ToString();
                this.chunks[i] .transform.parent = gameObject.transform; // set parent
                this.chunks[i] .layer = 8;

                // Update position and rotation
                this.chunks[i] .transform.position = new Vector3(xChunk * this.xChunkSize, 0, zChunk * this.zChunkSize);
                this.chunks[i] .transform.rotation = Quaternion.identity;

                // Add necessary components
                this.chunks[i] .AddComponent<MeshFilter>();
                this.chunks[i] .AddComponent<MeshCollider>();
                this.chunks[i] .AddComponent<MeshRenderer>();

                MeshRenderer meshRenderer = this.chunks[i] .GetComponent<MeshRenderer>();
                meshRenderer.material = (Material)Instantiate(this.material);

                ChunkMesh chunkMesh = this.chunks[i] .AddComponent<ChunkMesh>();
                chunkMesh.xChunk = xChunk;
                chunkMesh.zChunk = zChunk;
                chunkMesh.xChunkSize = this.xChunkSize;
                chunkMesh.zChunkSize = this.zChunkSize;
                chunkMesh.xNbPolygons = this.xNbPolygons;
                chunkMesh.zNbPolygons = this.zNbPolygons;

                chunkMesh.noiseScale = this.noiseScale;
                chunkMesh.noiseOctaves = this.noiseOctaves;
                chunkMesh.noiseAmplitudeMult = this.noiseAmplitudeMult;
                chunkMesh.noiseFrequencyMult = this.noiseFrequencyMult;
                chunkMesh.noiseMultiplier = this.noiseMultiplier;

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
        int i = 0;
        for (int xChunk = -1 * this.xHalfNbChunks; xChunk < this.xHalfNbChunks; xChunk++)
        {
            for (int zChunk = -1 * this.zHalfNbChunks; zChunk < this.zHalfNbChunks; zChunk++)
            {
                // Continue if chunk out of bounds (nb of chunks was increased but not yet generated)
                if (i >= this.chunks.Length) { continue; }

                // Get chunk's mesh script
                ChunkMesh chunkMesh = this.chunks[i] .GetComponent<ChunkMesh>();

                if ((zChunk >= this.zChunkPlayer - this.loadHighRadius) && (zChunk <= this.zChunkPlayer + this.loadHighRadius))
                {
                    if ((xChunk >= this.xChunkPlayer - this.loadHighRadius) && (xChunk <= this.xChunkPlayer + this.loadHighRadius))
                    {
                        chunkMesh.meshResolution = ChunkMesh.MeshResolution.High;
                    } else {
                        chunkMesh.meshResolution = ChunkMesh.MeshResolution.Medium;
                    };
                } else {
                    chunkMesh.meshResolution = ChunkMesh.MeshResolution.Medium;
                };

                i ++;
            }
        }

    }

}
