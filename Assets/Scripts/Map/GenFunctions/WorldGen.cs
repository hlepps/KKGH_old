using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldGen : MonoBehaviour
{
    public abstract float[,,] Algorithm(int width, int height, int length, Vector3 pos);

}

