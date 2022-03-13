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
    private int npcIndex = 0;
    private int[] posx = new int[] { -46, 60, -24, 52, -32, 44, -40, 36, -40, 68 };

    private NPCAnimationController animCon;

    public bool lastGenerate = false;

    private const float APPEAL_CENTER = 1.0f;
    private const float APPEAL_OTHERS = 0.1f;
    private const float SENSITIVITY_CENTER = 0f;
    private const float SENSITIVITY_OTHERS = 0.8f;
    [SerializeField] private ParameterTable m_table = null;
    private int npcNum;

    void Start()
    {
        //CustomManagerに付いてる他のスクリプトを取る
        facecustom = GetComponent<FaceCustom>();
        haircustom = GetComponent<HairCustom>();
        clothescustom = GetComponent<ClothesCustom>();
        networkexecute = GetComponent<NetworkExecute>();
        networkexecute.SetSrSw(lastGenerate);

        npcNum = m_table.HEIGHT * m_table.WIDTH;

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
        if (npcIndex >= npcNum) return;  // 元々は40

        InitTags(1);
        GenerateParameter();
        ChangeAllObject();
        SetSitPosition(npcIndex);
        SetAVCategory(npcIndex);
        npcIndex++;
    }

    void InitTags(int pattern)
    /*
     * キャラをカスタムするとき、各部位を探さなければならないので
     * 間違えないように前の生成済みのキャラのタグをリセットする
     */
    {
        GameObject.FindGameObjectWithTag("Body").tag = "Untagged";  // FindGameObjectWithTag("XXX"): "XXX"のTagがついた複数オブジェクトを取得
        GameObject.FindGameObjectWithTag("Face").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Hair").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Head").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Tops").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Shoes").tag = "Untagged";
        if (GameObject.FindGameObjectWithTag("Bottoms") != null)
            GameObject.FindGameObjectWithTag("Bottoms").tag = "Untagged";
        
        GameObject prefab;
        if (pattern == 1)
            prefab = Resources.Load<GameObject>("Prefabs/girl");  // Resources/Prefabs内にあるgirlを取得
        else
            prefab = Resources.Load<GameObject>("Prefabs/girl");
        if (prefab == null)
            return;

        character = Instantiate(prefab);
        character.transform.position = new Vector3(0, -10, 0);
        character.transform.localScale = Vector3.one;  // transform.localSclae: オブジェクトの拡大率を指定

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
     */
    {
        int row = i % npcNum / 8;
        int column = i % npcNum % 8;
        int block = i / npcNum;

        //生成したい位置
        character.transform.position = new Vector3(5f + 1f * row, 0.8f, 2f + 1f * column);
        character.transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    void SetAVCategory(int npcIndex)
    {
        animCon = character.GetComponent<NPCAnimationController>();
        character.name = character.name + "_" + npcIndex;

        /*
        if (npcIndex == 4 || npcIndex == 5 || npcIndex == 6 || npcIndex == 7 || npcIndex == 13 || npcIndex == 14 || npcIndex == 15 || npcIndex == 22 || npcIndex == 23 || npcIndex == 31 )
        {
            animCon.isTest = true;
            animCon.catArTest = 0;
            animCon.catVaTest = 0;
        } else if (npcIndex == 8 || npcIndex == 16 || npcIndex == 17 || npcIndex == 24 || npcIndex == 25 || npcIndex == 26 || npcIndex == 32 || npcIndex == 33 || npcIndex == 34)
        {
            animCon.isTest = true;
            animCon.catArTest = 2;
            animCon.catVaTest = 2;
        }
        */

        /*
        if (npcIndex == 10 || npcIndex == 11 || npcIndex == 12 || npcIndex == 13 || npcIndex == 18 || npcIndex == 19 || npcIndex == 20 || npcIndex == 21 || npcIndex == 26 || npcIndex == 27 || npcIndex == 28 || npcIndex == 29 )
        {
            animCon.isTest = true;
            animCon.catArTest = 2;
            animCon.catVaTest = 2;
        }
        */

        //animCon.appeal = UnityEngine.Random.Range(0.1f, 0.5f);

        // 感応度：0～0.5 0.5より大きくするとみんな一緒になり始める
        if (npcIndex == 27 || npcIndex == 28 || npcIndex == 35 || npcIndex == 36)  // 中央4人
        {
            animCon.isTest = true;
            animCon.catArTest = 2;
            animCon.catVaTest = 2;
            animCon.appeal = APPEAL_CENTER;
            animCon.sensitivity = SENSITIVITY_CENTER;
        } else
        {
            animCon.isTest = true;
            animCon.catArTest = 0;
            animCon.catVaTest = 0;
            animCon.appeal = APPEAL_OTHERS;
            //animCon.sensitivity = SENSITIVITY_OTHERS;
            animCon.sensitivity = UnityEngine.Random.Range(0f, 1f);
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
