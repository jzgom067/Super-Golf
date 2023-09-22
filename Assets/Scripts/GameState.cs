using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public Text txt;

    float timer;
    float timerMax;
    gameState next;
    bool useSetTurn;

    public enum gameState
    {
        menu,
        waiting,
        paused,
        underwater,
        changingTurn,
        aiming,
        swinging,
        flying,
        win
    }

    gameState gs;

    // Start is called before the first frame update
    void Start()
    {
        gs = gameState.menu;
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = gs.ToString();

        if (timer < timerMax)
        {
            timer += Time.deltaTime;

        }
        else if (timerMax != 0)
        {
            gs = next;
            timer = 0;
            timerMax = 0;
            if (useSetTurn)
            {
                Master.instance.setTurn();
            }
        }
    }

    public gameState getGameState()
    {
        return gs;
    }

    public void setGameState(gameState rq)
    {
        gs = rq;
    }

    public void Wait(gameState nextGS, float time, bool useST)
    {
        setGameState(gameState.waiting);
        timerMax = time;
        next = nextGS;
        useSetTurn = useST;
    }
}
