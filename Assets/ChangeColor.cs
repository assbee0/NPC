using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private GameObject _parent;
    private NPCAnimationController script;
    private Vector3 initPos; // 自身の初期座標
    private Vector3 initAngle; // 自身の初期角度

    // Start is called before the first frame update
    void Start()
    {
        _parent = transform.root.gameObject;
        script = _parent.GetComponent<NPCAnimationController>();
        initPos = transform.position;
        initAngle = transform.localRotation.eulerAngles;
        //changeColor();
    }

    // Update is called once per frame
    void Update()
    {
        // Planeがキャラクタに追従しないようにする（未完成）
        Vector3 parentAngle = transform.parent.transform.localRotation.eulerAngles;
        Vector3 parentPos = transform.parent.transform.localPosition;
        transform.localRotation = Quaternion.Euler(initAngle - parentAngle);
        transform.localPosition = initPos - parentPos;

        changeColor();
    }

    void changeColor() // 上部のNPCのA-V値を参照し、色を変更
    {
        byte r = (byte)script.Arousal;
        byte b = (byte)script.Valence;
        GetComponent<Renderer>().material.color = new Color32(r, 0, b, 1);
    }
}
