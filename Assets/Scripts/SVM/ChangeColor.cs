using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private GameObject nearestNPC;

    // Start is called before the first frame update
    void Start()
    {
        nearestNPC = GetNearestNPC(gameObject);
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

        ChangePlaneColor(nearestNPC);
    }

    GameObject GetNearestNPC(GameObject nowObj)
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

        float dist = 0;           //距離用一時変数
        float nearDist = 0;          //最も近いオブジェクトの距離
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obj in listObj)
        {
            //自身と取得したオブジェクトの距離を取得
            dist = Vector3.Distance(obj.transform.position, nowObj.transform.position);
            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            if (nearDist == 0 || nearDist > dist)
            {
                nearDist = dist;
                targetObj = obj;
            }
        }
        //最も近かったオブジェクトを返す
        return targetObj;
    }

    void ChangePlaneColor(GameObject nearestNPC) // 上部のNPCのA-V値を参照し、色を変更
    {
        NPCAnimationController nearNPCAnimCon = nearestNPC.GetComponent<NPCAnimationController>();
        CharacterParameter cp = nearestNPC.GetComponent<CharacterParameter>();
        byte r = (byte)((byte)cp.valueA - 1);
        byte b = (byte)((byte)cp.valueV - 1);
        gameObject.GetComponent<Renderer>().material.color = new Color32(r, 0, b, 1);
    }
}
