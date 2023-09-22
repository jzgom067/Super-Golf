using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitTiming : MonoBehaviour
{
    public GameState gs;

    public int max;
    public float currentVal;
    public float puttingTimingSpeed;

    public float perfectValue;

    public float timingMin;
    public float timingMax;

    public Transform bar;
    public Image timingBar;
    public GameObject practiceBar;

    public enum timingState
    {
        stopped,
        forward,
        backward
    }

    timingState state = timingState.stopped;

    bool isPutting;

    [SerializeField] float minTimingSpeed;
    [SerializeField] float maxTimingSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPutting)
        {
            if (state == timingState.forward)
            {
                currentVal += Time.unscaledDeltaTime * puttingTimingSpeed;
                if (currentVal >= max)
                {
                    state = timingState.backward;
                }
            }
            else if (state == timingState.backward)
            {
                currentVal -= Time.unscaledDeltaTime * puttingTimingSpeed;
                if (currentVal <= 0)
                {
                    state = timingState.forward;
                }
            } 
        }
        else
        {
            float stopValue = perfectValue - Mathf.Abs(perfectValue - currentVal);
            float speed = Mathf.Lerp(minTimingSpeed, maxTimingSpeed, stopValue / perfectValue);

            if (state == timingState.forward)
            {
                currentVal += Time.unscaledDeltaTime * speed;
                if (currentVal >= max)
                {
                    state = timingState.backward;
                }
            }
            else if (state == timingState.backward)
            {
                currentVal -= Time.unscaledDeltaTime * speed;
                if (currentVal <= 0)
                {
                    state = timingState.forward;
                }
            }
        }
    }

    public float getCurrentVal()
    {
        return currentVal;
    }

    public float getPowerVal()
    {
        float lerpValue = Mathf.InverseLerp(0, 50, perfectValue - Mathf.Abs(perfectValue - currentVal));
        return Mathf.Lerp(timingMin, timingMax, lerpValue);
    }

    public void startTiming(bool putting)
    {
        currentVal = 0;
        state = timingState.forward;
        isPutting = putting;
    }

    public float stopHit()
    {
        state = timingState.stopped;
        float lerpValue = Mathf.InverseLerp(0, 50, perfectValue - Mathf.Abs(perfectValue - currentVal));
        return Mathf.Lerp(timingMin, timingMax, lerpValue);
    }

    public void practiceSwing()
    {
        Transform pb = Instantiate(practiceBar, bar.transform).transform;
        pb.position = timingBar.transform.position;
    }

    public float getMaxValue()
    {
        return timingMax;
    }

    public float getMinValue()
    {
        return timingMin;
    }
}
