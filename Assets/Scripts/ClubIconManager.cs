using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubIconManager : MonoBehaviour
{
    [SerializeField] Text clubDisplay;
    [SerializeField] Animator[] animators;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClubStart(int i)
    {
        if (!animators[i].GetCurrentAnimatorStateInfo(0).IsName("Club Appeared Idle"))
        {
            animators[i].SetTrigger("Appear");
        }
    }

    public void ChangeClub(int i, int j)
    {
        animators[i].SetTrigger("Leave");
        animators[j].SetTrigger("Appear");
    }

    public void DisableIcons()
    {
        foreach (Animator a in animators)
        {
            if (a.GetCurrentAnimatorStateInfo(0).IsName("Club Appeared Idle"))
            {
                a.SetTrigger("Leave");
            }
        }

        clubDisplay.text = "";
    }
}
