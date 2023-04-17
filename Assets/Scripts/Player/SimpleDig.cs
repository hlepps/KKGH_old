using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDig : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public float digPower = 100f;
    public GameObject digObjectPrefab;
    private GameObject realDigObject;

    void Start()
    {
        realDigObject = Instantiate(digObjectPrefab);
    }
    Vector3 hitpos = Vector3.zero;
    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, (FlyingCam.instance != null) ? FlyingCam.instance.camera.transform.forward*15 : Vector3.zero);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(transform.position, hitpos);
    }
    void FixedUpdate()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, FlyingCam.instance.camera.transform.forward);
        if(Physics.Raycast(ray, out hit, 15, groundLayerMask))
        {
            hitpos = hit.point;
            realDigObject.transform.position = hit.point;
            if(Input.GetMouseButton(0))
            {
                MapGen.instance.UpdateMap(hit.point, realDigObject.transform.localScale.x, -digPower * Time.deltaTime);
            }
        }
    }
}
