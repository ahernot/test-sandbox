using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    // This
    public GameObject gameObject;
    public GameObject player;

    // List of chunks
    [HideInInspector]
    public GameObject[] chunks;

    // Number of chunks
    public int xHalfNbChunks = 64;
    public int zHalfNbChunks = 64;

    // Size of a chunk
    public int xChunkSize = 16;
    public int zChunkSize = 16;
    // Chunk mesh dimensions
    public float xMeshSize = 0.5f;
    public float zMeshSize = 0.5f;

    // Player chunk position
    int xChunkPlayer;
    int zChunkPlayer;

    // Chunks to load (square of side 2x+1)
    public int loadHighRadius = 8;

    public Material material;

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
                this.chunks[i] .transform.parent = this.gameObject.transform; // set parent
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
                chunkMesh.xMeshSize = this.xMeshSize;
                chunkMesh.zMeshSize = this.zMeshSize;

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
