using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class HairCustom : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform head;
    private GameObject hair1;
    private SkinnedMeshRenderer hairSmr;
    private ParameterManage pm;
    private List<HairData> hairDatas = new List<HairData>();
    private int currentindex = 1;
    void Start()
    {
        head = GameObject.FindGameObjectWithTag("Head").transform;
        hairSmr = GameObject.FindGameObjectWithTag("Hair").GetComponentInChildren<SkinnedMeshRenderer>();
        /*for (int i = 0; i <hairSmr.bones.Length; i++)
             print(i+" "+hairSmr.bones[i]);*/
        InitHairData();
    }

    // Update is called once per frame
    void Update()
    {
        pm = GetComponent<ParameterManage>();
        HairColor();
    }
    public void HairColor()
    {
        Material mat0 = hairSmr.materials[0];
        float hairR = pm.getParameter(34);
        float hairG = pm.getParameter(35);
        float hairB = pm.getParameter(36);
        mat0.color = new Color(hairR / 255, hairG / 255, hairB / 255);
        if(hairSmr.materials.Length > 2)
        {
            Material mat1 = hairSmr.materials[1];
            mat1.color = new Color(hairR / 255, hairG / 255, hairB / 255);
        }
    }
    void InitColor()
    {
        pm = GetComponent<ParameterManage>();
        Material mat = hairSmr.materials[0];
        Color haircolor = mat.color;
        pm.setParameter(34, (int)(haircolor.r * 255));
        pm.setParameter(35, (int)(haircolor.g * 255));
        pm.setParameter(36, (int)(haircolor.b * 255));
    }
    void InitHairData()
    {
        string filepath = "E:/character dataset/hairdata.txt";
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        HairData hairdata = new HairData();
        for(int i = 0;i < 5;i++)
        {
            hairdata = new HairData();
            hairdata.ReadHairData(sr);
            hairDatas.Add(hairdata);
        }
    }
    public void ChangeHair(Slider slider)
    {
        int index = (int)slider.value;
        currentindex = index;
        hair1 = GameObject.FindGameObjectWithTag("Hair");
        GameObject hair2 = Resources.Load<GameObject>("Hair/hair"+index);
        if (hair2 == null)
            return;
        GameObject hair2obj = Instantiate(hair2,head);
        hair2obj.transform.localPosition = hairDatas[index - 1].hairPosition;
        hair2obj.transform.localRotation = hair1.transform.localRotation;
        hair2obj.transform.localScale = hair1.transform.localScale;
        hair2obj.tag = "Hair";
        hairSmr = hair2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        Destroy(hair1);
       // InitColor();
        Transform parentSlider = slider.gameObject.transform.parent;
        Slider hairSlider = parentSlider.GetChild(5).GetComponent<Slider>();
        if(hairDatas[index - 1].hasAhoge)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        hairSlider = parentSlider.GetChild(4).GetComponent<Slider>();
        if (hairDatas[index - 1].hasTail)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        hairSlider = parentSlider.GetChild(6).GetComponent<Slider>();
        if (hairDatas[index - 1].hasSide)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
    }
    public void HairDetail()
    {
        HairData hairdata = hairDatas[currentindex-1];
        Transform[] hairbones = hairSmr.bones;
        Transform back = hairbones[hairdata.backNum[0]];
        Transform maegami = hairbones[hairdata.maegamiNum[0]];
        float backS = pm.getParameter(37);
        float maegamiS = pm.getParameter(38);
        back.parent.localScale = new Vector3(1, backS * 0.01f + 0.7f, 1);
        maegami.parent.localScale = new Vector3(maegamiS * 0.002f + 0.9f, maegamiS * 0.004f + 0.7f, maegamiS * 0.003f + 0.8f);
        if(hairdata.hasAhoge)
        {
            Transform ahoge = hairbones[hairdata.ahogeNum[0]];
            float ahogeS = pm.getParameter(40);
            ahoge.parent.localScale = new Vector3(1, 1, 1) * (ahogeS * 0.015f + 0.5f);
        }
        if (hairdata.hasTail)
        {
            Transform tail = hairbones[hairdata.tailNum[0]];
            float tailS = pm.getParameter(39);
            tail.parent.localScale = new Vector3(tailS * 0.006f + 0.8f, tailS * 0.013f + 0.7f, 1);
        }
        if (hairdata.hasSide)
        {
            Transform side = hairbones[hairdata.sideNum[0]];
            float sideS = pm.getParameter(41);
            side.parent.localScale = new Vector3(1, sideS * 0.007f + 0.8f, 1);
        }
    }
    public void RefindObject()
    {
        head = GameObject.FindGameObjectWithTag("Head").transform;
        hairSmr = GameObject.FindGameObjectWithTag("Hair").GetComponentInChildren<SkinnedMeshRenderer>();
        currentindex = 1;
    }
}

public class HairData : MonoBehaviour
{
    public Vector3 hairPosition;
    public bool hasAhoge;
    public bool hasTail;
    public bool hasSide;
    public List<int> backNum = new List<int>();
    public List<int> maegamiNum = new List<int>();
    public List<int> ahogeNum = new List<int>();
    public List<int> tailNum = new List<int>();
    public List<int> sideNum = new List<int>();

    public void ReadHairData(StreamReader sr)
    {
        string dataline = sr.ReadLine();
        string[] words = dataline.Split(' ');
        hairPosition = new Vector3(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
        hasAhoge = int.Parse(words[3]) == 1 ? true : false;
        hasTail = int.Parse(words[4]) == 1 ? true : false;
        hasSide = int.Parse(words[5]) == 1 ? true : false;
        int i = 6;
        while(words[i] != ",")
        {
            backNum.Add(int.Parse(words[i]));
            i++;
        }
        i++;
        while (words[i] != ",")
        {
            maegamiNum.Add(int.Parse(words[i]));
            i++;
        }
        i++;
        if (hasAhoge)
        {
            while (words[i] != "," )
            {
                ahogeNum.Add(int.Parse(words[i]));
                i++;
            }
            i++;
        }
        if (hasTail)
        {
            while (words[i] != "," )
            {
                tailNum.Add(int.Parse(words[i]));
                i++;
            }
            i++;
        }
        if (hasSide)
        {
            while (i < words.Length)
            {
                sideNum.Add(int.Parse(words[i]));
                i++;
            }
        }
    }
        
    
}