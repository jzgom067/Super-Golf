using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    Master master;
    Camera camera;

    Transform courseCenter;
    Transform courseTopDown;
    Transform hole;
    Vector3 tourOffset;
    float topDownRotation;

    public Transform viewReference;
    public Vector3 ballOffset;
    public Vector3 driverOffset;
    public Vector3 wedgeOffset;
    public Vector3 putterOffset;
    public Vector3 ballRefOffset;
    public float ballRefAngle;
    public float puttingRefAngle;
    public float tourRefAngle;

    public int ballFOV;
    public int driverFOV;
    public int wedgeFOV;
    public int putterFOV;
    public int tourFOV;
    public int topDownFOV;

    public float tourTurnSpeed;

    public float lerpThing;

    float preTourRotation;

    [SerializeField] float topDownZoomSpeed;
    float topDownZoomValue;
    [SerializeField] Vector3 topDownHoleMaxZoomOffset;

    bool tourRotate = true;

    public enum following
    {
        nothing,
        touring,
        ball,
        putter,
        wedge,
        driver,
        hole,
        topDown
    }

    public following currentFollow;

    // Start is called before the first frame update
    void Start()
    {
        currentFollow = following.touring;
        master = Master.instance;
        camera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (master.gameState.getGameState() != GameState.gameState.win)
        {
            courseCenter = master.getHole().GetTourCenter();
            courseTopDown = master.getHole().GetTopDown();
            topDownRotation = master.getHole().GetTopDownRotation();
            tourOffset = new Vector3(0, 0, -master.getHole().GetTourDistance());
            hole = master.getHole().GetGreen();
        }

        CameraFollow();

        TopDownZoomStuff();

        if (currentFollow != following.touring)
        {
            tourRotate = true;
        }
    }

    void CameraFollow()
    {
        if (currentFollow == following.ball)
        {
            viewReference.position = Vector3.Lerp(viewReference.position, master.getPlayer().transform.position + ballRefOffset, lerpThing);
            Vector3 angles = new Vector3(Mathf.Lerp(viewReference.eulerAngles.x, ballRefAngle, lerpThing), viewReference.eulerAngles.y, viewReference.eulerAngles.z);
            viewReference.eulerAngles = angles;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.Lerp(transform.localPosition, ballOffset, lerpThing);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, ballFOV, lerpThing);
        }
        else if (currentFollow == following.touring)
        {
            viewReference.position = Vector3.Lerp(viewReference.position, courseCenter.position, lerpThing);
            Vector3 angles = new Vector3(Mathf.Lerp(viewReference.eulerAngles.x, tourRefAngle, lerpThing), viewReference.eulerAngles.y, viewReference.eulerAngles.z);
            viewReference.eulerAngles = angles;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.Lerp(transform.localPosition, tourOffset, lerpThing);
            if (tourRotate)
            {
                viewReference.Rotate(0, tourTurnSpeed, 0, Space.World);
            }
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, tourFOV, lerpThing);
        }
        else if (currentFollow == following.topDown)
        {
            Vector3 zoomPosition = Vector3.Lerp(courseTopDown.position, hole.transform.position + topDownHoleMaxZoomOffset, topDownZoomValue / 100);

            viewReference.position = Vector3.Lerp(viewReference.position, zoomPosition, lerpThing);
            viewReference.eulerAngles = new Vector3(90, topDownRotation, 0);
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, lerpThing);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, topDownFOV, lerpThing);
        }
        else if (currentFollow == following.driver)
        {
            viewReference.position = Vector3.Lerp(viewReference.position, master.getPlayer().transform.position + ballRefOffset, lerpThing);
            Vector3 angles = new Vector3(Mathf.Lerp(viewReference.eulerAngles.x, ballRefAngle, lerpThing), viewReference.eulerAngles.y, viewReference.eulerAngles.z);
            viewReference.eulerAngles = angles;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.Lerp(transform.localPosition, driverOffset, lerpThing);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, driverFOV, lerpThing);
        }
        else if (currentFollow == following.wedge)
        {
            viewReference.position = Vector3.Lerp(viewReference.position, master.getPlayer().transform.position + ballRefOffset, lerpThing);
            Vector3 angles = new Vector3(Mathf.Lerp(viewReference.eulerAngles.x, ballRefAngle, lerpThing), viewReference.eulerAngles.y, viewReference.eulerAngles.z);
            viewReference.eulerAngles = angles;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.Lerp(transform.localPosition, wedgeOffset, lerpThing);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, wedgeFOV, lerpThing);
        }
        else if (currentFollow == following.putter)
        {
            viewReference.position = Vector3.Lerp(viewReference.position, master.getPlayer().transform.position + ballRefOffset, lerpThing);
            Vector3 angles = new Vector3(Mathf.Lerp(viewReference.eulerAngles.x, puttingRefAngle, lerpThing), viewReference.eulerAngles.y, viewReference.eulerAngles.z);
            viewReference.eulerAngles = angles;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.Lerp(transform.localPosition, putterOffset, lerpThing);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, putterFOV, lerpThing);
        }
    }

    public void setFollowing(following f)
    {
        if (currentFollow == following.touring && f != following.touring)
        {
            viewReference.transform.SetPositionAndRotation(viewReference.position, Quaternion.Euler(0, preTourRotation, 0));
        }
        else if (currentFollow == following.topDown && f != following.topDown)
        {
            viewReference.transform.SetPositionAndRotation(viewReference.position, Quaternion.Euler(0, preTourRotation, 0));
        }
        currentFollow = f;
        if (f == following.touring || f == following.topDown)
        {
            preTourRotation = viewReference.rotation.eulerAngles.y;
        }

        if (f == following.topDown)
        {
            topDownZoomValue = 0;
        }
    }

    public following getFollowing()
    {
        return currentFollow;
    }

    public bool IsFollowingBall()
    {
        return currentFollow == following.ball || currentFollow == following.driver || currentFollow == following.wedge || currentFollow == following.putter;
    }

    public void SetBallFollowFOV(int degrees)
    {
        ballFOV = degrees;
    }

    public void ResetCameraAngleY(float angle)
    {
        viewReference.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void SetCameraAngleY(float angle)
    {
        viewReference.rotation = Quaternion.Euler(viewReference.eulerAngles.x, angle, viewReference.eulerAngles.z);
    }

    void TopDownZoomStuff()
    {
        if (currentFollow == following.topDown)
        {
            topDownZoomValue += Input.mouseScrollDelta.y * topDownZoomSpeed;
        }

        topDownZoomValue = Mathf.Clamp(topDownZoomValue, 0, 100);
    }

    public void SetTourRotate(bool b)
    {
        tourRotate = b;
    }

    public void SetPreTourRotation()
    {
        preTourRotation = viewReference.eulerAngles.y;
    }
}
