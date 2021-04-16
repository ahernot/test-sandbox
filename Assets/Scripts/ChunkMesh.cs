﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkMesh : MonoBehaviour
{

    // Chunk coordinates
    int xChunk = 0;
    int zChunk = 0;
    int x;
    int z;

    // Chunk size (determined by ChunkManager)
    int xChunkSize = 16;
    int zChunkSize = 16;


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


    // public float[,] noiseMap;
    // public float noiseMultiplier;


    // Start is called before the first frame update
    void Start ()
    {
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

    public void SetMesh ()
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
        for (int zVertex = this.z; zVertex <= this.z + this.zChunkSize; zVertex += this.zChunkSize) // increment to only select bounds
        {
            for (int xVertex = this.x; xVertex <= this.x + this.xChunkSize; xVertex += this.xChunkSize) // increment to only select bounds
            {
                // Add vertex
                this.verticesLow [i] = new Vector3 (xVertex, 0, zVertex);

                // Add uv
                this.uvsLow [i] = new Vector2 (
                    (float) xVertex / this.xChunkSize,
                    (float) zVertex / this.zChunkSize
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

        // Generate vertices and uvs
        this.verticesMed = new Vector3 [xPoints * zPoints];
        this.uvsMed = new Vector2 [this.verticesMed .Length];
        int i = 0;

        for (int zVertex = this.z; zVertex < this.z + this.zChunkSize; zVertex += zStep)
        {
            for (int xVertex = this.x; xVertex < this.x + this.xChunkSize; xVertex += xStep)
            {
                // Add vertex
                this.verticesMed [i] = new Vector3 (xVertex, 0, zVertex);

                // Add uv
                this.uvsMed [i] = new Vector2 (
                    (float) xVertex / this.xChunkSize,
                    (float) zVertex / this.zChunkSize
                );

                i ++;
            }

            // Add end vertices and uvs for x=MAX
            // Add end vertex
            this.verticesMed [i] = new Vector3 (this.x + this.xChunkSize, 0, zVertex);
            // Add end uv
            this.uvsMed [i] = new Vector2 (
                (float) (this.x + this.xChunkSize) / this.xChunkSize,
                (float) zVertex / this.zChunkSize
            );
            i ++;
        }

        // Add end vertices and uvs for z=MAX
        for (int xVertex = this.x; xVertex < this.x + this.xChunkSize; xVertex += xStep)
        {
            // Add end vertex
            this.verticesMed [i] = new Vector3 (xVertex, 0, this.z + this.zChunkSize);
            // Add end uv
            this.uvsMed [i] = new Vector2 (
                (float) xVertex / this.xChunkSize,
                (float) (this.z + this.zChunkSize) / this.zChunkSize
            );
            i ++;
        }

        // Add final vertex
        this.verticesMed [i] = new Vector3 (this.x + this.xChunkSize, 0, this.z + this.zChunkSize);
        // Add end uv
        this.uvsMed [i] = new Vector2 (
            (float) (this.x + this.xChunkSize) / this.xChunkSize,
            (float) (this.z + this.zChunkSize) / this.zChunkSize
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
        this.verticesHigh = new Vector3 [(this.xChunkSize + 1) * (this.zChunkSize + 1)];
        this.uvsHigh = new Vector2 [this.verticesHigh .Length];
        int i = 0;
        for (int zVertex = this.z; zVertex <= this.z + this.zChunkSize; zVertex ++)
        {
            for (int xVertex = this.x; xVertex <= this.x + this.xChunkSize; xVertex ++)
            {
                // Add vertex
                this.verticesHigh [i] = new Vector3 (xVertex, 0, zVertex);

                // Add uv
                this.uvsHigh [i] = new Vector2 (
                    (float) xVertex / this.xChunkSize,
                    (float) zVertex / this.zChunkSize
                );

                i ++;
            }
        }

        // Generate triangles
        this.trianglesHigh = new int [this.xChunkSize * this.zChunkSize * 6];
        int vert = 0;
        int tris = 0;
        for (int zVertex = 0; zVertex < this.zChunkSize; zVertex ++)
        {
            for (int xVertex = 0; xVertex < this.xChunkSize; xVertex ++)
            {
                this.trianglesHigh [tris + 0] = vert + 0;
                this.trianglesHigh [tris + 1] = vert + this.xChunkSize + 1;
                this.trianglesHigh [tris + 2] = vert + 1;
                this.trianglesHigh [tris + 3] = vert + 1;
                this.trianglesHigh [tris + 4] = vert + this.xChunkSize + 1;
                this.trianglesHigh [tris + 5] = vert + this.xChunkSize + 2;

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
