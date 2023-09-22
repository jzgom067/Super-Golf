using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrajectoryPrediction : MonoBehaviour
{
    Scene simulationScene;
    PhysicsScene physicsScene;

    [SerializeField] Transform obstaclesParent;
    [SerializeField] LineRenderer aimingLine;
    [SerializeField] LineRenderer realTimeLine;
    [SerializeField] LineRenderer timeStopLine;
    [SerializeField] LineRenderer zacPlacingLine;
    [Tooltip("This and deltaTimeMultiplier should multiply to 400 for a good prediction.")]
    [SerializeField] int maxPhysicsIterations;
    [SerializeField] float deltaTimeMultiplier;
    [SerializeField] float lineSimplifyTolerance;
    [SerializeField] GameObject ballSimulator;
    [SerializeField] Image reticle;
    Transform cameraRef;
    Camera camera;

    bool hasCollided = false;

    private void Start()
    {
        CreatePhysicsScene();
        Physics.autoSimulation = false;
        camera = Camera.main;
        cameraRef = camera.GetComponentInParent<Transform>().parent;
        aimingLine.positionCount = maxPhysicsIterations;
        timeStopLine.positionCount = maxPhysicsIterations;
    }

    private void FixedUpdate()
    {
        SceneManager.GetActiveScene().GetPhysicsScene().Simulate(Time.deltaTime);
    }

    private void LateUpdate()
    {
        Vector3[] positions = new Vector3[aimingLine.positionCount];
        aimingLine.GetPositions(positions);

        zacPlacingLine.positionCount = positions.Length;
        zacPlacingLine.SetPositions(positions);
    }

    void CreatePhysicsScene()
    {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach (Transform obj in obstaclesParent)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            if (ghostObj.GetComponent<Terrain>() != null)
            {
                ghostObj.GetComponent<Terrain>().enabled = false;
            }
            else
            {
                ghostObj.GetComponent<Renderer>().enabled = false;
            }
            SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);
        }
    }

    public void SimulateTrajectory(GameObject player, Vector2 launchForce, bool isPutting)
    {
        GameObject ghostObj = Instantiate(ballSimulator, player.transform.position, Quaternion.Euler(0f, player.transform.rotation.eulerAngles.y, 0f));
        ghostObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);

        Vector3 forceVector = ghostObj.transform.forward * launchForce.x + Vector3.up * launchForce.y;

        ghostObj.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);

        aimingLine.positionCount = maxPhysicsIterations;

        for (int i = 0; i < maxPhysicsIterations; i++)
        {
            aimingLine.SetPosition(i, ghostObj.transform.position);
            physicsScene.Simulate(Time.fixedUnscaledDeltaTime * deltaTimeMultiplier);
            if (hasCollided && !isPutting && i > 0)
            {
                aimingLine.positionCount = i + 2;
                hasCollided = false;
                aimingLine.SetPosition(i + 1, ghostObj.transform.position);
                break;
            }
            else
            {
                hasCollided = false;
            }
        }

        reticle.transform.position = camera.WorldToScreenPoint(ghostObj.transform.position);

        Destroy(ghostObj);

        if (hasCollided)
        {
            //display graphic for landing position
        }


        aimingLine.Simplify(lineSimplifyTolerance);
    }

    public void SimulateRealTimeTrajectory(GameObject player, Vector2 launchForce)
    {
        GameObject ghostObj = Instantiate(ballSimulator, player.transform.position, Quaternion.Euler(0f, player.transform.rotation.eulerAngles.y, 0f));
        ghostObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);

        Vector3 forceVector = ghostObj.transform.forward * launchForce.x + Vector3.up * launchForce.y;

        ghostObj.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);

        realTimeLine.positionCount = maxPhysicsIterations;

        for (int i = 0; i < maxPhysicsIterations; i++)
        {
            realTimeLine.SetPosition(i, ghostObj.transform.position);
            physicsScene.Simulate(Time.fixedUnscaledDeltaTime * deltaTimeMultiplier);
            if (hasCollided && i > 0)
            {
                realTimeLine.positionCount = i + 2;
                hasCollided = false;
                realTimeLine.SetPosition(i + 1, ghostObj.transform.position);
                break;
            }
            else
            {
                hasCollided = false;
            }
        }

        Destroy(ghostObj);

        realTimeLine.Simplify(lineSimplifyTolerance);
    }

    public void TimeStopSimulateTrajectory(GameObject player, Vector3 launchForce)
    {
        GameObject ghostObj = Instantiate(ballSimulator, player.transform.position, Quaternion.identity);
        ghostObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);

        Vector3 forceVector = launchForce;

        ghostObj.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.VelocityChange);

        timeStopLine.positionCount = maxPhysicsIterations;

        for (int i = 0; i < maxPhysicsIterations; i++)
        {
            timeStopLine.SetPosition(i, ghostObj.transform.position);
            physicsScene.Simulate(Time.fixedUnscaledDeltaTime * deltaTimeMultiplier);
            if (hasCollided)
            {
                timeStopLine.positionCount = i + 2;
                hasCollided = false;
                reticle.transform.position = camera.WorldToScreenPoint(ghostObj.transform.position);
                timeStopLine.SetPosition(i + 1, ghostObj.transform.position);
                break;
            }
            else
            {
                hasCollided = false;
            }
        }

        Destroy(ghostObj);

        if (hasCollided)
        {
            //display graphic for landing position
        }


        timeStopLine.enabled = true;

        timeStopLine.Simplify(lineSimplifyTolerance);
    }

    public void SimulationCollision(Vector3 position)
    {
        hasCollided = true;
    }

    public void EnableTimeLineRender(bool b)
    {
        timeStopLine.enabled = b;
    }

    public LineRenderer GetLine()
    {
        return aimingLine;
    }
}
