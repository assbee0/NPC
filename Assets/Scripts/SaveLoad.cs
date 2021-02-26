using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SaveLoad : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public static int[] number = new int[10];

    void Start()
    {
        string filepath = Application.dataPath + "/NUM.txt";
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        for(int i = 0; i < 10; i++)
        {
            string nums = sr.ReadLine();
            number[i] = int.Parse(nums);
            
        }
        
        sr.Close();
        CreateDirect();

    }
    public void NumberPlus(int level)
    {
        number[level-1]++;
        string filepath = Application.dataPath + "/NUM.txt";
        if (!File.Exists(filepath))
            return;
        StreamWriter sw = new StreamWriter(filepath);
        for (int i = 0; i < 10; i++)
            sw.WriteLine(number[i]);
        sw.Close();
        CreateDirect();
    }
    public void NumberReset()
    {
        string filepath = Application.dataPath + "/NUM.txt";
        if (!File.Exists(filepath))
            return;
        StreamWriter sw = new StreamWriter(filepath);
        for (int i = 0; i < 10; i++)
        {
            sw.WriteLine(1);
            number[i] = 1;
        }
        sw.Close();
    }
    public void CreateDirect()
    {
        for(int i = 0; i < 10; i++)
            if (!Directory.Exists(Application.dataPath + "/character dataset/Level " + (i+1) + "/" + number[i]))
                Directory.CreateDirectory(Application.dataPath + "/character dataset/Level " + (i+1) + "/" + number[i]);
    }
}
