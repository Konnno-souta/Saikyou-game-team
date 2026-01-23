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
    // a
    [Range(0f, 1f)]
    public float darkMultiplier = 0.5f;
    [Range(0f, 1f)]
    public float colorInfluence = 0.15f;

    SpriteRenderer[] spriteTargets;
    MeshRenderer[] meshTargets;

    bool isActive = false;
    Color defaultDirColor;

    Color defaultBgColor;
    Color[] defaultSpriteColors;
    Color[] defaultMeshColors;



    void Start()
    {
        RefreshTargets();

        // 初期色保存
        if (backGround != null)
            defaultBgColor = backGround.color;

        if (spriteTargets != null)
        {
            defaultSpriteColors = new Color[spriteTargets.Length];
            for (int i = 0; i < spriteTargets.Length; i++)
                defaultSpriteColors[i] = spriteTargets[i].color;
        }

        if (meshTargets != null)
        {
            defaultMeshColors = new Color[meshTargets.Length];
            for (int i = 0; i < meshTargets.Length; i++)
            {
                if (meshTargets[i].material.HasProperty("_Color"))
                    defaultMeshColors[i] = meshTargets[i].material.color;
            }
        }

        // 最初はOFF
        SetSpotActive(false);

        if (directionalLight != null)
            defaultDirColor = directionalLight.color;
    }


    void ResetDirectionalLight()
    {
        if (directionalLight != null)
            directionalLight.color = defaultDirColor;
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
        if (!isActive) return;
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
    public void StartSpot()
    {
        if (isActive) return;

        isActive = true;
        InvokeRepeating(nameof(ChangeColors), 0f, colorChangeSpeed);
    }

    public void StopSpot()
    {
        if (!isActive) return;

        isActive = false;
        CancelInvoke(nameof(ChangeColors));

        // 色を元に戻す（任意）
        if (directionalLight != null)
            directionalLight.color = defaultDirColor;
    }

    public void SetSpotActive(bool on)
    {
        isActive = on;

        if (on)
        {
            foreach (Light l in lights)
            {
                if (l == null) continue;
                l.enabled = true;
                l.type = LightType.Spot;
            }

            InvokeRepeating(nameof(ChangeColors), 0f, colorChangeSpeed);
        }
        else
        {
            CancelInvoke(nameof(ChangeColors));

            // ライトOFF
            foreach (Light l in lights)
            {
                if (l == null) continue;
                l.enabled = false;
            }

            // ===== 色を元に戻す =====

            if (backGround != null)
                backGround.color = defaultBgColor;

            if (spriteTargets != null)
            {
                for (int i = 0; i < spriteTargets.Length; i++)
                {
                    if (spriteTargets[i] != null)
                        spriteTargets[i].color = defaultSpriteColors[i];
                }
            }

            if (meshTargets != null)
            {
                for (int i = 0; i < meshTargets.Length; i++)
                {
                    if (meshTargets[i] != null && meshTargets[i].material.HasProperty("_Color"))
                        meshTargets[i].material.color = defaultMeshColors[i];
                }
            }

            if (directionalLight != null)
                directionalLight.color = defaultDirColor;
        }
    }
}
