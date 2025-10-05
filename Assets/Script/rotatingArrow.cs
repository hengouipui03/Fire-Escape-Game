using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingArrow : MonoBehaviour
{
    public float speed, distance, point;

    // Update is called once per frame
    void Update()
    {
        Transform arrow = this.gameObject.transform;
        arrow.position = new Vector3(arrow.position.x, Mathf.PingPong(Time.time * speed, distance) + point, arrow.position.z);
        arrow.rotation = Quaternion.Euler(Vector3.down * 30f);
    }
}
