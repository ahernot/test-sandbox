using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    // This
    public GameObject gameObject;

    // List of chunks
    [HideInInspector]
    public GameObject[] chunks;

    public int xHalfNbChunks = 64;
    public int zHalfNbChunks = 64;

    public int xChunkSize = 16;
    public int zChunkSize = 16;

    public Material material;


    void DestroyChunks ()
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void Start ()
    {

        this.DestroyChunks();

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

                this.chunks[i] .AddComponent<ChunkMesh>();

                i++;
            }
        }
    }


}
