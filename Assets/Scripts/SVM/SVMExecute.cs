using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class SVMExecute : MonoBehaviour
{
    const int CLASSNUM = 3;
    public int result_A = -1;
    public int result_V = -1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Predict()
    {
        // Arousalについて
        //ParameterManage pm = GetComponent<ParameterManage>();
        //Hunit hunit = new Hunit();
        GetSVMParameter svmP = GetComponent<GetSVMParameter>();
        float[] envParam = svmP.EnvParam;
        OutunitTest outunit_A = new OutunitTest();
        //float[] input_A = new float[5] { 1, 9, 3, 6, 3 }; // ここをどうにかする！！！！！！！！！！
        //input = ParaTransform(input);
        outunit_A.Readb(0); // 0: Arousal, 1: Valence
        outunit_A.Readw(0);
        //outunit_A.Propagation(input_A);
        outunit_A.Propagation(envParam);
        //outunit.Propagation(hunit.output);
        result_A = Argmax(outunit_A.output); // result：カテゴリ
        //EvaluateOutput();

        // Valenceについて
        OutunitTest outunit_V = new OutunitTest();
        //float[] input_V = new float[2] {V_testParam1, V_testParam2}; // ここをどうにかする！！！！！！！！！！
        //float[] input_V = new float[5] {1, 9, 3, 6, 3}; // ここをどうにかする！！！！！！！！！！
        //input = ParaTransform(input);
        outunit_V.Readb(1);
        outunit_V.Readw(1);
        //outunit_V.Propagation(input_V);
        outunit_V.Propagation(envParam);
        result_V = Argmax(outunit_V.output); // result：カテゴリ
        //EvaluateOutput();
    }    

    public static float Dot(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            return 0;
        float d = 0;
        for (int i = 0; i < a.Length; i++)
        {
            d += a[i] * b[i];
        }
        return d;
    }

    public static float StepFunction(float x)
    {
        if (x > 0)
            return 1;
        else
            return 0;
    }

    public static float[] Softmax(float[] x)
    {
        float[] y = new float[CLASSNUM];
        float sum = 0;
        for(int i=0; i<x.Length;i++)
        {
            x[i] = Mathf.Exp(x[i]);
            sum += x[i];
        }
        for (int i = 0; i < x.Length; i++)
        {
            y[i] = x[i] / sum;
        }
        return y;
    }

    private int Argmax(float[] input)
    {
        float max = -1;
        int argmax = -1;
        for(int i = 0; i < input.Length; i++)
        {
            if (input[i] > max)
            {
                max = input[i];
                argmax = i;
            }

            if (input[i] > 0)
            {
                //return i;
            } 
        }
        //return argmax+1;
        return argmax;
        return -999;
    }
}

class OutunitTest
{
    const int CLASSNUM = 3; // それぞれHigh, Medium, Low
    const int HUNITNUM = 2; // 取得するパラメタの数
    public float[] b = new float[CLASSNUM];
    public float[][] w = new float[CLASSNUM][];
    public float[] u = new float[CLASSNUM];
    public float[] output = new float[CLASSNUM];

    void Start()
    {

    }

    public void Propagation(float[] x)
    {
        for (int i = 0; i < CLASSNUM; i++)
        {
            u[i] = SVMExecute.Dot(getRow(w, i), x) + b[i];
            output[i] = SVMExecute.StepFunction(u[i]);
        }
    }

    public void Readb(int catg)
    {
        string filepath = "";
        if (catg == 0) {
            filepath = Application.dataPath + "/SVM/Parameters/A_intercept.txt";
        } else if (catg == 1) {
            filepath = Application.dataPath + "/SVM/Parameters/V_intercept.txt";
        }
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for (int i = 0; i < CLASSNUM; i++)
        {
            string wline = sr.ReadLine();
            b[i] = float.Parse(wline);
        }
        sr.Close();
    }

    public void Readw(int catg) // coefが重みW
    {
        string filepath = "";
        if (catg == 0) {
            filepath = Application.dataPath + "/SVM/Parameters/A_coef.txt";
        } else if (catg == 1) {
            filepath = Application.dataPath + "/SVM/Parameters/V_coef.txt";
        }
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for(int i = 0; i < CLASSNUM; i++)
        {
            w[i] = new float[HUNITNUM];
            string wline = sr.ReadLine();
            string[] ws = wline.Split(' ');
            for(int j = 0; j < HUNITNUM; j++)
            {
                w[i][j] = float.Parse(ws[j]);
            }
        }
        sr.Close();
    }

    private float[] getRow(float[][] x, int row)
    {
        //Debug.Log(x.Length);
        float[] a = new float[x[0].Length];
        for (int i = 0; i < x[0].Length; i++)
        {
            //Debug.Log(i);
            a[i] = x[row][i]; // 修正したけど、これ合ってる？
            //Debug.Log(a[i]);
        }
        return a;
    }
}