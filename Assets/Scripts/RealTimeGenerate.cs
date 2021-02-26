using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class RealTimeGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    public int NPCNumbers = 0;
    private int initNPCNumbers = 80;
    private int maxNPCNumbers = 80;
    private bool initOver = false;
    private FaceCustom facecustom;
    private HairCustom haircustom;
    private ClothesCustom clothescustom;
    private NetworkExecute networkexecute;
    private GameObject character;
    private GameObject black;
    private GameObject mainCamera;
    private GameObject loadingCamera;
    private int[] posx = new int[] { -46, 60, -24, 52, -32, 44, -40, 36, -40, 68 };
    public bool lastGenerate = false;
    void Start()
    {
        //CustomManagerに付いてる他のスクリプトを取る
        facecustom = GetComponent<FaceCustom>();
        haircustom = GetComponent<HairCustom>();
        clothescustom = GetComponent<ClothesCustom>();
        networkexecute = GetComponent<NetworkExecute>();
        networkexecute.SetSrSw(lastGenerate);
        mainCamera = GameObject.Find("Main Camera");
        loadingCamera = GameObject.Find("Loading Camera");

        mainCamera.SetActive(false);
        loadingCamera.SetActive(true);


        //実行時間測る、普段はコメント化で
        /* System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
         sw.Start();
         for (int i = 0; i < 1000; i++)
         {
            // InitTags(1);
             GenerateParameter();
         }
         //networkexecute.sw.Close();
         sw.Stop();
         Debug.Log(sw.ElapsedMilliseconds + "ms");*/


    }

    // Update is called once per frame
    void Update()
    {
        //フレーム毎一人のキャラ生成
        if (!initOver)
            InitGenerate();
        

    }

    private void InitGenerate()
    {
        if (NPCNumbers < initNPCNumbers)
            GenerateOneNPC();
        else if (NPCNumbers == initNPCNumbers)
        {
            mainCamera.SetActive(true);
            mainCamera.GetComponent<CameraLockOn>().enabled = true;
            loadingCamera.SetActive(false);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = true;

            facecustom.enabled = false;
            haircustom.enabled = false;
            clothescustom.enabled = false;
            initOver = true;
            return;
        }
    }

    public void GenerateOneNPC()
    {
        if (NPCNumbers >= maxNPCNumbers)
            return;
        int randomIndex = Random.Range(1, 3);
        InitTags(randomIndex);
        GenerateParameter(randomIndex);
        ChangeAllObject(randomIndex);
        //SetSitPosition(num);
        SetPattern(NPCNumbers);
        NPCNumbers++;
    }

    void InitTags(int pattern)
    /*
     * キャラをカスタムするとき、各部位を探さなければならないので
     * 間違えないように前の生成済みのキャラのタグをリセットする
     */
    {
        if (GameObject.FindGameObjectWithTag("Body") != null)
            GameObject.FindGameObjectWithTag("Body").tag = "Untagged";
        if (GameObject.FindGameObjectWithTag("Face") != null)
            GameObject.FindGameObjectWithTag("Face").tag = "Untagged";
        if (GameObject.FindGameObjectWithTag("Hair") != null)
            GameObject.FindGameObjectWithTag("Hair").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Head").tag = "Untagged";
        if (GameObject.FindGameObjectWithTag("Tops") != null)
            GameObject.FindGameObjectWithTag("Tops").tag = "Untagged";
        else
            GameObject.FindGameObjectWithTag("Onepiece").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Shoes").tag = "Untagged";
        if (GameObject.FindGameObjectWithTag("Bottoms") != null)
            GameObject.FindGameObjectWithTag("Bottoms").tag = "Untagged";
        GameObject prefab;
        if (pattern == 1)
            prefab = Resources.Load<GameObject>("Prefabs/girlNPC");
        else
            prefab = Resources.Load<GameObject>("Prefabs/boyNPC");
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
    void GenerateParameter(int pattern)
    {
        //全パラメータをランダムに生成
        if (!lastGenerate)
        {
            networkexecute.ColorGenerate(pattern);
            //networkexecute.RandomGenerate();
        }
        else
            networkexecute.ReadGenerate();
    }
    void ChangeAllObject(int pattern)
    /*
     * モデルを一部入れ替える調整について（例：髪型、服）
     * カスタムと同じやり方だと実行順が混乱になっちゃうので
     * リアルタイム生成での特有な方法で調整する
     */
    {
        haircustom.RealTimeChangeHair(pattern);
        clothescustom.RealTimeChangeClothes(pattern);
        facecustom.RealTimeChangeFaceBody(pattern);
        // CombineMesh();
    }
    void SetPattern(int i)
    {
        character.GetComponent<NPCController>().SetPattern(i);
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
        character.transform.position = new Vector3(-10.5f + 2f * row, 0.8f, 2f + 1f * column);
        character.transform.rotation = Quaternion.Euler(0, -90, 0);
        character.transform.LookAt(new Vector3(-20, 0.8f, 0));
    }
    public void Rebirth()
    {
        print("start");
        InitTags(1);
        GenerateParameter(1);
        print("over");
    }

}
