using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DarkColorFix : MonoBehaviour
{
    [Range(0f, 1f)]
    public float darkLevel = 0.25f; // 0 = 黒, 1 = 白（0.2から0.3おすすめ）

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // マテリアルを複製（他オブジェクトに影響しない）
        rend.material = new Material(rend.material);
    }

    void Start()
    {
        ApplyDarkColor();
    }

    void ApplyDarkColor()
    {
        Color darkColor = new Color(darkLevel, darkLevel, darkLevel, 1f);
        rend.material.color = darkColor;
    }
}
