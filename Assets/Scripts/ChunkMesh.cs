using System;
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

    private int x; // x position in world
    private int z; // z position in world

    // Chunk dimensions (determined by ChunkManager)
    public int xChunkSize = 16;
    public int zChunkSize = 16;

    // Chunk mesh dimensions
    public int xNbPolygons = 32;
    public int zNbPolygons = 32;
    // public float xMeshSize = 0.5f;
    // public float zMeshSize = 0.5f;

    // Chunk vertex number (vertices per side = x + 1)
    // int xVerticesHigh;
    // int zVerticesHigh;

    // Mesh resolution
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
    public float noiseMultiplier = 1f; //20f;//5f;

    // Noise settings
    public float noiseScale = 3f;
    public int noiseOctaves = 4;
    public float noiseAmplitudeMult = 2f;
    public float noiseFrequencyMult = 10f;


    // Start is called before the first frame update
    void Start ()
    {
        // Calculate dimensions
        // this.xVerticesHigh = (int)Mathf.Ceil(this.xChunkSize / this.xMeshSize); // there are xVerticesHigh + 1 vertices per side
        // this.zVerticesHigh = (int)Mathf.Ceil(this.zChunkSize / this.zMeshSize);

        // Set world position of chunk (assuming that all the chunks are the same size)
        this.x = this.xChunk * this.xChunkSize;
        this.z = this.zChunk * this.zChunkSize;

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

    ~ChunkMesh ()
    {
        this.meshLow.Clear();
        this.meshMed.Clear();
        this.meshHigh.Clear();

        Array.Clear(this.verticesLow, 0, this.verticesLow.Length);
        Array.Clear(this.trianglesLow, 0, this.trianglesLow.Length);
        Array.Clear(this.uvsLow, 0, this.uvsLow.Length);

        Array.Clear(this.verticesMed, 0, this.verticesMed.Length);
        Array.Clear(this.trianglesMed, 0, this.trianglesMed.Length);
        Array.Clear(this.uvsMed, 0, this.uvsMed.Length);

        Array.Clear(this.verticesHigh, 0, this.verticesHigh.Length);
        Array.Clear(this.trianglesHigh, 0, this.trianglesHigh.Length);
        Array.Clear(this.uvsHigh, 0, this.uvsHigh.Length);

        Array.Clear(this.noiseMap, 0, this.noiseMap.Length);
    }

    // TO DO
    void GenerateNoiseMap ()
    {
        int mapWidth = this.xNbPolygons + 1;
        int mapHeight = this.zNbPolygons + 1;

        int xOffset = this.xChunk * this.xNbPolygons;
        int zOffset = this.zChunk * this.zNbPolygons;
        Vector2 offset = new Vector2 (xOffset, zOffset);

        int seed = 0;
        Noise noise = new Noise();

        // Ask for noise map with same coordinates as chunk; noise function will scale coordinates to its liking
        this.noiseMap = noise.GenerateNoiseMapNew (mapWidth, mapHeight, seed, this.noiseScale, this.noiseOctaves, this.noiseAmplitudeMult, this.noiseFrequencyMult, offset);
        
        // this.noiseMap = noise.GenerateNoiseMap (mapWidth, mapHeight, seed, scale, octaves, persistence, lacunarity, offset);
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

    // only called once
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

    // only called once
    private void GenerateMeshHigh ()
    {

        // Initialise mesh
        this.meshHigh = new Mesh();

        // Initialise vertices
        float[] xVerticesRel = this.LinearRange(0, this.xChunkSize, this.xNbPolygons + 1);
        float[] zVerticesRel = this.LinearRange(this.z, this.z + this.zChunkSize, this.zNbPolygons + 1);

        // Initialise vertices Vector3 array
        this.verticesHigh = new Vector3 [(this.xNbPolygons + 1) * (this.zNbPolygons + 1)];
        this.uvsHigh = new Vector2 [this.verticesHigh .Length];

        // Initialise relative vertex positions
        float xVertexRel;
        float yVertexRel;
        float zVertexRel;

        int i = 0;
        for (int zVertexId = 0; zVertexId <= this.zNbPolygons; zVertexId ++)
        {
            for (int xVertexId = 0; xVertexId <= this.xNbPolygons; xVertexId ++)
            {

                // Get x and z vertex coordinates
                xVertexRel = xVerticesRel [xVertexId];
                zVertexRel = zVerticesRel [zVertexId];

                // Get y vertex coordinate using the noise map
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

        // Initialise triangles int array
        this.trianglesHigh = new int [this.xNbPolygons * this.zNbPolygons * 6];

        int vert = 0;
        int tris = 0;
        for (int zVertexId = 0; zVertexId < this.zNbPolygons; zVertexId ++)
        {
            for (int xVertexId = 0; xVertexId < this.xNbPolygons; xVertexId ++)
            {
                this.trianglesHigh [tris + 0] = vert + 0;
                this.trianglesHigh [tris + 1] = vert + this.xNbPolygons + 1;
                this.trianglesHigh [tris + 2] = vert + 1;
                this.trianglesHigh [tris + 3] = vert + 1;
                this.trianglesHigh [tris + 4] = vert + this.xNbPolygons + 1;
                this.trianglesHigh [tris + 5] = vert + this.xNbPolygons + 2;

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



    private float[] LinearRange (float start, float stop, int nbPoints)
    {
        float[] range = new float [nbPoints];

        float step = (stop - start) / (float)(nbPoints - 1);
        float point;

        // Fill range
        for (int i = 0; i < nbPoints; i ++)
        {
            point = i * step;
            range [i] = point;
        }

        return range;
    }

}
