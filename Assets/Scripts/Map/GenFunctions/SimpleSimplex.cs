using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSimplex : WorldGen
{
    public float scale;
    public override float[,,] Algorithm(int width, int height, int length, Vector3 pos)
    {
        float[,,] ret = new float[width, height, length];
        for (int z = 0; z < length; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ret[x, y, z] = SimplexNoise.Noise.CalcPixel3D(x + (int)pos.x, y + (int)pos.y, z + (int)pos.z, scale);

                }
            }
        }
        return ret;
    }
}

    

