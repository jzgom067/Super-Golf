using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchEnabledText : MonoBehaviour
{
    Text self;
    [SerializeField] Text child;
    [SerializeField] Button button;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (child != null)
        {
            child.enabled = self.enabled;
        }

        if (button != null)
        {
            button.enabled = self.enabled;
        }
    }
}
