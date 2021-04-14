using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))] // Require a mesh filter component in the object
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    // Renderer renderer;

    Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;

    // Start is called before the first frame update
    void Start ()
    {
        this.mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;

        // this.renderer = GetComponent<Renderer>();
        // Material mat = Resources.Load("Assets/Materials/Sand-test-HD") as Material;
        // this.renderer.material = mat;

        this.CreateShape();
        this.UpdateMesh();
        
    }

    void CreateShape ()
    {
        this.vertices = new Vector3[(this.xSize + 1) * (this.zSize + 1)];

        // Create vertices
        int i = 0;
        for (int z = 0; z <= this.zSize; z++)
        {
            for (int x = 0; x <= this.xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                this.vertices [i] = new Vector3 (x, y, z);
                i++;
            }
        }

        this.triangles = new int[this.xSize * this.zSize * 6];

        // Create triangles
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < this.zSize; z++)
        {
            for (int x = 0; x < this.xSize; x++)
            {
                this.triangles [tris + 0] = vert + 0;
                this.triangles [tris + 1] = vert + this.xSize + 1;
                this.triangles [tris + 2] = vert + 1;
                this.triangles [tris + 3] = vert + 1;
                this.triangles [tris + 4] = vert + this.xSize + 1;
                this.triangles [tris + 5] = vert + this.xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }
    }


    void UpdateMesh ()
    {
        // Clear current mesh
        mesh.Clear();

        mesh.vertices = this.vertices;
        mesh.triangles = this.triangles;

        mesh.RecalculateNormals();
    }


    private void OnDrawGizmos ()
    {
        if (this.vertices == null)
        {
            return;
        }

        for (int i = 0; i < this.vertices.Length; i++)
        {
            Gizmos.DrawSphere(this.vertices[i], .1f);
        }
    }

}
