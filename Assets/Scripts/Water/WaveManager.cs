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

        float height = Mathf.PerlinNoise (position.x, position.y); //0f;
        for (int layerId = 0; layerId < this.waveLayers.Length; layerId ++)
        {
            WaveLayer layer = this.waveLayers [layerId];
            if (layer.renderLayer)
            {
                // float dotProduct = Vector2.Dot (position, layer.direction);
                float dotProduct = position.x * Mathf.Cos (layer.directionRadians) + position.y * Mathf.Sin (layer.directionRadians);
                float spaceOffset = 2 * Mathf.PI / layer.wavelength * dotProduct;
                float timeOffset = -1 * layer.speed * Time.time;
                height += (float) Mathf.Cos (spaceOffset + timeOffset ) * layer.amplitude;
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

    [Range(0, 360)]
    public float direction;
    [HideInInspector]
    public float directionRadians;
    
    public float amplitude; // 1.3

    // Pulsation
    public float speed; // 0.56

    // Wavelength
    public float wavelength;
}

// !! CLAMP WAVELENGTH > 0
