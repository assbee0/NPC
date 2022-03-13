using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class SVMExecute : MonoBehaviour
{
    const int CLASSNUM = 3;
    public int catArResult = -1;
    public int catVaResult = -1;

    public void Predict()
    {
        //////////////////////////////////////////////////////////
        // 環境パラメタ取得
        GetSVMParameter svmP = GetComponent<GetSVMParameter>();
        float[] envParam = svmP.envParam;

        //////////////////////////////////////////////////////////
        // A-Vカテゴリ決定
        // Arousalについて
        OutunitTest outunit_A = new OutunitTest();
        outunit_A.Readb(0); // 0: Arousal, 1: Valence
        outunit_A.Readw(0);
        outunit_A.Propagation(envParam);
        catArResult = Argmax(outunit_A.output); // result：カテゴリ

        // Valenceについて
        OutunitTest outunit_V = new OutunitTest();
        outunit_V.Readb(1);
        outunit_V.Readw(1);
        outunit_V.Propagation(envParam);
        catVaResult = Argmax(outunit_V.output); // result：カテゴリ
    }    

    public static float Dot(float[] a, float[] b)
    {
        if (a.Length != b.Length)   return 0;
        float d = 0;
        for (int i = 0; i < a.Length; i++)
            d += a[i] * b[i];
        return d;
    }

    public static float StepFunction(float x)
    {
        if (x > 0)    return 1;
        else          return 0;
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
            y[i] = x[i] / sum;
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
        return argmax;
    }
}

class OutunitTest
{
    const int CLASSNUM = 3; // 行：それぞれHigh, Medium, Low
    const int HUNITNUM = 2; // 列：取得するパラメタの数
    public float[] b = new float[CLASSNUM];  // 3行1列 y=sign(wx+b)
    public float[][] w = new float[CLASSNUM][];  // 3行2列 y=sign(wx+b)
    public float[] u = new float[CLASSNUM];
    public float[] output = new float[CLASSNUM];
    private const string PATH_AROUSAL_H = "/SVM/Parameters/A_intercept.txt";
    private const string PATH_VALENCE_H = "/SVM/Parameters/V_intercept.txt";
    private const string PATH_AROUSAL_W = "/SVM/Parameters/A_coef.txt";
    private const string PATH_VALENCE_W = "/SVM/Parameters/V_coef.txt";

    public void Propagation(float[] x)
    {
        for (int i = 0; i < CLASSNUM; i++)
        {
            u[i] = SVMExecute.Dot(getRow(w, i), x) + b[i];  // wx + b
            output[i] = SVMExecute.StepFunction(u[i]);  // sign(wx + b), 推定されたカテゴリの値だけ正、それ以外負
        }
    }

    public void Readb(int catg)
    {
        string filepath = "";
        if (catg == 0)
            filepath = Application.dataPath + PATH_AROUSAL_H;
        else if (catg == 1)
            filepath = Application.dataPath + PATH_VALENCE_H;
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
        if (catg == 0)
            filepath = Application.dataPath + PATH_AROUSAL_W;
        else if (catg == 1)
            filepath = Application.dataPath + PATH_VALENCE_W;
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
        float[] a = new float[x[0].Length];
        for (int i = 0; i < x[0].Length; i++)
            a[i] = x[row][i];
        return a;
    }
}