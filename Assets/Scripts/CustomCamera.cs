using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamera : MonoBehaviour
{
    public GameObject target;
    Transform cam;
    Camera c;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.transform;
        c = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cam.name=="Camera3")
            closeto();
        HorizontalRotate();
    }
    public void faraway()
    {
        cam.position = new Vector3(0f, 0.8f, 1.5f);
        cam.rotation = Quaternion.Euler(0, 180, 0);
    }
    public void closeto()
    {
        cam.position = new Vector3(0, 0.05f, 0.5f) + target.transform.position;
        cam.LookAt(target.transform.position + new Vector3(0, 0.05f));
    }
    private void HorizontalRotate()
    {
        float RotateX = 0;
       // float moveX = Input.GetAxis("PS4Horizontal2");
        float moveX = Input.GetAxis("Mouse X");
        if (Input.GetMouseButton(1))
        {
            if (moveX > 0)
            {
                RotateX = 90 * Time.deltaTime * 4 * moveX;
            }
            else if (moveX < -0)
            {
                RotateX = 90 * Time.deltaTime * 4 * moveX;
            }
        }
        transform.RotateAround(target.transform.position, Vector3.up, RotateX);
    }
    public void CaptureScreen(int level)
    {
        Rect r = new Rect(0, 0, 200, 400);
        RenderTexture rt = new RenderTexture((int)r.width, (int)r.height, 0);

        c.targetTexture = rt;
        c.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(r, 0, 0);
        screenShot.Apply();

        c.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        string s;
        if (cam.name == "Camera1")
            s = "side";
        else if (cam.name == "Camera2")
            s = "frontfull";
        else
            s = "fronthead";

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = "E:/character dataset2/Level " + level + "/" + SaveLoad.number[level-1] + "/" + s + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);

    }
}
