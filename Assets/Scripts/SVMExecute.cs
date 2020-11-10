using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVMExecute : MonoBehaviour
{
    const int CLASSNUM = 2;
    const int HUNITNUM = 150;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Readw()
    {
        string filepath = "C:\git\NPC 2.0\Assets\SVM\Parameters";
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for(int i = 0; i < HUNITNUM; i++)
        {
            w[i] = new float[CLASSNUM];
            string wline = sr.ReadLine();
            string[] ws = wline.Split(' ');
            w[i][0] = float.Parse(ws[0]);
            w[i][1] = float.Parse(ws[1]);
            //w[i][2] = float.Parse(ws[2]);
        }
        sr.Close();
    }
}
