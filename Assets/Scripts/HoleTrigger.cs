using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    GameState gs;
    GameObject[] players;
    Master master;
    [SerializeField] ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        gs = master.GetComponent<GameState>();
        players = master.getPlayerList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (other.gameObject.Equals(players[i]) && 
                players[i].GetComponent<Rigidbody>().velocity.magnitude < players[i].GetComponent<PlayerControls>().GetStopThreshold())
            {
                ps.Play();
                master.audioManager.Play("Hole");
                other.gameObject.SetActive(false);
                master.SetCompleted(i, true);
                gs.Wait(GameState.gameState.changingTurn, 3, true);
            }
        }
    }
}
