/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 TerrainNoiseManager v2.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainNoiseManager
{

    // ### TO REDO ###
    public float xPerlinOffset = 1000f;// 1000000f;
    public float yPerlinOffset = 1000f;// 1000000f;

    NoiseLayer[] noiseLayers;

    public TerrainNoiseManager (NoiseLayer[] noiseLayers) {
        this.noiseLayers = noiseLayers;
    }

    /**
    * Generate a noise chunk at coordinates between offset, offset+size
    */
    public float[,] GenerateNoiseChunk (Vector2 offset, Vector2 size, int vertsPerSide)
    {
        // Initialise noiseMap
        float[,] noiseMap = new float [vertsPerSide, vertsPerSide];
        
        // Initialise amplitude and frequency
        float amplitudeStart = 1f;
        float frequencyStart = 1f;


        // ### TO REDO ###
        // Calculate normalising factor
        float a = amplitudeStart;
        float heightRangeHalf = 0f;
        for (int octaveId = 0; octaveId < this.noiseLayers[0].octaves; octaveId ++)
        {
            heightRangeHalf += a; // 1 * a
            a *= this.noiseLayers[0].amplitudeMult; // increment (decrement) persistence after each octave
        }
        // ### TO REDO ###


        // Generate vertex coordinates
        float[] xVerticesRel = Functions.LinearRange (0, size.x, vertsPerSide);
        float[] yVerticesRel = Functions.LinearRange (0, size.y, vertsPerSide);

        // Initialise noise parameters
        float amplitude;
        float frequency;
        float noiseHeight;

        // Initialise coordinates
        float xVertexRel;
        float yVertexRel;

        // Loop through map pixels
        for (int yVertexId = 0; yVertexId < vertsPerSide; yVertexId ++)
        {
            // Get y vertex coordinate
            yVertexRel = offset.y + yVerticesRel[yVertexId];
            
            for (int xVertexId = 0; xVertexId < vertsPerSide; xVertexId ++)
            {
                // Get x vertex coordinate
                xVertexRel = offset.x + xVerticesRel[xVertexId];

                // Initialise noiseHeight
                noiseHeight = 0f;

                for (int noiseLayerId = 0; noiseLayerId < this.noiseLayers.Length; noiseLayerId ++)
                {

                    NoiseLayer noiseLayer = this.noiseLayers [noiseLayerId];
                    if (!noiseLayer.renderLayer) { continue; };

                    float noiseHeightLayer = 0f;

                    amplitude = amplitudeStart;
                    frequency = frequencyStart;

                    // Initialise sampling coordinates
                    float xSampling;
                    float ySampling;
                    float noiseValue;

                    for (int octaveId = 0; octaveId < noiseLayer.octaves; octaveId ++)
                    {
                        xSampling = xVertexRel / noiseLayer.scale * frequency;
                        ySampling = yVertexRel / noiseLayer.scale * frequency;
                        xSampling += this.xPerlinOffset;
                        ySampling += this.yPerlinOffset;

                        noiseValue = Mathf.PerlinNoise (xSampling, ySampling) * 2 - 1; // cast to range [-1, 1]

                        noiseHeightLayer += noiseValue * amplitude;

                        amplitude *= noiseLayer.amplitudeMult; // increment (decrement) persistence after each octave
                        frequency *= noiseLayer.frequencyMult; // increment frequency
                    }

                    noiseHeightLayer *= noiseLayer.multiplier;
                    noiseHeight += noiseHeightLayer;
                }
                
                // Apply to noiseMap
                noiseMap [xVertexId, yVertexId] = noiseHeight / (2 * heightRangeHalf);
            }
        }

        return noiseMap;

    }
}



[System.Serializable]
public struct NoiseLayer {
    public string name;
    public bool renderLayer;
    public int seed;

    public float scale; // = 3f;
    public int octaves; // = 4;
    public float amplitudeMult; // = 2f;
    public float frequencyMult; // = 10f;

    //useless
    public float multiplier; // = 1f;
}
