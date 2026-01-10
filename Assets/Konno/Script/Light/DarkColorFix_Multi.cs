using UnityEngine;

public class DarkColorFix_Multi : MonoBehaviour
{
    [Header("薄黒に固定したいオブジェクト")]
    public Renderer[] targets;

    [Range(0f, 1f)]
    public float darkLevel = 0.25f; // 0.2から0.3おすすめ

    void Start()
    {
        Color darkColor = new Color(darkLevel, darkLevel, darkLevel, 1f);

        foreach (Renderer rend in targets)
        {
            if (rend == null) continue;

            // マテリアルを複製（超重要）
            rend.material = new Material(rend.material);
            rend.material.color = darkColor;
        }
    }
}
