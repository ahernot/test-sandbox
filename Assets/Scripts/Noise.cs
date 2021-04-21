using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise // static because no need for multiple instances of this script
{

    public float xPerlinOffset = 1000000f;
    public float yPerlinOffset = 1000000f;

    public float[,] GenerateNoiseMap (int xSize, int ySize, int xNbVertices, int yNbVertices, int seed, float scale, int octaves, float amplitudeMult, float frequencyMult, Vector2 offset)
    {
        // Initialise noiseMap
        float[,] noiseMap = new float [xNbVertices, yNbVertices];
        
        // Initialise amplitude and frequency
        float amplitudeStart = 1f;
        float frequencyStart = 1f;

        // Calculate normalising factor
        float a = amplitudeStart;
        float heightRangeHalf = 0f;
        for (int octaveId = 0; octaveId < octaves; octaveId ++)
        {
            heightRangeHalf += a; // 1 * a
            a *= amplitudeMult; // increment (decrement) persistence after each octave
        }

        // Generate vertex coordinates
        float[] xVerticesRel = this.LinearRange (0, xSize, xNbVertices);
        float[] yVerticesRel = this.LinearRange (0, ySize, yNbVertices);

        // Initialise noise parameters
        float amplitude;
        float frequency;
        float noiseHeight;

        // Initialise coordinates
        float xVertexRel;
        float yVertexRel;

        // Loop through map pixels
        for (int yVertexId = 0; yVertexId < yNbVertices; yVertexId ++)
        {
            // Get y vertex coordinate
            yVertexRel = offset.y + yVerticesRel[yVertexId];
            
            for (int xVertexId = 0; xVertexId < xNbVertices; xVertexId ++)
            {
                // Get x vertex coordinate
                xVertexRel = offset.x + xVerticesRel[xVertexId];

                amplitude = amplitudeStart;
                frequency = frequencyStart;
                noiseHeight = 0;

                // Initialise sampling coordinates
                float xSampling;
                float ySampling;
                float noiseValue;

                for (int octaveId = 0; octaveId < octaves; octaveId ++)
                {
                    xSampling = xVertexRel / scale * frequency;
                    ySampling = yVertexRel / scale * frequency;
                    xSampling += this.xPerlinOffset;
                    ySampling += this.yPerlinOffset;

                    noiseValue = Mathf.PerlinNoise (xSampling, ySampling) * 2 - 1; // cast to range [-1, 1]

                    noiseHeight += noiseValue * amplitude;

                    amplitude *= amplitudeMult; // increment (decrement) persistence after each octave
                    frequency *= frequencyMult; // increment frequency
                }
                
                // Apply to noiseMap
                noiseMap [xVertexId, yVertexId] = noiseHeight / (2 * heightRangeHalf);
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
