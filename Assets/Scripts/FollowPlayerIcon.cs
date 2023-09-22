using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowPlayerIcon : MonoBehaviour
{
    [SerializeField] int playerNum;

    Camera camera;

    [SerializeField] Icon icon;

    [SerializeField] float rectOffset;

    // Start is called before the first frame update
    void Start()
    {
        icon.follow = Master.instance.getPlayerList()[playerNum].transform;
        camera = Camera.main;
    }

    private void LateUpdate()
    {
        float angle = Vector3.SignedAngle(camera.transform.forward, icon.follow.position - camera.transform.position, Vector3.up);

        if (angle > 90 || angle < -90)
        {
            GetComponent<Image>().enabled = false;
        }

        if (Master.instance.getNumOfPlayers() < playerNum + 1)
        {
            GetComponent<Image>().enabled = false;
        }

        if (!Master.instance.getPlayerList()[playerNum].activeInHierarchy)
        {
            GetComponent<Image>().enabled = false;
        }

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + rectOffset);
    }
}
