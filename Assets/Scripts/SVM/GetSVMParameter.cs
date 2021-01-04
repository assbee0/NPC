using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSVMParameter : MonoBehaviour
{
    NPCAnimationController NPCAnimCon;
    public int radius = 100;
    public float[] envParam; // publicじゃないとSVMExecuteでエラー吐く
    private int VARNUM = 5; // 取得するパラメタの数
    //public int Param1 = 8; // 仮（Debug用）
    //public int Param2 = 10; // 仮（Debug用）
    //public int Param3 = 8; // 仮（Debug用）
    //public int Param4 = 9; // 仮（Debug用）
    //public int Param5 = 9; // 仮（Debug用）
    private float AROUSAL;
    private float VALENCE;
    private GameObject testManager;
    private EnvParameterGenerate envParaGen;

    // Start is called before the first frame update
    void Start()
    {
        envParam = new float[VARNUM];
        NPCAnimationController ownAnimCon = gameObject.GetComponent<NPCAnimationController>();
        AROUSAL = ownAnimCon.Arousal;
        VALENCE = ownAnimCon.Valence;
        testManager = GameObject.Find("TestManager");
        envParaGen = testManager.GetComponent<EnvParameterGenerate>();
        //getAroundAVValue();
    }

    // Update is called once per frame
    void Update()
    {
        float[] tmpArray = getAroundAVValue();
        /*
        envParam[0] = tmpArray[0];
        envParam[1] = tmpArray[1];
        envParam[2] = Param1;
        envParam[3] = Param2;
        */

        envParam[0] = envParaGen.Param1;
        envParam[1] = envParaGen.Param2;
        envParam[2] = envParaGen.Param3;
        envParam[3] = envParaGen.Param4;
        envParam[4] = envParaGen.Param5;

        //envParam = 

        //Debug.Log("Param[0]: " + envParam[0]);
        //Debug.Log("Param[1]: " + envParam[1]);

    }

    /*
    envParam[0]: 周囲のArousal値
    envParam[1]: 周囲のValence値
    envParam[2]: Param1(仮)
    envParam[3]: Param2(仮)
    */

    public float[] getAroundAVValue()
    {
        // 中心:自分の位置, 半径:radiusの球内に存在するものを検出
        Collider[] arrayCld = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> listObj = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをlistObjリストに追加
        foreach (Collider cld in arrayCld) {
            if (cld.tag == "NPC") {
                listObj.Add(cld.gameObject);
            }
        }

        float[] avValue = new float[2];
        foreach (GameObject obj in listObj) {
            // 対象となるGameObjectとの距離を調べる
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            // 対象となるGameObjectのA-V値を調べる
            NPCAnimCon = obj.gameObject.GetComponent<NPCAnimationController>();
            float arousal = NPCAnimCon.Arousal;
            float valence = NPCAnimCon.Valence;

            // 周囲の累計A-V値を更新
            updateAVValue(avValue, dist, arousal, valence);
        }
        return avValue;
    }

    public void updateAVValue(float[] avValue, float dist, float ar, float va)
    {
        //avValue[0] = ar;
        //avValue[1] = va;

        //avValue[0] = avValue[0] + ar - avValue[0];
        //avValue[1] = avValue[1] + va - avValue[1];

        //avValue[0] = avValue[0] + (ar - AROUSAL) / dist;
        //avValue[1] = avValue[1] + (va - VALENCE) / dist;

        avValue[0] = avValue[0] + (ar - AROUSAL)/3; // 後で要修正
        avValue[1] = avValue[1] + (va - VALENCE)/3;
    }

    public float[] EnvParam {
        get{ return envParam; }
        set{ envParam = value;}
    }
}
