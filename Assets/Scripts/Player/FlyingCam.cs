using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCam : MonoBehaviour
{
    public static FlyingCam instance;
    public float speed = 0.5f;
    public Transform camera;

    void Start()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    void Update()
    {
        transform.position += (Input.GetAxis("Horizontal") * camera.right + Input.GetAxis("Vertical") * camera.forward) * speed;
    }
}
