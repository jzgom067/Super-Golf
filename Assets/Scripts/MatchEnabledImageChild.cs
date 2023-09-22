using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchEnabledImageChild : MonoBehaviour
{
    [SerializeField] Image child;

    // Update is called once per frame
    void Update()
    {
        child.enabled = GetComponent<Text>().enabled;
    }
}
