using UnityEngine;

public class SparkleSpotlights_ColorOnly : MonoBehaviour
{
    public Light[] lights;          // 4つのスポットライト
    public float colorChangeSpeed = 0.15f;

    void Start()
    {
        foreach (Light l in lights)
        {
            l.type = LightType.Spot;

            // 初期色
            l.color = Random.ColorHSV(
                0f, 1f,
                0.8f, 1f,
                1f, 1f
            );
        }

        InvokeRepeating(nameof(ChangeColors), 0f, colorChangeSpeed);
    }

    void ChangeColors()
    {
        foreach (Light l in lights)
        {
            l.color = Random.ColorHSV(
                0f, 1f,
                0.8f, 1f,
                1f, 1f
            );
        }
    }
}
