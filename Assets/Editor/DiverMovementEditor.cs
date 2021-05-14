/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 DiverMovementEditor v1.1
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (DiverMovement))]
public class DiverMovementEditor : Editor
{
    // Override default inspector
    public override void OnInspectorGUI()
    {
        // Fetch target
        DiverMovement diverMovement = (DiverMovement)target;

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
