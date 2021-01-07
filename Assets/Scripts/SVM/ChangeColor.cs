using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private GameObject _parent;
    private NPCAnimationController script;
    private Vector3 initPos; // 自身の初期座標
    private Vector3 initAngle; // 自身の初期角度
    [SerializeField] private GameObject nearNPC;


    // Start is called before the first frame update
    void Start()
    {
        /*
        _parent = transform.root.gameObject;
        script = _parent.GetComponent<NPCAnimationController>();
        initPos = transform.position;
        initAngle = transform.localRotation.eulerAngles;
        //changeColor();
        */

        nearNPC = SearchNearNPC(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Planeがキャラクタに追従しないようにする（未完成）
        Vector3 parentAngle = transform.parent.transform.localRotation.eulerAngles;
        Vector3 parentPos = transform.parent.transform.localPosition;
        transform.localRotation = Quaternion.Euler(initAngle - parentAngle);
        transform.localPosition = initPos - parentPos;
        */

        changeColor(nearNPC);
    }

    GameObject SearchNearNPC(GameObject nowObj)
    {
        int radius = 10;
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

    void changeColor(GameObject nearNPC) // 上部のNPCのA-V値を参照し、色を変更
    {
        NPCAnimationController nearNPCAnimCon = nearNPC.GetComponent<NPCAnimationController>(); ;
        byte r = (byte)((byte)nearNPCAnimCon.Arousal - 1);
        byte b = (byte)((byte)nearNPCAnimCon.Valence - 1);
        gameObject.GetComponent<Renderer>().material.color = new Color32(r, 0, b, 1);
    }
}
