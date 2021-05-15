/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 FogBoxManager v1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogBoxManager : MonoBehaviour
{
    [Tooltip("Player")]
    public GameObject player;

    public FogBoxPref[] fogBoxPrefs;

    GameObject[] fogBoxes;


    void Start()
    {
        this.DestroyFogBoxes();
        this.GenerateFogBoxes();
    }

    public void GenerateFogBoxes ()
    {
        // Initialise list of fog boxes
        this.fogBoxes = new GameObject [this.fogBoxPrefs.Length];

        for (int fogBoxId = 0; fogBoxId < this.fogBoxPrefs.Length; fogBoxId ++)
        {
            FogBoxPref fogBoxPref = this.fogBoxPrefs [fogBoxId];

            this.fogBoxes [fogBoxId] = new GameObject();
            this.fogBoxes [fogBoxId] .name = fogBoxPref.name; // set name
            this.fogBoxes [fogBoxId] .transform.parent = gameObject.transform; // set as chuld of FogBoxManager instance

            // Initialise around player
            this.fogBoxes [fogBoxId] .transform.position = new Vector3 (
                this.player .transform.position.x,
                0f,
                this.player .transform.position.z
            );

            // Add components
            this.fogBoxes [fogBoxId] .AddComponent<MeshFilter>();
            this.fogBoxes [fogBoxId] .AddComponent<MeshRenderer>();

            // Apply material
            this.fogBoxes [fogBoxId] .GetComponent<MeshRenderer>() .material = (Material)Instantiate (fogBoxPref.material);

            // Create FogBoxMesh instance
            FogBoxMesh fogBoxMesh = this.fogBoxes [fogBoxId] .AddComponent<FogBoxMesh>();
            fogBoxMesh.yMin = fogBoxPref.yMin;
            fogBoxMesh.yMax = fogBoxPref.yMax;
            fogBoxMesh.radius = fogBoxPref.radius;
            fogBoxMesh.resolution = fogBoxPref.resolution;
            fogBoxMesh.yResolution = fogBoxPref.yResolution;

            // Active status
            if (fogBoxPref.renderFogBox) {
                this.fogBoxes [fogBoxId] .SetActive (true);
            } else {
                this.fogBoxes [fogBoxId] .SetActive (false);
            };
        }
    }

    public void DestroyFogBoxes ()
    {
        foreach (Transform child in transform) {
            GameObject.Destroy (child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Follow player
        for (int fogBoxId = 0; fogBoxId < this.fogBoxPrefs.Length; fogBoxId ++)
        {
            this.fogBoxes [fogBoxId] .transform.position = new Vector3 (
                this.player .transform.position.x,
                0f,
                this.player .transform.position.z
            );
        }
    }
}


[System.Serializable]
public struct FogBoxPref {
    public string name;
    public bool renderFogBox;

    [Space(30)]

    [Tooltip("Box material")]
    public Material material;

    [Space(30)]

    [Header("Box size")]
    public int yMin;
    public int yMax;
    public int radius;

    [Space(30)]

    [Header("Box resolution")]
    [Tooltip("Half-circle number of vertices")]
    [Range(3, 512)]
    public int resolution;
    [Tooltip("Vertical number of vertices")]
    [Range(2, 64)]
    public int yResolution;
}
