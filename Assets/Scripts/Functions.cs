﻿// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

public class Functions
{
    public float[] LinearRange (float start, float stop, int nbPoints)
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