using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvParameterGenerate : MonoBehaviour
{
    public int Param1 = 8; // 仮（Debug用）
    public int Param2 = 10; // 仮（Debug用）
    public int Param3 = 8; // 仮（Debug用）
    public int Param4 = 9; // 仮（Debug用）
    public int Param5 = 9; // 仮（Debug用）

    public int test_catA = 0;
    public int test_catV = 0;

    public bool isOnlyWave = false;

    public float sensitivity = 10;

    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CountUp", 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CountUp()
    {
        count += 1;
        //Debug.Log("count: " + count);
    }

    /*
    public float[] EnvParam
    {
        get { return envParam; }
        set { envParam = value; }
    }
    */
}
