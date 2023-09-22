using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class PlayerControls : MonoBehaviour
{
    BallMove ballMove;
    Rigidbody rb;
    BaseSpell bs;
    Camera camera;

    public GameState gs;
    public Master master;
    public HitTiming hitTiming;
    public Clubs clubs;
    public FollowBall followBall;
    public Text clubText;
    public GameObject placeholder;
    [SerializeField] int playerID;

    int numClubs;
    int currentClub;

    public float turnTime;
    public float turnMultiplier;
    public AnimationCurve turnSpeed;

    Stopwatch stopwatch = new Stopwatch();
    public float flyCheck;
    [SerializeField] float stopThreshold;

    public bool isInSpellEffect = false;

    public ParticleSystem swingEffect;

    float[] strokes;
    float totalStrokes;
    public Text strokeDisplay;

    public float normalSwingStrokes;
    public float timeStopSwingStrokes;

    [SerializeField] LineRenderer line;
    [SerializeField] Image reticle;
    [SerializeField] LineRenderer realTimeLine;
    [SerializeField] LineRenderer puttingLine;
    [SerializeField] LineRenderer zacPlacingLine;
    bool hasSetUpAiming;
    bool hasEndedAiming;

    bool isPutting;

    [SerializeField] Animator barAnimator;

    [SerializeField] ClubIconManager clubIconManager;

    [SerializeField] float cameraVelocityFollowLerp;

    float mouseX;
    [SerializeField] float mouseTourTurnSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        ballMove = GetComponent<BallMove>();
        rb = GetComponent<Rigidbody>();
        bs = GetComponent<BaseSpell>();
        camera = Camera.main;

        currentClub = 0;
        numClubs = clubs.getNum();

        strokes = new float[master.getNumOfHoles()];

        totalStrokes = 0;
        updateStrokes();

        isPutting = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkTurn();
        if (master.getPlayerNumber() == playerID)
        {
            if (gs.getGameState() != GameState.gameState.flying)
            {
                stopwatch.Stop();
                stopwatch.Reset();
            }

            /*
            if (gs.getGameState() != GameState.gameState.aiming && gs.getGameState() != GameState.gameState.swinging)
            {
                hasSetUpAiming = false;
                if (!hasEndedAiming)
                    EndAiming();
                hasEndedAiming = true;
            }
            else
            {
                hasEndedAiming = false;
                if (!hasSetUpAiming)
                    SetUpAiming();
                hasSetUpAiming = true;
            }
            */
        }
        stopCheck();
    }

    void checkTurn()
    {
        if (master.getPlayerNumber() == playerID)
        {
            if (gs.getGameState() == GameState.gameState.menu)
            {
                //menu things
            }
            else if (gs.getGameState() == GameState.gameState.changingTurn)
            {
                master.setTurn();
            }
            else if (gs.getGameState() == GameState.gameState.aiming)
            {
                setPlaceholder(false);
                if (!bs.checkZacStatus())
                {
                    aim();
                    ChangeClub();
                }
                if (followBall.IsFollowingBall())
                {
                    if (!hasSetUpAiming)
                    {
                        SetUpAiming();
                    }
                }
                else
                {
                    if (!hasEndedAiming)
                    {
                        EndAiming();
                    }
                }

                //disable line and reticle during touring
                /*
                if (camera.GetComponent<FollowBall>().getFollowing() == FollowBall.following.touring)
                {
                    line.enabled = false;
                    reticle.enabled = false;
                }
                else
                {
                    line.enabled = true;
                    reticle.enabled = true;
                }
                */
            }
            else if (gs.getGameState() == GameState.gameState.swinging)
            {
                swing();
            }
            else if (gs.getGameState() == GameState.gameState.flying)
            {
                SmoothVelocityCameraRotate();

                SetFOV();
            }

            //determines if it should change turn when the ball stops
            if (gs.getGameState() == GameState.gameState.flying &&
                rb.velocity.magnitude < stopThreshold &&
                stopwatch.ElapsedMilliseconds > flyCheck * 1000 &&
                ballMove.getColliding() &&
                !isInSpellEffect)
            {
                ballMove.setRespawn();
                setPlaceholder(true);
                gs.setGameState(GameState.gameState.changingTurn);
                bs.resetCharges();
            }

            if (!isInSpellEffect)
            {
                castSpells();
            }

            //trajectory line displays, code to enable and disable them should only be here
            if (followBall.IsFollowingBall())
            {
                transform.rotation = Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, 0);
                if ((gs.getGameState() == GameState.gameState.aiming || gs.getGameState() == GameState.gameState.swinging) && isPutting)
                {
                    puttingLine.enabled = true;
                    line.enabled = false;
                    reticle.enabled = false;
                    realTimeLine.enabled = false;
                    puttingLine.transform.position = transform.position;
                    puttingLine.transform.SetPositionAndRotation(puttingLine.transform.position, Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, 0));
                }
                else if (gs.getGameState() == GameState.gameState.aiming && !isPutting) {
                    line.enabled = true;
                    reticle.enabled = true;
                    realTimeLine.enabled = false;
                    puttingLine.enabled = false;
                    master.GetComponent<TrajectoryPrediction>().SimulateTrajectory(gameObject, GetMaxClubHit(), isPutting);
                }
                else if (gs.getGameState() == GameState.gameState.swinging && !isPutting)
                {
                    line.enabled = true;
                    reticle.enabled = true;
                    realTimeLine.enabled = true;
                    puttingLine.enabled = false;
                    master.GetComponent<TrajectoryPrediction>().SimulateRealTimeTrajectory(gameObject, GetCurrentClubHit());
                }
                else
                {
                    line.enabled = false;
                    reticle.enabled = false;
                    realTimeLine.enabled = false;
                    puttingLine.enabled = false;
                }
                zacPlacingLine.enabled = false;
            }
            else if ((followBall.getFollowing() == FollowBall.following.topDown || followBall.getFollowing() == FollowBall.following.touring) && !isPutting)
            {
                zacPlacingLine.enabled = true;
                master.GetComponent<TrajectoryPrediction>().SimulateTrajectory(gameObject, GetMaxClubHit(), isPutting);
            }
            else
            {
                line.enabled = false;
                reticle.enabled = false;
                realTimeLine.enabled = false;
                puttingLine.enabled = false;
                zacPlacingLine.enabled = false;
            }

            if (followBall.getFollowing() == FollowBall.following.touring)
            {
                TourDrag();
            }

            //other stuff that should just happen every frame that it's your turn, like UI updates
            updateStrokes();

            bs.UpdateFlyingSpellEffects();

            bs.UpdateSpellColors();

            BarAnimation();
        }
    }

    void SetUpAiming()
    {
        hasSetUpAiming = true;
        hasEndedAiming = false;
        line.GetComponent<FollowObject>().setObjectToFollow(transform);
        ChangeBallFollow();
    }

    void EndAiming()
    {
        hasEndedAiming = true;
        hasSetUpAiming = false;
    }

    void BarAnimation()
    {
        if ((gs.getGameState() == GameState.gameState.aiming || gs.getGameState() == GameState.gameState.swinging) && followBall.IsFollowingBall())
        {
            if (barAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle Down") || barAnimator.GetCurrentAnimatorStateInfo(0).IsName("Timing Bar Lower"))
                barAnimator.SetTrigger("Appear");
        }
        else
        {
            if (barAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle Up") || barAnimator.GetCurrentAnimatorStateInfo(0).IsName("Timing Bar Rise"))
                barAnimator.SetTrigger("Disappear");
        }
    }

    void aim()
    {
        int previousClub = currentClub;

        //selecting club
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentClub -= 1;
            if (currentClub < 0)
            {
                currentClub = numClubs - 1;
            }
            clubIconManager.ChangeClub(previousClub, currentClub);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentClub += 1;
            if (currentClub >= numClubs)
            {
                currentClub = 0;
            }
            clubIconManager.ChangeClub(previousClub, currentClub);
        }

        ChangeBallFollow();
        clubText.text = clubs.getName(currentClub);

        if (followBall.IsFollowingBall())
        {

            //turning ball/camera
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                turnTime += Time.unscaledDeltaTime;
                ballMove.rotateBall(-turnSpeed.Evaluate(turnTime) * turnMultiplier * Time.deltaTime);
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                turnTime = 0;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                turnTime += Time.unscaledDeltaTime;
                ballMove.rotateBall(turnSpeed.Evaluate(turnTime) * turnMultiplier * Time.deltaTime);
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                turnTime = 0;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                followBall.setFollowing(FollowBall.following.touring);
            }

            //player preparing to swing
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gs.setGameState(GameState.gameState.swinging);
                hitTiming.startTiming(isPutting);
            }
        }
        else if (followBall.getFollowing() == FollowBall.following.touring)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                followBall.setFollowing(FollowBall.following.ball);
            }
        }

    }

    void swing()
    {
        //practice swing
        if (Input.GetKeyDown(KeyCode.Return))
        {
            hitTiming.practiceSwing();
        }
        //actually swing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 launchAngle = clubs.getAngle(currentClub) * clubs.getMultiplier(currentClub) * Mathf.Sqrt(hitTiming.stopHit());
            ballMove.LaunchBall(launchAngle);
            gs.setGameState(GameState.gameState.flying);
            stopwatch.Start();
            swingEffect.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(rb.velocity));
            swingEffect.Play();
            addStrokes(normalSwingStrokes);
            EndAiming();

            clubIconManager.DisableIcons();
            master.SetLastRotation();

            PlayClubSound(clubs.getName(currentClub));
        }
        //go back to aiming
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gs.setGameState(GameState.gameState.aiming);
            hitTiming.stopHit();
        }
    }

    void PlayClubSound(string club)
    {
        master.audioManager.Play(club);
    }

    public void timeStopHit(Vector3 launchAngle)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rb.velocity = Vector3.zero;
            ballMove.LaunchBallDirect(launchAngle);
            bs.resumeTime();
            swingEffect.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(rb.velocity));
            swingEffect.Play();

            addStrokes(timeStopSwingStrokes);
            bs.AddToStrokeRefundBank(timeStopSwingStrokes);
        }
    }

    void TourDrag()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            followBall.SetTourRotate(false);
            followBall.SetCameraAngleY(camera.transform.eulerAngles.y + ((Input.mousePosition.x - mouseX) * mouseTourTurnSpeed * Time.deltaTime));
        }
        else
            followBall.SetTourRotate(true);
        mouseX = Input.mousePosition.x;
    }

    void castSpells()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ballMove.CastSpell(0); //The World
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            ballMove.CastSpell(1); //Reverb
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            //ballMove.CastSpell(2); //Reset
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ballMove.CastSpell(3); //Zac Pull
        }
    }

    public void setPlaceholder(bool visible)
    {
        GetComponent<MeshRenderer>().enabled = !visible;
        GetComponent<Collider>().enabled = !visible;
        GetComponent<Rigidbody>().isKinematic = visible;
        placeholder.transform.position = transform.position;
        placeholder.SetActive(visible);
    }

    public void setIsInSpellEffect(bool fact)
    {
        isInSpellEffect = fact;
    }

    public void stopCheck()
    {
        if (gs.getGameState() == GameState.gameState.flying && rb.velocity.magnitude == 0)
        {
            stopwatch.Start();
        }
        else
        {
            stopwatch.Stop();
            stopwatch.Reset();
        }
    }

    void updateStrokes()
    {
        if (gs.getGameState() != GameState.gameState.menu && gs.getGameState() != GameState.gameState.win)
        {
            strokeDisplay.text = "Hole " + (master.getHole().GetHoleIndex() + 1) + "\nPlayer " + (playerID + 1) + "\nStrokes: " + strokes[master.getHoleNumber()];
        }
        else
        {
            strokeDisplay.text = "";
        }
    }

    public void addStrokes(float i)
    {
        strokes[master.getHoleNumber()] += i;
        totalStrokes += i;
        updateStrokes();
    }

    /// <summary>
    /// This also can be used to call the trajectory prediction function with current club.
    /// </summary>
    void ChangeClub()
    {
        if (clubs.getName(currentClub) == "Putter")
        {
            isPutting = true;
            line.enabled = false;
        }
        else
        {
            isPutting = false;
            line.enabled = true;
        }
    }

    public void ChangeBallFollow()
    {
        if (followBall.IsFollowingBall())
        {
            switch (clubs.getName(currentClub))
            {
                case "Driver":
                    followBall.setFollowing(FollowBall.following.driver);
                    break;
                case "Wedge":
                    followBall.setFollowing(FollowBall.following.wedge);
                    break;
                case "Putter":
                    followBall.setFollowing(FollowBall.following.putter);
                    break;
                default:
                    followBall.setFollowing(FollowBall.following.wedge);
                    break;
            }
        }
    }

    void SmoothVelocityCameraRotate()
    {
        if (rb.velocity.magnitude > stopThreshold && !bs.getTimeStop())
        {
            Transform viewReference = camera.GetComponentInParent<Transform>();
            float angleToVelocity = Vector3.SignedAngle(FlattenVector(viewReference.forward), FlattenVector(rb.velocity), Vector3.up);

            followBall.SetCameraAngleY(Mathf.Lerp(viewReference.eulerAngles.y, viewReference.eulerAngles.y + angleToVelocity, cameraVelocityFollowLerp));
        }
    }

    void SetFOV()
    {
        followBall.setFollowing(FollowBall.following.ball);
    }

    public Vector2 GetMaxClubHit()
    {
        return clubs.getAngle(currentClub) * clubs.getMultiplier(currentClub) * Mathf.Sqrt(hitTiming.getMaxValue());
    }

    public Vector2 GetCurrentClubHit()
    {
        return clubs.getAngle(currentClub) * clubs.getMultiplier(currentClub) * Mathf.Sqrt(hitTiming.getPowerVal());
    }

    public float GetStopThreshold()
    {
        return stopThreshold;
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    public void ResetClub()
    {
        currentClub = 0;
    }
    
    public int GetCurrentClub()
    {
        return currentClub;
    }

    public float[] GetStrokeArray()
    {
        return strokes;
    }

    public float GetStrokes()
    {
        return totalStrokes;
    }

    Vector3 FlattenVector(Vector3 v)
    {
        v.y = 0;
        return v;
    }
}
