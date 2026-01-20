
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("ステータス")]
    public float baseSpeed = 5f;
    public float speed = 5f;
    public float baseJump = 2f;
    public float Jump = 2f;

    // かご（スケールを変える対象）
    [Header("Basket (Goal)")]
    [SerializeField] private Transform basket;              // ここに拡大したいオブジェクトの Transform を割り当て
    [SerializeField] private float basketScaleMultiplier = 1.5f; // 拡大倍率
    [SerializeField] private float basketScaleTweenTime = 0.2f;  // 拡大・縮小の補間時間

    // 籠の元スケール
    private Vector3 basketBaseScale;
    private Coroutine basketScaleRoutine;

    [Header("Control Effects")]
    [SerializeField] private bool reverseControls = false; // 操作反転フラグ

    private Rigidbody rb;
    private bool isGrounded = false;

    // ====== 効果のレイヤー（系統）分類 ======
    private enum EffectLayer
    {
        Speed,        // 速度系（SpeedUp/Down など）
        Jump,        // ジャンプ系（JumpUp/Down など）
        ControlLock,     // 操作不可（Bom）
        ControlReverse, // 操作反転
        Status,      // 無敵など
        Basket,      // ゴールサイズなど
        // ほか必要なら追加…
    }

    // 効果の適用ポリシー
    private enum EffectPolicy
    {
        Overwrite,   // レイヤーのスロットを占有。既存があれば上書き
        Instant      // 即時実行後終了。スロットを占有しない
    }

    // 効果ハンドル（タイマー・後始末）
    private class EffectHandle
    {
        public Coroutine routine;
        public Action cleanup;
        public string name;
        public EffectLayer layer;
        public float endsAt; // デバッグ用（終了予定時刻）
    }

    // アクティブ効果
    private readonly Dictionary<EffectLayer, EffectHandle> activeByLayer =
        new Dictionary<EffectLayer, EffectHandle>();

    // 効果定義
    private class EffectSpec
    {
        public string tag;
        public EffectLayer layer;
        public EffectPolicy policy;
        public float duration;       
        public Action apply;
        public Action cleanup;      
    }


    // デバフとみなすタグ
    private static readonly HashSet<string> DebuffTags = new HashSet<string>(StringComparer.Ordinal)
    {
      "SpeedDown", "JumpDown", "Bom", "-Score", "-Time","Reverse"
    };


    // 無敵の状態管理
    private int invincibleCharges = 0;
    private bool isInvincible = false;

    // Control 系の合成状態
    private int lockCount = 0;      // 1以上で操作不可
    private int reverseCount = 0;   // 奇数で反転、偶数で通常

    // タグ→定義 の辞書
    private Dictionary<string, EffectSpec> effectMap;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        speed = baseSpeed;
        Jump = baseJump;

        BuildEffectDefinitions();
    }

    void Update()
    {
        bool locked = lockCount > 0;
        float inv = (reverseCount % 2 == 1) ? -1f : 1f;

        if (!locked)
        {
            if (Input.GetKey(KeyCode.W))
                transform.position += inv * speed * transform.forward * Time.deltaTime;

            if (Input.GetKey(KeyCode.S))
                transform.position -= inv * speed * transform.forward * Time.deltaTime;

            if (Input.GetKey(KeyCode.D))
                transform.position += inv * speed * transform.right * Time.deltaTime;

            if (Input.GetKey(KeyCode.A))
                transform.position -= inv * speed * transform.right * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !locked)
        {
            rb.AddForce(Vector3.up * Jump, ForceMode.Impulse);
            isGrounded = false;
        }
    }


    // 起動時にベーススケールをキャプチャ
    void Awake()
    {
        if (basket != null)
        {
            basketBaseScale = basket.localScale;
        }
    }

    // エディタで値を変えた時もベースを更新（任意）
    void OnValidate()
    {
        if (basket != null)
        {
            basketBaseScale = basket.localScale;
        }
    }

    // かごのやつ
    private IEnumerator TweenBasketScale(Vector3 target, float time, bool unscaled = false)
    {
        if (basket == null) yield break;

        Vector3 start = basket.localScale;
        if (time <= 0f)
        {
            basket.localScale = target;
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            float dt = unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt / time;
            float ease = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            basket.localScale = Vector3.LerpUnclamped(start, target, ease);
            yield return null;
        }
        basket.localScale = target;
    }

    // ---------------- 効果定義をまとめる ----------------
    private void BuildEffectDefinitions()
    {
        effectMap = new Dictionary<string, EffectSpec>(StringComparer.Ordinal);

        // ===== Move レイヤー =====
        effectMap["SpeedUp"] = new EffectSpec
        {
            tag = "SpeedUp",
            layer = EffectLayer.Speed,
            policy = EffectPolicy.Overwrite,
            duration = 5f,
            apply = () => { speed = baseSpeed + 2f; Debug.Log($"[SpeedUp] speed={speed}"); },
            cleanup = () => { speed = baseSpeed; Debug.Log($"[SpeedUp End] speed={speed}"); }
        };

        effectMap["SpeedDown"] = new EffectSpec
        {
            tag = "SpeedDown",
            layer = EffectLayer.Speed,
            policy = EffectPolicy.Overwrite,
            duration = 5f,
            apply = () => { speed = Mathf.Max(0f, baseSpeed - 2f); Debug.Log($"[SpeedDown] speed={speed}"); },
            cleanup = () => { speed = baseSpeed; Debug.Log($"[SpeedDown End] speed={speed}"); }
        };

        // ===== Jump レイヤー =====
        effectMap["JumpUp"] = new EffectSpec
        {
            tag = "JumpUp",
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { Jump = baseJump + 2f; Debug.Log($"[JumpUp] jump={Jump}"); },
            cleanup = () => { Jump = baseJump; Debug.Log($"[JumpUp End] jump={Jump}"); }
        };

        effectMap["JumpDown"] = new EffectSpec
        {
            tag = "JumpDown",
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { Jump = Mathf.Max(0f, baseJump - 2f); Debug.Log($"[JumpDown] jump={Jump}"); },
            cleanup = () => { Jump = baseJump; Debug.Log($"[JumpDown End] jump={Jump}"); }
        };

        // ===== Control レイヤー =====
        effectMap["Bom"] = new EffectSpec
        {
            tag = "Bom",
            layer = EffectLayer.ControlLock,
            policy = EffectPolicy.Overwrite,
            duration = 2f,
            apply = () =>
            {
                speed = Mathf.Max(0f, baseSpeed - 5f);
                Jump = Mathf.Max(0f, baseJump - 2f);
                Debug.Log("[Bom] 動けない");
            },
            cleanup = () =>
            {
                speed = baseSpeed;
                Jump = baseJump;
                Debug.Log($"[Bom End] speed={speed}, jump={Jump}");
            }
        };

        // ===== Status レイヤー =====
        effectMap["Invincible"] = new EffectSpec
        {
            tag = "Invincible",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Overwrite,
            duration = 0f, 
            apply = () =>
            {
                isInvincible = true;
                invincibleCharges = 2; // ★ 2回分チャージ
                Debug.Log($"[Invincible] ON (charges={invincibleCharges})");
            },
            cleanup = () =>
            {
                isInvincible = false;
                invincibleCharges = 0;
                Debug.Log("[Invincible] OFF");
            }
        };

        // ===== Basket レイヤー =====
        effectMap["BigBasket"] = new EffectSpec
        {
            tag = "BigBasket",
            layer = EffectLayer.Basket,
            policy = EffectPolicy.Overwrite,   // 同効果が来たら上書き開始（Refresh/Extendにしたいなら後述）
            duration = 3f,
            apply = () =>
            {
                if (basket == null)
                {
                    Debug.LogWarning("[BigBasket] basket が未割り当てです");
                    return;
                }

                // 多重起動対策：前の補間を止める
                if (basketScaleRoutine != null)
                    StopCoroutine(basketScaleRoutine);

                // 目標スケール（ベース×倍率）
                Vector3 target = basketBaseScale * basketScaleMultiplier;

                // 補間で拡大（TimeScaleの影響を受けたくない場合は第3引数 true）
                basketScaleRoutine = StartCoroutine(TweenBasketScale(target, basketScaleTweenTime, false));
                Debug.Log("[BigBasket] ON");
            },
            cleanup = () =>
            {
                if (basket == null) return;

                if (basketScaleRoutine != null)
                    StopCoroutine(basketScaleRoutine);

                // 元のサイズへ戻す
                basketScaleRoutine = StartCoroutine(TweenBasketScale(basketBaseScale, basketScaleTweenTime, false));
                Debug.Log("[BigBasket] OFF");
            }
        };


        // ===== Instant（スロット非占有） =====
        effectMap["-Score"] = new EffectSpec
        {
            tag = "-Score",
            layer = EffectLayer.Status,  // どこでもOK（参照しない）
            policy = EffectPolicy.Instant,
            duration = 0f,
            apply = () => { /* スコア減算 */ Debug.Log("[-Score] 即時処理"); },
            cleanup = null
        };

        effectMap["-Time"] = new EffectSpec
        {
            tag = "-Time",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Instant,
            duration = 0f,
            apply = () => { /* タイム減少 */ Debug.Log("[-Time] 即時処理"); },
            cleanup = null
        };

        effectMap["ScrollStop"] = new EffectSpec
        {
            tag = "ScrollStop",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Instant,
            duration = 3f,
            apply = () => { Debug.Log("ScrollStop"); },
            cleanup = null
        };

        // ===== Control レイヤー =====
        effectMap["Reverse"] = new EffectSpec
        {
            tag = "Reverse",
            layer = EffectLayer.ControlReverse,       // ← Status ではなく Control レイヤーに
            policy = EffectPolicy.Overwrite,   // 同効果を再入手でタイマーをリセットしたい（延長なら Extend/Refresh運用）
            duration = 3f,                     // 反転の継続秒数
            apply = () =>
            {
                reverseControls = true;
                Debug.Log("[Reverse] 操作反転 ON");
            },
            cleanup = () =>
            {
                reverseControls = false;
                Debug.Log("[Reverse] 操作反転 OFF");
            }
        };

        effectMap["しかいせまくなる"] = new EffectSpec
        {
            tag = "???",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Instant,
            duration = 3f,
            apply = () => { Debug.Log("???"); },
            cleanup = null
        };
    }

    // ---------------- レイヤーごとの上書き処理 ----------------
    private void StartEffectByLayer(EffectSpec spec)
    {
        if (spec.policy == EffectPolicy.Instant)
        {
            // 即時系：スロットを占有せず、その場で適用して終わり
            try { spec.apply?.Invoke(); }
            catch (Exception e) { Debug.LogWarning($"[Effect Instant] 例外: {e.Message}"); }
            return;
        }

        // Overwrite 系：同じレイヤー内の既存効果を終了→新規開始
        CancelLayer(spec.layer);

        // 新規効果適用
        try { spec.apply?.Invoke(); }
        catch (Exception e) { Debug.LogWarning($"[Effect Apply] 例外: {e.Message}"); }

        var handle = new EffectHandle
        {
            name = spec.tag,
            cleanup = spec.cleanup,
            layer = spec.layer,
            endsAt = Time.time + Mathf.Max(0f, spec.duration)
        };

        handle.routine = StartCoroutine(RunEffectTimerByLayer(handle, spec.duration));
        activeByLayer[spec.layer] = handle;

        Debug.Log($"[Effect Start] {spec.tag} layer={spec.layer} dur={spec.duration:F2}s");
    }

    private IEnumerator RunEffectTimerByLayer(EffectHandle handle, float duration)
    {
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        // 同じハンドルが現役なら終了
        if (activeByLayer.TryGetValue(handle.layer, out var current) && current == handle)
        {
            try { handle.cleanup?.Invoke(); }
            catch (Exception e) { Debug.LogWarning($"[Effect Cleanup] 例外: {e.Message}"); }

            activeByLayer.Remove(handle.layer);
            Debug.Log($"[Effect End] {handle.name} layer={handle.layer}");
        }
    }

    private void CancelLayer(EffectLayer layer)
    {
        if (!activeByLayer.TryGetValue(layer, out var handle)) return;

        // 後始末 → コルーチン停止 → 取り消し
        try { handle.cleanup?.Invoke(); }
        catch (Exception e) { Debug.LogWarning($"[Effect Cancel Cleanup] 例外: {e.Message}"); }

        if (handle.routine != null)
            StopCoroutine(handle.routine);

        activeByLayer.Remove(layer);
        Debug.Log($"[Effect Cancel] {handle.name} layer={layer}");
    }

    // ---------------- タグから効果を適用 ----------------
    private void ApplyBallEffect(string tagName)
    {
        if (!effectMap.TryGetValue(tagName, out var spec))
        {
            Debug.LogWarning($"未対応のタグ: {tagName}");
            return;
        }

        StartEffectByLayer(spec);
    }

    // ---------------- 衝突処理 ----------------
    private void OnCollisionEnter(Collision collision)
    {
        // Ground に着地
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // タグが効果辞書に登録済みなら適用
        string t = collision.gameObject.tag;
        if (effectMap != null && effectMap.ContainsKey(t))
        {
            ApplyBallEffect(t);
            Destroy(collision.gameObject);
        }
    }
    // ごめんねまーちゃんごめんね
}
