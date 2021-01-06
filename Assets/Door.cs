using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Transform axis;
    private BoxCollider bc;
    public float maxAngle = 120;
    public float rotateSpeed = 100;
    private float angleCount = 0;
    private bool flag = false;
    private bool state = false;
    // Start is called before the first frame update
    void Start()
    {
        axis = transform.GetChild(1);
        bc = axis.GetChild(0).GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            if(state)
                DoorCloseAnimation();
            else
                DoorOpenAnimation();
        }
    }

    public void DoorOpenAnimation()
    {
        if (angleCount >= maxAngle)
        {
            state = true;
            flag = false;
            bc.enabled = true;
            return;
        }
        float angle = rotateSpeed * Time.deltaTime;
        angleCount += angle;
        axis.Rotate(Vector3.up, angle);
    }

    public void DoorCloseAnimation()
    {
        if (angleCount >= maxAngle)
        {
            state = false;
            flag = false;
            bc.enabled = true;
            return;
        }
        float angle = rotateSpeed * Time.deltaTime;
        angleCount += angle;
        axis.Rotate(Vector3.up, -angle);
    }

    public void ActiveFlag()
    {
        if (flag)
            return;
        angleCount = 0;
        bc.enabled = false;
        flag = true;
    }
}
