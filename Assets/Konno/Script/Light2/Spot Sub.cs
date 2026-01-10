using UnityEngine;
using UnityEngine.UI;

public class SpotSub : MonoBehaviour
{
    [Header("Spot Lights")]
    public Light[] lights;
    public float colorChangeSpeed = 0.15f;

    [Header("Directional Light")]
    public Light directionalLight;

    [Header("Linked Dark Objects (Auto)")]
    public Transform targetsRoot;   // 子に追加するだけ
    public RawImage backGround;

    [Range(0f, 1f)]
    public float darkMultiplier = 0.5f;
    [Range(0f, 1f)]
    public float colorInfluence = 0.15f;

    SpriteRenderer[] spriteTargets;
    MeshRenderer[] meshTargets;

    void Start()
    {
        RefreshTargets();
        InvokeRepeating(nameof(ChangeColors), 0f, colorChangeSpeed);
    }

    // 子が増えたとき用（任意）
    void OnTransformChildrenChanged()
    {
        RefreshTargets();
    }

    void RefreshTargets()
    {
        if (targetsRoot == null) return;

        spriteTargets = targetsRoot.GetComponentsInChildren<SpriteRenderer>(true);
        meshTargets = targetsRoot.GetComponentsInChildren<MeshRenderer>(true);

        // MeshRenderer はマテリアルを複製（超重要）
        foreach (MeshRenderer mr in meshTargets)
        {
            if (mr == null) continue;
            mr.material = new Material(mr.material);
        }
    }

    void ChangeColors()
    {
        /* ===== スポットライト ===== */
        Color sparkleColor = Random.ColorHSV(
            0f, 1f,
            0.8f, 1f,
            1f, 1f
        );

        foreach (Light l in lights)
        {
            if (l == null) continue;
            l.type = LightType.Spot;
            l.color = sparkleColor;
        }

        /* ===== Directional Light から暗さ取得 ===== */
        Color dirColor = directionalLight.color;

        Color baseDark = new Color(
            dirColor.r * darkMultiplier,
            dirColor.g * darkMultiplier,
            dirColor.b * darkMultiplier,
            1f
        );

        Color linkedColor = Color.Lerp(baseDark, sparkleColor, colorInfluence);

        /* ===== 背景 ===== */
        if (backGround != null)
            backGround.color = baseDark;

        /* ===== SpriteRenderer ===== */
        if (spriteTargets != null)
        {
            foreach (SpriteRenderer sr in spriteTargets)
            {
                if (sr == null) continue;
                sr.color = linkedColor;
            }
        }

        /* ===== MeshRenderer ===== */
        if (meshTargets != null)
        {
            foreach (MeshRenderer mr in meshTargets)
            {
                if (mr == null) continue;

                // Lit / Standard 両対応
                if (mr.material.HasProperty("_Color"))
                    mr.material.color = linkedColor;
            }
        }
    }
}
