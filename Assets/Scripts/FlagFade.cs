using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagFade : MonoBehaviour
{
    public Material[] flagMats;
    public GameState gs;
    public Master master;
    Transform flag;
    Transform currentBall;

    public float startFade;
    public float endFade;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gs.getGameState() != GameState.gameState.win)
        {
            currentBall = master.getPlayer().transform;
            flag = master.getHole().GetGreen();

            //find distance in terms of x and y instead because center of flag isnt on bottom
            Vector3 b = currentBall.position;
            b.y = 0;
            Vector3 f = flag.position;
            f.y = 0;

            float distance = Vector3.Distance(b, f);
            foreach (Material mat in flagMats)
            {
                if (distance > startFade)
                {
                    mat.SetFloat("_Alpha", 1);
                }
                else if (distance < startFade && distance > endFade)
                {
                    mat.SetFloat("_Alpha", Mathf.InverseLerp(endFade, startFade, distance));
                }
                else
                {
                    mat.SetFloat("_Alpha", 0f);
                }
            }
        }
    }
}
