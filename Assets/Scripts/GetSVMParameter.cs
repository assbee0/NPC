using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSVMParameter : MonoBehaviour
{
    public int[] var;
    //Collider[] targets;
    public int radius = 100;
    NPCAnimationController script;
    private int VARNUM = 3;
    public float[,] test;
    public float var1;
    public float var2;
    public float var3;

    // Start is called before the first frame update
    void Start()
    {
        //getAroundNPCValue();
    }

    // Update is called once per frame
    void Update()
    {
        test = getAroundNPCValue();
        //Debug.Log(test);
    }

    public float[,] getAroundNPCValue()
    {
        // 半径radius、中心自分の位置の球内に存在するものを検出
        Collider[] targets_cld = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> targets = new List<GameObject>();
        foreach (Collider cld in targets_cld) {
            if (cld.tag == "NPC") {
                targets.Add(cld.gameObject);
                Debug.Log("aaaaa");
            }
        }

        int targetCount = targets.Count;
        float[,] parameters = new float[targetCount, VARNUM];
        int i = 0;
        
        foreach (GameObject obj in targets) {
            Debug.Log(obj);
            // 対象となるGameObject(Collider)との距離を調べる
            parameters[i, 0] = Vector3.Distance(obj.transform.position, transform.position);
            // 対象となるGameObject(Collider)のA-V値を調べる
            script = obj.gameObject.GetComponent<NPCAnimationController>();
            parameters[i, 1] = script.arousal;
            parameters[i, 2] = script.valence;
        }
        return parameters;
    }
}
