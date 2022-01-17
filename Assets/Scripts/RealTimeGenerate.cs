using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class RealTimeGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    private FaceCustom facecustom;
    private HairCustom haircustom;
    private ClothesCustom clothescustom;
    private NetworkExecute networkexecute;
    private GameObject character;
    private GameObject black;
    private int num = 0;
    private int[] posx = new int[] { -46, 60, -24, 52, -32, 44, -40, 36, -40, 68 };

    private Animator animator;
    private NPCAnimationController animCon;
    private CharacterParameter cp;
    private GlobalParameter gp;

    public bool lastGenerate = false;

    void Start()
    {
        cp = gameObject.GetComponent<CharacterParameter>();
        gp = gameObject.GetComponent<GlobalParameter>();

        //CustomManagerに付いてる他のスクリプトを取る
        facecustom = GetComponent<FaceCustom>();
        haircustom = GetComponent<HairCustom>();
        clothescustom = GetComponent<ClothesCustom>();
        networkexecute = GetComponent<NetworkExecute>();
        networkexecute.SetSrSw(lastGenerate);

        //facecustom.enabled = false;
        //haircustom.enabled = false;
        //clothescustom.enabled = false;


        //実行時間測る、普段はコメント化で
        /* System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
         sw.Start();
         for (int i = 0; i < 1000; i++)
         {
            // InitTags(1);
             GenerateParameter();
         }
         networkexecute.sw.Close();
         sw.Stop();
         Debug.Log(sw.ElapsedMilliseconds + "ms");*/


    }

    // Update is called once per frame
    void Update()
    {
        //フレーム毎一人のキャラ生成

        if (num >= 64) // 元々は40
        { 
            return;
        }
        InitTags(1);
        GenerateParameter();
        ChangeAllObject();
        SetSitPosition(num);
        SetAVCategory(num);
        num++;
    }

    void InitTags(int pattern)
    /*
     * キャラをカスタムするとき、各部位を探さなければならないので
     * 間違えないように前の生成済みのキャラのタグをリセットする
     */
    {

        GameObject.FindGameObjectWithTag("Body").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Face").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Hair").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Head").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Tops").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Shoes").tag = "Untagged";
        if (GameObject.FindGameObjectWithTag("Bottoms") != null)
            GameObject.FindGameObjectWithTag("Bottoms").tag = "Untagged";
        GameObject prefab;
        if (pattern == 1)
            prefab = Resources.Load<GameObject>("Prefabs/girl");
        else
            prefab = Resources.Load<GameObject>("Prefabs/girl");
        if (prefab == null)
            return;

        character = Instantiate(prefab);
        character.transform.position = new Vector3(0, -10, 0);
        character.transform.localScale = Vector3.one;

        //タグで各部位を確定する
        facecustom.RefindObject();
        haircustom.RefindObject();
        clothescustom.RefindObject();


    }
    void GenerateParameter()
    {
        //全パラメータをランダムに生成
        if (!lastGenerate)
            networkexecute.RandomGenerate();
        else
            networkexecute.ReadGenerate();
    }
    void ChangeAllObject()
    /*
     * モデルを一部入れ替える調整について（例：髪型、服）
     * カスタムと同じやり方だと実行順が混乱になっちゃうので
     * リアルタイム生成での特有な方法で調整する
     */
    {
        haircustom.RealTimeChangeHair();
        clothescustom.RealTimeChangeClothes();
        clothescustom.RealTimeChangeBottoms();
        clothescustom.RealTimeChangeShitagi();
        clothescustom.RealTimeChangeShoes();
    }
    void SetPosition(int i)
    {
        float posz = Random.Range(-1f, 1f);
        character.transform.position = new Vector3(posx[i], 0, posz);
        character.transform.rotation = Quaternion.Euler(0, 90+i*180, 0);
    }
    void SetSitPosition(int i)
    /*
     * 立ち位置固定のキャラを設置
     * 体育館では4ブロックがあってそれぞれ5行8列がある
     */
    {
        int row = i % 64 / 8;
        int column = i % 64 % 8;
        int block = i / 64;

        //生成したい位置
        character.transform.position = new Vector3(5f + 1f * row, 0.8f, 2f + 1f * column);
        //character.transform.position = new Vector3(5f + 2f * row, 0.8f, 2f + 1f * column);
        //character.transform.position = new Vector3(5.88f + 2f * row, 0.8f, 2f + 1f * column);
        //character.transform.position = new Vector3(-10.5f + 2f * row, 0.8f, 2f + 1f * column);
        character.transform.rotation = Quaternion.Euler(0, -90, 0);
        //character.transform.LookAt(new Vector3(-20, 0.8f, 0));
    }

    void SetAVCategory(int num)
    {
        animCon = character.GetComponent<NPCAnimationController>();
        cp = character.GetComponent<CharacterParameter>();
        gp = character.GetComponent<GlobalParameter>();
        character.name = character.name + "_" + num;

        /*
        if (num == 4 || num == 5 || num == 6 || num == 7 || num == 13 || num == 14 || num == 15 || num == 22 || num == 23 || num == 31 )
        {
            gp.isLegend = true;
            cp.d_categoryA = 0;
            cp.d_categoryV = 0;
        } else if (num == 8 || num == 16 || num == 17 || num == 24 || num == 25 || num == 26 || num == 32 || num == 33 || num == 34)
        {
            gp.isLegend = true;
            cp.d_categoryA = 2;
            cp.d_categoryV = 2;
        }
        */


        /*
        if (num == 10 || num == 11 || num == 12 || num == 13 || num == 18 || num == 19 || num == 20 || num == 21 || num == 26 || num == 27 || num == 28 || num == 29 )
        {
            gp.isLegend = true;
            cp.d_categoryA = 2;
            cp.d_categoryV = 2;
        }
        */
        //cp.appeal = UnityEngine.Random.Range(0.1f, 0.5f);
        cp.appeal = 0.1f;

        if (num == 27 || num == 28 || num == 35 || num == 36)
        {
            gp.isLegend = true;
            cp.d_categoryA = 2;
            cp.d_categoryV = 2;
            cp.appeal = 1f;
            cp.sensitivity = 0f;
        } 
        else
        {
            Debug.Log("aaaaaa");
            gp.isLegend = true;
            cp.d_categoryA = 0;
            cp.d_categoryV = 0;
            cp.appeal = 0.1f;
            //cp.sensitivity = 0.8f;
            cp.sensitivity = UnityEngine.Random.Range(0f, 1f);
        }
    }

    public void Rebirth()
    {
        print("start");
        InitTags(1);
        GenerateParameter();
        SetPosition(Random.Range(0,2));
        print("over");
    }


}
