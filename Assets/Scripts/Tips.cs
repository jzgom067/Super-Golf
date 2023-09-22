using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tips : MonoBehaviour
{
    TextMeshProUGUI self;
    [SerializeField] string tipTitle;
    [TextArea]
    [SerializeField] string[] tips;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<TextMeshProUGUI>();
        self.text = tipTitle + "\n" + tips[Random.Range(0, tips.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
