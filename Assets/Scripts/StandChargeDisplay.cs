using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandChargeDisplay : MonoBehaviour
{
    Master master;
    GameObject currentBall;
    BaseSpell ballSpells;

    Text[] charges;

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.Find("Manager").GetComponent<Master>();
        charges = GetComponentsInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentBall = master.getPlayer();
        ballSpells = currentBall.GetComponent<BaseSpell>();

        int[] chargeValues = ballSpells.getSpellCharges();

        for (int i = 0; i < charges.Length; i++)
        {
            charges[i].text = chargeValues[i] + "";
        }
    }
}
