using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSetting : MonoBehaviour
{
    private GameObject nearNPC;
    private GameObject plane;
    private int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("GeneratePlane", 10);
    }

    // Update is called once per frame
    void Update()
    {
        //GeneratePlane();
    }

    void GeneratePlane()
    {
        //フレーム毎一人のキャラ生成
        for (num = 0; num < 40; num++)
        {
            //InitTags(1);
            //GenerateParameter();
            //ChangeAllObject();
            GameObject prefab = Resources.Load<GameObject>("Prefabs/plane");
            plane = Instantiate(prefab);
            SetPosition(num);
            //nearNPC = SearchNearNPC(plane);
            //ChangeColor(nearNPC);
            //num++;
        }
    }
    void SetPosition(int i)
    /*
     * 立ち位置固定のキャラを設置
     * 体育館では4ブロックがあってそれぞれ5行8列がある
     */
    {
        int row = i % 40 / 8;
        int column = i % 40 % 8;
        int block = i / 40;

        //生成したい位置
        plane.transform.position = new Vector3(5.88f + 2f * row, 1f, 2f + 1f * column);
        plane.transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    GameObject SearchNearNPC(GameObject nowObj)
    {
        int radius = 100;
        // 中心:自分(平面)の位置, 半径:radiusの球内に存在するものを検出
        Collider[] arrayCld = Physics.OverlapSphere(nowObj.transform.position, radius);
        List<GameObject> listObj = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをlistObjリストに追加
        foreach (Collider cld in arrayCld)
        {
            if (cld.tag == "NPC")
            {
                listObj.Add(cld.gameObject);
            }
        }

        float tmpDis = 0;           //距離用一時変数
        float nearDis = 0;          //最も近いオブジェクトの距離
        //string nearObjName = "";    //オブジェクト名称
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obs in listObj)
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            //一時変数に距離を格納
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                //nearObjName = obs.name;
                targetObj = obs;
            }

        }
        //最も近かったオブジェクトを返す
        //return GameObject.Find(nearObjName);
        return targetObj;
    }

    void ChangeColor(GameObject nearNPC) // 上部のNPCのA-V値を参照し、色を変更
    {
        NPCAnimationController nearNPCAnimCon = nearNPC.GetComponent<NPCAnimationController>(); ;
        byte r = (byte)nearNPCAnimCon.Arousal;
        byte b = (byte)nearNPCAnimCon.Valence;
        plane.GetComponent<Renderer>().material.color = new Color32(r, 0, b, 1);
    }
}
