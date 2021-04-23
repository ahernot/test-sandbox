﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseManager
{

    // Number of chunks
    public int xHalfNbChunks;
    public int zHalfNbChunks;

    // Chunk mesh poly count (fineness)
    public int xNbPolygons;
    public int zNbPolygons;

    // Noise parameters
    public NoiseType[] noiseLayers;

    public List<float[,]> noiseChunks;
    public float noiseNormalise = 1f;


    public void GenerateNoiseChunks ()
    {

        int xNbChunks = this.xHalfNbChunks * 2 + 1;
        int zNbChunks = this.zHalfNbChunks * 2 + 1;

        int i = 0;
        for (int xChunkId = -1 * this.xHalfNbChunks; xChunkId < this.xHalfNbChunks; xChunkId++)
        {
            for (int zChunkId = -1 * this.zHalfNbChunks; zChunkId < this.zHalfNbChunks; zChunkId++)
            {
                
                Vector2 vertexStart = new Vector2 (this.xNbPolygons * xChunkId, this.zNbPolygons * zChunkId);
                Vector2 vertexStop = new Vector2 (vertexStart.x + this.xNbPolygons, vertexStart.y + this.zNbPolygons);

                // Generate noise chunk
                this.noiseChunks [i] = this.GenerateNoiseMap (vertexStart, vertexStop, this.xNbPolygons + 1, this.zNbPolygons + 1);

                i ++;
            }
        }

    }


    public float[,] GenerateNoiseMap (Vector2 vertexStart, Vector2 vertexStop, int xNbVertices, int yNbVertices)
    {
        
        // Starting values
        float amplitudeStart = 1f;
        float frequencyStart = 1f;

        // Initialise noiseMap
        float[,] noiseMap = new float [xNbVertices, yNbVertices];

        // Generate vertex coordinates
        float[] xVertices = this.LinearRange (vertexStart.x, vertexStop.x, xNbVertices);
        float[] yVertices = this.LinearRange (vertexStart.y, vertexStop.y, yNbVertices);

        // Loop through map layers
        for (int layerId = 0; layerId < noiseLayers.Length; layerId ++)
        {
            
            // Retrieve noise layer
            NoiseType noiseLayer = this.noiseLayers [layerId];

            // Skip layer
            if (noiseLayer.renderLayer == false)
            {
                continue;
            }

            float amplitude;
            float frequency;

            float xVertex;
            float yVertex;

            // Loop through map pixels
            for (int yVertexId = 0; yVertexId < yNbVertices; yVertexId ++)
            {
                // Get y vertex coordinate
                yVertex = yVertices [yVertexId];
                
                for (int xVertexId = 0; xVertexId < xNbVertices; xVertexId ++)
                {
                    // Get x vertex coordinate
                    xVertex = xVertices [xVertexId];

                    amplitude = amplitudeStart;
                    frequency = frequencyStart;

                    // Initialise sampling coordinates
                    float xSampling;
                    float ySampling;

                    float noiseHeight = 0f;

                    for (int octaveId = 0; octaveId < noiseLayer.noiseOctaves; octaveId ++)
                    {
                        xSampling = xVertex / noiseLayer.noiseScale * frequency;
                        ySampling = yVertex / noiseLayer.noiseScale * frequency;

                        float noiseValue = Mathf.PerlinNoise (xSampling, ySampling) * 2 - 1; // cast to range [-1, 1]

                        noiseHeight += noiseValue * amplitude;

                        amplitude *= noiseLayer.noiseAmplitudeMult; // increment (decrement) persistence after each octave
                        frequency *= noiseLayer.noiseFrequencyMult; // increment frequency
                    }
                    
                    // Apply to noiseMap
                    noiseMap [xVertexId, yVertexId] = noiseHeight; // not normalised

                    // Add normalising factor
                    if (Mathf.Abs(noiseHeight) > this.noiseNormalise)
                    {
                        this.noiseNormalise = Mathf.Abs(noiseHeight);
                    }
                }
            }
        }

    return noiseMap;

    }


    private float[] LinearRange (float start, float stop, int nbPoints)
    {
        float[] range = new float [nbPoints];

        float step = (stop - start) / (float)(nbPoints - 1);
        float point;

        // Fill the first nbPoints-1 points of the range
        for (int i = 0; i < nbPoints - 1; i ++)
        {
            point = i * step;
            range [i] = point;
        }

        // Make sure that the last point is (float)stop
        range [nbPoints - 1] = (float)stop;

        return range;
    }


}

[System.Serializable]
public struct NoiseType {
    public string name;
    public bool renderLayer;

    public float noiseScale; // = 3f;
    public int noiseOctaves; // = 4;
    public float noiseAmplitudeMult; // = 2f;
    public float noiseFrequencyMult; // = 10f;

    public float noiseMultiplier; // = 1f;
}
