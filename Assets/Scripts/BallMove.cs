using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class BallMove : MonoBehaviour
{
    public Rigidbody rb;
    public Transform reference;
    public GameState gs;
    public PlayerControls pc;
    Master master;

    public HitTiming ht;
    public FollowBall fb;

    public BaseSpell bs;

    public Vector3 respawnPos;

    float stopTimer;
    bool timerOn;

    bool colliding;

    public float waterResetPenalty;
    public float enemyWaterResetPenalty;

    Stopwatch stopwatch = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        respawnPos = transform.position;
        bs = GetComponent<BaseSpell>();
        pc = GetComponent<PlayerControls>();
        master = GameObject.Find("Manager").GetComponent<Master>();
    }

    // Update is called once per frame
    void Update()
    {
        respawnTimer();


        if (master.getPlayer() != gameObject)
        {
            updateTimer();
            placeholderCheck();
        }
    }

    void updateTimer()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude == 0 && GetComponent<MeshRenderer>().enabled == true)
        {
            timerOn = true;
        }
        else
        {
            timerOn = false;
            stopTimer = 0;
        }

        if (timerOn)
        {
            stopTimer += Time.deltaTime;
        }
    }

    void respawnTimer()
    {
        if (stopwatch.ElapsedMilliseconds >= 2000)
        {
            stopwatch.Stop();
            stopwatch.Reset();
            respawn();
            fb.setFollowing(FollowBall.following.ball);
        }
    }

    public void LaunchBall(float forward, float upward)
    {
        Vector3 direction = reference.forward * forward;
        direction.y = upward;
        rb.AddForce(direction, ForceMode.Impulse);
    }

    public void LaunchBall(Vector2 dir)
    {
        Vector3 direction = reference.forward * dir.x;
        direction.y = dir.y;
        rb.AddForce(direction, ForceMode.Impulse);
    }

    public void LaunchBall(float x, float y, float z)
    {
        rb.AddForce(x, y, z, ForceMode.Impulse);
    }

    public void LaunchBall(Vector3 direction)
    {
        rb.AddForce(direction.x, direction.y, direction.z, ForceMode.Impulse);
    }

    public void LaunchBallDirect(Vector3 direction)
    {
        rb.AddForce(direction.x, direction.y, direction.z, ForceMode.VelocityChange);
    }

    public void rotateBall(float degrees)
    {
        Vector3 rotation = new Vector3(0, degrees, 0);
        reference.Rotate(rotation, Space.World);
        transform.Rotate(rotation, Space.World);
    }

    public void CastSpell(int index)
    {
        bs.CastSpell(index);
    }

    public void setRespawn()
    {
        respawnPos = transform.position;
    }

    public void respawn()
    {
        transform.position = respawnPos;
        master.GetFollowBall().ResetCameraAngleY(master.GetPreSwingRotation());
        rb.velocity = Vector3.zero;
        bs.resetCharges();
        gs.setGameState(GameState.gameState.aiming);
    }

    public void waterReset(int i)
    {
        fb.setFollowing(FollowBall.following.nothing);
        stopwatch.Restart();
        pc.addStrokes(waterResetPenalty);
        bs.RefundSpells();
    }

    public void softWaterReset()
    {
        transform.position = respawnPos;
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        colliding = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        colliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        colliding = false;
    }

    public bool getColliding()
    {
        return colliding;
    }

    public void placeholderCheck()
    {
        if (rb.velocity.magnitude == 0 && stopTimer > 1f)
        {
            GetComponent<PlayerControls>().setPlaceholder(true);
            setRespawn();
        }
    }
}
