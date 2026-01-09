using UnityEngine;

public class SparkleSpotlights : MonoBehaviour
{
    public Light[] lights;          // 4‚Â‚Ìƒ‰ƒCƒg
    public float colorChangeSpeed = 0.05f;
    public float moveRadius = 2f;
    public float moveSpeed = 2f;

    Vector3[] basePositions;

    void Start()
    {
        basePositions = new Vector3[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            basePositions[i] = lights[i].transform.localPosition;

            lights[i].type = LightType.Spot;
            lights[i].color = Random.ColorHSV(
                0f, 1f,
                0.8f, 1f,
                1f, 1f
            );
        }

        InvokeRepeating(nameof(ChangeColors), 0f, colorChangeSpeed);
    }

    void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            float t = Time.time * moveSpeed + i * Mathf.PI * 0.5f;

            Vector3 offset = new Vector3(
                Mathf.Sin(t),
                Mathf.Cos(t * 0.7f),
                Mathf.Cos(t)
            ) * moveRadius;

            lights[i].transform.localPosition = basePositions[i] + offset;
        }
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
