using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (ChunkManager))]
public class ChunkManagerEditor : Editor
{
    // Override default inspector
    public override void OnInspectorGUI()
    {
        ChunkManager chunkManager = (ChunkManager)target; // cast to a MapGenerator object

        // if (DrawDefaultInspector ())
        // {
        //     // Auto update
        //     if (mapGen.autoUpdate)
        //     {
        //         mapGen.GenerateMap ();
        //     }
        // }

        if (DrawDefaultInspector())
        {
            DrawDefaultInspector();
        }

        if (GUILayout.Button ("Generate"))
        {
            chunkManager.DestroyChunks();
            chunkManager.GenerateChunks();
        }

    }
}
