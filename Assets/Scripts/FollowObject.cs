using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform objectToFollow;
    Transform cameraReference;

    // Start is called before the first frame update
    void Start()
    {
        cameraReference = Camera.main.GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0f, cameraReference.rotation.y, 0f));
    }

    public void setObjectToFollow(Transform obj)
    {
        objectToFollow = obj;
    }
}
