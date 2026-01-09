using UnityEngine;

[RequireComponent(typeof(Light))]
public class SpotlightLook : MonoBehaviour
{
    void Start()
    {
        Light light = GetComponent<Light>();

        light.type = LightType.Spot;
        light.color = Color.white;

        // スポットライト感
        light.intensity = 100f;
        light.range = 20f;
        light.spotAngle = 30f;

        // 影
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.9f;
    }
}
