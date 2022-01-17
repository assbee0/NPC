using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSetting : MonoBehaviour
{
    private GameObject plane;
    private int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("GeneratePlane", 15);
    }

    // Update is called once per frame
    void Update()
    {
        //GeneratePlane();
    }

    void GeneratePlane()
    {
        //フレーム毎一人のキャラ生成
        for (num = 0; num < 64; num++)
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
        int row = i % 64 / 8;
        int column = i % 64 % 8;
        int block = i / 64;

        //生成したい位置
        //plane.transform.position = new Vector3(5.88f + 2f * row, 0.81f, 2f + 1f * column);
        //plane.transform.position = new Vector3(5f + 2f * row, 0.81f, 2f + 1f * column);
        plane.transform.position = new Vector3(5f + 1f * row, 0.81f, 2f + 1f * column);
        plane.transform.rotation = Quaternion.Euler(0, -90, 0);
    }
}
