using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockOn : MonoBehaviour {

    public float up = 2.0f;
    public float back = 4.5f;
    private Camera cam;
    private Transform targetObject = null;
    private float RotateY = 0;
    private float RotateX = 0;
    // Use this for initialization
    void Start ()
    {
        cam = GetComponent<Camera>();
        cam.rect = new Rect(0, 0, 1, 1);
        targetObject = GameObject.Find("hadakacenter").transform;
        transform.position = targetObject.position - Vector3.forward * back + Vector3.up * up;
        transform.LookAt(targetObject.transform);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = targetObject.position - Vector3.forward * back + Vector3.up * up;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //bool L1 = Input.GetButton("PS4L1");
        // if (!L1)
        VerticalRotate();
        HorizontalRotate();
        ZoomInOut();
        transform.LookAt(targetObject.transform);
        cam.cullingMask = -1;
        RaycastHit hit;
        if (Physics.Linecast(targetObject.position, transform.position, out hit))
        {
            string name = hit.collider.gameObject.tag;
            if (name == "Wall")
            {
                float currentDistance = Vector3.Distance(hit.point, targetObject.position);
                if (currentDistance < back)
                {
                    transform.position = hit.point;
                }
                if (currentDistance < 0.5f)
                {
                    cam.cullingMask = ~(1 << 10);
                }
            }
        }

    }

    private void HorizontalRotate()
    {
        //float moveX = Input.GetAxis("PS4Horizontal2");
        float moveX = Input.GetAxis("Mouse X");
        if (Input.GetMouseButton(1))
        {
            RotateX += 90 * Time.deltaTime * 4 * moveX;
        }
        transform.RotateAround(targetObject.position, Vector3.up, RotateX);
    }

    private void VerticalRotate()
    {
        //float moveY = -Input.GetAxis("PS4Vertical2");
        float rotateY = RotateY;
        float moveY = Input.GetAxis("Mouse Y");
        if (Input.GetMouseButton(1))
        {
            rotateY += -90 * Time.deltaTime * 4 * moveY;
            if (rotateY > -20 && rotateY < 40)
                RotateY = rotateY;
        }
        transform.RotateAround(targetObject.position, transform.right, RotateY);
    }
    
    private void ZoomInOut()
    {
        //float zoom = Input.GetAxis("PS4Vertical2");
        float zoom = -Input.GetAxis("Mouse ScrollWheel");
        if(cam.fieldOfView <= 100 && zoom > 0 || cam.fieldOfView >= 20 && zoom < 0)
            cam.fieldOfView += zoom * Time.deltaTime * 1000;
    }
}
