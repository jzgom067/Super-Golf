using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Icon", menuName = "Objects/Icon")]
public class Icon : ScriptableObject
{
    public Material image = null;
    public Transform follow;
    public Vector3 offset;

    [Header("Game States")]
    public bool menu;
    public bool changingTurn;
    public bool underwater;
    public bool aiming;
    public bool swinging;
    public bool flying;
    public bool winning;
    [Header("Camera Positions")]
    public bool touring;
    public bool driver;
    public bool wedge;
    public bool putter;
    [Header("Spells")]
    public bool theWorld;
    public bool preZacPull;
    public bool postZacPull;
    public bool zacPlacing;
    public bool reverbing;

    public bool isActive;

    public bool GetIsActive()
    {
        return isActive;
    }
}
