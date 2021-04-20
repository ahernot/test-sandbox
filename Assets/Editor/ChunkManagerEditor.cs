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
        // Fetch target
        ChunkManager chunkManager = (ChunkManager)target;

        // Draw default inspector
        if (DrawDefaultInspector())
        {
            DrawDefaultInspector();
        }

        // Draw Generate button
        if (GUILayout.Button ("Generate"))
        {
            chunkManager.DestroyChunks();
            chunkManager.GenerateChunks();
        }

    }
}
