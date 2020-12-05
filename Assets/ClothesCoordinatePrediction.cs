using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public class ClothesCoordinatePrediction : MonoBehaviour
{
    const int nodeNum = 5;
    private List<CPD> cpds = new List<CPD>();
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
        int[] set = { (int)Tops.parka, (int)Bottoms.jeans, (int)Socks.none, (int)Shoes.boots };
        print(JointCost(set));
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
}
class CPD
{
    public string nodeName;
    public string[] variableNames;
    public int parentNum;
    public List<List<float>> cpdData = new List<List<float>>();
    public void ReadCPD(StreamReader sr)
    {
        string dataline = sr.ReadLine();
        variableNames = dataline.Split(' ');
        nodeName = variableNames[0];
        parentNum = variableNames.Length - 1;
        while((dataline = sr.ReadLine())!= "")
        {
            string[] datas = dataline.Split(' ');
            List<float> oneRow = new List<float>();
            foreach(string d in datas)
            {
                oneRow.Add(float.Parse(d));
            }
            cpdData.Add(oneRow);
        }
    }
}