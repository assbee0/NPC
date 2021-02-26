using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    // Start is called before the first frame update
    private float valence = 0;
    private float arousal = 0;
    private float initValence = 0;
    private NavMeshAgent nma = null;
    private Animator animator = null;
    private Seat mySeat;
    private PerformanceSlider performance = null;

    enum AIState
    {
        Spawn,
        Pathing,
        Sit,
        Stand,
        Leave
    }

    private AIState curState;

    private Vector3 startPos;
    private Vector3 curDst;
    private int pattern = 0;
    private float standardSpeed = 2.0f;
    private bool isRun = false;
    private bool isSit = false;
    // Use this for initialization
    void Start()
    {
        initValence = Random.Range(0f, 100f);
        valence = initValence;
        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        curState = AIState.Spawn;
    }

    // Update is called once per frame
    void Update()
    {
        valence += arousal * Time.deltaTime / 50;
        if (valence >= 100 + initValence)
            valence = 100 + initValence;
        switch (curState)
        {
            case AIState.Spawn:
                DoSpawn();
                break;
            case AIState.Pathing:
                DoPathing();
                break;
            case AIState.Sit:
                DoSit();
                break;
            case AIState.Stand:
                DoStand();
                break;
            case AIState.Leave:
                DoLeave();
                break;
            default:
                break;
        }
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        
        
    }
    void UpdateAnimation()
    {
        animator.SetBool("isSit", isSit);
        animator.SetBool("isRun", isRun);
        float animeSpeed = (nma.velocity.magnitude - 2)/ 2;
        animator.SetFloat("runSpeed", animeSpeed >0? animeSpeed : 0);
    }

    public void SetPattern(int p)
    {
        pattern = p;
        curState = AIState.Spawn;
    }

    private void DoSpawn()
    {
        int randomSide = Random.Range(1, 4);
        float randomPos = Random.Range(-30f, 30f);
        switch (randomSide)
        {
            case 1:
                startPos = new Vector3(randomPos, 0f, -45 + randomPos / 7);
                break;
            case 2:
                startPos = new Vector3(randomPos, 0f, 45 + randomPos / 7);
                break;
            case 3:
                startPos = new Vector3(45 + randomPos / 7, 0f, randomPos);
                break;
            default:
                break;
        }
        nma.Warp(startPos);
        OnEnterPathing();
    }
    private void OnEnterPathing()
    {
        curState = AIState.Pathing;

        Seats seatsNPC = GameObject.Find("ChairsGroup").GetComponent<GymManager>().GetSeats();
        int index = (int)((100 - valence) * seatsNPC.IdleSeatsCount() / 100f);
        Vector2 tempDst = seatsNPC.GetSeatPos(index);
        curDst = new Vector3(tempDst.x - 0.2f, 0.833f, tempDst.y);
        mySeat = seatsNPC.UseSeat(index);

        nma.SetDestination(curDst);

        nma.speed = standardSpeed + valence / 50f;
        isRun = true;
    }
    private void DoPathing()
    {
        if (isRun && Mathf.Abs(nma.remainingDistance) < 0.3f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-Vector3.right);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            if (Quaternion.Angle(transform.rotation, lookRotation) <= 0.01f)
            {
                if (arousal != 100)
                    OnEnterSit();
                else
                    OnEnterStand();
            }
        }
    }

    private void OnEnterSit()
    {
        curState = AIState.Sit;

        isRun = false;
        isSit = true;

    }

    private void DoSit()
    {
        if(!performance)
            performance = GameObject.Find("Canvas_Play").GetComponentInChildren<PerformanceSlider>();
        else
            arousal = performance.GetFever() * 10f;

        if (arousal == 100)
            OnEnterStand();

        if (valence < 0)
            OnEnterLeave();
    }

    private void OnEnterStand()
    {
        curState = AIState.Stand;

        isRun = false;
        isSit = false;

    }

    private void DoStand()
    {
        if (!performance)
            performance = GameObject.Find("Canvas_Play").GetComponentInChildren<PerformanceSlider>();
        else
            arousal = performance.GetFever() * 10f;

        if (arousal < 100)
            OnEnterSit();

        if (valence < 0)
            OnEnterLeave();
    }

    private void OnEnterLeave()
    {
        curState = AIState.Leave;

        float randomPos = Random.Range(-30f, 30f);
        startPos = new Vector3(45, 0f, randomPos);
        nma.SetDestination(startPos);

        Seats seatsNPC = GameObject.Find("ChairsGroup").GetComponent<GymManager>().GetSeats();
        seatsNPC.ReleaseSeat(mySeat);

        isRun = true;
        isSit = false;
    }

    private void DoLeave()
    {
        if (isRun && Vector3.Distance(transform.position, startPos) < 0.3f)
        {
            Destroy(gameObject);
            performance.DestoryNPC();
        }
    }

    public void ArousalPlus()
    {
        if (arousal < 100)
            arousal += 10;
    }

    public void ArousalMinus()
    {
        if (arousal > -100)
            arousal -= 10;
    }
}
