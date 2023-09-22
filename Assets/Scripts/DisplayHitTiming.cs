using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayHitTiming : MonoBehaviour
{
    [SerializeField] RectTransform bar;
    Master master;
    HitTiming ht;
    [SerializeField] float scaleMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        ht = master.GetComponent<HitTiming>();
    }

    // Update is called once per frame
    void Update()
    {
        float startingPos = -bar.rect.width / 2;
        float currentPosition = ht.getCurrentVal() * bar.rect.width / ht.max;
        GetComponent<RectTransform>().localPosition = new Vector2((currentPosition + startingPos) * scaleMultiplier, transform.localPosition.y);
    }
}
