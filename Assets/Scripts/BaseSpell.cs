using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BaseSpell : MonoBehaviour
{
    bool reverbing = false;
    public ParticleSystem dust;
    public Color originalTrailColor;
    public Color reverbTrailColor;
    public TrailRenderer baseTrail;
    public TrailRenderer specialTrail;
    [SerializeField] LineRenderer zacPullLine;
    public ParticleSystem specialParticles;
    public ParticleSystem reverbTrailParticles;
    public ParticleSystem zacPullParticles;

    int spell1Charges;
    int spell2Charges;
    int spell3Charges;
    int spell4Charges;
    int spell5Charges;
    int spell6Charges;
    int spell7Charges;

    bool timeStopped = false;

    public float timeSlowMultiplier = 10;
    public float timeStopEffectsMaxTime;
    public float timeResumeEffectsMaxTime;
    float timeStopEffectsTimer = 0;

    Master master;
    FollowBall followBall;
    Camera camera;
    Rigidbody rb;

    //effects
    public AudioManager am;
    public ParticleSystem glassBreak;
    public Volume volume;

    Vignette vignette;

    public float timeStopVignetteIntensity;

    public AnimationCurve timeStopCurve;

    public PlayerControls pc;
    public HitTiming ht;
    public BallMove bm;

    public float timeStopAdditionalForce;
    bool timeResumed = true;

    [SerializeField] float timeStopStopThreshold;

    public ParticleSystem timeStopParticles;

    Vector3 zacPosition;
    bool zacPlacing;
    public Image zacPullIcon;
    public Text zacPullTargetUI;
    public float zacPullPower;

    public GameObject ballIconsObject;
    Image[] ballIcons;

    bool iconsOn;

    bool hasZacPulled;

    bool colliding;
    bool groundReverbing;
    [SerializeField] float groundReverbThreshold;
    [SerializeField] float groundReverbDragMultiplier;
    [SerializeField] float groundReverbStopThreshold;
    
    [SerializeField] Image theWorld;
    [SerializeField] Image reverb;
    [SerializeField] Image zacPull;

    [SerializeField] Color theWorldColor;
    [SerializeField] Color reverbColor;
    [SerializeField] Color zacPullColor;

    float strokeRefundBank;
    
    [Header("Stroke Values")]
    public float reverbStrokes;
    public float zacPullStrokes;

    // Start is called before the first frame update
    void Start()
    {
        reverbing = false;
        master = GameObject.Find("Manager").GetComponent<Master>();
        ht = GameObject.Find("Manager").GetComponent<HitTiming>();
        pc = GetComponent<PlayerControls>();
        bm = GetComponent<BallMove>();
        camera = Camera.main;
        followBall = camera.GetComponent<FollowBall>();
        rb = GetComponent<Rigidbody>();
        am = AudioManager.instance;

        volume.profile.TryGet(out vignette);

        resetCharges();

        ballIcons = ballIconsObject.GetComponentsInChildren<Image>();

        strokeRefundBank = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (reverbing)
        {
            reverbStuff();
        }
        else
        {
            unReverbStuff();
        }

        if (groundReverbing)
        {
            reverbStuff();
            if (rb.velocity.magnitude < groundReverbStopThreshold)
            {
                groundReverbing = false;
                rb.drag /= groundReverbDragMultiplier;
            }
        }


        if (timeStopped)
        {
            timeStopStuff();
        }

        if (!timeResumed)
        {
            timeResumeStuff();
        }

        if (zacPlacing)
        {
            zacPlaceStuff();
        }

        if (iconsOn)
        {
            updateIconPositions();
        }

        if (master.gameState.getGameState() == GameState.gameState.aiming)
            resetCharges();
    }

    public void CastSpell(int i)
    {
        switch (i)
        {
            case 0:
                if (spell1Charges > 0)
                {
                    TheWorld();
                }
                break;
            case 1:
                if (spell2Charges > 0)
                {
                    Reverb();
                }
                break;
            case 2:
                if (spell3Charges > 0)
                {
                    ResetAttempt();
                }
                break;
            case 3:
                if (spell4Charges > 0)
                {
                    ZacPull();
                }
                break;
            case 4:
                if (spell5Charges > 0)
                {
                    PlaceholderSpell();
                }
                break;
            case 5:
                if (spell6Charges > 0)
                {
                    PlaceholderSpell();
                }
                break;
            case 6:
                if (spell7Charges > 0)
                {
                    PlaceholderSpell();
                }
                break;
        }
    }

    public void TheWorld()
    {
        if (master.gameState.getGameState() == GameState.gameState.flying)
        {
            timeStopped = true;
            glassBreak.Play();
            pc.setIsInSpellEffect(true);
            timeStopEffectsTimer = 0;
            timeStopParticles.Play();
            spell1Charges--;
            am.Play("Time Stop");
        }
    }

    public void Reverb()
    {
        if (master.gameState.getGameState() == GameState.gameState.flying)
        {
            if (colliding && Mathf.Abs(rb.velocity.y) < groundReverbThreshold)
            {
                groundReverbing = true;
                rb.drag *= groundReverbDragMultiplier;
            }
            else
            {
                bm.LaunchBall(0, -30, 0);
                reverbing = true;    

                pc.setIsInSpellEffect(true);
                am.Play("Precision Start");
            }
            spell2Charges--;
            
            pc.addStrokes(reverbStrokes);
            strokeRefundBank += reverbStrokes;
        }
    }

    public void ResetAttempt()
    {
        if (master.gameState.getGameState() == GameState.gameState.flying)
        {
            baseTrail.emitting = false;
            specialTrail.emitting = false;
            bm.respawn();
            if (timeStopped)
            {
                resumeTime();
            }
            reverbing = false;
            resetCharges();
            pc.ChangeBallFollow();
        }
        foreach (GameObject ball in master.getPlayerList())
        {
            if (master.getPlayer() != ball && ball.GetComponent<MeshRenderer>().enabled)
            {
                ball.GetComponent<BallMove>().respawn();
            }
        }
    }

    public void ZacPull()
    {
        if (master.gameState.getGameState() == GameState.gameState.aiming)
        {
            if (followBall.IsFollowingBall())
            {
                followBall.setFollowing(FollowBall.following.topDown);
                zacPlacing = true;
                zacPullIcon.enabled = true;
                activateIcons();
            }
            else if (followBall.getFollowing() == FollowBall.following.topDown)
            {
                followBall.setFollowing(FollowBall.following.ball);
                zacPlacing = false;
                zacPullIcon.enabled = false;
                disableIcons();
            }
        }
        else if (master.gameState.getGameState() == GameState.gameState.flying)
        {
            Vector3 launchDir = (zacPosition - transform.position);
            launchDir.y = 0;
            launchDir.Normalize();
            launchDir.y = .5f;
            launchDir *= zacPullPower;
            rb.velocity = Vector3.zero;
            bm.LaunchBall(launchDir);
            zacPullParticles.transform.LookAt(transform.position + launchDir);
            zacPullParticles.Play();

            am.Play("Zac Pull");

            /*
            GameObject playerTarget = master.getPlayerList()[zacPullTarget];
            playerTarget.GetComponent<PlayerControls>().setPlaceholder(false);
            launchDir = (zacPosition - playerTarget.transform.position);
            launchDir.y = 0;
            launchDir.Normalize();
            launchDir.y = .5f;
            launchDir *= zacPullPower;
            playerTarget.GetComponent<BallMove>().LaunchBall(launchDir);
            */

            pc.addStrokes(zacPullStrokes);
            strokeRefundBank += zacPullStrokes;

            hasZacPulled = true;

            spell4Charges--;
        }
    }

    public void zacPlaceStuff()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                zacPosition = hit.point;
            }

            /*
            Vector3 mousePos = Input.mousePosition;
            zacPosition = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1f));
            RaycastHit hit;
            Physics.Raycast(camera.transform.position, zacPosition - camera.transform.position, out hit, 1000f);
            zacPosition = hit.point;
            */
        }

        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            zacPullTarget--;
            if (zacPullTarget == -1)
            {
                zacPullTarget = master.getNumOfPlayers() - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            zacPullTarget++;
            if (zacPullTarget > master.getNumOfPlayers() - 1)
            {
                zacPullTarget = 0;
            }
        }

        if (master.getPlayerList()[zacPullTarget] == this.gameObject)
        {
            zacPullTarget++;
            if (zacPullTarget > master.getNumOfPlayers() - 1)
            {
                zacPullTarget = 0;
            }
        }
        */

        zacPullIcon.transform.position = camera.WorldToScreenPoint(zacPosition);
        
        /*
        zacPullTargetUI.text = "Selected Target: Player " + (zacPullTarget + 1);
        zacPullTargetUI.color = master.getPlayerList()[zacPullTarget].GetComponent<MeshRenderer>().material.color;
        */
    }

    public void PlaceholderSpell()
    {
        print(bm.name);
    }

    void reverbStuff()
    {
        baseTrail.emitting = false;
        specialTrail.emitting = true;
        if (!specialParticles.isPlaying)
        {
            specialParticles.Play();
        }
        if (!reverbTrailParticles.isPlaying)
        {
            reverbTrailParticles.Play();
        }
    }

    void unReverbStuff()
    {
        baseTrail.emitting = true;
        specialTrail.emitting = false;
        if (specialParticles.isPlaying)
        {
            specialParticles.Stop();
        }
        if (reverbTrailParticles.isPlaying)
        {
            reverbTrailParticles.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        colliding = true;

        if (reverbing)
        {
            rb.velocity = Vector3.up * 10;
            dust.Play();
            am.Play("Reverb Impact");
            reverbing = false;
            pc.setIsInSpellEffect(false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        colliding = true;

        if (reverbing)
        {
            rb.velocity = Vector3.up * 10;
            dust.Play();
            reverbing = false;
            pc.setIsInSpellEffect(false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        colliding = false;
    }

    public void resetCharges()
    {
        spell1Charges = 1;
        spell2Charges = 1;
        spell3Charges = 1;
        spell4Charges = 1;
        spell5Charges = 1;
        spell6Charges = 1;
        spell7Charges = 1;

        hasZacPulled = false;
        strokeRefundBank = 0;
    }
    
    public bool getTimeStop()
    {
        return timeStopped;
    }

    void timeStopStuff()
    {
        //effects
        timeStopEffectsTimer += Time.unscaledDeltaTime;

        float timeStopTimer = timeStopEffectsTimer / timeStopEffectsMaxTime;
        float timeStopValue = timeStopCurve.Evaluate(timeStopTimer);

        float scaleDiff = master.normalTimeScale - master.normalTimeScale / timeSlowMultiplier;
        Time.timeScale = master.normalTimeScale - scaleDiff * timeStopValue;

        float fixedDiff = master.normalFixedTime - master.normalFixedTime / timeSlowMultiplier;
        Time.fixedDeltaTime = master.normalFixedTime - fixedDiff * timeStopValue;


        //vignette

        vignette.intensity.value = timeStopVignetteIntensity * timeStopTimer;
        
        if (timeStopEffectsTimer >= timeStopEffectsMaxTime)
        {
            Time.timeScale = master.normalTimeScale / timeSlowMultiplier;
            Time.fixedDeltaTime = master.normalFixedTime / timeSlowMultiplier;
            vignette.intensity.value = timeStopVignetteIntensity;
        }


        if (timeStopEffectsTimer >= timeStopEffectsMaxTime)
        {
            //camera rotation
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pc.turnTime += Time.unscaledDeltaTime;
                bm.rotateBall(-pc.turnSpeed.Evaluate(pc.turnTime) * pc.turnMultiplier * Time.deltaTime * timeSlowMultiplier);
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                pc.turnTime = 0;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                pc.turnTime += Time.unscaledDeltaTime;
                bm.rotateBall(pc.turnSpeed.Evaluate(pc.turnTime) * pc.turnMultiplier * Time.deltaTime * timeSlowMultiplier);
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                pc.turnTime = 0;
            }


            //reset
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                resumeTime();
                return;
            }

            //aiming and simulation
            Vector3 velocity = rb.velocity;

            //calculate angle between current velocity and camera
            velocity.y = 0;
            Vector3 cameraY = camera.transform.forward;
            cameraY.y = 0;
            float y = Vector3.SignedAngle(velocity, cameraY, Vector3.up);

            //rotate velocity vector along y-axis
            Vector3 newVelocity = Quaternion.Euler(0, y, 0) * rb.velocity + new Vector3(0, timeStopAdditionalForce, 0);

            //simulate trajectory
            master.GetComponent<TrajectoryPrediction>().TimeStopSimulateTrajectory(gameObject, newVelocity);
            
            //actually hit the ball when button is pressed
            pc.timeStopHit(newVelocity);
        }

        //get kicked out of the world if stopped
        if (rb.velocity.magnitude < timeStopStopThreshold)
        {
            resumeTime();
        }
    }

    void timeResumeStuff()
    {
        timeStopEffectsTimer += Time.unscaledDeltaTime;

        float timeResumeTimer = timeStopEffectsTimer / timeResumeEffectsMaxTime;
        float timeResumeValue = timeStopCurve.Evaluate(timeResumeTimer);

        float scaleDiff = master.normalTimeScale - master.normalTimeScale / timeSlowMultiplier;
        Time.timeScale = master.normalTimeScale - scaleDiff + scaleDiff * timeResumeValue;

        float fixedDiff = master.normalFixedTime - master.normalFixedTime / timeSlowMultiplier;
        Time.fixedDeltaTime = master.normalFixedTime - fixedDiff + fixedDiff * timeResumeValue;


        //vignette
        vignette.intensity.value = timeStopVignetteIntensity - timeStopVignetteIntensity * timeResumeTimer;


        if (timeStopEffectsTimer >= timeResumeEffectsMaxTime)
        {
            Time.timeScale = master.normalTimeScale;
            Time.fixedDeltaTime = master.normalFixedTime;
            timeResumed = true;
            vignette.intensity.value = 0f;
        }
    }

    public void resumeTime()
    {
        timeStopped = false;
        timeResumed = false;
        //Time.timeScale = master.normalTimeScale;
        //Time.fixedDeltaTime = master.normalFixedTime;
        pc.setIsInSpellEffect(false);
        timeStopEffectsTimer = 0;
        timeStopParticles.Stop();
        ht.stopHit();
        master.GetComponent<TrajectoryPrediction>().EnableTimeLineRender(false);

        am.Play("Time Resume");
    }

    public void cancelTimeStop()
    {
        spell1Charges++;
        resumeTime();
    }

    public void activateIcons()
    {
        for (int i = 0; i < master.getNumOfPlayers(); i++)
        {
            ballIcons[i].enabled = true;
        }
        iconsOn = true;
    }

    public void disableIcons()
    {
        foreach (Image image in ballIcons)
        {
            image.enabled = false;
        }
        iconsOn = false;
    }

    void updateIconPositions()
    {
        for (int i = 0; i < master.getNumOfPlayers(); i++)
        {
            if (ballIcons[i].enabled)
            {
                ballIcons[i].transform.position = camera.WorldToScreenPoint(master.getPlayerList()[i].transform.position);
            }
            else
            {
                ballIcons[i].transform.position = new Vector3(12000, 12000, 12000);
            }
        }
    }

    public void UpdateSpellColors()
    {
        if (spell1Charges > 0)
            theWorld.color = theWorldColor;
        else
            theWorld.color = Color.white;

        if (spell2Charges > 0)
            reverb.color = reverbColor;
        else
            reverb.color = Color.white;

        if (spell4Charges > 0)
            zacPull.color = zacPullColor;
        else
            zacPull.color = Color.white;
    }

    /// <summary>
    /// Returns zac placing status.
    /// </summary>
    /// <returns></returns>
    public bool checkZacStatus()
    {
        return zacPlacing;
    }

    /// <summary>
    /// Returns if player has used zac pull yet.
    /// </summary>
    /// <returns></returns>
    public bool GetHasPulled()
    {
        return hasZacPulled;
    }

    public int[] getSpellCharges()
    {
        return new int[] {spell1Charges, spell2Charges, spell3Charges, spell4Charges, spell5Charges, spell6Charges, spell7Charges};
    }

    public void UpdateFlyingSpellEffects()
    {
        if (master.gameState.getGameState() == GameState.gameState.flying)
        {
            if (!hasZacPulled)
            {
                zacPullLine.enabled = true;
                zacPullLine.transform.LookAt(zacPosition);
                zacPullLine.transform.eulerAngles = new Vector3(0, zacPullLine.transform.eulerAngles.y, 0);
                zacPullLine.transform.position = transform.position;
            }
            else
            {
                zacPullLine.enabled = false;
            }
        }
    }

    public void SetZacPosition(Vector3 pos)
    {
        zacPosition = pos;
    }

    public void RefundSpells()
    {
        pc.addStrokes(-strokeRefundBank);
    }

    public void AddToStrokeRefundBank(float amount)
    {
        strokeRefundBank += amount;
    }
}
