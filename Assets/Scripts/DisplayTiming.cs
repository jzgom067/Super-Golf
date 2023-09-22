using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTiming : MonoBehaviour
{
    public Slider slider;
    public HitTiming hitTiming;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = hitTiming.getCurrentVal();
    }
}
