using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clubs : MonoBehaviour
{
    public string[] clubNames;
    public float[] multipliers;
    public Vector2[] angles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getNum()
    {
        return clubNames.Length;
    }

    public string getName(int i)
    {
        return clubNames[i];
    }

    public float getMultiplier(int i)
    {
        return multipliers[i];
    }

    public Vector2 getAngle(int i)
    {
        return angles[i];
    }
}
