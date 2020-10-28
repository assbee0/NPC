using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCustom : MonoBehaviour
{
    public GameObject model;
    private SkinnedMeshRenderer rend;
    private ParameterManage pm;
    // Start is called before the first frame update
    void Start()
    {
        rend = model.GetComponent<SkinnedMeshRenderer>();


    }

    // Update is called once per frame
    void LateUpdate()
    {
        pm = this.GetComponent<ParameterManage>();
        float headWidth = pm.getParameter(0);
        float headLength = pm.getParameter(1);
        float height = pm.getParameter(2);
        // float eyeY = pm.p[3];
        Transform neck = rend.bones[17];
        Transform body = rend.bones[0];
        neck.localScale = new Vector3(0.005f * headWidth + 0.75f, 0.005f * headLength + 0.75f, 1);
        body.localScale = new Vector3(1, 1, 1) * (height * 0.002f + 0.9f);
    }
}
