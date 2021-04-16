using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))] // Require a mesh filter component in the object
// [RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    // Renderer renderer;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public float[,] noiseMap;
    public float noiseMultiplier;

    // Chunk coordinates
    public int xChunk = 0;
    public int zChunk = 0;

    // Chunk size
    public int xSize = 16;
    public int zSize = 16;

    // Start is called before the first frame update
    void Start ()
    {
        this.mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;
        this.CreateShape();
        this.UpdateMesh();
        
    }

    void CreateShape ()
    {
        this.vertices = new Vector3[(this.xSize + 1) * (this.zSize + 1)];
        this.uvs = new Vector2[this.vertices.Length]; // uvs unwrap the shape to map it to a 2D plane in [0,1]**2 for texture-mapping

        // Create vertices & uvs
        int i = 0;
        for (int z = 0; z <= this.zSize; z++)
        {
            for (int x = 0; x <= this.xSize; x++)
            {
                // float xReal = transform.position.x + x;
                // float zReal = transform.position.z + z;
                // float y = Mathf.PerlinNoise(xReal * .3f, zReal * .3f) * 5f;
                int xRealChunk = this.xChunk * this.xSize + x;
                int zRealChunk = this.zChunk * this.zSize + z;
                float y = this.noiseMap[xRealChunk, zRealChunk] * this.noiseMultiplier;
                this.vertices [i] = new Vector3 (x, y, z); // add vertex
                this.uvs [i] = new Vector2 ((float)x / this.xSize, (float)z / this.zSize); // add uv
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
        this.mesh.Clear();

        this.mesh.vertices = this.vertices;
        this.mesh.triangles = this.triangles;
        this.mesh.uv = this.uvs;

        this.mesh.RecalculateNormals();

        // Generate the mesh collider maybe
        // MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        // meshCollider.sharedMesh = meshToCollide;
        GetComponent<MeshCollider>().sharedMesh = this.mesh;
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
