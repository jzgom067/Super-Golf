using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeSwingDisplay : MonoBehaviour
{
    [SerializeField] float fadeSpeed;
    [SerializeField] Color startingColor;
    Image self;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Image>();
        self.color = startingColor;
    }

    // Update is called once per frame
    void Update()
    {
        self.color = new Color(self.color.r, self.color.g, self.color.b, self.color.a - (fadeSpeed * Time.deltaTime));
        if (self.color.a <= 0)
        {
            Destroy(this);
        }
    }
}
