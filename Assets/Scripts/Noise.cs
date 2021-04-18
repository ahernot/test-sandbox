﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise // static because no need for multiple instances of this script
{

    float xPerlinOffset = 1000000f;
    float yPerlinOffset = 1000000f;

    // returns a 2D array of floats
    public float[,] GenerateNoiseMap (int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random (seed); // pseudo-random number generator
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next (-100000, 100000) + offset.x;
            float offsetY = prng.Next (-100000, 100000) + offset.y;
            octaveOffsets [i] = new Vector2 (offsetX, offsetY); // populate array
        }

        // Clamp scale to avoid errors
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // Initialise min and max noise height values
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // coordinates of middle of map
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        // Loop through map pixels
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Octaves loop
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1; // cast to range [-1, 1]
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence; // increment (decrement) persistence after each octave
                    frequency *= lacunarity; // increment frequency
                }

                // Update max and min noise height values
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
            
                // Apply to noiseMap
                noiseMap [x, y] = noiseHeight;
            }
        }

        // Normalise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]); // cast to range [0, 1]
            }
        }

        return noiseMap;
    }


    public float[,] GenerateNoiseMapNew (int xPoints, int yPoints, int seed, float scale, int octaves, float amplitudeMult, float frequencyMult, Vector2 offset)
    {
        float[,] noiseMap = new float [xPoints, yPoints];

        float amplitudeStart = 1f;
        float frequencyStart = 1f;

        float a = amplitudeStart;
        float heightRangeHalf = 0f;
        for (int octaveId = 0; octaveId < octaves; octaveId ++)
        {
            heightRangeHalf += a; // 1 * a
            a *= amplitudeMult; // increment (decrement) persistence after each octave
        }


        float amplitude;
        float frequency;
        float noiseHeight;
        // Loop through map pixels
        for (int yId = 0; yId < yPoints; yId ++)
        {
            for (int xId = 0; xId < xPoints; xId ++)
            {
                amplitude = amplitudeStart;
                frequency = frequencyStart;
                noiseHeight = 0;

                float xSampling;
                float ySampling;
                float noiseValue;

                for (int octaveId = 0; octaveId < octaves; octaveId ++)
                {
                    xSampling = this.xPerlinOffset + (offset.x + xId) / scale * frequency;
                    ySampling = this.yPerlinOffset + (offset.y + yId) / scale * frequency;
                    noiseValue = Mathf.PerlinNoise (xSampling, ySampling) * 2 - 1; // cast to range [-1, 1]

                    noiseHeight += noiseValue * amplitude;

                    amplitude *= amplitudeMult; // increment (decrement) persistence after each octave
                    frequency *= frequencyMult; // increment frequency
                }
                
                // Apply to noiseMap
                noiseMap [xId, yId] = noiseHeight / (2 * heightRangeHalf);
            }
        }

        return noiseMap;
    }


    public float[,] GenerateNoiseMapTest (int mapWidth, int mapHeight, int seed, float scale, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float edgeValue;
        float perlinValue;
        for (int yRel = 0; yRel < mapHeight; yRel ++)
        {
            for (int xRel = 0; xRel < mapWidth; xRel ++)
            {
                perlinValue = Mathf.PerlinNoise ((xRel + offset.x) * .3f, (yRel + offset.y) * .3f);
                // Debug.Log(perlinValue);
                edgeValue = Mathf.Exp(Mathf.Max(Mathf.Abs(offset.x + xRel), Mathf.Abs(offset.y + yRel)) * 0.002f) - 1;
                noiseMap[xRel, yRel] = perlinValue * scale + edgeValue;
            }
        }

        return noiseMap;
    }
}
