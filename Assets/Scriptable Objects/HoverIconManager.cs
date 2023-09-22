using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverIconManager : MonoBehaviour
{
    [SerializeField] Button buttonTrigger;
    [SerializeField] HoverIcon hoverIcon;

    Image self;
    EventTrigger eventTrigger;

    // Start is called before the first frame update
    void Start()
    {
        buttonTrigger = GetComponentInChildren<Button>();
        hoverIcon.SetTrigger(buttonTrigger);
        self = GetComponent<Image>();
        eventTrigger = GetComponentInChildren<EventTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (self != null && eventTrigger != null)
        {
            eventTrigger.enabled = self.enabled;
            if (!eventTrigger.enabled && hoverIcon != null)
            {
                EndIcon();
            }
        }
    }

    public void StartIcon()
    {
        hoverIcon.CreateHover();
    }

    public void EndIcon()
    {
        hoverIcon.DestroyHover();
    }
}
