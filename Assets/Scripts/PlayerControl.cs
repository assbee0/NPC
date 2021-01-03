using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float speed = 6.0f;
    public float gravity = 20.0f;
    private Rigidbody rb = null;
    private Animator animator = null;
    private Vector3 moveForward = Vector3.zero;
    private bool isRun = false;
    private bool isJump = false;
    private bool canSit = false;
    private bool isGrounded = true;
    private Vector2 move = new Vector2(0, 0);
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        Transform camera = Camera.main.transform;
        Vector3 cameraF = camera.forward;
        Vector3 playerF = new Vector3(cameraF.x, 0, cameraF.z);

        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        //move = move.normalized;
        move.x = move.x * Mathf.Sqrt(1 - (move.y * move.y) / 2.0f);
        move.y = move.y * Mathf.Sqrt(1 - (move.x * move.x) / 2.0f);
        isJump = Input.GetKeyDown("space");
        if (Input.GetKey("left shift"))
        {
            move = move / 4;
        }
        isRun = false;

        if (Input.GetKeyDown(KeyCode.F) && canSit)
        {
            animator.SetBool("isSit", true);
        }
        if (move.x != 0 || move.y != 0 )
            animator.SetBool("isSit", false);

        //Grounded();
        /*if (isRun)
        {
            if (movex > 0)
            {
                transform.Rotate(0, 90 * Time.deltaTime, 0);
            }
            else if (movex < 0)
            {
                transform.Rotate(0, -90 * Time.deltaTime, 0);
            }
        }if (movez > 0)
        {
            isRun = true;
            move = Vector3.forward * movez;
        }
        else if (movez < 0)
        {
            isRun = true;
            move = Vector3.forward * movez / 2;
        }*/

    }

    void FixedUpdate()
    {
        Transform camera = Camera.main.transform;
        Vector3 cameraF = camera.forward;
       // rb.AddForce(0, -gravity, 0, ForceMode.Acceleration);
        cameraF = new Vector3(cameraF.x, 0, cameraF.z).normalized;
        Vector3 moveForward = cameraF * move.y + Camera.main.transform.right * move.x;
        if (moveForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveForward);
            targetRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            transform.rotation = targetRotation;
            isRun = true;
            rb.velocity = moveForward * speed;// + new Vector3(0, rb.velocity.y, 0);
        }
        else
            rb.velocity = Vector3.zero;
        Grounded();
        UpdateAnimation();
    }

    private void Grounded()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + 0.8f * Vector3.up, Vector3.down, Color.red, 5);
        if (Physics.Raycast(transform.position + 0.8f * Vector3.up, Vector3.down, out hit, 2f)) 
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                transform.position = new Vector3(transform.position.x, hit.collider.gameObject.transform.position.y, transform.position.z);
                print(hit.collider.gameObject.name);
            }
        }
    }

    private void UpdateAnimation ()
    {
        animator.SetBool("isRun", isRun);
        animator.SetFloat("runSpeed", new Vector3(move.x, 0, move.y).magnitude);
    }

    public bool getIsRun()
    {
        return isRun;
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.tag == "Wall")
        {
            if (rb.velocity.magnitude <= 0.1)
                animator.SetBool("isRun", false);
            animator.SetFloat("runSpeed", rb.velocity.magnitude / 4f);
        }
    }
    private void OnTriggerStay(Collider collider)
    {
        if(collider.tag == "Chair")
        {
            canSit = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Chair")
        {
            canSit = false;
        }
    }
}
