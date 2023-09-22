using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cam;

    public bool position;
    public bool rotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (position && rotation)
        {
            transform.SetPositionAndRotation(cam.position, cam.rotation);
        }
        else if (position)
        {
            transform.position = cam.position;
        }
    }
}
