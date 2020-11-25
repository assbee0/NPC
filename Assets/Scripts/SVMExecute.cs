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
    public int A_testParam1;
    public int A_testParam2;
    public int V_testParam1;
    public int V_testParam2;

    // Start is called before the first frame update
    void Start()
    {
        A_testParam1 = Random.Range(0, 11);
        A_testParam2 = Random.Range(0, 11);
        V_testParam1 = Random.Range(0, 11);
        V_testParam2 = Random.Range(0, 11);
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
        GetSVMParameter svmp = GetComponent<GetSVMParameter>();
        float[] param = svmp.ArrayParm;
        OutunitTest outunit_A = new OutunitTest();
        float[] input_A = new float[2] {A_testParam1, A_testParam2}; // ここをどうにかする！！！！！！！！！！
        //float[] input_A = new float[2] {8, 9};
        //input_A[0] = param[0];
        //input_A[1] = param[1];
        //input = ParaTransform(input);
        //outunit.Readw();
        outunit_A.Readb(0); // 0: Arousal, 1: Valence
        outunit_A.Readw(0);
        outunit_A.Propagation(input_A);
        //outunit.Propagation(hunit.output);
        result_A = Argmax(outunit_A.output); // result：カテゴリ
        //EvaluateOutput();

        // Valenceについて
        OutunitTest outunit_V = new OutunitTest();
        float[] input_V = new float[2] {V_testParam1, V_testParam2}; // ここをどうにかする！！！！！！！！！！
        //input = ParaTransform(input);
        //outunit.Readw();
        outunit_V.Readb(1);
        outunit_V.Readw(1);
        outunit_V.Propagation(input_V);
        //outunit.Propagation(hunit.output);
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
        }
        //return argmax+1;
        return argmax;
    }
}

class OutunitTest
{
    const int CLASSNUM = 3;
    const int HUNITNUM = 2;
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