/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 TerrainChunkManagerEditor v1.1
*/

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

        EditorGUILayout.Space();

        // Draw Generate button
        if (GUILayout.Button ("Generate Chunks"))
        {
            terrainChunkManager.DestroyChunks();
            terrainChunkManager.GenerateChunks();
        }

    }
}
