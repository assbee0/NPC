using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;
using System.Linq;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;
    private float pastAr;
    private float pastVa;
    [SerializeField] private float destAr;
    [SerializeField] private float destVa;
    [SerializeField] private float coheAr = 0;
    [SerializeField] private float coheVa = 0;

    [SerializeField] private int catA;
    [SerializeField] private int catV;

    public float timeOut = 20f; // 閾値
    private float timeElapsed = 0.0f; // 累計時間
    private const float SPEED = 0.5f; // Parameterの遷移スピード
    [SerializeField] private float _progress = 0f;
    [SerializeField] private int cycle = 0;

    CharacterParameter cp;
    GlobalParameter gp;
    NPCAnimationController ownAnimCon;
    [SerializeField] RuntimeAnimatorController[] animConArr;  // animConArrの各配列の中身はInspector上で指定
    private GameObject testManager;
    private EnvParameterGenerate envParaGen;
    private GameObject targetObject; // 注視したいオブジェクト

    public bool isLegend;

    public float appeal; // 注目度，魅力
    public float radius = 3;

    private float lookAtWeight;
    private float bodyWeight;
    private float headWeight;
    private float eyesWeight;

    private bool isInitialize;

    // Start is called before the first frame update
    void Start()
    {
        cp = gameObject.GetComponent<CharacterParameter>();
        gp = gameObject.GetComponent<GlobalParameter>();
        animator = GetComponent<Animator>();
        ownAnimCon = gameObject.GetComponent<NPCAnimationController>();
        GetCategoryFromName(this.gameObject);

        testManager = GameObject.Find("TestManager");
        envParaGen = testManager.GetComponent<EnvParameterGenerate>();
        targetObject = GameObject.Find("TargetObject");

        // 感応度：0～0.5 0.5より大きくするとみんな一緒になり始める
        //cp.sensitivity = UnityEngine.Random.Range(0.0f, 1.0f);
        //cp.sensitivity = 1.0f; // 一定にしたいとき

        // animConArrの各配列の中身はInspector上で指定
        // 左利きにチェンジ．左利きの人の割合は10%らしい
        if (!gp.isOnlyWave) {   // 複数動作
            if (UnityEngine.Random.value > 0.9) 
                animator.runtimeAnimatorController = animConArr[0];
            else
                animator.runtimeAnimatorController = animConArr[1];
        } else {                // 単一動作
            if (UnityEngine.Random.value > 0.9)
                animator.runtimeAnimatorController = animConArr[2];
            else
                animator.runtimeAnimatorController = animConArr[3];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gp.COUNT == 15 && isInitialize == false)
        {
            //////////////////////////////////////////////////////////
            // 環境パラメタ取得 & A-Vカテゴリ決定 & A-V値決定 & 動作選定
            Initialize();
        }
        else if (gp.COUNT == 17 && isInitialize == true)
        {
            //////////////////////////////////////////////////////////
            // A-Vコヒーレンスパラメタ取得
            GetAroundAVValue();
            //////////////////////////////////////////////////////////
            // 収束A-V値決定
            destAr = GetNextTerminalValue(pastAr, coheAr);
            destVa = GetNextTerminalValue(pastVa, coheVa);
            isInitialize = false;
        }
        else if (gp.COUNT > 19)
        {
            Execute();
        }
    }

    void Initialize()
    {
        //////////////////////////////////////////////////////////
        // 環境パラメタ取得 & 初期A-Vカテゴリ決定
        SVMExecute svmExecute = gameObject.GetComponent<SVMExecute>();
        svmExecute.Predict();

        // A-V値の遷移
        if (gp.isLegend == false)  // 通常時
        { 
            catA = cp.categoryA;
            catV = cp.categoryV;
        }
        else  // 凡例時
        {   
            catA = cp.d_categoryA;
            catV = cp.d_categoryA;
        }

        //////////////////////////////////////////////////////////
        // 初期A-V値決定
        float intAr = GetParameterFromCategory(catA);
        float intVa = GetParameterFromCategory(catV);
        animator.SetInteger("categoryArousal", catA);
        animator.SetInteger("categoryValence", catV);
        animator.SetFloat("Arousal", intAr);
        animator.SetFloat("Valence", intVa);

        cp.valueA = intAr;
        cp.valueV = intVa;
        pastAr = intAr;
        pastVa = intVa;

        //////////////////////////////////////////////////////////
        // 動作選定
        // AnimationController上で実行
        //////////////////////////////////////////////////////////

        isInitialize = true;
    }

    public void Execute()
    {
        // 一定間隔で実行する場合
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut)
        {
            timeElapsed = 0.0f;
        }
        
        //this.transform.LookAt(targetObject.transform);

        if (_progress > 1f) // サイクル終了
        {
            _progress = 0f;
            pastAr = cp.valueA;  // 現時点でのArousal値を始点に
            pastVa = cp.valueV;  // 現時点でのValence値を始点に
            destAr = GetNextTerminalValue(pastAr, coheAr);
            destVa = GetNextTerminalValue(pastVa, coheVa);
            cycle++;

            if (catA != GetCategoryFromValue(cp.valueA)) {  // 覚醒度カテゴリに変更がある場合
                catA = GetCategoryFromValue(cp.valueA);
                animator.SetInteger("categoryArousal", catA);
            }
            if (catV != GetCategoryFromValue(cp.valueV)) {  // 感情価カテゴリに変更がある場合
                catV = GetCategoryFromValue(cp.valueV);
                animator.SetInteger("categoryValence", catV);
            }

            if (this.name.Equals("girl(Clone)_19"))
            {
                Debug.Log("cycle: " + cycle);
            }

            //////////////////////////////////////////////////////////
            // A-Vコヒーレンスパラメタ取得
            coheAr = 0;
            coheVa = 0;
            GetAroundAVValue();
            if (this.name.Equals("girl(Clone)_18"))
            {
                Debug.Log("success!!");
            }
        }
        _progress = _progress + SPEED * Time.deltaTime;
        //Debug.Log("_progress: " + _progress);

        cp.valueA = Mathf.Lerp(pastAr, destAr, _progress);
        cp.valueV = Mathf.Lerp(pastVa, destVa, _progress);
        animator.SetFloat("Arousal", cp.valueA);
        animator.SetFloat("Valence", cp.valueV);
    }

    float GetParameterFromCategory(int catg)
    {
        float boundary = 256.0f / 3.0f;
        float random = 0;
        if (catg == 0)
        {
            random = UnityEngine.Random.Range(0.0f, boundary);
        }
        else if (catg == 1)
        {
            random = UnityEngine.Random.Range(boundary, boundary * 2.0f);
        }
        else if (catg == 2)
        {
            random = UnityEngine.Random.Range(boundary * 2.0f, 256.0f);
        }
        return random;
    }

    public float GetNextTerminalValue(float pastValue, float cohePara) // 補間値を乱数で取得
    {
        float result = pastValue - cp.sensitivity * cohePara;
        float MIN = 0.0f;
        float MAX = 256.0f;
        return Mathf.Clamp(result, MIN, MAX);
    }

    public void GetCategoryFromName(GameObject obj)
    {
        if (obj.name == "Ch20_nonPBR_Legend") {
            cp.d_categoryA = 0;
            cp.d_categoryA = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (1)") {
            cp.d_categoryA = 0;
            cp.d_categoryA = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (2)") {
            cp.d_categoryA = 0;
            cp.d_categoryA = 2;
        } else if (obj.name == "Ch20_nonPBR_Legend (3)") {
            cp.d_categoryA = 1;
            cp.d_categoryA = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (4)") {
            cp.d_categoryA = 1;
            cp.d_categoryA = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (5)") {
            cp.d_categoryA = 1;
            cp.d_categoryA = 2;
        } else if (obj.name == "Ch20_nonPBR_Legend (6)") {
            cp.d_categoryA = 2;
            cp.d_categoryA = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (7)") {
            cp.d_categoryA = 2;
            cp.d_categoryA = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (8)") {
            cp.d_categoryA = 2;
            cp.d_categoryA = 2;
        }
    }

    public void GetAroundAVValue()
    {
        // 中心:自分の位置, 半径:radiusの球内に存在するものを検出
        Collider[] arrayCld = Physics.OverlapSphere(transform.position, 1.5f);
        List<GameObject> listObj = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをlistObjリストに追加
        foreach (Collider cld in arrayCld)
        {
            if (cld.tag == "NPC" && !cld.name.Equals(this.name))
            {
                listObj.Add(cld.gameObject);
            }
        }

        float objectNum = listObj.Count();
        float appealSum = 0f;  // 周辺NPCノ影響度の総和
        //Debug.Log(objectNum);
        foreach (GameObject obj in listObj)
        {
            NPCAnimationController aroundAnimCon = obj.gameObject.GetComponent<NPCAnimationController>();
            appealSum += aroundAnimCon.appeal;
        }

        foreach (GameObject obj in listObj)
        {
            // 対象となるGameObject objとの距離を調べる
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            // 対象となるGameObject objのA-V値を調べる
            NPCAnimationController aroundAnimCon = obj.gameObject.GetComponent<NPCAnimationController>();
            CharacterParameter aroundCP = obj.gameObject.GetComponent<CharacterParameter>();
            float diffAr = cp.valueA - aroundCP.valueA;
            float diffVa = cp.valueV - aroundCP.valueV;
            // A-Vコヒーレンスパラメタの計算
            coheAr += diffAr * GetAttenuationFromDistance(dist) * aroundAnimCon.appeal / appealSum;
            coheVa += diffVa * GetAttenuationFromDistance(dist) * aroundAnimCon.appeal / appealSum;

            //if (this.name.Equals("girl(Clone)_19") && obj.name.Equals("girl(Clone)_27"))
            //{
            //    Debug.Log(gameObject.name + ": obj=" + obj);
            //    Debug.Log(gameObject.name + ": own.Ar=" + cp.valueA);
            //    Debug.Log(gameObject.name + ": obj.Ar=" + aroundAnimCon.Arousal);
            //    Debug.Log(gameObject.name + ": diffAr=" + diffAr);
            //    Debug.Log(gameObject.name + ": coheAr=" + coheAr);
            //}
        }
        coheAr = coheAr / objectNum;
        coheVa = coheVa / objectNum;

        //Debug.Log(gameObject.name + ": coheAr=" + coheAr);
    }

    public float GetAttenuationFromDistance(float dist)  // 距離減衰を計算
    {
        return (float)Math.Exp(-1 / 25 * ((double)dist - 1));
    }

    private int GetCategoryFromValue(float value)
    {
        float boundary = 256.0f / 3.0f;
        if (0.0f <= value && value < boundary)
        {
            return 0;
        } else if (boundary <= value && value < boundary * 2.0f) {
            return 1;
        } else if (boundary * 2.0f <= value && value <= 256.0f) {
            return 2;
        } else {
            return -99;
        }
    }

    public bool LookAtObject()
    {
        float threshold_value = 0;
        if (catA == 1 && catV == 1)
        {
            threshold_value = 0.4f;
        }
        else if (catA == 1 && catV == 2)
        {
            threshold_value = 0.6f;
        }
        else if (catA == 2 && catV == 1)
        {
            threshold_value = 0.6f;
        }
        else if (catA == 2 && catV == 2)
        {
            threshold_value = 0.8f;
        } else
        {
            threshold_value = 0.0f;
        }

        if (UnityEngine.Random.value < threshold_value)
        {
            return true;
        } else {
            return false;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (targetObject != null)
        {
            if (cp.valueA > cp.valueV)
            {
                lookAtWeight = cp.valueA / 256.0f;
                bodyWeight = cp.valueA / 256.0f;
                headWeight = cp.valueA / 256.0f;
                eyesWeight = cp.valueA / 256.0f;
            } else {
                lookAtWeight = cp.valueV / 256.0f;
                bodyWeight = cp.valueV / 256.0f;
                headWeight = cp.valueV / 256.0f;
                eyesWeight = cp.valueV / 256.0f;
            }

            this.animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight);
            this.animator.SetLookAtPosition(targetObject.transform.position);
        }
    }
}
