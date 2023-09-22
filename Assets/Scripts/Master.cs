using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//THINGS LEFT TO DO

//Add more holes
//Par for each course ?
//Add effect to show strokes going up after using spell
//Add sound for desperation
//Add effect to give precision more impact
//Fix trail clipping into camera ?
//Show which player is up when turn changes (use an animation)

//At end of game, highlight who did the best on each hole and show who won
//Fix club display

//149.9908 - 148.6 = 1.3908 LOWER

public class Master : MonoBehaviour
{
    #region singleton
    public static Master instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Master instance found!");
            return;
        }
        instance = this;
    }
    #endregion

    public int numPlayers;
    public GameObject[] players;
    bool[] playerCompletion;
    int currentPlayer;
    public FollowBall followBall;
    public ClubIconManager clubIconManager;

    public AudioManager audioManager;

    [SerializeField] Transform[] holes;
    [SerializeField] Transform[] holeSpawnPoints;
    [SerializeField] Transform[] tourCenters;
    [SerializeField] Transform[] topDownReferences;

    [SerializeField] Hole[] holeObjects;

    public GameState gameState;

    [SerializeField] Transform viewReference;

    int currentHole;

    public float normalTimeScale;
    public float normalFixedTime;

    public float[] playerLastRotation;
    public float lastRotation;

    [SerializeField] float timeUntilFade;
    float fadeTimer;
    [SerializeField] float fadeEffectLength;
    float fadeEffectTimer;
    [SerializeField] Image fade;
    bool fading = true;

    [SerializeField] Text gameEndText;
    [SerializeField] Text strokeText;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = 0;
        currentHole = 0;

        audioManager = AudioManager.instance;

        playerLastRotation = new float[numPlayers];

        followBall.ResetCameraAngleY(playerLastRotation[currentPlayer]);

        for (int i = 0; i < holeObjects.Length; i++)
        {
            holeObjects[i].SetReferences(holes[i], holeSpawnPoints[i], tourCenters[i], topDownReferences[i]);
        }

        DisablePlayers();
        ResetLastRotations();
        ResetZacPulls();
    }

    // Update is called once per frame
    void Update()
    {
        //DevCommands();

        MainMenuCamera();

        if (gameState.getGameState() == GameState.gameState.win)
        {
            EndGame();
        }

        Pause();
    }

    public void setTurn()
    {
        if (gameState.getGameState() == GameState.gameState.changingTurn)
        {
            playerLastRotation[currentPlayer] = viewReference.rotation.eulerAngles.y;

            if (CheckCompletion(playerCompletion))
            {
                if (AdvanceHole())
                {
                    gameState.setGameState(GameState.gameState.win);
                    return;
                }
                NoMoreCompletion();
                ResetClubs();
                followBall.setFollowing(FollowBall.following.touring);
                followBall.SetPreTourRotation();
            }

            GameObject p = CheckDisabledPlayers();
            if (p != null)
            {
                currentPlayer = p.GetComponent<PlayerControls>().GetPlayerID();
                p.transform.position = holeSpawnPoints[currentHole].position;
                p.GetComponent<BallMove>().setRespawn();
                p.SetActive(true);
                gameState.setGameState(GameState.gameState.aiming);
            }
            else
            {
                int player = 0;
                float distance = 0;
                for (int i = 0; i < numPlayers; i++)
                {
                    if (players[i].gameObject.activeInHierarchy)
                    {
                        float newDistance = Vector3.Distance(players[i].transform.position, holes[currentHole].position);
                        if (newDistance > distance)
                        {
                            player = i;
                            distance = newDistance;
                        }
                    }
                }
                currentPlayer = player;
                gameState.setGameState(GameState.gameState.aiming);
            }

            followBall.ResetCameraAngleY(playerLastRotation[currentPlayer]);

            clubIconManager.ClubStart(getPlayer().GetComponent<PlayerControls>().GetCurrentClub());
        }
    }

    public int getPlayerNumber()   
    {
        return currentPlayer;
    }
    
    /// <summary>
    /// Returns the current player.
    /// </summary>
    /// <returns></returns>
    public GameObject getPlayer()
    {
        return players[currentPlayer];
    }

    public GameObject[] getPlayerList()
    {
        return players;
    }

    public Hole getHole()
    {
        return holeObjects[currentHole];
    }

    public int getHoleNumber()
    {
        return currentHole;
    }

    public int getNumOfHoles()
    {
        return holes.Length;
    }

    public int getNumOfPlayers()
    {
        return numPlayers;
    }

    public void DisablePlayers()
    {
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerControls>().setPlaceholder(false);
            p.SetActive(false);
        }
    }

    public GameObject CheckDisabledPlayers()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            if (!players[i].activeInHierarchy && !playerCompletion[i])
            {
                return players[i];
            }
        }
        return null;
    }

    public bool CheckCompletion(bool[] pc)
    {
        foreach (bool state in pc)
        {
            if (!state)
            {
                return false;
            }
        }
        return true;
    }

    public void SetCompleted(int i, bool value)
    {
        playerCompletion[i] = value;
    }

    void NoMoreCompletion()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            playerCompletion[i] = false;
        }
    }

    bool AdvanceHole()
    {
        currentHole++;
        if (currentHole >= holes.Length)
        {
            return true;
        }
        viewReference.eulerAngles = new Vector3(0, holeObjects[currentHole].GetStartRotation(), 0);
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.rotation = Quaternion.Euler(0, holeObjects[currentHole].GetStartRotation(), 0);
        }
        ResetLastRotations();
        ResetZacPulls();

        return false;
    }

    public void SetHole(int i)
    {
        currentHole = i;
    }

    void AdvanceHoleMenu()
    {
        currentHole++;
        if (currentHole >= holes.Length)
        {
            currentHole = 0;
        }
        viewReference.position = holeObjects[currentHole].GetTourCenter().position;
        followBall.transform.localPosition = new Vector3(0, 0, -holeObjects[currentHole].GetTourDistance());
    }

    void ResetClubs()
    {
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerControls>().ResetClub();
        }
    }

    void DevCommands()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            DisablePlayers();
            AdvanceHole();
            gameState.setGameState(GameState.gameState.changingTurn);
            setTurn();
        }
    }

    //set last rotation as the course default
    void ResetLastRotations()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            playerLastRotation[i] = holeObjects[currentHole].GetStartRotation();
        }
    }

    public void SetNumberOfPlayers(int n)
    {
        numPlayers = n;

        playerCompletion = new bool[numPlayers];
        playerLastRotation = new float[numPlayers];
    }

    void MainMenuCamera()
    {
        if (gameState.getGameState() == GameState.gameState.menu)
        {
            followBall.setFollowing(FollowBall.following.touring);

            fadeTimer += Time.deltaTime;

            if (fadeTimer >= timeUntilFade)
            {
                if (Fade())
                {
                    fadeTimer = 0;
                }
            }
        }
    }

    bool Fade()
    {
        fadeEffectTimer += Time.deltaTime;

        Color tempColor = fade.color;

        if (fading)
        {
            tempColor.a = Mathf.InverseLerp(0, fadeEffectLength, fadeEffectTimer);

            if (tempColor.a >= 1)
            {
                fading = false;
                fadeEffectTimer = 0;
                tempColor.a = 1;
                AdvanceHoleMenu();
            }
        }
        else
        {
            tempColor.a = Mathf.InverseLerp(fadeEffectLength, 0, fadeEffectTimer);

            if (tempColor.a <= 0)
            {
                fading = true;
                fadeEffectTimer = 0;
                tempColor.a = 0;
                fade.color = tempColor;
                return true;
            }
        }

        fade.color = tempColor;

        return false;
    }

    public FollowBall GetFollowBall()
    {
        return followBall;
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    void ResetZacPulls()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            players[i].GetComponent<BaseSpell>().SetZacPosition(getHole().GetGreen().position);
        }
    }

    public void SetLastRotation()
    {
        lastRotation = viewReference.eulerAngles.y;
    }

    public float GetPreSwingRotation()
    {
        return lastRotation;
    }

    void Pause()
    {
        if (gameState.getGameState() == GameState.gameState.aiming)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameState.setGameState(GameState.gameState.paused);
            }
        }
        else if (gameState.getGameState() == GameState.gameState.paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameState.setGameState(GameState.gameState.aiming);
            }
        }
    }

    void EndGame()
    {
        gameEndText.enabled = true;
        gameEndText.text = "GAME OVER";
        for (int i = 0; i < numPlayers; i++)
        {
            gameEndText.text += "\nPlayer " + (i + 1) + ": " + players[i].GetComponent<PlayerControls>().GetStrokes() + " Strokes";
        }

        strokeText.text = "";
    }
}
