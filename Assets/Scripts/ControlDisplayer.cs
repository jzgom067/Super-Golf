using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlDisplayer : MonoBehaviour
{
    [SerializeField] ControlsDisplay aiming;
    [SerializeField] ControlsDisplay flying;
    [SerializeField] ControlsDisplay swinging;
    [SerializeField] ControlsDisplay timeStop;
    [SerializeField] ControlsDisplay touring;
    [SerializeField] ControlsDisplay zacPlacing;

    Text self;
    [SerializeField] TextMeshProUGUI centerText;

    Master master;
    GameState gs;
    PlayerControls pc;
    BaseSpell bs;
    FollowBall fb;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        gs = master.gameState;
        fb = master.GetFollowBall();

        self = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateReferences();
        DisplayControls();
    }

    void UpdateReferences()
    {
        pc = master.getPlayer().GetComponent<PlayerControls>();
        bs = master.getPlayer().GetComponent<BaseSpell>();
    }

    void DisplayControls()
    {
        if (gs.getGameState() == GameState.gameState.menu || gs.getGameState() == GameState.gameState.win || gs.getGameState() == GameState.gameState.paused)
        {
            centerText.text = "";
            self.text = "";
            return;
        }

        if (fb.getFollowing() == FollowBall.following.topDown)
        {
            centerText.text = zacPlacing.GetString();
            self.text = "";
        }
        else if (fb.getFollowing() == FollowBall.following.touring)
        {
            centerText.text = touring.GetString();
            self.text = "";
        }
        else if (fb.IsFollowingBall())
        {
            if (bs.getTimeStop())
            {
                centerText.text = timeStop.GetString();
                self.text = "";
                return;
            }

            if (gs.getGameState() == GameState.gameState.aiming)
            {
                centerText.text = "";
                self.text = aiming.GetString();
            }
            else if (gs.getGameState() == GameState.gameState.swinging)
            {
                centerText.text = swinging.GetString();
                self.text = "";
            }
            else if (gs.getGameState() == GameState.gameState.flying)
            {
                centerText.text = "";
                self.text = flying.GetString();
            }
        }
        else
            self.text = "";
    }
}
