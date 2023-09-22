using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagIcon : MonoBehaviour
{
    Master master;
    [SerializeField] Icon flagIcon;
    Image flagImage;

    Camera camera;
    Transform flag;

    [SerializeField] float startFade;
    [SerializeField] float endFade;

    [SerializeField] float verticalOffset;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        flagImage = GetComponent<Image>();
        camera = Camera.main;
    }

    // I use FixedUpdate because it will make the movement with the camera smoother, as it will update at the same rate
    void FixedUpdate()
    {
        if (master.gameState.getGameState() != GameState.gameState.win && master.gameState.getGameState() == GameState.gameState.flying)
        {
            //Stuff();
        }
    }

    //Switching between Update and FixedUpdate makes it smooth when aiming and when flying
    //LateUpdate ensures that this overrides anything else that changes the enabled of the image
    void LateUpdate()
    {
        Stuff();

        float angle = Vector3.SignedAngle(camera.transform.forward, flag.position - camera.transform.position, Vector3.up);

        if (angle > 90 || angle < -90)
        {
            flagImage.enabled = false;
        }

        if (master.GetFollowBall().getFollowing() == FollowBall.following.topDown)
        {
            flagIcon.offset.y = 0;
        }
        else
        {
            flagIcon.offset.y = verticalOffset;
        }
    }

    void Stuff()
    {
        if (master.GetGameState().getGameState() != GameState.gameState.win)
        {
            flag = master.getHole().GetGreen();
        }

        flagIcon.follow = flag;

        //find distance in terms of x and y instead because center of flag isnt on bottom
        Vector3 b = camera.transform.position;
        b.y = 0;
        Vector3 f = flag.position;
        f.y = 0;

        float distance = Vector3.Distance(b, f);

        Color tempColor = flagImage.color;

        if (distance > startFade)
        {
            tempColor.a = 1;
        }
        else if (distance < startFade && distance > endFade)
        {
            tempColor.a = Mathf.InverseLerp(endFade, startFade, distance);
        }
        else
        {
            tempColor.a = 0;
        }

        flagImage.color = tempColor;
    }
}
