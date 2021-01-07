using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using UnityEngine.UI;


public class NetworkExecute : MonoBehaviour
{
    const int FEATURE = 42;
    const int CLASSNUM = 3;
    private int result = -1;
    //public GameObject textobj;
    //public GameObject sliderBeautyobj;
    private Text tex;
    private Slider sliderBeauty;
    public StreamWriter sw;
    public StreamReader sr;
    private Color[] haircolor = { new Color(246, 229, 213), new Color(75, 66, 111), new Color(236, 148, 147), new Color(150, 113, 105),
                                  new Color(243, 205, 192), new Color(229, 237, 248),new Color(254, 244, 185),new Color(0, 0, 0),
                                  new Color(64, 70, 80), new Color(170, 61, 69), new Color(83, 88, 128), new Color(48, 63, 70), new Color(227, 166, 116)};

    private Color[] clothesPalette =
    {
        new Color(190, 183, 178), new Color(38, 47, 72),  new Color(144, 129, 119),
        new Color(235, 229f, 227), new Color(113, 95, 90), new Color(10, 11, 12),
        new Color(43, 119, 178), new Color(87, 34, 27), new Color(244, 245, 249),
        new Color(182, 157, 149), new Color(47, 39, 46), new Color(56, 89, 122),
        new Color(209, 209, 211), new Color(28, 26, 31), new Color(139, 184, 239),
        new Color(239, 207, 172), new Color(76, 67, 69), new Color(118, 131, 164),
    };
    // Start is called before the first frame update
    void Start()
    {
        //tex = textobj.GetComponent<Text>();
        //sliderBeauty = sliderBeautyobj.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Predict()
    {
        ParameterManage pm = GetComponent<ParameterManage>();
        Hunit hunit = new Hunit();
        Outunit outunit = new Outunit();
        float[] input = pm.getParameters();
        input = ParaTransform(input);
        outunit.Readw();
        hunit.Readb();
        hunit.Readw();
        hunit.Propagation(input);
        outunit.Propagation(hunit.output);
        result = Argmax(outunit.output);
        EvaluateOutput();
    }
    public void Generate()
    {
        int level = (int)sliderBeauty.value;
        ParameterManage pm = GetComponent<ParameterManage>();
        Hunit hunit = new Hunit();
        Outunit outunit = new Outunit();
        outunit.Readw();
        hunit.Readb();
        hunit.Readw();
        int count = 0;
        while (true)
        {
            count++;
            float[] input = pm.FullRandomTemp();
            float[] input2 = ParaTransform(input);
            hunit.Propagation(input2);
            outunit.Propagation(hunit.output);
            result = Argmax(outunit.output);
            if (result == level)
            {
                pm.setAllParameter(input);
                break;
            }
        }
    }
    public void RTGenerate(int level)
    {
        ParameterManage pm = GetComponent<ParameterManage>();
        Hunit hunit = new Hunit();
        Outunit outunit = new Outunit();
        outunit.Readw();
        hunit.Readb();
        hunit.Readw();
        while (true)
        {
            float[] input = pm.FullRandomTemp();
            float[] input2 = ParaTransform(input);
            hunit.Propagation(input2);
            outunit.Propagation(hunit.output);
            result = Argmax(outunit.output);
            if (result == level)
            {
                pm.setAllParameter(input);
                int i = Random.Range(0, 13);
              //  pm.OutputParameterVectors(sw);
                pm.setParameter(34, haircolor[i].r);
                pm.setParameter(35, haircolor[i].g);
                pm.setParameter(36, haircolor[i].b);
                break;
            }
        }
    }
    public void RandomGenerate()
    {
        ParameterManage pm = GetComponent<ParameterManage>();
        pm.FullRandom();

        int i = Random.Range(0, 13);
        pm.setParameter(34, haircolor[i].r);
        pm.setParameter(35, haircolor[i].g);
        pm.setParameter(36, haircolor[i].b);

        i = Random.Range(0, 18);
        int j = Random.Range(0, 18);
        pm.setParameter(54, clothesPalette[i].r);
        pm.setParameter(55, clothesPalette[i].g);
        pm.setParameter(56, clothesPalette[i].b);
        pm.setParameter(57, clothesPalette[j].r);
        pm.setParameter(58, clothesPalette[j].g);
        pm.setParameter(59, clothesPalette[j].b);
        i = Random.Range(0, 18);
        pm.setParameter(60, clothesPalette[i].r);
        pm.setParameter(61, clothesPalette[i].g);
        pm.setParameter(62, clothesPalette[i].b);
        i = Random.Range(0, 18);
        pm.setParameter(63, clothesPalette[i].r);
        pm.setParameter(64, clothesPalette[i].g);
        pm.setParameter(65, clothesPalette[i].b);
        pm.OutputParameterVectors(sw);
    }
    public void ReadGenerate()
    {
        ParameterManage pm = GetComponent<ParameterManage>();
        string pline = sr.ReadLine();
        string[] pstrings = pline.Split(' ');
        float[] ps = new float[66];
        for (int i = 0; i < 66; i++)
            ps[i] = float.Parse(pstrings[i]);
        if (pm.setAllParameter(ps) >= 40)
            sr.Close();
    }
    public void SetSrSw(bool lastGenerate)
    {
        if(lastGenerate)
            sr = new StreamReader(Application.dataPath + "/ParametersLog.txt", Encoding.UTF8);
        else
            sw = new StreamWriter(Application.dataPath + "/ParametersLog.txt");
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
    public static float ReLU(float x)
    {
        if (x > 0)
            return x;
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
    private float[] ParaTransform(float[] input)
    {
        float[] output = new float[FEATURE];
        for(int i = 0,j = 0; i < input.Length; i++)
        {
            if (i >= 8 && i <= 10)
                continue;
            if (i >= 43 && i <= 47)
                continue;
            output[j] = input[i];
            j++;
        }
        float sum = 0;
        foreach (float i in output)
            sum += i;
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = output[i] / sum;
        }
        return output;
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
        return argmax+1;
    }
    private void EvaluateOutput()
    {
        if (result == -1)
            tex.text = "";
        else if (result == 1)
            tex.text = "Good Looking";
        else if (result == 2)
            tex.text = "Normal";
        else if (result == 3)
            tex.text = "Monster";
    }
}
class Outunit : MonoBehaviour
{
    const int CLASSNUM = 3;
    const int HUNITNUM = 150;
    private float[] b = new float[CLASSNUM] { 10.79991321470518f, 0.9563400598343171f, -11.889494561924744f };
    private float[][] w = new float[HUNITNUM][];
    private float[] u = new float[CLASSNUM];
    public float[] output = new float[CLASSNUM];
    void Start()
    {
        
    }
    public void Propagation(float[] x)
    {
        for (int i = 0; i < CLASSNUM; i++)
        {
            u[i] = NetworkExecute.Dot(getRow(w, i), x) + b[i];
        }
        output = NetworkExecute.Softmax(u);
    }
    public void Readw()
    {
        string filepath = "E:/character dataset/outunit_w.txt";
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
            w[i][2] = float.Parse(ws[2]);
        }
        sr.Close();
    }
    private float[] getRow(float[][] x, int row)
    {
        float[] a = new float[x.Length];
        for (int i = 0; i < x.Length; i++)
        {
            a[i] = x[i][row];
        }
        return a;
    }
}
class Hunit : MonoBehaviour
{
    const int FEATURE = 42;
    const int HUNITNUM = 150;
    public float[] b = new float[HUNITNUM];
    public float[][] w = new float[FEATURE][];
    public float[] u = new float[HUNITNUM];
    public float[] output = new float[HUNITNUM];
    void Start()
    {

    }
    public void Propagation(float[] x)
    {
        for (int i = 0; i < HUNITNUM; i++)
        {
            u[i] = NetworkExecute.Dot(getRow(w, i), x) + b[i];
            output[i] = NetworkExecute.ReLU(u[i]);
        }
    }
    public void Readb()
    {
        string filepath = "E:/character dataset/hunit_b.txt";
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for (int i = 0; i < HUNITNUM; i++)
        {
            string wline = sr.ReadLine();
            b[i] = float.Parse(wline);
        }
        sr.Close();
    }
    public void Readw()
    {
        string filepath = "E:/character dataset/hunit_w.txt";
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for (int i = 0; i < FEATURE; i++)
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
        float[] a = new float[x.Length];
        for(int i = 0; i < x.Length; i++)
        {
            a[i] = x[i][row];
        }
        return a;
    }
}
