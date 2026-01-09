using UnityEngine;

public class UISpotLightMove : MonoBehaviour
{
    public RectTransform targetText;
    public float radius = 150f;
    public float speed = 2f;
    public float phaseOffset = 0f;

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed + phaseOffset) * radius;
        float y = Mathf.Cos(Time.time * speed * 0.7f + phaseOffset) * 30f;

        rect.position = targetText.position + new Vector3(x, y, 0);
    }
}
