using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothesCustom : MonoBehaviour
{
    //モデル性別　1: 女の子　2: 男の子
    private int modelIndex;
    //体モデル
    private GameObject bodyModel;
    //パラメータ管理者
    private ParameterManage pm;
    private SkinnedMeshRenderer bodySmr;
    private SkinnedMeshRenderer topsSmr;
    private SkinnedMeshRenderer bottomsSmr;
    private SkinnedMeshRenderer shoesSmr;
    private List<BoneIndex> boneindex;
    private int topsIndex = 1;
    private int bottomsIndex = 1;
    public static int[] topsPatterns = { 2, 2, 1, 2, 1, 1, 1, 2, 2, 2 };
    public static int[] bottomsPatterns = { 4, 4, 1, 1, 1, 3, 2};
    private Color[] clothesPalette =
    {
        new Color(0.745f, 0.718f, 0.698f), new Color(0.149f, 0.184f, 0.282f),  new Color(0.565f, 0.506f, 0.467f),
        new Color(0.926f, 0.898f, 0.890f), new Color(0.44f, 0.373f, 0.353f), new Color(0.039f, 0.043f, 0.047f),
        new Color(0.169f, 0.467f, 0.698f), new Color(1f, 1f, 1f), new Color(1f, 1f, 1f),
        new Color(0.714f, 0.616f, 0.584f), new Color(0.184f, 0.153f, 0.180f), new Color(0.22f, 0.349f, 0.478f),
    };
    // Start is called before the first frame update
    void Start()
    {
        modelIndex = 1;
        bodyModel = GameObject.FindGameObjectWithTag("Body");
        bodySmr = bodyModel.GetComponent<SkinnedMeshRenderer>();
        topsSmr = GameObject.FindGameObjectWithTag("Tops").GetComponentInChildren<SkinnedMeshRenderer>();
        bottomsSmr = GameObject.FindGameObjectWithTag("Bottoms").GetComponentInChildren<SkinnedMeshRenderer>();
        shoesSmr = GameObject.FindGameObjectWithTag("Shoes").GetComponentInChildren<SkinnedMeshRenderer>();
        //for (int i = 0; i < bodySmr.bones.Length; i++)
         //print(i + " " + bodySmr.bones[i]);
        BoneIndexInit();

        //キャラクタと服のボーンを共有させる
        if (topsSmr != null) 
            ShareBones(topsSmr);
        if (bottomsSmr != null)
            ShareBones(bottomsSmr);
        if (shoesSmr != null)
            ShareBones(shoesSmr);
    }

    // Update is called once per frame
    void Update()
    {
        pm = GetComponent<ParameterManage>();
        TopsColor();
        if (bottomsSmr != null)
            BottomsColor();
        ShoesColor();
       // DeleteBottoms();
    }
    public void ChangeClothes(Slider slider)
    /*
     *　トップスチェンジ、Slider Tops Styleの値が変わるとき実行
     *　上着はTopsとOnepiece二種類がある
     */
    {
        //スライダから値を取る
        int index = (int)slider.value;
        //現在のトップスを探す
        GameObject tops1 = GameObject.FindGameObjectWithTag("Tops");
        //Topsがない場合はOnepieceを探す
        if (tops1 == null)
            tops1 = GameObject.FindGameObjectWithTag("Onepiece");

        //性別によって新トップスをResourcesからロード
        GameObject tops2;
        if (modelIndex == 1) 
            tops2 = Resources.Load<GameObject>("Clothes/Girl/tops" + index);
        else
            tops2 = Resources.Load<GameObject>("Clothes/Boy/btops" + index);
        if (tops2 == null)
            return;

        //新トップスを実体化
        GameObject tops2obj = Instantiate(tops2, bodyModel.transform.parent);
        tops2obj.transform.position = bodyModel.transform.position;
        topsSmr = tops2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        topsIndex = index;

        //各トップスモデルによってパターンの数が変わるのでスライダの最大値を調整
        Slider pattern = slider.gameObject.transform.parent.GetChild(1).GetComponent<Slider>();
        pattern.maxValue = topsPatterns[topsIndex-1];
        pattern.value = 1;

        //トップスがOnepieceの場合はボトムスが着れないようにbottomsボタンを無効にする
        //現在のボトムスも削除する
        Button bottomB = slider.gameObject.transform.parent.parent.parent.GetChild(1).GetComponent<Button>();
        GameObject bottoms = GameObject.FindGameObjectWithTag("Bottoms");
        if (tops2.tag == "Onepiece")
        {
            bottomB.interactable = false;
            if (bottoms != null)
                Destroy(bottoms);
        }
        else
        //新トップスがTopsでボトムスがない場合は前削除されたボトムスを再生する
        {
            bottomB.interactable = true;
            if (bottoms == null)
            {
                GameObject bottom = Resources.Load<GameObject>("Clothes/Girl/bottoms" + bottomsIndex);
                if (bottom == null)
                    return;
                bottoms = Instantiate(bottom, bodyModel.transform.parent);
                bottomsSmr = bottoms.GetComponentInChildren<SkinnedMeshRenderer>();
                ShareBones(bottomsSmr);
            }
        }
        //旧トップスを削除
        Destroy(tops1);
        //新トップスとキャラクタのボーンを共有させる
        ShareBones(topsSmr);
    }
    public void ChangeTopsPattern(Slider slider)
    {
        int index = (int)slider.value;
        topsSmr.material = Resources.Load<Material>("Materials/Tops/tops" + topsIndex + "_m" + index);
    }
    public void TopsColor()
    {
        Material topsm = topsSmr.material;
        float topsr1 = pm.getParameter(54) / 255;
        float topsg1 = pm.getParameter(55) / 255;
        float topsb1 = pm.getParameter(56) / 255;
        float topsr2 = pm.getParameter(57) / 255;
        float topsg2 = pm.getParameter(58) / 255;
        float topsb2 = pm.getParameter(59) / 255;
        topsm.SetColor("_Color", new Color(topsr1, topsg1, topsb1));
        topsm.SetColor("_SubColor", new Color(topsr2, topsg2, topsb2));
    }
    public void BottomsColor()
    {
        Material bottomsm = bottomsSmr.material;
        float bottomsr = pm.getParameter(60) / 255;
        float bottomsg = pm.getParameter(61) / 255;
        float bottomsb = pm.getParameter(62) / 255;
        bottomsm.SetColor("_Color", new Color(bottomsr, bottomsg, bottomsb));
    }
    public void ShoesColor()
    {
        Material shoesm = shoesSmr.material;
        float shoesr = pm.getParameter(63) / 255;
        float shoesg = pm.getParameter(64) / 255;
        float shoesb = pm.getParameter(65) / 255;
        shoesm.SetColor("_Color", new Color(shoesr, shoesg, shoesb));
    }
    public void ChangeBottoms(Slider slider)
    /*
     *　ボトムスチェンジ、Slider Bottoms Styleの値が変わるとき実行
     */
    {
        //スライダから値を取る
        int index = (int)slider.value;
        //現在のボトムスを探す
        GameObject bottoms1 = GameObject.FindGameObjectWithTag("Bottoms");

        //新ボトムスをResourcesからロード
        GameObject bottoms2 = Resources.Load<GameObject>("Clothes/Girl/bottoms" + index);
        if (bottoms2 == null)
            return;

        //新ボトムスを実体化
        GameObject bottoms2obj = Instantiate(bottoms2, bodyModel.transform.parent);
        bottomsSmr = bottoms2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        bottomsIndex = index;

        //各ボトムスモデルによってパターンの数が変わるのでスライダの最大値を調整
        Slider pattern = slider.gameObject.transform.parent.GetChild(1).GetComponent<Slider>();
        pattern.maxValue = bottomsPatterns[bottomsIndex - 1];
        pattern.value = 1;

        //旧ボトムスを削除
        Destroy(bottoms1);
        //新ボトムスとキャラクタのボーンを共有させる
        ShareBones(bottomsSmr);
    }
    public void ChangeBottomsPattern(Slider slider)
    {
        int index = (int)slider.value;
        bottomsSmr.material = Resources.Load<Material>("Materials/Bottoms/bottoms" + bottomsIndex + "_m" + index);
    }
    public void ChangeShitagi(Slider slider)
    {
        int index = (int)slider.value;
        bodySmr.material = Resources.Load<Material>("Materials/Skin/bodyskin" + index);
    }
    public void BottomsDetail(Slider slider)
    {
        /*int index = (int)slider.value;
        if (bottomsIndex == 1)
        {
            bottomsSmr.bones[7].parent.localScale = new Vector3(1, index, 1);
            bottomsSmr.bones[26].parent.localScale = new Vector3(1, index, 1);
        }
        else if (bottomsIndex == 2)
        {
            bottomsSmr.material = Resources.Load<Material>("Materials/Bottoms/bottoms" + bottomsIndex + "_m" + index);
        }*/
        
    }
    public void ChangeShoes(Slider slider)
    {
        int index = (int)slider.value;
        GameObject shoes1 = GameObject.FindGameObjectWithTag("Shoes");

        GameObject shoes2 = Resources.Load<GameObject>("Clothes/Girl/shoes" + index);
        if (shoes2 == null)
            return;
        GameObject shoes2obj = Instantiate(shoes2, bodyModel.transform.parent);
        shoesSmr = shoes2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        Destroy(shoes1);
        ShareBones(shoesSmr);
    }
    void ShareBones(SkinnedMeshRenderer partSmr)
    /*
     *  ボーン共有
     *  服をキャラクタの体型に合わせるように必要
     */
    {
        //服のボーンを全部取ってboneListに保存する
        List<Transform> boneList = new List<Transform>();
        for (int i = 0; i < partSmr.bones.Length; i++)
        {
            boneList.Add(partSmr.bones[i]);
        }

        //boneListからキャラクタと同じ名前を持つボーンを見つけたら入れ替わる
        for (int i = 0; i < boneList.Count; i++)
        {
            bool flag = false;
            if (boneList[i] == null)
                continue;
            for (int j = 0; j < bodySmr.bones.Length; j++)
            {
                if (boneList[i].name == bodySmr.bones[j].name)
                {
                    boneList[i] = bodySmr.bones[j];
                    flag = true;
                }
            }
            if(!flag)
            {
                boneList[i] = null;
            }
        }

        //新ボーンを服に戻す
        partSmr.rootBone = bodySmr.rootBone;
        partSmr.bones = boneList.ToArray();
    }
    void ChangeBones(SkinnedMeshRenderer partSmr)
    {
        Mesh m = partSmr.sharedMesh;
        BoneWeight[] bw = m.boneWeights;
    }
    public void RefindObject()
    {
        bodyModel = GameObject.FindGameObjectWithTag("Body");
        bodySmr = bodyModel.GetComponent<SkinnedMeshRenderer>();
        topsSmr = GameObject.FindGameObjectWithTag("Tops").GetComponentInChildren<SkinnedMeshRenderer>();
        bottomsSmr = GameObject.FindGameObjectWithTag("Bottoms").GetComponentInChildren<SkinnedMeshRenderer>();
        shoesSmr = GameObject.FindGameObjectWithTag("Shoes").GetComponentInChildren<SkinnedMeshRenderer>();
        ShareBones(topsSmr);
        ShareBones(bottomsSmr);
        ShareBones(shoesSmr);
    }
    public void SetModelIndex(int model)
    {
        modelIndex = model;
        if (modelIndex == 1)
        {
            pm.setSlider(43, 1, 7);
            pm.setSlider(44, 1, 4);
            pm.setSlider(45, 1, 6);
            pm.setSlider(51, 1, 4);
        }
        else
        {
            pm.setSlider(43, 1, 4);
            pm.setSlider(44, 1, 1);
            pm.setSlider(45, 1, 1);
            pm.setSlider(51, 1, 1);
        }
    }
    public void DeleteBottoms()
    {
        if (modelIndex == 1 && topsIndex == 5)
        { 
            GameObject bottoms = GameObject.FindGameObjectWithTag("Bottoms");
            if (bottoms != null)
                Destroy(bottoms);
        }

    }
    void BoneIndexInit()
    /*
     *  時間短縮用、まだ使ってない、開発中
     */
    {
        boneindex = new List<BoneIndex>();
        BoneIndex bone = new BoneIndex();
        bone.Generate("Root", 0);
        boneindex.Add(bone);
        bone.Generate("J_Bip_C_Hips", 1);
        boneindex.Add(bone);
        bone.Generate("J_Bip_C_Spine", 24);
        boneindex.Add(bone);
        bone.Generate("J_Bip_C_Chest", 25);
        boneindex.Add(bone);
        bone.Generate("J_Bip_C_UpperChest", 26);
        boneindex.Add(bone);
        bone.Generate("J_Bip_C_Neck", 75);
        boneindex.Add(bone);
        bone.Generate("J_Sec_R_Bust1", 91);
        boneindex.Add(bone);
        bone.Generate("J_Sec_R_Bust2", 92);
        boneindex.Add(bone);
        bone.Generate("J_Sec_L_Bust1", 93);
        boneindex.Add(bone);
        bone.Generate("J_Sec_L_Bust2", 94);
        boneindex.Add(bone);

    }

    public void RealTimeChangeClothes()
    /*
     *　トップスチェンジ、Slider Tops Styleの値が変わるとき実行
     *　上着はTopsとOnepiece二種類がある
     */
    {
        //スライダから値を取る
        int index = (int)pm.getParameter(44);
        //現在のトップスを探す
        GameObject tops1 = GameObject.FindGameObjectWithTag("Tops");
        //Topsがない場合はOnepieceを探す
        if (tops1 == null)
            tops1 = GameObject.FindGameObjectWithTag("Onepiece");

        //性別によって新トップスをResourcesからロード
        GameObject tops2;
        if (modelIndex == 1)
            tops2 = Resources.Load<GameObject>("Clothes/Girl/tops" + index);
        else
            tops2 = Resources.Load<GameObject>("Clothes/Boy/btops" + index);
        if (tops2 == null)
            return;

        //新トップスを実体化
        GameObject tops2obj = Instantiate(tops2, bodyModel.transform.parent);
        tops2obj.transform.position = bodyModel.transform.position;
        tops2obj.transform.GetChild(0).gameObject.layer = 8;
        topsSmr = tops2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        topsIndex = index;

        GameObject bottoms = GameObject.FindGameObjectWithTag("Bottoms");
        if (tops2.tag == "Onepiece")
        {
            if (bottoms != null)
                Destroy(bottoms);
        }
        else
        //新トップスがTopsでボトムスがない場合は前削除されたボトムスを再生する
        {
            if (bottoms == null)
            {
                GameObject bottom = Resources.Load<GameObject>("Clothes/Girl/bottoms" + bottomsIndex);
                if (bottom == null)
                    return;
                bottoms = Instantiate(bottom, bodyModel.transform.parent);
                bottomsSmr = bottoms.GetComponentInChildren<SkinnedMeshRenderer>();
                ShareBones(bottomsSmr);
            }
        }
        //旧トップスを削除
        Destroy(tops1);
        //新トップスとキャラクタのボーンを共有させる
        ShareBones(topsSmr);

        index = (int)pm.getParameter(50);
        topsSmr.material = Resources.Load<Material>("Materials/Tops/tops" + topsIndex + "_m" + index);
    }

    public void RealTimeChangeBottoms()
    /*
     *　ボトムスチェンジ、Slider Bottoms Styleの値が変わるとき実行
     */
    {
        //スライダから値を取る
        int index = (int)pm.getParameter(43);
        //現在のボトムスを探す
        GameObject bottoms1 = GameObject.FindGameObjectWithTag("Bottoms");

        //新ボトムスをResourcesからロード
        GameObject bottoms2 = Resources.Load<GameObject>("Clothes/Girl/bottoms" + index);
        if (bottoms2 == null)
            return;

        //新ボトムスを実体化
        GameObject bottoms2obj = Instantiate(bottoms2, bodyModel.transform.parent);
        bottoms2obj.transform.GetChild(0).gameObject.layer = 8;
        bottomsSmr = bottoms2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        bottomsIndex = index;

        //旧ボトムスを削除
        Destroy(bottoms1);
        //新ボトムスとキャラクタのボーンを共有させる
        ShareBones(bottomsSmr);
    }

    public void RealTimeChangeShitagi()
    {
        int index = (int)pm.getParameter(45);
        bodySmr.material = Resources.Load<Material>("Materials/Skin/bodyskin" + index);
    }

    public void RealTimeChangeShoes()
    {
        int index = (int)pm.getParameter(51);
        GameObject shoes1 = GameObject.FindGameObjectWithTag("Shoes");

        GameObject shoes2 = Resources.Load<GameObject>("Clothes/Girl/shoes" + index);
        if (shoes2 == null)
            return;
        GameObject shoes2obj = Instantiate(shoes2, bodyModel.transform.parent);
        shoes2obj.transform.GetChild(0).gameObject.layer = 8;
        shoesSmr = shoes2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        Destroy(shoes1);
        ShareBones(shoesSmr);
    }
}
class BoneIndex
{
    public string name;
    public int index;
    public void Generate(string nam, int inde)
    {
        name = nam;
        index = inde;
    }
}
