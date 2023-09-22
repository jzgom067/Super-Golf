using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconManagement : MonoBehaviour
{
    Master master;
    HitTiming hitTiming;
    GameState gameState;
    FollowBall followBall;
    PlayerControls playerControls;

    [SerializeField] Image[] images;
    [SerializeField] Icon[] iconObjects;

    private void Start()
    {
        if (images.Length != iconObjects.Length)
        {
            Debug.LogWarning("Icons and IconObjects lists have unequal lengths!");
        }

        master = Master.instance;
        hitTiming = master.GetComponent<HitTiming>();
        gameState = master.GetComponent<GameState>();
        followBall = Camera.main.GetComponent<FollowBall>();
    }

    private void FixedUpdate()
    {
        if (gameState.getGameState() == GameState.gameState.flying)
        {
            UpdateComponents();

            UpdateIconConditions();
        }
    }

    private void Update()
    {
        if (gameState.getGameState() != GameState.gameState.flying)
        {
            UpdateComponents();

            UpdateIconConditions(); 
        }
    }

    void UpdateComponents()
    {
        playerControls = master.getPlayer().GetComponent<PlayerControls>();
    }

    void UpdateIconConditions()
    {
        bool menu = gameState.getGameState() == GameState.gameState.menu;
        bool changingTurn = gameState.getGameState() == GameState.gameState.changingTurn;
        bool underwater = gameState.getGameState() == GameState.gameState.underwater;
        bool aiming = gameState.getGameState() == GameState.gameState.aiming;
        bool swinging = gameState.getGameState() == GameState.gameState.swinging;
        bool flying = gameState.getGameState() == GameState.gameState.flying;
        bool winning = gameState.getGameState() == GameState.gameState.win;
        bool touring = followBall.getFollowing() == FollowBall.following.touring && gameState.getGameState() != GameState.gameState.menu;
        bool driver = followBall.getFollowing() == FollowBall.following.driver;
        bool wedge = followBall.getFollowing() == FollowBall.following.wedge;
        bool putter = followBall.getFollowing() == FollowBall.following.putter;
        bool theWorld = false;
        bool preZacPull = false;
        bool postZacPull = false;
        bool zacPlacing = followBall.getFollowing() == FollowBall.following.topDown;
        bool reverbing = false;

        foreach (Icon icon in iconObjects)
        {
            if (icon.menu && menu ||
                icon.changingTurn && changingTurn ||
                icon.underwater && underwater ||
                icon.aiming && aiming ||
                icon.swinging && swinging ||
                icon.flying && flying ||
                icon.winning && winning ||
                icon.touring && touring ||
                icon.driver && driver ||
                icon.wedge && wedge ||
                icon.putter && putter ||
                icon.theWorld && theWorld ||
                icon.preZacPull && preZacPull ||
                icon.postZacPull && postZacPull ||
                icon.zacPlacing && zacPlacing ||
                icon.reverbing && reverbing)
            {
                icon.isActive = true;
            }

            else
            {
                icon.isActive = false;
            }
        }

        for (int i = 0; i < images.Length; i++)
        {
            images[i].enabled = iconObjects[i].GetIsActive();

            if (iconObjects[i].follow != null)
            {
                images[i].rectTransform.position = Camera.main.WorldToScreenPoint(iconObjects[i].follow.position + iconObjects[i].offset);
            }
        }
    }
}
