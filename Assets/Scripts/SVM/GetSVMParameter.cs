using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetSVMParameter : MonoBehaviour
{
    public float[] envParam; // publicじゃないとSVMExecuteでエラー吐く
    private const int VARIABLE_NUM = 2; // 取得するパラメタの数
    private const int ENV_PARAM1 = 8; // 仮（Debug用）
    private const int ENV_PARAM2 = 10; // 仮（Debug用）
    private const int ENV_PARAM3 = 8; // 仮（Debug用）
    private const int ENV_PARAM4 = 9; // 仮（Debug用）
    private const int ENV_PARAM5 = 9; // 仮（Debug用）

    // Start is called before the first frame update
    void Start()
    {
        envParam = new float[VARIABLE_NUM];
        envParam[0] = ENV_PARAM1;
        envParam[1] = ENV_PARAM2;
        //envParam[2] = ENV_PARAM3;
        //envParam[3] = ENV_PARAM4;
        //envParam[4] = ENV_PARAM5;
    }
}
