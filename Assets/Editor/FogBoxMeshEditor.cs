/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 FogBoxMeshEditor v1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FogBoxMesh))]
public class FogBoxMeshEditor : Editor
{
    // Override default inspector
    public override void OnInspectorGUI()
    {
        // Fetch target
        FogBoxMesh fogBoxMesh = (FogBoxMesh)target;

        // Draw default inspector
        if (DrawDefaultInspector())
        {
            DrawDefaultInspector();
        }

        EditorGUILayout.Space();

        // Draw RegenerateForces button
        if (GUILayout.Button ("Regenerate Forces"))
        {
            diverMovement.RegenerateForces();
        }

        // Draw ResetMovement button
        if (GUILayout.Button ("Reset Movement"))
        {
            diverMovement.ResetMovement();
        }

    }
}
