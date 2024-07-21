using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFX : MonoBehaviour, IPlayable {
    public Light2D light2D;
    public float time = 0.5f;
    public float startIntensity = 1f;
    public float endIntensity = 0f;
    void Reset() { light2D = GetComponent<Light2D>(); }
    public void Play() {
        StopAllCoroutines();
        StartCoroutine(Animation());
    }
    IEnumerator Animation() {
        light2D.enabled = true;
        light2D.intensity = startIntensity;
        for (float i = 0f; i < time; i += Time.deltaTime) {
            light2D.intensity = Mathf.Lerp(startIntensity, endIntensity, i / time);
            yield return null;
        }
        light2D.intensity = endIntensity;
        if (light2D.intensity == 0) {
            light2D.enabled = false;
        }
    }
}
