/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 FogBox v1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogBox : MonoBehaviour
{

    public Material material;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public float xHalfSize;
    public float zHalfSize;

    public float yMax = 0f;
    public float yMin = -50f;

    void Start()
    {
        // Generate mesh
        this.GenerateMesh();

        // Apply mesh
        gameObject .AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = this.mesh;

        // Apply material
        gameObject .AddComponent<MeshRenderer>();
        MeshRenderer meshRenderer = gameObject .GetComponent<MeshRenderer>();
        meshRenderer.material = (Material)Instantiate(this.material);
    }

    void GenerateMesh ()
    {
        this.mesh = new Mesh();
        this.vertices = new Vector3 [8];

        this.vertices [0] = new Vector3 (-1 * this.xHalfSize, yMin, -1 * this.zHalfSize);
        this.vertices [1] = new Vector3 (-1 * this.xHalfSize, yMax, -1 * this.zHalfSize);
        this.vertices [2] = new Vector3 (-1 * this.xHalfSize, yMax, this.zHalfSize);
        this.vertices [3] = new Vector3 (-1 * this.xHalfSize, yMin, this.zHalfSize);

        this.vertices [4] = new Vector3 (this.xHalfSize, yMin, this.zHalfSize);
        this.vertices [5] = new Vector3 (this.xHalfSize, yMax, this.zHalfSize);
        this.vertices [6] = new Vector3 (this.xHalfSize, yMax, -1 * this.zHalfSize);
        this.vertices [7] = new Vector3 (this.xHalfSize, yMin, -1 * this.zHalfSize);
        
        this.triangles = new int [24] {
            0, 1, 2,
            0, 2, 3,

            3, 2, 5,
            3, 5, 4,

            4, 5, 6,
            4, 6, 7,

            7, 6, 1,
            7, 1, 0
        };

        this.mesh .Clear();
        this.mesh.vertices = this.vertices;
        this.mesh.triangles = this.triangles;
        this.mesh .RecalculateNormals();
    }

}
