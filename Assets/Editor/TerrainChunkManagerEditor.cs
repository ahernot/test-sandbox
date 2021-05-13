using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (TerrainChunkManager))]
public class TerrainChunkManagerEditor : Editor
{
    // Override default inspector
    public override void OnInspectorGUI()
    {
        // Fetch target
        TerrainChunkManager terrainChunkManager = (TerrainChunkManager)target;

        // Draw default inspector
        if (DrawDefaultInspector())
        {
            DrawDefaultInspector();
        }

        // Draw Generate button
        if (GUILayout.Button ("Generate"))
        {
            terrainChunkManager.DestroyChunks();
            terrainChunkManager.GenerateChunks();
        }

    }
}
