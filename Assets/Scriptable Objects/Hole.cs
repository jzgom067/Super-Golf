using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hole", menuName = "Objects/Hole")]
public class Hole : ScriptableObject
{
    [Header("Hole Information")]
    [SerializeField] int holeIndex;
    [SerializeField] Transform green;
    [SerializeField] Transform spawnPosition;
    [SerializeField] Transform tourCenter;
    [SerializeField] float tourDistance;
    [SerializeField] Transform topDownReference;
    [SerializeField] float startingRotation;
    [SerializeField] float topDownRotation;

    public void SetReferences(Transform g, Transform sp, Transform tc, Transform td)
    {
        green = g;
        spawnPosition = sp;
        tourCenter = tc;
        topDownReference = td;
    }

    public float GetHoleIndex()
    {
        return holeIndex;
    }

    public Transform GetGreen()
    {
        return green;
    }

    public Transform GetSpawn()
    {
        return spawnPosition;
    }

    public Transform GetTourCenter()
    {
        return tourCenter;
    }

    public float GetTourDistance()
    {
        return tourDistance;
    }

    public Transform GetTopDown()
    {
        return topDownReference;
    }

    public float GetStartRotation()
    {
        return startingRotation;
    }

    public float GetTopDownRotation()
    {
        return topDownRotation;
    }

    //133.8216
    //132.39

    //1.4316 LOWER THAN TERRAIN
}
