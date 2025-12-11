using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltBg : MonoBehaviour
{
    public float tiltSpeed = 5;
    public float maxMoveDistance = 50;
  

    void FixedUpdate()
    {

        transform.Translate(Input.acceleration.x * tiltSpeed * Time.deltaTime, 0, 0);
       // Debug.Log("tilt "+transform.localPosition.x);
        if (transform.localPosition.x > maxMoveDistance)
        {
            transform.localPosition = new Vector3(49.9f, 0, 0);

        }

        if (transform.localPosition.x < -maxMoveDistance)
        {
            transform.localPosition = new Vector3(-49.9f, 0, 0);

        }
    }
}
