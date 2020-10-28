using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    // Start is called before the first frame update
    public int pattern = 1;
    public float speed = 2.0f;
    public float gravity = 20.0f;
    private Rigidbody rb = null;
    private Animator animator = null;
    private Vector3 moveForward = Vector3.zero;
    private float randomspeed;
    private bool isRun = false;
    private Vector2 move = new Vector2(0, 0);
    // Use this for initialization
    void Start()
    {
        isRun = true;
        move = transform.forward/3;
        randomspeed = Random.Range(-0.5f, 1f);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (pattern == 1)
        {
            rb.velocity = transform.forward * (speed + randomspeed);
            UpdateAnimation();
        } 
        else
            transform.rotation = Quaternion.Euler(0, 90, 0);
        
    }
    void UpdateAnimation()
    {
        animator.SetBool("isRun", isRun);
        animator.SetFloat("runSpeed", move.magnitude + randomspeed);
    }
}
