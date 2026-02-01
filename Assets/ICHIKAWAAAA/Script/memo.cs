using NUnit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private float msFirst;
    private Vector3 targetPos;

    [Header("ジャンプ")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("SE")]
    [SerializeField] private AudioSource jumpSE;

    [Header("ステータス")]
    public float baseSpeed = 10f;
    public float speed = 5f;
    public float baseJump = 8f;
    public float jump = 7f;

    // かご（スケールを変える対象）
    [Header("Basket (Goal)")]
    [SerializeField] private Transform basket;              // ここに拡大したいオブジェクトの Transform を割り当て
    [SerializeField] private float basketScaleMultiplier = 1.5f; // 拡大倍率
    [SerializeField] private float basketScaleTweenTime = 0.2f;  // 拡大・縮小の補間時間

    [Header("壁死亡")]
    public string gameOverSceneName = "ResultScene";
    private bool isDead = false;

    // 籠の元スケール
    private Vector3 basketBaseScale;
    private Coroutine basketScaleRoutine;

    [Header("Control Effects")]
    [SerializeField] private bool reverseControls = false; // 操作反転フラグ

    [Header("Invincible Barrier")]
    [SerializeField] private GameObject invincibleBarrier;

    [Header("キャラ画像設定")]
    public Sprite spriteLeft;
    public Sprite spriteRight;
    private SpriteRenderer spriteRenderer;

    [Header("Bom Effect")]
    [SerializeField] private GameObject bomEffectPrefab;
    [SerializeField] private Vector3 bomOffset = Vector3.zero; // 少し上に出したい場合


    private bool isGrounded = false;

    [SerializeField] private Image ballEffectImage;
    [System.Serializable]
    public class BallImagePair
    {
        public string tag;
        public Sprite sprite;
    }

    [SerializeField] private BallImagePair[] ballImages;
    private Dictionary<string, Sprite> ballImageDict;

    [SerializeField] private float ballImageShowTime = 0.3f;
    private Coroutine ballImageRoutine;

    // ====== 効果のレイヤー（系統）分類 ======
    private enum EffectLayer
    {
        Speed,        // 速度系（SpeedUp/Down など）
        Jump,        // ジャンプ系（JumpUp/Down など）
        ControlLock,     // 操作不可（Bom）
        ControlReverse, // 操作反転
        Status,      // 無敵など
        Basket,      // ゴールサイズなど
        Debafu,      // スコア、タイムのマイナス効果
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

    private Rigidbody rb;
    bool move;
    private float sideMoveSmooth;
    public bool Move { get { return move; } }

    public class AutoDestroy : MonoBehaviour
    {
        public float lifeTime = 0.5f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        speed = baseSpeed;
        jump = baseJump;

        BuildEffectDefinitions();
        if (invincibleBarrier != null)
            invincibleBarrier.SetActive(false);
    }

    void Update()
    {
        bool locked = lockCount > 0;
        float inv = (reverseCount % 2 == 1) ? -1f : 1f;
        HandleJumpInput();
        Vector3 move = Vector3.zero;

        if (!locked)
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += inv * speed * transform.right * Time.deltaTime;
                spriteRenderer.sprite = spriteRight; // 右画像
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= inv * speed * transform.right * Time.deltaTime;
                spriteRenderer.sprite = spriteLeft; // 左画像
            }
        }
       
        Check();
    }
    void Check()
    {
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance < 0.05f)
        {
            move = false;
            baseSpeed = msFirst;
        }
    }
    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || jumpCount < maxJumps)
            {
                Debug.Log("ジャンプしてる");
                Jump();
                jumpCount++;
                isGrounded = false;
            }
        }
    }

    // ジャンプ
    void Jump()
    {
        // ▼ ジャンプSE
        if (jumpSE != null)
            jumpSE.PlayOneShot(jumpSE.clip);

        Vector3 vel = rb.velocity;
        //Vector3 vel = rb.linearVelocity;
        vel.y = 0;                   // 上方向の速度をリセット
        rb.velocity = vel;

        rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        // 壁
        if (other.CompareTag("Wall"))
        {
            Die();
            return;
        }
    }
        void Die()
    {
        if (isDead) return;   
        isDead = true;

        Debug.Log("GAME OVER");

        // 
        //rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        SceneManager.LoadScene("ResultScene");
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
            apply = () => { speed = baseSpeed + 5f; Debug.Log($"[SpeedUp] speed={speed}"); },
            cleanup = () => { speed = baseSpeed; Debug.Log($"[SpeedUp End] speed={speed}"); }
        };

        effectMap["SpeedDown"] = new EffectSpec
        {
            tag = "SpeedDown",
            layer = EffectLayer.Speed,
            policy = EffectPolicy.Overwrite,
            duration = 5f,
            apply = () => { speed = Mathf.Max(0f, baseSpeed - 5f); Debug.Log($"[SpeedDown] speed={speed}"); },
            cleanup = () => { speed = baseSpeed; Debug.Log($"[SpeedDown End] speed={speed}"); }
        };

        // ===== Jump レイヤー =====
        effectMap["JumpUp"] = new EffectSpec
        {
            tag = "JumpUp",
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { jump = baseJump + 4f; Debug.Log($"[JumpUp] jump={jump}"); },
            cleanup = () => { jump = baseJump; Debug.Log($"[JumpUp End] jump={jump}"); }
        };

        effectMap["JumpDown"] = new EffectSpec
        {
            tag = "JumpDown",
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { jump = Mathf.Max(0f, baseJump - 4f); Debug.Log($"[JumpDown] jump={jump}"); },
            cleanup = () => { jump = baseJump; Debug.Log($"[JumpDown End] jump={jump}"); }
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
                speed = Mathf.Max(0f, baseSpeed - 10f);
                jump = Mathf.Max(0f, baseJump - 8f);
                Debug.Log("[Bom] 動けない");
            },
            cleanup = () =>
            {
                speed = baseSpeed;
                jump = baseJump;
                Debug.Log($"[Bom End] speed={speed}, jump={jump}");
            }
        };

        // ===== Status レイヤー =====
        effectMap["Invincible"] = new EffectSpec
        {
            tag = "Invincible",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Overwrite,
            duration = Mathf.Infinity,
            apply = () =>
            {
                isInvincible = true;
                invincibleCharges = 2;

                if (invincibleBarrier != null)
                    invincibleBarrier.SetActive(true);

                Debug.Log($"Barrier activeSelf={invincibleBarrier.activeSelf}");
                Debug.Log($"Barrier activeInHierarchy={invincibleBarrier.activeInHierarchy}");


                Debug.Log($"[Invincible] ON (charges={invincibleCharges})");
            },
            cleanup = () =>
            {
                isInvincible = false;
                invincibleCharges = 0;

                if (invincibleBarrier != null)
                    invincibleBarrier.SetActive(false);

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
            layer = EffectLayer.Debafu,  // どこでもOK（参照しない）
            policy = EffectPolicy.Instant,
            duration = 0f,
            apply = () => { /* スコア減算 */ Debug.Log("[-Score] 即時処理"); },
            cleanup = null
        };

        effectMap["-Time"] = new EffectSpec
        {
            tag = "-Time",
            layer = EffectLayer.Debafu,  // どこでもOK（参照しない）
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
            layer = EffectLayer.ControlReverse,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () =>
            {
                reverseCount++;
                Debug.Log($"[Reverse] ON (count={reverseCount})");
            },
            cleanup = () =>
            {
                reverseCount = Mathf.Max(0, reverseCount - 1);
                Debug.Log($"[Reverse] OFF (count={reverseCount})");
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
       //Ground に着地
      if (collision.gameObject.CompareTag("Ground"))
     {
          isGrounded = true;
       }

        string t = collision.gameObject.tag;

        // ★ 無敵チェック（デバフ無効化）
        if (isInvincible && DebuffTags.Contains(t))
        {
            invincibleCharges--;
            Debug.Log($"[Invincible] Block {t} (remain={invincibleCharges})");

            if (invincibleCharges <= 0)
            {
                isInvincible = false;

                if (invincibleBarrier != null)
                    invincibleBarrier.SetActive(false);

                Debug.Log("[Invincible] Charges exhausted");
            }


            Destroy(collision.gameObject);
            return; // ← ここで処理終了（効果は適用しない）
        }

        // 通常の効果処理
        if (effectMap != null && effectMap.ContainsKey(t))
        {
            ApplyBallEffect(t);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator ShowBallImageBom(Sprite sprite)
    {
        ballEffectImage.sprite = sprite;
        ballEffectImage.enabled = true;

        RectTransform rect = ballEffectImage.rectTransform;
        CanvasGroup cg = ballEffectImage.GetComponent<CanvasGroup>();
        if (cg == null) cg = ballEffectImage.gameObject.AddComponent<CanvasGroup>();

        // 初期状態
        rect.localScale = Vector3.zero;
        cg.alpha = 1f;

        // ① ポン！と出る
        float t = 0f;
        float popTime = 0.15f;
        while (t < popTime)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1.2f, t / popTime);
            rect.localScale = Vector3.one * s;
            yield return null;
        }

        // ② 少し戻す（弾み）
        t = 0f;
        float settleTime = 0.1f;
        Vector3 start = rect.localScale;
        while (t < settleTime)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(start, Vector3.one, t / settleTime);
            yield return null;
        }

        // ③ フェードアウト
        t = 0f;
        float fadeTime = 0.4f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            rect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, t / fadeTime);
            yield return null;
        }

        ballEffectImage.enabled = false;
    }

    private void BuildBallImageDict()
    {
        ballImageDict = new Dictionary<string, Sprite>(StringComparer.Ordinal);

        foreach (var pair in ballImages)
        {
            if (!ballImageDict.ContainsKey(pair.tag))
            {
                ballImageDict.Add(pair.tag, pair.sprite);
            }
        }

        if (ballEffectImage != null)
            ballEffectImage.enabled = false;
    }

    private void ChangeBallImage(string tag)
    {
        if (ballEffectImage == null) return;

        if (ballImageDict.TryGetValue(tag, out var sprite))
        {
            if (ballImageRoutine != null)
                StopCoroutine(ballImageRoutine);

            if (tag == "Bom")
            {
                ballImageRoutine = StartCoroutine(ShowBallImageBom(sprite));
            }
            else
            {
                ballImageRoutine = StartCoroutine(ShowBallImageOnce(sprite));
            }
        }
    }

    private IEnumerator ShowBallImageOnce(Sprite sprite)
    {
        ballEffectImage.sprite = sprite;
        ballEffectImage.enabled = true;

        yield return new WaitForSeconds(ballImageShowTime);

        ballEffectImage.enabled = false;
    }
    // ごめんねまーちゃんごめんね
}
