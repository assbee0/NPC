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
    private int[] posx = new int[] {-46, 60,-24,52,-32,44,-40,36,-40,68};
    void Start()
    {
        facecustom = GetComponent<FaceCustom>();
        haircustom = GetComponent<HairCustom>();
        clothescustom = GetComponent<ClothesCustom>();
        networkexecute = GetComponent<NetworkExecute>();
        black = GameObject.Find("Black");
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
        if (num == 120)
        {
           // black.SetActive(false);
            return;
        }
        if (num >= 30)
        {
            if(num % 3 !=0)
            {
                num++;
                return;
            }
            InitTags(2);
            GenerateParameter();
            SetSitPosition(num/3-10);
            num++;
            return;
        }
        if (num % 3 !=0)
        {
            num++;
            return;
        }
        InitTags(1);
        GenerateParameter();
        SetPosition(num/3);
        num++;
    }

    void InitTags(int pattern)
    {
        GameObject.FindGameObjectWithTag("Body").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Face").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Hair").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Head").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Tops").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Shoes").tag = "Untagged";
        GameObject.FindGameObjectWithTag("Bottoms").tag = "Untagged";
        GameObject prefab;
        if(pattern == 1)
            prefab = Resources.Load<GameObject>("Prefabs/hadaka");
        else
            prefab = Resources.Load<GameObject>("Prefabs/hadakasit");
        if (prefab == null)
            return;
        character = Instantiate(prefab);
        character.transform.position = new Vector3(0, -10, 0);
        character.transform.localScale = Vector3.one;
        facecustom.RefindObject();
        haircustom.RefindObject();
        clothescustom.RefindObject();
        
        
    }
    void GenerateParameter()
    {
        networkexecute.RTGenerate(1);
    }
    void SetPosition(int i)
    {
        float posz = Random.Range(-1f, 1f);
        character.transform.position = new Vector3(posx[i], 0, posz);
        character.transform.rotation = Quaternion.Euler(0, 90+i*180, 0);
    }
    void SetSitPosition(int i)
    {
        int hang = i / 5;
        int lie = i % 5;
        character.transform.position = new Vector3(2.5f - 1.25f * lie, 0, 9.3f- 1.35f * hang);
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
