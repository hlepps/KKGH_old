using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterSimplex : WorldGen
{
    public List<float> scalesMultipliers = new List<float>();
    public List<float> scales = new List<float>();
    public float redistribution = 0.5f;
    public override float[,,] Algorithm(int width, int height, int length, Vector3 pos)
    {
        float[,,] ret = new float[width, height, length];
        for (int z = 0; z < length; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float finalScale = 0;
                    float sum = 0;
                    for(int i = 0; i < scales.Count; i++)
                    {
                        finalScale += scalesMultipliers[i] * SimplexNoise.Noise.CalcPixel3D(x + (int)pos.x, y + (int)pos.y, z + (int)pos.z, scales[i]);
                        sum += scalesMultipliers[i];
                    }
                    finalScale /= sum;

                    ret[x, y, z] = Mathf.Pow(finalScale, redistribution);

                }
            }
        }
        return ret;
    }

}

    

