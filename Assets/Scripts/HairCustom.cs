using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class HairCustom : MonoBehaviour
{

    //モデル性別　1: 女の子　2: 男の子
    private int modelIndex;
    //実装された髪型数（男の子）
    private int boyHairStyleNum = 3;
    //実装された髪型数（女の子）
    private int girlHairStyleNum = 8;
    //実装された髪型数
    private int hairStyleNum;
    private Transform head;
    private GameObject hair1;
    private SkinnedMeshRenderer hairSmr;
    private int boyIndex = 1;
    private int girlIndex = 1;
    //パラメータ管理者
    private ParameterManage pm;
    //髪型プリセットデータ
    private List<HairData> hairDatas = new List<HairData>();
    //現在使ってる髪型
    private int currentindex = 1;                           

    // Start is called before the first frame update
    void Start()
    {
        hairStyleNum = boyHairStyleNum + girlHairStyleNum;
        modelIndex = 1;
        head = GameObject.FindGameObjectWithTag("Head").transform;
        hairSmr = GameObject.FindGameObjectWithTag("Hair").GetComponentInChildren<SkinnedMeshRenderer>();
        //髪型ボーンのインデックス出力、普段はコメント化
        //for (int i = 0; i <hairSmr.bones.Length; i++)
         //    print(i+" "+hairSmr.bones[i]);                 
        InitHairData();
    }

    // Update is called once per frame
    void Update()
    {
        pm = GetComponent<ParameterManage>();
        //一番最後の髪型は男の坊主頭だから細かい調整はない
        if (modelIndex == 2 && currentindex == hairStyleNum)
            return;
        HairColor();
        HairDetail();
    }
    public void HairColor()
    {
        Material mat0 = hairSmr.materials[0];
        float hairR = pm.getParameter(34);
        float hairG = pm.getParameter(35);
        float hairB = pm.getParameter(36);
        mat0.color = new Color(hairR / 255, hairG / 255, hairB / 255);
        if(hairSmr.materials.Length >= 2)
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
        string filepath = Application.dataPath + "/hairdata.txt";
        if (!File.Exists(filepath))
            return;
        StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
        HairData hairdata = new HairData();
        for(int i = 0;i < hairStyleNum;i++)
        {
            hairdata = new HairData();
            hairdata.ReadHairData(sr);
            hairDatas.Add(hairdata);
        }
    }
    public void ChangeHair(Slider slider)
    /*
     *　髪型チェンジ、Slider Hair Styleの値が変わるとき実行
     */
    {
        //スライダから値を取る
        int index = (int)slider.value;
        currentindex = index;
        //今の髪型を探す
        hair1 = GameObject.FindGameObjectWithTag("Hair");
        GameObject hair2;

        //性別によって別々処理する、Resourcesからロード
        if (modelIndex == 1)
        {
            hair2 = Resources.Load<GameObject>("Hair/Girl/hair" + index);
        }
        else
        {
            currentindex = index + girlHairStyleNum;
            if (index == 3)
            {
                
                Destroy(hair1);
                return;
            }
            hair2 = Resources.Load<GameObject>("Hair/Boy/bhair" + index);
        }
        
        if (hair2 == null)
            return;

        //シーン内で実体化
        GameObject hair2obj = Instantiate(hair2,head);

        //新髪型の位置をプリセットデータから初期化
        hair2obj.transform.localPosition = hairDatas[currentindex - 1].hairPosition;
        hair2obj.tag = "Hair";
        hairSmr = hair2obj.GetComponentInChildren<SkinnedMeshRenderer>();

        //旧髪型削除
        if (hair1 != null) 
            Destroy(hair1);
        // InitColor();

        //UI処理、髪型の各部位の有無によって各スライダのインタラクティブ性を調整
        Transform parentSlider = slider.gameObject.transform.parent;
        Slider hairSlider = parentSlider.GetChild(2).GetComponent<Slider>();
        if (hairDatas[currentindex - 1].hasBack)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        hairSlider = parentSlider.GetChild(3).GetComponent<Slider>();
        if (hairDatas[currentindex - 1].hasMaegami)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        hairSlider = parentSlider.GetChild(4).GetComponent<Slider>();
        if (hairDatas[currentindex - 1].hasTail)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        hairSlider = parentSlider.GetChild(5).GetComponent<Slider>();
        if (hairDatas[currentindex - 1].hasAhoge)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        hairSlider = parentSlider.GetChild(6).GetComponent<Slider>();
        if (hairDatas[currentindex - 1].hasSide)
            hairSlider.interactable = true;
        else
            hairSlider.interactable = false;
        
    }
    public void HairDetail()
    /*
     *　髪型の細かい部分を調整する
     *　髪型によって前髪、後ろ髪、アホ毛、テール、両サイドがある
     */
    {
        HairData hairdata = hairDatas[currentindex-1];
        Transform[] hairbones = hairSmr.bones;
        if (hairdata.hasBack)
        {
            Transform back = hairbones[hairdata.backNum[0]];
            float backS = pm.getParameter(37);
            back.parent.localScale = new Vector3(1, backS * 0.01f + 0.7f, 1);
        }
        if (hairdata.hasMaegami)
        {
            Transform maegami = hairbones[hairdata.maegamiNum[0]];
            float maegamiS = pm.getParameter(38);
            maegami.parent.localScale = new Vector3(maegamiS * 0.002f + 0.9f, maegamiS * 0.004f + 0.7f, maegamiS * 0.003f + 0.8f);
        }
        if (hairdata.hasAhoge)
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

    public void ChangeGlasses(Slider slider)
    {
        int index = (int)slider.value;
        if (index == 1)
        {
            GameObject glasses = GameObject.FindGameObjectWithTag("Glasses");
            if (glasses != null)
                Destroy(glasses);
        }
        else if (index == 2)
        {
            GameObject glasses = Resources.Load<GameObject>("Clothes/Accessories/glasses");
            GameObject glassesobj = Instantiate(glasses, head);
            glassesobj.transform.localPosition = new Vector3(0, -1.374f, 0.007f);
        }
    }
    public void ChangeNecklace(Slider slider)
    {
        int index = (int)slider.value;
        if (index == 1)
        {
            GameObject necklace = GameObject.FindGameObjectWithTag("Necklace");
            if (necklace != null)
                Destroy(necklace);
        }
        else if (index == 2)
        {
            GameObject necklace = Resources.Load<GameObject>("Clothes/Accessories/ribbon");
            GameObject glassesobj = Instantiate(necklace, head.parent.parent);
            glassesobj.transform.localPosition = new Vector3(0, -1.19f, -0.01f);
        }
    }
    public void RefindObject()
    {
        head = GameObject.FindGameObjectWithTag("Head").transform;
        if (GameObject.FindGameObjectWithTag("Hair") != null)
        {
            hairSmr = GameObject.FindGameObjectWithTag("Hair").GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }
    public void SetModelIndex(int model)
    {
        RefindObject();
        if (modelIndex == 1)
            girlIndex = currentindex;
        else
            boyIndex = currentindex - girlHairStyleNum;
        modelIndex = model;
        if (model == 1)
        {
            print(girlIndex);
            pm.setSlider(33, girlIndex, girlHairStyleNum);
            currentindex = girlIndex;
        }
        else
        {
            pm.setSlider(33, boyIndex, boyHairStyleNum);
            currentindex = boyIndex + girlHairStyleNum;
        }  
    }
}

public class HairData
/*
 *　髪型のプリセットデータを保存用データ構造
 */
{
    public Vector3 hairPosition;
    public bool hasBack;
    public bool hasMaegami;
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
        hasBack = int.Parse(words[3]) == 1 ? true : false;
        hasMaegami = int.Parse(words[4]) == 1 ? true : false;
        hasAhoge = int.Parse(words[5]) == 1 ? true : false;
        hasTail = int.Parse(words[6]) == 1 ? true : false;
        hasSide = int.Parse(words[7]) == 1 ? true : false;
        int i = 8;
        if (hasBack)
        {
            while (words[i] != ",")
            {
                backNum.Add(int.Parse(words[i]));
                i++;
            }
            i++;
        }
        if (hasMaegami)
        {
            while (words[i] != ",")
            {
                maegamiNum.Add(int.Parse(words[i]));
                i++;
            }
            i++;
        }
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