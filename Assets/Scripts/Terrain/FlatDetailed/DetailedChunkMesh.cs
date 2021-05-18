using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DetailedChunkMesh : MonoBehaviour
{

    [Tooltip("List of detail meshes, from highest to lowest poly count")]
    public Mesh[] detailMeshes; // error if no meshes

    public Vector3 detailMeshRotation;

    // public float meshRotation;

    [Range(0,5)]
    public int detailMeshId;

    public Material material;

    Mesh mesh;

    void Start ()
    {
        this.detailMeshRotation = new Vector3 (270, 0, 0);
        gameObject.transform.eulerAngles = this.detailMeshRotation;

        this.SetMaterial();
        this.SetMesh();

        // this.RotateMesh();
    }


    public void SetMaterial () {
        MeshRenderer meshRenderer = gameObject .GetComponent<MeshRenderer>();
        meshRenderer.material = (Material)Instantiate (this.material);        
    }

    public void SetMesh () {
        // this.mesh = new Mesh();
        // this.mesh .vertices = this.detailMeshes [this.detailMeshId] .vertices;
        // this.mesh .uv = this.detailMeshes [this.detailMeshId] .uv;
        // this.mesh .triangles = this.detailMeshes [this.detailMeshId] .triangles;
        this.mesh = this.detailMeshes [this.detailMeshId];

        GetComponent<MeshFilter>().mesh = this.mesh;
    }

    void Update () {

        this.SetMesh();

        // Vector3 size = GetComponent<Renderer>().bounds.size;
    }


    // public void RotateMesh () 
    // {

    //     Vector3[] vertices = this.mesh .vertices;
    //     Vector3[] verticesRotated = new Vector3 [vertices.Length];

    //     Quaternion qAngle = Quaternion.AngleAxis( this.meshRotation, Vector3.up );
    //     for (int vertexId = 0; vertexId < vertices.Length; vertexId ++)
    //     {
    //         verticesRotated [vertexId] = qAngle * vertices [vertexId];
    //     }
        
    //     this.mesh .vertices = verticesRotated;
    // }

}
