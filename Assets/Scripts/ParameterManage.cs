using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ParameterManage : MonoBehaviour
{
    const int PARAMETERNUM = 60;
    public GameObject[] customParameter = new GameObject[PARAMETERNUM];
    private ParameterSlider[] paraslider = new ParameterSlider[PARAMETERNUM];
    //[HideInInspector]
    private float[] p = new float[PARAMETERNUM];
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < paraslider.Length; i++)
        {
            paraslider[i] = new ParameterSlider();
            p[i] = 1;
        }
        print(p.Length);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < customParameter.Length; i++)
        {
            if (customParameter[i] != null)
            {
                paraslider[i].slider = customParameter[i].GetComponent<Slider>();
                paraslider[i].pname = customParameter[i].name;
                p[i] = paraslider[i].slider.value;
            }
        }
    }
    public float getParameter(int i)
    {
        return p[i];
    }
    public float[] getParameters()
    {
        return p;
    }
    public void setParameter(int i, float value)
    {
        paraslider[i].slider.value = value;
        p[i] = value;
    }
    public Slider getSlider(int i)
    {
        return paraslider[i].slider;
    }
    public void setSlider(int i, int value, int maxValue)
    {
        paraslider[i].slider.maxValue = maxValue;
        if (value != 0) 
            paraslider[i].slider.value = value;   
    }

    public void FullRandom()
    {
        for (int i = 0; i < customParameter.Length; i++)
        {
            Slider si = customParameter[i].GetComponent<Slider>();
            float minvalue = si.minValue;
            float maxvalue = si.maxValue;
            si.value = Random.Range(minvalue, maxvalue);
            p[i] = si.value;
        }
    }
    public void HalfRandom()
    {
        for (int i = 0; i < customParameter.Length; i++)
        {
            if (i <= 32 || i >= 37 && i <= 42 || i >= 47)
                continue;
            Slider si = customParameter[i].GetComponent<Slider>();
            float minvalue = si.minValue;
            float maxvalue = si.maxValue;
            si.value = Random.Range(minvalue, maxvalue);
            p[i] = si.value;
        }
    }
    public float[] FullRandomTemp()
    {
        float[] output = new float[PARAMETERNUM];
        for (int i = 0; i < customParameter.Length; i++)
        {
            Slider si = customParameter[i].GetComponent<Slider>();
            float minvalue = si.minValue;
            float maxvalue = si.maxValue;
            output[i] = Random.Range(minvalue, maxvalue);
        }
        return output;
    }
    public void setAllParameter(float[] ps)
    {
        for (int i = 0; i < customParameter.Length; i++)
        {
            Slider si = customParameter[i].GetComponent<Slider>();
            si.value = ps[i];
            p[i] = ps[i];
        }
    }
    public void OutputParameter(int level)
    {
        StreamWriter sw;
        FileInfo filepath = new FileInfo("E:/character dataset2/Level " + level + "/" + SaveLoad.number[level-1] + "/parameter.txt");
        sw = filepath.CreateText();
        sw.Close();
        sw = filepath.AppendText();
        for (int i = 0; i < p.Length; i++)
        {
            sw.WriteLine(p[i].ToString());
        }
        sw.Close();
    }
    public void OutputParameterVectors(StreamWriter sw)
    {
        for (int i = 0; i < p.Length; i++)
        {
            sw.Write(((int)p[i]).ToString());
            sw.Write(" ");
        }
        sw.WriteLine();
        print(count++);
    }
}

class ParameterSlider : MonoBehaviour
{
    public Slider slider;
    public string pname;

    public void setParameter(Slider s, string n)
    {
        slider = s;
        pname = n;
    }
}