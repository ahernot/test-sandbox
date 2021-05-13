/*
 Copyright Anatole Hernot, 2021
 All rights reserved
 Inspired by https://www.youtube.com/watch?v=_Ij24zRI9J0
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
// [RequireComponent(typeof(MeshCollider))]
public class WaterChunkMesh : MonoBehaviour
{

    // Chunk coordinates (chunk manager always at x=0,z=0)
    [HideInInspector]
    public int xChunk = 0; // in chunks
    public int xOffset = 0; // in world
    [HideInInspector]
    public int zChunk = 0; // in chunks
    public int zOffset = 0; // in world


    [Header("Generation Settings")]
    // Chunk dimensions (determined by ChunkManager)
    public int xChunkSize = 16;
    public int zChunkSize = 16;

    // Chunk mesh dimensions
    public int xNbPolygons = 32;
    public int zNbPolygons = 32;
    float[] xVerticesRel;
    float[] zVerticesRel;
    
    [Header("Mesh Resolution")]
    [Range(1, 128)]
    public int xReductionRatio = 4;
    [Range(1, 128)]
    public int zReductionRatio = 4;

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
    // Medium mesh parameters (calculated once)
    private int xNbPolygonsMed;
    private int zNbPolygonsMed;
    private int xIdStep;
    private int zIdStep;

    // High = high poly
    Vector3[] verticesHigh;
    int[] trianglesHigh;
    Vector2[] uvsHigh;

    // Wave parameters
    public WaveLayer[] waveLayers;
    WaveManager waveManager;



    /**
    * Clamp parameters to prevent errors
    **/
    void ClampParameters ()
    {
        if (this.xChunkSize <= 0)
        {
            this.xChunkSize = 1;
        }
        if (this.zChunkSize <= 0)
        {
            this.zChunkSize = 1;
        }

        if (this.xNbPolygons <= 0)
        {
            this.xNbPolygons = 1;
        }
        if (this.zNbPolygons <= 0)
        {
            this.zNbPolygons = 1;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int layerId = 0; layerId < this.waveLayers.Length; layerId ++)
        {
            WaveLayer layer = this.waveLayers [layerId];
            if (layer.renderLayer)
            {
                Vector3 direction = new Vector3 ( Mathf.Cos(layer.directionRadians), 0f, Mathf.Sin(layer.directionRadians) );
                Gizmos.DrawRay(transform.position, direction);
            }
        }
    }

    /**
    * Initialise chunk
    * Start is called before the first frame update
    **/
    void Start ()
    {

        this.waveManager = new WaveManager (this.waveLayers);

        // Check parameters
        this.ClampParameters();

        // Calculate coordinates in world
        this.xOffset = this.xChunk * this.xChunkSize;
        this.zOffset = this.zChunk * this.zChunkSize;

        // Initialise vertices' relative positions
        this.xVerticesRel = new Functions().LinearRange (0, this.xChunkSize, this.xNbPolygons + 1);
        this.zVerticesRel = new Functions().LinearRange (0, this.zChunkSize, this.zNbPolygons + 1);

        // Initialise medium mesh parameters
        // Calculate number of polygons per side
        this.xNbPolygonsMed = (int) Mathf.Ceil (this.xNbPolygons / this.xReductionRatio); // max between this and 1
        if (this.xNbPolygonsMed <= 0) { this.xNbPolygonsMed = 1; }
        this.zNbPolygonsMed = (int) Mathf.Ceil (this.zNbPolygons / this.zReductionRatio); // max between this and 1
        if (this.zNbPolygonsMed <= 0) { this.zNbPolygonsMed = 1; }
        // Calculate index step
        this.xIdStep = (int) Mathf.Floor (this.xNbPolygons / this.xNbPolygonsMed); // floor to avoid overrun
        this.zIdStep = (int) Mathf.Floor (this.zNbPolygons / this.zNbPolygonsMed); // floor to avoid overrun

        // Generate all meshes
        this.GenerateMeshLow();
        this.GenerateMeshMed();
        this.GenerateMeshHigh();
    }

    /**
    * Update chunk
    **/
    void Update ()
    {        
        this.UpdateMeshHigh();
        // this.UpdateMeshMed();

        // Set chosen mesh
        this.SetMesh();

    }

    /**
    * Destroy chunk
    **/
    ~WaterChunkMesh ()
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
    }

    /**
    * Set mesh (low / med / high)
    **/
    void SetMesh ()
    {
        if (this.meshResolution == MeshResolution.Low) {
            GetComponent<MeshFilter>().mesh = this.meshLow;
            // GetComponent<MeshCollider>().sharedMesh = this.meshLow;
        } else if (this.meshResolution == MeshResolution.Medium) {
            GetComponent<MeshFilter>().mesh = this.meshMed;
            // GetComponent<MeshCollider>().sharedMesh = this.meshMed;
        } else if (this.meshResolution == MeshResolution.High) {
            GetComponent<MeshFilter>().mesh = this.meshHigh;
            // GetComponent<MeshCollider>().sharedMesh = this.meshHigh;
        }

    }

    /**
    * Generate low resolution mesh (flat)
    * Run on instance initialisation
    **/
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
            0, 1, 3,
            0, 3, 2
        };

        // Fill mesh
        this.meshLow .Clear();
        this.meshLow.vertices = this.verticesLow;
        this.meshLow.triangles = this.trianglesLow;
        this.meshLow.uv = this.uvsLow;
        this.meshLow .RecalculateNormals();
    }

    /**
    * Generate medium resolution mesh (low detail)
    * Run on instance initialisation
    **/
    private void GenerateMeshMed ()
    {
        // Initialise mesh
        this.meshMed = new Mesh();

        // Initialise vertices Vector3 array
        this.verticesMed = new Vector3 [(this.xNbPolygonsMed + 1) * (this.zNbPolygonsMed + 1)];
        // Initialise uvs Vector2 array
        this.uvsMed = new Vector2 [this.verticesMed .Length];

        // Initialise relative vertex positions
        float xVertexRel;
        float yVertexRel;
        float zVertexRel;

        int i = 0;
        for (int zVertexId = 0; zVertexId < this.zNbPolygonsMed; zVertexId ++)
        {
            // Get z vertex coordinate
            zVertexRel = this.zVerticesRel [zVertexId * this.zIdStep];

            for (int xVertexId = 0; xVertexId < this.xNbPolygonsMed; xVertexId ++)
            {
                // Get x vertex coordinate
                xVertexRel = this.xVerticesRel [xVertexId * this.xIdStep];

                // Get y vertex coordinate using the noise map
                yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

                // Add vertex
                this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

                // Add uv
                this.uvsMed [i] = new Vector2 (
                    (float) xVertexRel / this.xChunkSize,
                    (float) zVertexRel / this.zChunkSize
                );

                i ++;
            }

            // Add end vertices and uvs for x=X_MAX
            xVertexRel = this.xVerticesRel [this.xNbPolygons];
            yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

            this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

            this.uvsMed [i] = new Vector2 (
                (float) xVertexRel / this.xChunkSize,
                (float) zVertexRel / this.zChunkSize
            );

            i ++;

        }

        // Add end vertices and uvs for z=Z_MAX
        zVertexRel = this.zVerticesRel [this.zNbPolygons];
        for (int xVertexId = 0; xVertexId < this.xNbPolygonsMed; xVertexId ++)
        {
            xVertexRel = this.xVerticesRel [xVertexId * this.xIdStep];
            yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

            this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

            this.uvsMed [i] = new Vector2 (
                (float) xVertexRel / this.xChunkSize,
                (float) zVertexRel / this.zChunkSize
            );

            i ++;
        }

        // Add final vertex
        xVertexRel = this.xVerticesRel [this.xNbPolygons];
        zVertexRel = this.zVerticesRel [this.zNbPolygons];
        yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

        this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

        this.uvsMed [i] = new Vector2 (
            (float) xVertexRel / this.xChunkSize,
            (float) zVertexRel / this.zChunkSize
        );

        // Generate triangles
        this.trianglesMed = new int [this.xNbPolygonsMed * this.zNbPolygonsMed * 6];
        int vert = 0;
        int tris = 0;
        for (int zVertexId = 0; zVertexId < this.zNbPolygonsMed; zVertexId ++)
        {
            for (int xVertexId = 0; xVertexId < this.xNbPolygonsMed; xVertexId ++)
            {
                this.trianglesMed [tris + 0] = vert + 0;
                this.trianglesMed [tris + 2] = vert + xNbPolygonsMed + 1;
                this.trianglesMed [tris + 1] = vert + 1;
                this.trianglesMed [tris + 3] = vert + 1;
                this.trianglesMed [tris + 5] = vert + this.xNbPolygonsMed + 1;
                this.trianglesMed [tris + 4] = vert + this.xNbPolygonsMed + 2;

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

    /**
    * Generate high resolution mesh (high detail)
    * Run on instance initialisation
    **/
    private void GenerateMeshHigh ()
    {

        // Initialise mesh
        this.meshHigh = new Mesh();

        // Initialise vertices Vector3 array
        this.verticesHigh = new Vector3 [(this.xNbPolygons + 1) * (this.zNbPolygons + 1)];
        // Initialise uvs Vector2 array
        this.uvsHigh = new Vector2 [this.verticesHigh .Length];

        // Initialise relative vertex positions
        float xVertexRel;
        float yVertexRel;
        float zVertexRel;

        int i = 0;
        for (int zVertexId = 0; zVertexId <= this.zNbPolygons; zVertexId ++)
        {
            // Get z vertex coordinate
            zVertexRel = this.zVerticesRel [zVertexId];

            for (int xVertexId = 0; xVertexId <= this.xNbPolygons; xVertexId ++)
            {
                // Get x vertex coordinate
                xVertexRel = this.xVerticesRel [xVertexId];

                // Get y vertex coordinate using the noise map
                yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

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
                this.trianglesHigh [tris + 2] = vert + this.xNbPolygons + 1;
                this.trianglesHigh [tris + 1] = vert + 1;
                this.trianglesHigh [tris + 3] = vert + 1;
                this.trianglesHigh [tris + 5] = vert + this.xNbPolygons + 1;
                this.trianglesHigh [tris + 4] = vert + this.xNbPolygons + 2;

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

    private void UpdateMeshMed ()
    {

        // Initialise relative vertex positions
        float xVertexRel;
        float yVertexRel;
        float zVertexRel;

        int i = 0;
        for (int zVertexId = 0; zVertexId < this.zNbPolygonsMed; zVertexId ++)
        {
            // Get z vertex coordinate
            zVertexRel = this.zVerticesRel [zVertexId * this.zIdStep];

            for (int xVertexId = 0; xVertexId < this.xNbPolygonsMed; xVertexId ++)
            {
                // Get x vertex coordinate
                xVertexRel = this.xVerticesRel [xVertexId * this.xIdStep];

                // Get y vertex coordinate using the noise map
                yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

                // Add vertex
                this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

                // Add uv
                this.uvsMed [i] = new Vector2 (
                    (float) xVertexRel / this.xChunkSize,
                    (float) zVertexRel / this.zChunkSize
                );

                i ++;
            }

            // Add end vertices and uvs for x=X_MAX
            xVertexRel = this.xVerticesRel [this.xNbPolygons];
            yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

            this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

            this.uvsMed [i] = new Vector2 (
                (float) xVertexRel / this.xChunkSize,
                (float) zVertexRel / this.zChunkSize
            );

            i ++;

        }

        // Add end vertices and uvs for z=Z_MAX
        zVertexRel = this.zVerticesRel [this.zNbPolygons];
        for (int xVertexId = 0; xVertexId < this.xNbPolygonsMed; xVertexId ++)
        {
            xVertexRel = this.xVerticesRel [xVertexId * this.xIdStep];
            yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

            this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

            this.uvsMed [i] = new Vector2 (
                (float) xVertexRel / this.xChunkSize,
                (float) zVertexRel / this.zChunkSize
            );

            i ++;
        }

        // Add final vertex
        xVertexRel = this.xVerticesRel [this.xNbPolygons];
        zVertexRel = this.zVerticesRel [this.zNbPolygons];
        yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

        this.verticesMed [i] = new Vector3 (xVertexRel, yVertexRel, zVertexRel);

        this.uvsMed [i] = new Vector2 (
            (float) xVertexRel / this.xChunkSize,
            (float) zVertexRel / this.zChunkSize
        );

        // Fill mesh
        this.meshMed.vertices = this.verticesMed;
        this.meshMed.uv = this.uvsMed;
        this.meshMed .RecalculateNormals();
    }

    private void UpdateMeshHigh ()
    {

        // Initialise relative vertex positions
        float xVertexRel;
        float yVertexRel;
        float zVertexRel;

        int i = 0;
        for (int zVertexId = 0; zVertexId <= this.zNbPolygons; zVertexId ++)
        {
            // Get z vertex coordinate
            zVertexRel = this.zVerticesRel [zVertexId];

            for (int xVertexId = 0; xVertexId <= this.xNbPolygons; xVertexId ++)
            {
                // Get x vertex coordinate
                xVertexRel = this.xVerticesRel [xVertexId];

                // Get y vertex coordinate using the noise map
                // yVertexRel = this.WaveHeight (xVertexRel, zVertexRel);
                yVertexRel = this.waveManager.WaveHeight (xVertexRel + this.xOffset, zVertexRel + this.zOffset);

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

        // Fill mesh
        this.meshHigh.vertices = this.verticesHigh;
        this.meshHigh.uv = this.uvsHigh;
        this.meshHigh .RecalculateNormals();
    }

}
