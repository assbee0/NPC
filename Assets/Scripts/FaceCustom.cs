using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceCustom : MonoBehaviour
{
    private bool isRealTime = false;
    private int modelIndex;                                 //モデル性別　1: 女の子　2: 男の子
    private GameObject faceModel;
    private GameObject bodyModel;
    private SkinnedMeshRenderer faceSmr;
    private SkinnedMeshRenderer bodySmr;
    private ParameterManage pm;
    private Color[] hadaIros =
    {
        new Color(1f, 1f, 1f), new Color(1f, 0.965f, 0.929f),  new Color(1f, 0.969f, 0.894f),
        new Color(1f, 0.922f, 0.878f), new Color(0.996f, 0.914f, 0.792f), new Color(1f, 0.894f, 0.741f),
        new Color(1f, 0.831f, 0.565f), new Color(0.847f, 0.643f, 0.49f)
    };

    // Start is called before the first frame update
    void Start()
    {
        modelIndex = 1;
        faceModel = GameObject.FindGameObjectWithTag("Face");
        bodyModel = GameObject.FindGameObjectWithTag("Body");
        faceSmr = faceModel.GetComponent<SkinnedMeshRenderer>();
        bodySmr = bodyModel.GetComponent<SkinnedMeshRenderer>();
         //for (int i = 0; i < faceSmr.bones.Length; i++)
        // print(i+" "+faceSmr.bones[i]);
    }

    // Update is called once per frame
    void Update()
    {
        pm = this.GetComponent<ParameterManage>();
        if(!isRealTime)
        { 
            HeadCustom();
            EyebrowsCustom();
            EyebrowsColor();
            EyesCustom();
            EyesColor();
            NoseCustom();
            MouthCustom();
            EarsCustom();
            BodyCustom();
            BodyColor();
        }
    }
    void HeadCustom()
    {
        Transform neck;
        Transform jaw;
        Transform cheekL;
        Transform cheekR;
        if (modelIndex == 1)
        {
            neck = faceSmr.bones[99];
            jaw = faceSmr.bones[107];
            cheekL = faceSmr.bones[108];
            cheekR = faceSmr.bones[109];
        }
        else
        {
            neck = faceSmr.bones[51];
            jaw = faceSmr.bones[59];
            cheekL = faceSmr.bones[60];
            cheekR = faceSmr.bones[61];
        }
        
        float headWidth = pm.getParameter(0);
        float headLength = pm.getParameter(1);
        float jawY = pm.getParameter(20);
        float cheekround = pm.getParameter(23);
        cheekL.localPosition = new Vector3(-0.037f - 1e-4f * (cheekround - 50), 0.003f - 1e-4f * (cheekround - 50), 0.075f);
        cheekR.localPosition = new Vector3(0.037f + 1e-4f * (cheekround - 50), 0.003f - 1e-4f * (cheekround - 50), 0.075f);
        neck.localScale = new Vector3(0.002f * headWidth + 0.9f, 0.002f * headLength + 0.9f, 1);
        jaw.localScale = new Vector3(1, 0.008f * jawY + 0.4f, 1);
    }
    void EyebrowsCustom()
    {
        Transform eyebrowL;
        Transform eyebrowR;
        if (modelIndex == 1)
        {
            eyebrowL = faceSmr.bones[110];
            eyebrowR = faceSmr.bones[111];
        }
        else
        {
            eyebrowL = faceSmr.bones[62];
            eyebrowR = faceSmr.bones[63];
        }
        
        float eyebrowY = pm.getParameter(12);
        float eyebrowX = pm.getParameter(13);
        float eyebrowLen = pm.getParameter(14);
        float eyebrowT = pm.getParameter(15);
        float eyebrowD = pm.getParameter(16);
        float eyebrowA = pm.getParameter(24);
        eyebrowL.localPosition = new Vector3(-0.047f - 1e-4f * (eyebrowX - 80), 0.096f + 2e-4f * (eyebrowY - 50), 0.089f - 1e-4f * (eyebrowD - 50) + 5e-5f * (eyebrowY - 50));
        eyebrowR.localPosition = new Vector3(0.047f + 1e-4f * (eyebrowX - 80), 0.096f + 2e-4f * (eyebrowY - 50), 0.089f - 1e-4f * (eyebrowD - 50) + 5e-5f * (eyebrowY - 50));
        eyebrowL.localScale = eyebrowR.localScale = new Vector3(0.006f * eyebrowLen + 0.7f, 0.008f * eyebrowT + 0.6f, 1);
        eyebrowL.localRotation = Quaternion.Euler(0, 0, 0.4f * (eyebrowA - 60));
        eyebrowR.localRotation = Quaternion.Euler(0, 0, -0.4f * (eyebrowA - 60));

    }
    void EyebrowsColor()
    {
        Material matBodyHair = bodySmr.materials[1];
        Material matEyebrow;
        if (modelIndex == 1)
            matEyebrow = faceSmr.materials[4];
        else
            matEyebrow = faceSmr.materials[0];
        float hairR = pm.getParameter(34);
        float hairG = pm.getParameter(35);
        float hairB = pm.getParameter(36);
        float eyebrowT = pm.getParameter(42);
        matBodyHair.color = new Color(hairR / 255, hairG / 255, hairB / 255);
        matEyebrow.color = new Color(hairR / 255, hairG / 255, hairB / 255, eyebrowT / 255);
    }
    void EyesCustom()
    {
        Transform eyeL;
        Transform eyeR;
        Transform irisL;
        Transform irisR;
        if (modelIndex == 1)
        {
            eyeL = faceSmr.bones[103];
            eyeR = faceSmr.bones[101];
            irisL = faceSmr.bones[104].parent;
            irisR = faceSmr.bones[102].parent;
        }
        else
        {
            eyeL = faceSmr.bones[55];
            eyeR = faceSmr.bones[53];
            irisL = faceSmr.bones[56].parent;
            irisR = faceSmr.bones[54].parent;
        }

        float eyeY = pm.getParameter(3);
        float eyeX = pm.getParameter(4);
        float eyeW = pm.getParameter(5);
        float eyeH = pm.getParameter(6);
        float eyeD = pm.getParameter(7);
        float eyeA = pm.getParameter(25);
        float irisY = pm.getParameter(26);
        float irisX = pm.getParameter(27);
        float irisW = pm.getParameter(28);
        float irisH = pm.getParameter(29);
        eyeL.localPosition = new Vector3(-0.042f - 5e-5f * (eyeX - 50), 0.059f + 1e-4f * (eyeY - 50), 0.083f - 1e-4f * (eyeD - 50));
        eyeR.localPosition = new Vector3(0.042f + 5e-5f * (eyeX - 50), 0.059f + 1e-4f * (eyeY - 50), 0.083f - 1e-4f * (eyeD - 50));
        irisL.localPosition = new Vector3(-5e-5f * (irisX - 50), 5e-5f * (irisY - 50), 0);
        irisR.localPosition = new Vector3(5e-5f * (irisX - 50), 5e-5f * (irisY - 50), 0);
        eyeL.localScale = eyeR.localScale = new Vector3(0.002f * eyeW + 0.9f, 0.002f * eyeH + 0.9f, 1);
        irisL.localScale = irisR.localScale = new Vector3(0.002f * irisW + 0.9f, 0.002f * irisH + 0.9f, 1);
        eyeL.localRotation = irisR.localRotation = Quaternion.Euler(0, 0, 0.2f * (eyeA - 80));
        eyeR.localRotation = irisL.localRotation = Quaternion.Euler(0, 0, -0.2f * (eyeA - 80));
    }
    void EyesColor()
    {
        Material mat;
        if (modelIndex == 1)
            mat = faceSmr.materials[5];
        else
            mat = faceSmr.materials[2];
        float eyeCR = pm.getParameter(8);
        float eyeCG = pm.getParameter(9);
        float eyeCB = pm.getParameter(10);
        mat.color = new Color(eyeCR / 255, eyeCG / 255, eyeCB / 255);
    }
    void NoseCustom()
    {
        Transform nose;
        if (modelIndex == 1)
            nose = faceSmr.bones[105];
        else
            nose = faceSmr.bones[57];
        float noseY = pm.getParameter(17);
        float size = pm.getParameter(18);
        float angle = pm.getParameter(19);
        nose.localPosition = new Vector3(0, 0.0325f + 1e-4f * (noseY - 50), 0.1f);
        nose.localScale = new Vector3(1, 1, 1) * (size * 0.006f + 0.7f);
        nose.localRotation = Quaternion.Euler(0.2f * (angle - 50), 0, 0);
    }
    void MouthCustom()
    {
        Transform mouth;
        if (modelIndex == 1)
            mouth = faceSmr.bones[106];
        else
            mouth = faceSmr.bones[58];
        float mouthY = pm.getParameter(21);
        float size = pm.getParameter(22);
        mouth.localPosition = new Vector3(0, 1e-4f * (mouthY - 50), 0.0926f + 1e-4f * (mouthY - 50));
        mouth.localScale = new Vector3(size * 0.006f + 0.7f, 1, 1);
    }
    void EarsCustom()
    {
        Transform earL;
        Transform earR;
        if (modelIndex == 1)
        {
            earL = faceSmr.bones[112];
            earR = faceSmr.bones[113];
        }
        else
        {
            earL = faceSmr.bones[64];
            earR = faceSmr.bones[65];
        }
        float earW = pm.getParameter(31);
        float earLen = pm.getParameter(32);
        float earY = pm.getParameter(30);
        earL.localPosition = new Vector3(-0.083f, 0.05f + 1e-4f * (earY - 50), -2e-4f * (earY - 50));
        earR.localPosition = new Vector3(0.083f, 0.05f + 1e-4f * (earY - 50), -2e-4f * (earY - 50));
        earL.localScale = earR.localScale = new Vector3(0.004f * earW + 0.8f, 0.004f * earLen + 0.8f, 0.004f * earW + 0.8f);
    }
    void BodyCustom()
    {
        Transform bodyroot = faceSmr.bones[0];
        Transform spine;
        if (modelIndex == 1)
        {
            spine = faceSmr.bones[40];
            float breastSize = pm.getParameter(11);
            float buttSize = pm.getParameter(49);
            Transform breastL = faceSmr.bones[117];
            Transform breastR = faceSmr.bones[115];
            Transform buttL = faceSmr.bones[119];
            Transform buttR = faceSmr.bones[120];
            breastL.localScale = breastR.localScale = new Vector3(1, 1, 1) * (breastSize * 0.01f + 0.5f);
            buttL.localScale = buttR.localScale = new Vector3((buttSize * 0.005f + 0.6f), 1, (buttSize * 0.005f + 0.6f));
        }
        else
        { 
            spine = faceSmr.bones[10];
        }
            
        float height = pm.getParameter(2);
        float fatness = pm.getParameter(48);
        bodyroot.localScale = new Vector3(fatness * 0.002f + 0.9f, height * 0.0025f + 0.8125f, fatness * 0.002f + 0.9f);
        spine.localScale = Vector3Divide(Vector3.one, (bodyroot.localScale - Vector3.one) * 0.5f + Vector3.one);
    }
    public void BodyColor()
    {
        Material bodyhada = bodySmr.material;                               //体の肌色
        Material facehada;                                                  //顔の肌色
        if (modelIndex == 1)
            facehada = faceSmr.materials[7];
        else
            facehada = faceSmr.materials[5];
        int colorindex = (int)pm.getParameter(47);
        bodyhada.color = facehada.color = hadaIros[colorindex-1];
    }
    public void RefindObject()
    /*　処理するオブジェクト改めて探る　*/
    {
        faceModel = GameObject.FindGameObjectWithTag("Face");
        bodyModel = GameObject.FindGameObjectWithTag("Body");
        faceSmr = faceModel.GetComponent<SkinnedMeshRenderer>();
        bodySmr = bodyModel.GetComponent<SkinnedMeshRenderer>();
    }
    public void SetModelIndex(int model)
    {
        modelIndex = model;
    }
    public void SetRealTime(bool realTime)
    {
        isRealTime = realTime;
    }

    public void RealTimeChangeFaceBody(int pattern)
    {
        modelIndex = pattern;
        HeadCustom();
        EyebrowsCustom();
        EyebrowsColor();
        EyesCustom();
        EyesColor();
        NoseCustom();
        MouthCustom();
        EarsCustom();
        BodyCustom();
        BodyColor();
        print("fuck");
    }

    Vector3 Vector3Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
