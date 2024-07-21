using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Pool<T> where T : Component {
    T _object;
    Queue<T> queue = new();
    public Pool(T obj, int defaultInstance = 0) {
        _object = obj;
        for (int i = 0; i < defaultInstance; i++) {
            T newObj = Component.Instantiate<T>(_object);
            newObj.transform.SetParent(_object.transform.parent);
            newObj.gameObject.SetActive(false);
            queue.Enqueue(newObj);
        }
    }
    public T Create(Transform parent = null) {
        T obj = queue.Count == 0 ? Component.Instantiate<T>(_object) : queue.Dequeue();
        obj.transform.SetParent(parent == null ? _object.transform.parent : parent);
        if (obj.gameObject.TryGetComponent<RectTransform>(out RectTransform rect)) {
            Vector3 pos = rect.anchoredPosition3D;
            pos.z = 0f;
            obj.GetComponent<RectTransform>().anchoredPosition3D = pos;
        } else {
            obj.transform.position = _object.transform.position;
        }
        obj.transform.localScale = _object.transform.localScale;
        obj.transform.SetAsLastSibling();
        obj.gameObject.SetActive(true);
        return obj;
    }
    public T Return(T obj) {
        obj.transform.SetParent(_object.transform.parent);
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
        return obj;
    }
    public void Release() {
        while (queue.Count > 0) {
            Component.Destroy(queue.Dequeue());
        }
    }
}
