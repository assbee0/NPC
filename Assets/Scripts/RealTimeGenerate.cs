using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        //CustomManagerに付いてる他のスクリプトを取る
        facecustom = GetComponent<FaceCustom>();
        haircustom = GetComponent<HairCustom>();
        clothescustom = GetComponent<ClothesCustom>();
        networkexecute = GetComponent<NetworkExecute>();

        
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
        if (num >= 40)
            return;
        InitTags(1);
        GenerateParameter();
        ChangeAllObject();
        SetSitPosition(num);
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
        networkexecute.RandomGenerate();
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
        int row = i % 40 / 8;
        int column = i % 40 % 8;
        int block = i / 40;

        //生成したい位置
        character.transform.position = new Vector3(5f + 2f * row, 0.8f, 2f + 1f * column);
        character.transform.rotation = Quaternion.Euler(0, -90, 0);
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
