using UnityEngine;

public class UI : MonoBehaviour
{
    public float amplitude = 10f;   // ã‰ºˆÚ“®‚Ì•
    public float speed = 3f;        // “®‚­‘¬‚³

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
