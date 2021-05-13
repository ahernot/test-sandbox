using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (WaterChunkManager))]
public class WaterChunkManagerEditor : Editor
{
    // Override default inspector
    public override void OnInspectorGUI()
    {
        // Fetch target
        WaterChunkManager waterChunkManager = (WaterChunkManager)target;

        // Draw default inspector
        if (DrawDefaultInspector())
        {
            DrawDefaultInspector();
        }

        EditorGUILayout.Space();

        // Draw Generate button
        if (GUILayout.Button ("Generate Chunks"))
        {
            waterChunkManager.DestroyChunks();
            waterChunkManager.GenerateChunks();
        }

    }
}
