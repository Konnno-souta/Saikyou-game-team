using UnityEngine;
using UnityEngine.UI;

public class SparkleSpotlights_Linked : MonoBehaviour
{
    [Header("Spot Lights")]
    public Light[] lights;
    public float colorChangeSpeed = 0.15f;

    [Header("Linked Dark Objects")]
    public SpriteRenderer[] targets;

    public RawImage backGround;


    [Range(0f, 1f)]
    public float darkLevel = 0.25f;     // ベースの暗さ
    [Range(0f, 1f)]
    public float colorInfluence = 0.15f; // ライト色の影響度

    void Start()
    {
        // マテリアル複製
        foreach (Renderer r in targets)
        {
            if (r == null) continue;
            r.material = new Material(r.material);
        }

        InvokeRepeating(nameof(ChangeColors), 0f, colorChangeSpeed);
    }

    void ChangeColors()
    {
        // ライトの色を決定
        Color sparkleColor = Random.ColorHSV(
            0f, 1f,
            0.8f, 1f,
            1f, 1f
        );

        foreach (Light l in lights)
        {
            l.type = LightType.Spot;
            l.color = sparkleColor;
        }

        // オブジェクト側も連動
        Color baseDark = new Color(darkLevel, darkLevel, darkLevel, 1f);
        Color linkedColor = Color.Lerp(baseDark, sparkleColor, colorInfluence);
        backGround.color = baseDark;
        foreach (Renderer r in targets)
        {
            if (r == null) continue;
            r.material.color = linkedColor;
        }
    }
}
