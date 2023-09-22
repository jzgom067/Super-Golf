using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchEnabledImage : MonoBehaviour
{
    Image self;
    [SerializeField] Text child;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        child.enabled = self.enabled;
    }
}
