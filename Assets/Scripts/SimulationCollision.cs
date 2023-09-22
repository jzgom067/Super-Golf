using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationCollision : MonoBehaviour
{
    Master master;

    void Awake()
    {
        master = Master.instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        master.GetComponent<TrajectoryPrediction>().SimulationCollision(collision.GetContact(0).point);
    }
}
