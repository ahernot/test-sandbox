using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int octaves = 2;


        float height = 0f;
        for (int octaveId = 0; octaveId < octaves; octaveId ++)
        {
            // height += Mathf.Cos ();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



[System.Serializable]
public struct WaveLayer {
    public string name;
    public bool renderLayer;

    public float noiseScale; // = 3f;
    public int octaves; // = 4;
    public float noiseAmplitudeMult; // = 2f;
    public float noiseFrequencyMult; // = 10f;

    public float noiseMultiplier; // = 1f;
}

// public Vector2 speed;
// public Vector2 scale;
// public float height;
// public bool alternate;
