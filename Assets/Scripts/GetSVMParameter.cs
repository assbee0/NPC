using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSVMParameter : MonoBehaviour
{
    NPCAnimationController script;
    public int radius = 100;
    private int VARNUM = 3; // 取得するパラメタの数

    // Start is called before the first frame update
    void Start()
    {
        //getAroundNPCValue();
    }

    // Update is called once per frame
    void Update()
    {
        getAroundNPCValue();
    }

    public float[,] getAroundNPCValue()
    {
        // 中心:自分の位置, 半径:radiusの球内に存在するものを検出
        Collider[] targets_cld = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> targets = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをtargetsリストに追加
        foreach (Collider cld in targets_cld) {
            if (cld.tag == "NPC") {
                targets.Add(cld.gameObject);
            }
        }

        int targetCount = targets.Count;
        float[,] parameters = new float[targetCount, VARNUM];
        int i = 0;
        
        foreach (GameObject obj in targets) {
            // 対象となるGameObject(Collider)との距離を調べる
            parameters[i, 0] = Vector3.Distance(obj.transform.position, transform.position);
            // 対象となるGameObject(Collider)のA-V値を調べる
            script = obj.gameObject.GetComponent<NPCAnimationController>();
            parameters[i, 1] = script.Arousal;
            parameters[i, 2] = script.Valence;
        }
        return parameters;
    }
}
