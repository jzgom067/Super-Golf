using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleDisplay : MonoBehaviour
{
    Text self;
    [SerializeField] int secretOdds;
    [SerializeField] string[] secretTitles;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Text>();
        if (Random.Range(0, secretOdds) >= secretOdds - 1)
        {
            self.text = secretTitles[Random.Range(0, secretTitles.Length)];
        }
        else
        {
            self.text = Application.productName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
