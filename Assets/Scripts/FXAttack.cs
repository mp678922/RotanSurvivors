using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class FXAttack : MonoBehaviour, IPlayable {
    public List<ParticleSystem> particles;
    public List<GameObject> fxList;
    public float lifeTime = 1.5f;
    public Action<FXAttack> onReturn;
    public void Play() {
        foreach (var i in particles) { i.Play(); }
        foreach (var i in fxList) {
            i.GetComponent<IPlayable>()?.Play();
        }
        foreach (Rigidbody2D i in FindObjectsOfType<Rigidbody2D>()) {
            float distance = Vector2.Distance(i.position, transform.position);
            if (distance < 5f) {
                Vector3 force = ((Vector2)transform.position - i.position).normalized;
                force *= distance - 5f * 60f;
                i.AddForce(force);
            }
        }
        Invoke("Return", lifeTime);
    }
    void Return() {
        onReturn?.Invoke(this);
    }
}
