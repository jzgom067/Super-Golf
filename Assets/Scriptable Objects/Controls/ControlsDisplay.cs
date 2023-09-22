using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Controls Display", menuName = "Objects/Controls Display")]
public class ControlsDisplay : ScriptableObject
{
    [TextArea(10, 10)]
    [SerializeField] string controls;

    public string GetString()
    {
        return controls;
    }
}
