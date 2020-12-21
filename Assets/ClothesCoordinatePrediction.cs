using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using System;
using Random = UnityEngine.Random;

public class ClothesCoordinatePrediction : MonoBehaviour
{
    const int nodeNum = 5;
    private List<CPD> cpds = new List<CPD>();
    private List<Vector3> palette = new List<Vector3>();
    private int[] oneset = new int[nodeNum];
    private enum DressCode {StudentCasual};
    private enum Tops { onepiece, parka, shirt, sweater, tshirt };
    private enum Bottoms { jeans, longskirt, miniskirt, none, pants, shortpants, skirt };
    private enum Socks { anklets, kneesocks, none, socks, tights};
    private enum Shoes { boots, loafers, pumps, sneakers};
    // Start is called before the first frame update
    void Start()
    {
        ReadCPD();
        ReadPalette();
        int[] setexample = { (int)Tops.parka, (int)Bottoms.jeans, (int)Socks.none, (int)Shoes.boots };
        Vector3[] color = { new Vector3(190, 183, 178), new Vector3(38, 47, 72), new Vector3(144, 129, 119), new Vector3(235, 229, 227) };
        
        float cost = 10000;
        for(int i=0; i<10; i++)
        {
            float tempcost;
            int[] tempset = { Random.Range(0, 5), Random.Range(0, 7), Random.Range(0, 5), Random.Range(0, 4) };
            Vector3[] tempcolorset = new Vector3[4];
            for (int k = 0; k < 4; k++)
            {
                tempcolorset[k].x = Random.Range(0, 255);
                tempcolorset[k].y = Random.Range(0, 255);
                tempcolorset[k].z = Random.Range(0, 255);
            }
            tempcost = 5 * JointCost(tempset) + MarginalCost(tempset) + PaletteCost(tempcolorset);
            if (tempcost < cost)
            {
                cost = tempcost;
                setexample = tempset;
                color = tempcolorset;
            }
        }
        print(Enum.GetName(typeof(Tops), setexample[0]));
        print(Enum.GetName(typeof(Bottoms), setexample[1]));
        print(Enum.GetName(typeof(Socks), setexample[2]));
        print(Enum.GetName(typeof(Shoes), setexample[3]));
        for (int i = 0; i < 4; i++)
        {
            print(color[i]);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private float JointCost(int[] set)
    {
        float[] multi = new float[nodeNum-1];
        float joint = 1;
        multi[0] = cpds.Find(c => c.nodeName == "Tops").cpdData[set[0]][0];
        multi[1] = cpds.Find(c => c.nodeName == "Bottoms").cpdData[set[1]][set[0]];
        multi[2] = cpds.Find(c => c.nodeName == "Socks").cpdData[set[2]][set[1]];
        multi[3] = cpds.Find(c => c.nodeName == "Shoes").cpdData[set[3]][set[1]*5+set[2]];
        for (int i = 0; i < nodeNum - 1; i++) 
        {
            joint = joint * multi[i];
        }
        return 1 - joint;
    }
    private float MarginalCost(int[] set)
    {
        float sum = 0;
        sum += cpds.Find(c => c.nodeName == "Tops").cpdMarginal[set[0]];
        sum += cpds.Find(c => c.nodeName == "Bottoms").cpdMarginal[set[1]];
        sum += cpds.Find(c => c.nodeName == "Socks").cpdMarginal[set[2]];
        sum += cpds.Find(c => c.nodeName == "Shoes").cpdMarginal[set[3]];
        return 1 - sum / (nodeNum - 1);
    }

    private float PaletteCost(Vector3[] colorset)
    {
        float threshold = 0;
        float sum = 0;
        for (int i = 0; i < colorset.Length; i++)
        {
            float min = 1000;
            for (int j = 0; j < palette.Count; j++)
            {
                float dis = Math.Abs(colorset[i].x - palette[j].x) 
                          + Math.Abs(colorset[i].y - palette[j].y)
                          + Math.Abs(colorset[i].z - palette[j].z);
                if (dis < min)
                    min = dis;
            }
            float finaldis = (min - threshold) > 0 ? (min - threshold) : 0;
            sum += finaldis;
        }
        return sum / 4 / 255 / 1.73205f;
    }

    private void ReadCPD()
    {
        string filepath = Application.dataPath + "/Resources/Parameters/cpd.txt";
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for (int i = 0; i < nodeNum; i++)
        {
            CPD c = new CPD();
            c.ReadCPD(sr);
            cpds.Add(c);
        }
        sr.Close();
    }
    
    private void ReadPalette()
    {
        string filepath = Application.dataPath + "/Resources/Parameters/palette.txt";
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        string dataline;
        while (!string.IsNullOrWhiteSpace(dataline = sr.ReadLine()))
        {
            string[] datas = dataline.Split(' ');
            Vector3 col = new Vector3();
            col.x = int.Parse(datas[0]);
            col.y = int.Parse(datas[1]);
            col.z = int.Parse(datas[2]);
            palette.Add(col);
        }
        sr.Close();
    }
}
class CPD
{
    public string nodeName;
    public string[] variableNames;
    public int parentNum;
    public List<List<float>> cpdData = new List<List<float>>();
    public List<float> cpdMarginal = new List<float>();
    public void ReadCPD(StreamReader sr)
    {
        string dataline = sr.ReadLine();
        variableNames = dataline.Split(' ');
        nodeName = variableNames[0];
        parentNum = variableNames.Length - 1;
        while((dataline = sr.ReadLine())!= "Marginal")
        {
            string[] datas = dataline.Split(' ');
            List<float> oneRow = new List<float>();
            foreach(string d in datas)
            {
                oneRow.Add(float.Parse(d));
            }
            cpdData.Add(oneRow);
        }
        dataline = sr.ReadLine();
        string[] marginals = dataline.Split(' ');
        foreach(string m in marginals)
        {
            cpdMarginal.Add(float.Parse(m));
        }
    }
}