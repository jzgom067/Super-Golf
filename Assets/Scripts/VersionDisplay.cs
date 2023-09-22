using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
    Text self;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Text>();
        self.text = "Version " + Application.version;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
