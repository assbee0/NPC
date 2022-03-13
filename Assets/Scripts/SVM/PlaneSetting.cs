using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSetting : MonoBehaviour
{
    private GameObject plane;
    [SerializeField] private ParameterTable m_table = null;
    private int npcNum;

    // Start is called before the first frame update
    void Start()
    {
        npcNum = m_table.HEIGHT * m_table.WIDTH;
        Invoke("GeneratePlane", 15);
    }

    void GeneratePlane()
    {
        //フレーム毎一人のキャラ生成
        for (int num = 0; num < npcNum; num++)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/plane");
            plane = Instantiate(prefab);
            SetPosition(num);
            plane.name = plane.name + "_" + num;
        }
    }
    void SetPosition(int i)
    /*
     * 立ち位置固定のキャラを設置
     * 体育館では4ブロックがあってそれぞれ5行8列がある
     */
    {
        int row = i % npcNum / 8;
        int column = i % npcNum % 8;
        int block = i / npcNum;

        //生成したい位置
        plane.transform.position = new Vector3(5f + 1f * row, 0.81f, 2f + 1f * column);
        plane.transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    void ChangeColor(GameObject nearNPC) // 上部のNPCのA-V値を参照し、色を変更
    {
        NPCAnimationController nearNPCAnimCon = nearNPC.GetComponent<NPCAnimationController>(); ;
        byte r = (byte)nearNPCAnimCon.Arousal;
        byte b = (byte)nearNPCAnimCon.Valence;
        plane.GetComponent<Renderer>().material.color = new Color32(r, 0, b, 1);
    }
}
