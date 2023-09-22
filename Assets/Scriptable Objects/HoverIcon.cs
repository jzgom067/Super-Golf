using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Hover Icon", menuName = "Objects/Hover Icon")]
public class HoverIcon : ScriptableObject
{
    Button hoverTrigger;
    [SerializeField] GameObject iconToShow;
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 size;

    GameObject icon;

    public void SetTrigger(Button button)
    {
        hoverTrigger = button;
    }

    public void CreateHover()
    {
        icon = Instantiate(iconToShow, hoverTrigger.transform);

        icon.GetComponent<RectTransform>().localPosition = offset;
        icon.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void DestroyHover()
    {
        Destroy(icon);
        icon = null;
    }
}
