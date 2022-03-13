using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvParameterGenerate : MonoBehaviour
{
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
    }
}
