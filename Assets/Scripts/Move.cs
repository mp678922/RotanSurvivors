using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public float speed = 1f;
    void Start() {

    }
    void Update() {
        //把物體目前的座標拿出來
        Vector3 pos = transform.position;
        //改變座標的x軸位置
        pos.x = pos.x + (speed * Time.deltaTime);
        //把改變後的座標，放回物體上
        transform.position = pos;
    }
}
