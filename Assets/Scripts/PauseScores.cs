using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseScores : MonoBehaviour
{
    Master master;
    Text self;
    [SerializeField] Text child;
    [SerializeField] Text paused;
    [SerializeField] GameObject scoreCard;
    [SerializeField] Color regularColor;
    [SerializeField] Color playerHighlightColor;
    [SerializeField] Color bestScoreColor;
    TextMeshProUGUI[,] scores;

    GameObject[] players;
    int numPlayers;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        self = GetComponent<Text>();
        players = master.getPlayerList();
        TextMeshProUGUI[] tempScores = scoreCard.GetComponentsInChildren<TextMeshProUGUI>();
        scores = new TextMeshProUGUI[4, 9];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                scores[i, j] = tempScores[(9 * i) + 9 + j];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        numPlayers = master.getNumOfPlayers();
        GameState.gameState gs = master.GetGameState().getGameState();

        if (gs == GameState.gameState.paused || gs == GameState.gameState.win)
        {
            scoreCard.SetActive(true);
            self.enabled = true;
            
            if (gs == GameState.gameState.paused)
            {
                child.text = "CURRENT SCORES";
                paused.text = "PAUSED";
            }
            else
            {
                child.text = "";
                paused.text = "GAME OVER";
            }
            
            
            for (int i = 0; i < numPlayers; i++)
            {
                float[] strokes = master.getPlayerList()[i].GetComponent<PlayerControls>().GetStrokeArray();
                for (int j = 0; j < 9; j++)
                {
                    if (gs == GameState.gameState.paused)
                    {
                        if (master.getPlayerNumber() == i && master.getNumOfPlayers() > 1)
                        {
                            scores[i, j].color = playerHighlightColor;
                        }
                        else
                        {
                            scores[i, j].color = regularColor;
                        }
                    }

                    if (j == 0)
                    {
                        scores[i, j].text = "P" + (i + 1);
                    }
                    else if (j == 8)
                    {
                        scores[i, j].text = master.getPlayerList()[i].GetComponent<PlayerControls>().GetStrokes().ToString();
                    }
                    else
                    {
                        if (strokes[j - 1] != 0 || master.getHoleNumber() == j - 1)
                        {
                            scores[i, j].text = strokes[j - 1].ToString();
                        }
                    }
                }
            }
            if (gs == GameState.gameState.win)
            {
                //set all colors to white
                for (int i = 0; i < numPlayers; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        scores[i, j].color = regularColor;
                    }
                }
                //if more than 1 player, highlight each course best
                if (master.getNumOfPlayers() > 1)
                {
                    int winner = 0;
                    for (int i = 1; i < 9; i++)
                    {
                        if (!scores[0, i].text.Equals(""))
                        {
                            float min = float.Parse(scores[0, i].text);
                            bool[] minI = new bool[4];
                            minI[0] = true;
                            for (int j = 1; j < numPlayers; j++)
                            {
                                float current = float.Parse(scores[j, i].text);
                                if (current < min)
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        minI[k] = false;
                                    }
                                    min = current;
                                    minI[j] = true;
                                }
                                else if (current == min)
                                {
                                    minI[j] = true;
                                }
                            }
                            int winCount = 0;
                            for (int j = 0; j < numPlayers; j++)
                            {
                                if (minI[j])
                                {
                                    scores[j, i].color = bestScoreColor;
                                    winCount++;
                                    winner = j;
                                }
                            }
                            if (winCount > 1)
                            {
                                winner = -1;
                            }
                        }
                    }
                    if (winner != -1)
                    {
                        child.text = "Player " + (winner + 1) + " wins!";
                    }
                    else
                    {
                        child.text = "It's a tie!";
                    }
                }
            }
        }
        else
        {
            self.enabled = false;
            scoreCard.SetActive(false);
            child.text = "";
        }
    }
}
