using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkMesh : MonoBehaviour
{

    // Chunk coordinates
    [HideInInspector]
    public int xChunk = 0;
    [HideInInspector]
    public int zChunk = 0;

    // Chunk dimensions (determined by ChunkManager)
    public int xChunkSize = 16;
    public int zChunkSize = 16;
    // Chunk mesh dimensions
    public float xMeshSize = 1f;
    public float zMeshSize = 1f;
    // Chunk vertex dimensions (vertices per side = x + 1)
    int xVerticesHigh;
    int zVerticesHigh;

    public enum MeshResolution {Low, Medium, High};
    public MeshResolution meshResolution;

    // Initialise mesh resolutions
    Mesh meshLow;
    Mesh meshMed;
    Mesh meshHigh;

    // Low = default (flat)
    Vector3[] verticesLow;
    int[] trianglesLow;
    Vector2[] uvsLow;

    // Med = low poly relief
    Vector3[] verticesMed;
    int[] trianglesMed;
    Vector2[] uvsMed;

    // High = high poly
    Vector3[] verticesHigh;
    int[] trianglesHigh;
    Vector2[] uvsHigh;

    float[,] noiseMap;
    public float noiseMultiplier = 20f;//5f;


    // Start is called before the first frame update
    void Start ()
    {
        // Calculate dimensions
        this.xVerticesHigh = (int)Mathf.Ceil(this.xChunkSize / this.xMeshSize); // there are xVerticesHigh + 1 vertices per side
        this.zVerticesHigh = (int)Mathf.Ceil(this.zChunkSize / this.zMeshSize);

        // Generate noise map
        this.GenerateNoiseMap();

        // Generate all meshes
        this.GenerateMeshLow();
        this.GenerateMeshMed();
        this.GenerateMeshHigh();
    }

    void Update ()
    {
        // Set chosen mesh
        this.SetMesh();
    }


    void GenerateNoiseMap ()
    {
        int mapWidth = this.xVerticesHigh + 1;
        int mapHeight = this.zVerticesHigh + 1;

        int xOffset = this.xChunk * this.xVerticesHigh;
        int zOffset = this.zChunk * this.zVerticesHigh;
        Vector2 offset = new Vector2 (xOffset, zOffset);

        int seed = 0;
        float scale = 0.2f * Mathf.Min(this.xMeshSize, this.zMeshSize);
        int octaves = 4;
        float persistence = 1f;
        float lacunarity = 0.3f;

        Noise noise = new Noise();
        // this.noiseMap = noise.GenerateNoiseMap (mapWidth, mapHeight, seed, scale, octaves, persistence, lacunarity, offset);
        this.noiseMap = noise.GenerateNoiseMapTest (mapWidth, mapHeight, seed, scale, offset);
    }
    
    void SetMesh ()
    {
        if (this.meshResolution == MeshResolution.Low) {
            GetComponent<MeshFilter>().mesh = this.meshLow;
            GetComponent<MeshCollider>().sharedMesh = this.meshLow;
        } else if (this.meshResolution == MeshResolution.Medium) {
            GetComponent<MeshFilter>().mesh = this.meshMed;
            GetComponent<MeshCollider>().sharedMesh = this.meshMed;
        } else if (this.meshResolution == MeshResolution.High) {
            GetComponent<MeshFilter>().mesh = this.meshHigh;
            GetComponent<MeshCollider>().sharedMesh = this.meshHigh;
        }

    }

    // only called once
    private void GenerateMeshLow ()
    {
        // Initialise mesh
        this.meshLow = new Mesh();

        // Generate vertices and uvs
        this.verticesLow = new Vector3 [4];
        this.uvsLow = new Vector2 [4];
        int i = 0;
        for (int zVertexRel = 0; zVertexRel <= this.zChunkSize; zVertexRel += this.zChunkSize) // increment to only select bounds
        {
            for (int xVertexRel = 0; xVertexRel <= this.xChunkSize; xVertexRel += this.xChunkSize) // increment to only select bounds
            {
                // Add vertex
                this.verticesLow [i] = new Vector3 (xVertexRel, 0, zVertexRel);

                // Add uv
                this.uvsLow [i] = new Vector2 (
                    (float) xVertexRel / this.xChunkSize,
                    (float) zVertexRel / this.zChunkSize
                );

                i ++;
            }
        }

        // Generate triangles
        this.trianglesLow = new int [6] {
            0, 3, 1,
            0, 2, 3
        };

        // Fill mesh
        this.meshLow .Clear();
        this.meshLow.vertices = this.verticesLow;
        this.meshLow.triangles = this.trianglesLow;
        this.meshLow.uv = this.uvsLow;
        this.meshLow .RecalculateNormals();
    }

    private void GenerateMeshMed ()
    {
        // Initialise mesh
        this.meshMed = new Mesh();

        int xStep = 4;
        int zStep = 4;
        int xPoints = (int)Mathf.Ceil(this.xChunkSize / xStep) + 1; // nb of points to plot
        int zPoints = (int)Mathf.Ceil(this.zChunkSize / zStep) + 1;

        float yNoise;

        // Generate vertices and uvs
        this.verticesMed = new Vector3 [xPoints * zPoints];
        this.uvsMed = new Vector2 [this.verticesMed .Length];
        int i = 0;

        for (int zVertexRel = 0; zVertexRel < this.zChunkSize; zVertexRel += zStep)
        {
            for (int xVertexRel = 0; xVertexRel < this.xChunkSize; xVertexRel += xStep)
            {
                // Add vertex
                yNoise = this.noiseMap[xVertexRel, zVertexRel];
                this.verticesMed [i] = new Vector3 (xVertexRel, yNoise * this.noiseMultiplier, zVertexRel);

                // Add uv
                this.uvsMed [i] = new Vector2 (
                    (float) xVertexRel / this.xChunkSize,
                    (float) zVertexRel / this.zChunkSize
                );

                i ++;
            }

            // Add end vertices and uvs for x=MAX
            yNoise = this.noiseMap[this.xChunkSize, zVertexRel];
            this.verticesMed [i] = new Vector3 (this.xChunkSize, yNoise * this.noiseMultiplier, zVertexRel);
            this.uvsMed [i] = new Vector2 (
                (float) this.xChunkSize / this.xChunkSize,
                (float) zVertexRel / this.zChunkSize
            );
            i ++;
        }

        // Add end vertices and uvs for z=MAX
        for (int xVertexRel = 0; xVertexRel < this.xChunkSize; xVertexRel += xStep)
        {
            // Add end vertex
            yNoise = this.noiseMap[xVertexRel, this.zChunkSize];
            this.verticesMed [i] = new Vector3 (xVertexRel, yNoise * this.noiseMultiplier, this.zChunkSize);
            // Add end uv
            this.uvsMed [i] = new Vector2 (
                (float) xVertexRel / this.xChunkSize,
                (float) this.zChunkSize / this.zChunkSize
            );
            i ++;
        }

        // Add final vertex
        yNoise = this.noiseMap[this.xChunkSize, this.zChunkSize];
        this.verticesMed [i] = new Vector3 (this.xChunkSize, yNoise * this.noiseMultiplier, this.zChunkSize);
        // Add end uv
        this.uvsMed [i] = new Vector2 (
            (float) this.xChunkSize / this.xChunkSize,
            (float) this.zChunkSize / this.zChunkSize
        );


        // Generate triangles
        this.trianglesMed = new int [(xPoints - 1) * (zPoints - 1) * 6];
        int vert = 0;
        int tris = 0;
        for (int zVertexId = 0; zVertexId < zPoints - 1; zVertexId ++)
        {
            for (int xVertexId = 0; xVertexId < xPoints - 1; xVertexId ++)
            {
                this.trianglesMed [tris + 0] = vert + 0;
                this.trianglesMed [tris + 1] = vert + xPoints;
                this.trianglesMed [tris + 2] = vert + 1;
                this.trianglesMed [tris + 3] = vert + 1;
                this.trianglesMed [tris + 4] = vert + xPoints;
                this.trianglesMed [tris + 5] = vert + xPoints + 1;

                vert ++;
                tris += 6;
            }
            vert ++;
        }

        // Fill mesh
        this.meshMed .Clear();
        this.meshMed.vertices = this.verticesMed;
        this.meshMed.triangles = this.trianglesMed;
        this.meshMed.uv = this.uvsMed;
        this.meshMed .RecalculateNormals();
    }

    private void GenerateMeshHigh ()
    {

        // Initialise mesh
        this.meshHigh = new Mesh();
        
        // Generate vertices and uvs
        this.verticesHigh = new Vector3 [(this.xVerticesHigh + 1) * (this.zVerticesHigh + 1)];
        this.uvsHigh = new Vector2 [this.verticesHigh .Length];

        float xVertexRel;
        float yVertexRel;
        float zVertexRel;
        int i = 0;
        for (int zVertexId = 0; zVertexId <= this.zVerticesHigh; zVertexId ++)
        {
            for (int xVertexId = 0; xVertexId <= this.zVerticesHigh; xVertexId ++)
            {

                xVertexRel = xVertexId * this.xMeshSize;
                zVertexRel = zVertexId * this.zMeshSize;
                yVertexRel = this.noiseMap[xVertexId, zVertexId] * this.noiseMultiplier;

                // Add vertex
                this.verticesHigh [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

                // Add uv
                this.uvsHigh [i] = new Vector2 (
                    (float) xVertexRel / this.xChunkSize,
                    (float) zVertexRel / this.zChunkSize
                );

                i ++;
            }
        }

        // Generate triangles
        this.trianglesHigh = new int [this.xVerticesHigh * this.zVerticesHigh * 6];
        int vert = 0;
        int tris = 0;
        for (int zVertexId = 0; zVertexId < this.xVerticesHigh; zVertexId ++)
        {
            for (int xVertexId = 0; xVertexId < this.zVerticesHigh; xVertexId ++)
            {
                this.trianglesHigh [tris + 0] = vert + 0;
                this.trianglesHigh [tris + 1] = vert + this.xVerticesHigh + 1;
                this.trianglesHigh [tris + 2] = vert + 1;
                this.trianglesHigh [tris + 3] = vert + 1;
                this.trianglesHigh [tris + 4] = vert + this.xVerticesHigh + 1;
                this.trianglesHigh [tris + 5] = vert + this.xVerticesHigh + 2;

                vert ++;
                tris += 6;
            }
            vert ++;
        }

        // Fill mesh
        this.meshHigh .Clear();
        this.meshHigh.vertices = this.verticesHigh;
        this.meshHigh.triangles = this.trianglesHigh;
        this.meshHigh.uv = this.uvsHigh;
        this.meshHigh .RecalculateNormals();
    }

}
