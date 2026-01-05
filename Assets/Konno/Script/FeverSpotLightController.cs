using UnityEngine;
using System.Collections;

public class FeverSpotLightController : MonoBehaviour
{
    public Light[] spotLights;     // 左右4つ
    public Transform target;       // Fever文字（中央）

    public float maxIntensity = 6f;
    public float fadeSpeed = 2f;
    public float rotateSpeed = 5f;

    void Start()
    {
        // 初期状態で消灯
        foreach (var light in spotLights)
        {
            light.intensity = 0f;
        }
    }

    void Update()
    {
        // 常に中央を向かせる
        foreach (var light in spotLights)
        {
            Vector3 dir = target.position - light.transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            light.transform.rotation =
                Quaternion.Slerp(light.transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
        }
    }

    // ===== Fever開始 =====
    public void StartFeverLight()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    // ===== Fever終了 =====
    public void StopFeverLight()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        while (spotLights[0].intensity < maxIntensity)
        {
            foreach (var light in spotLights)
            {
                light.intensity += Time.deltaTime * fadeSpeed;
            }
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (spotLights[0].intensity > 0f)
        {
            foreach (var light in spotLights)
            {
                light.intensity -= Time.deltaTime * fadeSpeed;
            }
            yield return null;
        }
    }
}
