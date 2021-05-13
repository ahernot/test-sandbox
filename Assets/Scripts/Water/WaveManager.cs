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
            if (layer.renderLayer)
            {
                float dotProduct = Vector2.Dot (position, layer.direction);
                height += (float) Mathf.Cos (dotProduct - layer.speed * Time.time) * layer.amplitude;
            }
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

    public Vector2 direction; // (1, 3)
    public float amplitude; // 1.3
    public float speed; // 0.56
}
