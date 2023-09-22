using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraXZ : MonoBehaviour
{
    Camera camera;
    [SerializeField] float waterHeight;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(camera.transform.position.x, waterHeight, camera.transform.position.z);
    }
}
