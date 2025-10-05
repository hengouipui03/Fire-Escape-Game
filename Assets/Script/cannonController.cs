using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonController : MonoBehaviour
{
    public int speed;
    public float friction, lerpspeed;
    float xdeg, ydeg;
    Quaternion fromRot, toRot;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        xdeg -= Input.GetAxis("Mouse Y") * speed * friction;
        ydeg -= Input.GetAxis("Mouse X") * speed * friction;
        fromRot = transform.rotation;
        toRot = Quaternion.Euler(0, ydeg, xdeg);
        transform.rotation = Quaternion.Lerp(fromRot, toRot, Time.deltaTime * lerpspeed);
    }
}
