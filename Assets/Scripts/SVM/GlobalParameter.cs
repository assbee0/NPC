using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalParameter : MonoBehaviour
{
    public int ENVPARAMNUM = 2;     // 取得するパラメタの数
    public bool isOnlyWave;
    public bool isLegend;
    public int COUNT;
    // Start is called before the first frame update
    void Start()
    {
        isOnlyWave = true;
        isLegend = true;
        InvokeRepeating("CountUp", 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CountUp()
    {
        COUNT += 1;
        //Debug.Log("count: " + count);
    }
}
