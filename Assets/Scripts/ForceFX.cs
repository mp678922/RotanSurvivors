using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ParticleSystemForceField))]
public class Force2D : MonoBehaviour, IPlayable {
    public ParticleSystemForceField force2D;
    public float time = 0.5f;
    public float startGravity = 1f;
    public float endGravity = 0f;
    void Reset() { force2D = GetComponent<ParticleSystemForceField>(); }
    public void Play() {
        StopAllCoroutines();
        StartCoroutine(Animation());
    }
    IEnumerator Animation() {
        force2D.enabled = true;
        ParticleSystem.MinMaxCurve cruve = new ParticleSystem.MinMaxCurve();
        cruve.constant = startGravity;
        force2D.gravity = cruve;
        for (float i = 0f; i < time; i += Time.deltaTime) {
            cruve.constant = Mathf.Lerp(startGravity, endGravity, i / time);
            force2D.gravity = cruve;
            yield return null;
        }
        cruve.constant = endGravity;
        force2D.gravity = cruve;
        if (cruve.constant == 0f) {
            force2D.enabled = false;
        }
    }
}
