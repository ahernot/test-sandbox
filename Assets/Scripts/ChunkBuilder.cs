using System.Collections;
 using UnityEngine;
 
public class ChunkBuilder : MonoBehaviour
{
    // public GameObject prefab;

    public GameObject gameObject;
    public Material material;

    // Number of chunks
    public int xSize = 8;
    public int zSize = 8;

    // Size of a chunk
    public int xChunkSize = 16;
    public int zChunkSize = 16;

    // Noise
    public Vector2 offset = Vector2.zero;
    public int seed = 0;
    public float scale;
    public int octaves = 4;
    public float persistence;
    public float lacunarity;
    public float multiplier;

    void Awake ()
    {

        GameObject chunkPrefab = new GameObject();
        chunkPrefab.name = "_SampleChunk";
        chunkPrefab.AddComponent<MeshFilter>();
        chunkPrefab.AddComponent<MeshCollider>();
        chunkPrefab.AddComponent<MeshRenderer>();

        
        // Set layer
        chunkPrefab.layer = 8;

        // Vector2 offset = new Vector2 (this.xChunk * xSize, this.zChunk * zSize);
        

        float[,] noiseMap = Noise.GenerateNoiseMap(this.xSize * this.xChunkSize + 1, this.zSize * this.zChunkSize + 1, seed, scale, octaves, persistence, lacunarity, offset);

        for (int x = 0; x < this.xSize; x++) {
            for (int z = 0; z < this.zSize; z++) {
                
                Vector3 chunkPosition = new Vector3(x * xChunkSize, 0, z * zChunkSize);
                Quaternion chunkRotation = Quaternion.identity;

                GameObject chunk = (GameObject)Instantiate(chunkPrefab, chunkPosition, chunkRotation);
                chunk.transform.parent = this.gameObject.transform;
                chunk.name = "Chunk" + x.ToString() + "-" + z.ToString();

                // Add Mesh Generator script
                MeshGenerator meshGenerator = chunk.AddComponent<MeshGenerator>();

                meshGenerator.xChunk = x;
                meshGenerator.zChunk = z;
                meshGenerator.noiseMap = noiseMap;
                meshGenerator.noiseMultiplier = this.multiplier;

                MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();
                meshRenderer.material = (Material)Instantiate(this.material);

            }
        }
    }

}
