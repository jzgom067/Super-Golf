using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterReset : MonoBehaviour
{
    public GameState gs;
    Master master;

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.Find("Manager").GetComponent<Master>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (master.getPlayer() == other.gameObject)
        {
            gs.setGameState(GameState.gameState.underwater);
            if (other.GetComponent<BallMove>() != null)
            {
                other.GetComponent<BallMove>().waterReset(2);
            }
        }
        else
        {
            other.GetComponent<BallMove>().softWaterReset();
        }
    }
}
