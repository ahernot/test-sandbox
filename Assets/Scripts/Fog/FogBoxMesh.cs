/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 FogBoxMesh v1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogBoxMesh : MonoBehaviour
{
    public float yMin;
    public float yMax;
    public float radius;

    [Range(3, 512)]
    public int resolution; // half resolution

    [Range(2, 64)]
    public int yResolution = 2;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        // Generate & apply mesh
        this.GenerateMesh();
        this.SetMesh(); 
    }

    public void GenerateMesh ()
    {
        this.mesh = new Mesh();

        // Initialise vertices array
        this.vertices = new Vector3 [(this.resolution * 2) * (this.yResolution)];

        // Calculate radius squared
        float radiusSquared = Mathf.Pow (this.radius, 2);
        int verticesPerCircle = this.resolution * 2;

        // Match range [-1, 1] to [-π/2, π/2] to sample points from sin(x)
        float stepLin = (float) Mathf.PI / (this.resolution - 1);
        
        float xVertexLin;
        float yVertexLin;
        float xVertexRel;
        float yVertexRel;
        float zVertexRel;

        // Fill vertices array
        for (int yVertexId = 0; yVertexId < this.yResolution; yVertexId ++)
        {
            // Calculate vertex y-coordinate
            yVertexLin = yVertexId / (this.yResolution - 1); // match to range [0, 1]
            yVertexRel = (this.yMax - this.yMin) * yVertexLin + this.yMin; // linear curve

            // Number of vertices already added
            int vertIncrement = yVertexId * verticesPerCircle;

            // Initial vertex
            xVertexLin = -1f * Mathf.PI / 2f;

            for (int circVertexId = 0; circVertexId < this.resolution; circVertexId ++)
            {
                xVertexRel = Mathf.Sin (xVertexLin) * this.radius;
                zVertexRel = Mathf.Sqrt ( radiusSquared - Mathf.Pow (xVertexRel, 2) );

                // Add vertices (symmetrical shape)
                this.vertices [vertIncrement + circVertexId] = new Vector3 (xVertexRel, yVertexRel, -1 * zVertexRel); // bottom half of the circle (zVertexRel <= 0)
                this.vertices [vertIncrement + (verticesPerCircle - circVertexId - 1)] = new Vector3 (xVertexRel, yVertexRel, zVertexRel); // top half of the circle (zVertexRel >= 0)

                // Increment xVertexLin
                xVertexLin += stepLin;
            }
        }

        // Initialise triangles array
        this.triangles = new int [(verticesPerCircle * 2) * (this.yResolution - 1) * 6];

        // Fill triangles array
        int tris = 0;
        for (int yVertexId = 0; yVertexId < this.yResolution - 1; yVertexId ++)
        {
            // Number of vertices already added
            int vertIncrement = yVertexId * verticesPerCircle;

            for (int vert = 0; vert < 2 * this.resolution - 1; vert ++)
            {

                this.triangles [tris + 0] = vertIncrement + vert;
                this.triangles [tris + 2] = vertIncrement + (vert + verticesPerCircle);
                this.triangles [tris + 1] = vertIncrement + (vert + verticesPerCircle + 1);

                this.triangles [tris + 3] = vertIncrement + vert;
                this.triangles [tris + 5] = vertIncrement + (vert + verticesPerCircle + 1);
                this.triangles [tris + 4] = vertIncrement + (vert + 1);

                tris += 6;
            }

            this.triangles [tris + 0] = vertIncrement + (verticesPerCircle - 1);
            this.triangles [tris + 2] = vertIncrement + (2 * verticesPerCircle - 1);
            this.triangles [tris + 1] = vertIncrement + verticesPerCircle;

            this.triangles [tris + 3] = vertIncrement + (verticesPerCircle - 1);
            this.triangles [tris + 5] = vertIncrement + verticesPerCircle;
            this.triangles [tris + 4] = vertIncrement;

            tris += 6;
        }

        // Fill mesh
        this.mesh .Clear();
        this.mesh.vertices = this.vertices;
        this.mesh.triangles = this.triangles;
        this.mesh .RecalculateNormals();

    }


    public void SetMesh ()
    {
        GetComponent<MeshFilter>().mesh = this.mesh;
    }
}
