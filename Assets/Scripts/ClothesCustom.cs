using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothesCustom : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject bodyModel;
    private ParameterManage pm;
    private SkinnedMeshRenderer bodySmr;
    private SkinnedMeshRenderer topsSmr;
    private SkinnedMeshRenderer bottomsSmr;
    private SkinnedMeshRenderer shoesSmr;
    private List<BoneIndex> boneindex;
    private int topsIndex = 1;
    private int bottomsIndex = 1;
    private Color[] hadaIros =
    {
        new Color(1f, 1f, 1f), new Color(1f, 0.965f, 0.929f),  new Color(1f, 0.969f, 0.894f),
        new Color(1f, 0.922f, 0.878f), new Color(0.996f, 0.914f, 0.792f), new Color(1f, 0.894f, 0.741f),
        new Color(1f, 0.831f, 0.565f), new Color(0.847f, 0.643f, 0.49f)
    };
    void Start()
    {
        bodyModel = GameObject.FindGameObjectWithTag("Body");
        bodySmr = bodyModel.GetComponent<SkinnedMeshRenderer>();
        topsSmr = GameObject.FindGameObjectWithTag("Tops").GetComponentInChildren<SkinnedMeshRenderer>();
        bottomsSmr = GameObject.FindGameObjectWithTag("Bottoms").GetComponentInChildren<SkinnedMeshRenderer>();
        shoesSmr = GameObject.FindGameObjectWithTag("Shoes").GetComponentInChildren<SkinnedMeshRenderer>();
        //for (int i = 0; i < bodySmr.bones.Length; i++)
         //print(i + " " + bodySmr.bones[i]);
        BoneIndexInit();
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

    }
    public void ChangeClothes(Slider slider)
    {
        int index = (int)slider.value;
        GameObject tops1 = GameObject.FindGameObjectWithTag("Tops");
        if (tops1 == null)
            tops1 = GameObject.FindGameObjectWithTag("Onepiece");

        GameObject tops2 = Resources.Load<GameObject>("Clothes/tops" + index);
        if (tops2 == null)
            return;
        GameObject tops2obj = Instantiate(tops2, bodyModel.transform.parent);
        topsSmr = tops2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        topsIndex = index;
        Slider pattern = slider.gameObject.transform.parent.GetChild(1).GetComponent<Slider>();
        switch (topsIndex)
        {
            case 1: pattern.maxValue = 4; break;
            case 2: pattern.maxValue = 6; break;
            case 3: pattern.maxValue = 5; break;
            case 4: pattern.maxValue = 8; break;
            case 5: pattern.maxValue = 3; break;
            case 6: pattern.maxValue = 7; break;
            case 7: pattern.maxValue = 1; break;
        }
        pattern.value = 1;
        Button bottomB = slider.gameObject.transform.parent.parent.parent.GetChild(1).GetComponent<Button>();
        GameObject bottoms = GameObject.FindGameObjectWithTag("Bottoms");
        if (tops2.tag == "Onepiece")
        {
            bottomB.interactable = false;
            if (bottoms != null)
                Destroy(bottoms);
        }
        else
        {
            bottomB.interactable = true;
            if (bottoms == null)
            {
                GameObject bottom = Resources.Load<GameObject>("Clothes/bottoms" + bottomsIndex);
                if (bottom == null)
                    return;
                bottoms = Instantiate(bottom, bodyModel.transform.parent);
                bottomsSmr = bottoms.GetComponentInChildren<SkinnedMeshRenderer>();
                ShareBones(bottomsSmr);
            }
        }
        Destroy(tops1);
        ShareBones(topsSmr);
    }
    public void ChangeTopsPattern(Slider slider)
    {
        int index = (int)slider.value;
        topsSmr.material = Resources.Load<Material>("Materials/Tops/tops" + topsIndex + "_m" + index);
    }
    public void ChangeBottoms(Slider slider)
    {
        int index = (int)slider.value;
        GameObject bottoms1 = GameObject.FindGameObjectWithTag("Bottoms");

        GameObject bottoms2 = Resources.Load<GameObject>("Clothes/bottoms" + index);
        if (bottoms2 == null)
            return;
        GameObject bottoms2obj = Instantiate(bottoms2, bodyModel.transform.parent);
        bottomsSmr = bottoms2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        bottomsIndex = index;
        Slider pattern = slider.gameObject.transform.parent.GetChild(1).GetComponent<Slider>();
        switch (bottomsIndex)
        {
            case 1: pattern.maxValue = 5; break;
            case 2: pattern.maxValue = 1; break;
            case 3: pattern.maxValue = 5; break;
            case 4: pattern.maxValue = 5; break;
        }
        pattern.value = 1;
        Slider length = slider.gameObject.transform.parent.GetChild(2).GetComponent<Slider>();
        length.value = 1;
        Destroy(bottoms1);
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
        bodySmr.material = Resources.Load<Material>("Materials/bodyskin" + index);
    }
    public void BottomsDetail(Slider slider)
    {
        int index = (int)slider.value;
        if (bottomsIndex == 1)
        {
            bottomsSmr.bones[7].parent.localScale = new Vector3(1, index, 1);
            bottomsSmr.bones[26].parent.localScale = new Vector3(1, index, 1);
        }
        else if (bottomsIndex == 2)
        {
            bottomsSmr.material = Resources.Load<Material>("Materials/Bottoms/bottoms" + bottomsIndex + "_m" + index);
        }
        
    }
    public void ChangeShoes(Slider slider)
    {
        int index = (int)slider.value;
        GameObject shoes1 = GameObject.FindGameObjectWithTag("Shoes");

        GameObject shoes2 = Resources.Load<GameObject>("Clothes/shoes" + index);
        if (shoes2 == null)
            return;
        GameObject shoes2obj = Instantiate(shoes2, bodyModel.transform.parent);
        shoesSmr = shoes2obj.GetComponentInChildren<SkinnedMeshRenderer>();
        Destroy(shoes1);
        ShareBones(shoesSmr);
    }
    void ShareBones(SkinnedMeshRenderer partSmr)
    {
        List<Transform> boneList = new List<Transform>();
        for (int i = 0; i < partSmr.bones.Length; i++)
        {
            boneList.Add(partSmr.bones[i]);
        }

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
    void BoneIndexInit()
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
