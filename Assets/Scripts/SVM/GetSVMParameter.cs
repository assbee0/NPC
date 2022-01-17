using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetSVMParameter : MonoBehaviour
{
    NPCAnimationController NPCAnimCon;
    public float[] envParam; // publicじゃないとSVMExecuteでエラー吐く
    //public int Param1 = 8; // 仮（Debug用）
    //public int Param2 = 10; // 仮（Debug用）
    //public int Param3 = 8; // 仮（Debug用）
    //public int Param4 = 9; // 仮（Debug用）
    //public int Param5 = 9; // 仮（Debug用）
    private GameObject testManager;
    private EnvParameterGenerate envParaGen;
    private float sensitivity; // 感応度合い

    public float coheAr = 0;
    public float coheVa = 0;

    // Start is called before the first frame update
    void Start()
    {
        NPCAnimationController ownAnimCon = gameObject.GetComponent<NPCAnimationController>();
        CharacterParameter cp = gameObject.GetComponent<CharacterParameter>();
        testManager = GameObject.Find("TestManager");
        envParaGen = testManager.GetComponent<EnvParameterGenerate>();

        //int MIN = 1;
        //int MAX = 10;

        /*
        envParam[0] = Mathf.Clamp(envParaGen.Param1 + UnityEngine.Random.Range(-3, 4), MIN, MAX);
        envParam[1] = Mathf.Clamp(envParaGen.Param2 + UnityEngine.Random.Range(-3, 4), MIN, MAX);
        envParam[2] = Mathf.Clamp(envParaGen.Param3 + UnityEngine.Random.Range(-3, 4), MIN, MAX);
        envParam[3] = Mathf.Clamp(envParaGen.Param4 + UnityEngine.Random.Range(-3, 4), MIN, MAX);
        envParam[4] = Mathf.Clamp(envParaGen.Param5 + UnityEngine.Random.Range(-3, 4), MIN, MAX);
        */
        
        //cp.envParameter[0] = envParaGen.Param1;
        //cp.envParameter[1] = envParaGen.Param2;
        //envParam[2] = envParaGen.Param3;
        //envParam[3] = envParaGen.Param4;
        //envParam[4] = envParaGen.Param5;
        
    }

        // Update is called once per frame
    void Update()
    {

    }

    public float[] EnvParam {
        get{ return envParam; }
        set{ envParam = value;}
    }
}
