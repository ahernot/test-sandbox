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

    void Awake()
    {

        GameObject chunkPrefab = new GameObject();
        chunkPrefab.AddComponent<MeshFilter>();
        chunkPrefab.AddComponent<MeshCollider>();
        chunkPrefab.AddComponent<MeshRenderer>();

        
        // Set layer
        chunkPrefab.layer = 8;

        // noiseMap = Noise().GenerateNoiseMap(this.xChunkSize * this.xSize, this.zChunkSize * this.zSize, 0, 0.5, 4, 0.2, 0.1, Vector2.null);


        for (int x = 0; x < this.xSize; x++) {
            for (int z = 0; z < this.zSize; z++) {
                
                Vector3 chunkPosition = new Vector3(x * xChunkSize, 0, z * zChunkSize);
                Quaternion chunkRotation = Quaternion.identity;

                GameObject chunk = (GameObject)Instantiate(chunkPrefab, chunkPosition, chunkRotation);
                chunk.transform.parent = this.gameObject.transform;
                chunk.name = "chunk" + x.ToString() + "-" + z.ToString();

                // Add Mesh Generator script
                chunk.AddComponent<MeshGenerator>();

                MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();
                meshRenderer.material = (Material)Instantiate(this.material);

            }
        }

    }
}
