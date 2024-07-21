using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CreateObjectOnClick : MonoBehaviour {
    public Component obj;
    public Camera cam;
    Pool<Component> pool;
    void Reset() {
        cam = Camera.main;
    }
    void Awake() {
        pool = new Pool<Component>(obj);
    }
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            foreach (Collider2D i in FindObjectsOfType<Collider2D>()) {
                if (i.OverlapPoint(pos)) { return; }
            }
            Component newObject = pool.Create();
            newObject.transform.position = pos;
            newObject.GetComponent<IPlayable>().Play();
            StartCoroutine(ReturnOjb(newObject));
        }
    }
    IEnumerator ReturnOjb(Component obj) {

        yield return new WaitForSeconds(3f);
        pool?.Return(obj);
    }
}
