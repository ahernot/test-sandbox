using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager
{

    public WaveLayer[] waveLayers;

    // Constructor
    public WaveManager (WaveLayer[] waveLayers)
    {
        this.waveLayers = waveLayers;
    }

    // Start is called before the first frame update
    public float WaveHeight (Vector2 position)
    {

        float height = 0f;
        for (int layerId = 0; layerId < this.waveLayers.Length; layerId ++)
        {
            WaveLayer layer = this.waveLayers [layerId];
            float dotProduct = Vector2.Dot (position, layer.direction);
            height += (float) Mathf.Cos (dotProduct - layer.speed * Time.time) * layer.amplitude;
        }
        return height;
        
    }


    public float WaveHeight (float x, float y)
    {
        Vector2 position = new Vector2 (x, y);
        return this.WaveHeight (position);
    }

}


[System.Serializable]
public struct WaveLayer {
    public bool renderLayer;

    public Vector2 direction;
    public float amplitude;
    public float speed;
}

// public Vector2 speed;
// public Vector2 scale;
// public float height;
// public bool alternate;
