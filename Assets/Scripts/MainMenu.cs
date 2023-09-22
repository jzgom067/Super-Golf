using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Master master;
    GameState gameState;
    FollowBall followBall;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        gameState = master.gameState;
        followBall = master.GetFollowBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.getGameState() == GameState.gameState.menu)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartGame(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartGame(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                StartGame(3);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                StartGame(4);
        }
    }

    public void StartGame(int num)
    {
        master.SetNumberOfPlayers(num);
        master.SetHole(0);
        gameState.setGameState(GameState.gameState.changingTurn);
        followBall.setFollowing(FollowBall.following.touring);
        followBall.ResetCameraAngleY(master.getHole().GetStartRotation());
        followBall.SetPreTourRotation();
        master.setTurn();
        gameObject.SetActive(false);
    }
}
