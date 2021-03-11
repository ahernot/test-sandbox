using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class objectBehaviour : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true; // auto updating
    // public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    // public FaceRenderMask faceRenderMask;




    public Vector3 position;
    public Color color;
    // public Mesh objectMesh;

    public objectBehaviour () {
        position = new Vector3();
        color = new Color();
        // objectMesh = new Mesh();

        updatePosition();
        Initialize();

        // updateMesh();
    }
 
    public void updateMesh ()
    {
        // GetComponent<MeshFilter>().mesh = this.objectMesh; // update object mesh
    }

    void Initialize()
    {

        // Initialize shapeGenerator
        shapeGenerator.UpdateSettings(shapeSettings);

        // Initialize colourGenerator
        colourGenerator.UpdateSettings(colourSettings);
        
        // Reinitialise mesh filters if array is empty
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>(); // .sharedMaterial = new Material(Shader.Find("Standard")); // mesh renderer
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            // Set material
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);

            // Render face by face
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }

    }
    

    public void updatePosition ()
    {
        // Vector3 positionDelta = new Vector3(2f, 2f, 0);
        // position = position + positionDelta;
    }


}
