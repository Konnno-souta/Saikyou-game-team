using System;
using System.Collections;
using System.Collections.Generic;
using NUnit;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;


public class PPPP : MonoBehaviour
{
    [SerializeField] PlayerMovement pTR;
    [SerializeField] FloorHitCheck fhc;

    private float stepDistance;   // 1???T?C?h?X?e?b?v??
    public float moveSpeed;      // ??????x?i??????????????????B?j
    private float msFirst;

    private Vector3 targetPos;


    [Header("ジャンプ")]
    public int maxJumps = 2;
    private int jumpCount = 0;
    //private int jumpCount = 0;


    [Header("ステータス")]
    public float baseSpeed = 10f;
    public float speed = 5f;
    public float baseJump = 8f;
    public float jump = 7f;
    //public float airControl = 0.4f;   // ??????????????

    [Header("キャラ画像設定")]
    public Sprite spriteLeft;
    public Sprite spriteRight;
    private SpriteRenderer spriteRenderer;

    [Header("壁死亡")]
    public string gameOverSceneName = "ResultScene";
    private bool isDead = false;

    private Rigidbody rb;
    bool move;
    private float sideMoveSmooth;
    public bool Move { get { return move; } }



    //public float baseSp { get { return baseSpeed; } }[Header("Ball Effect UI")]
    [SerializeField] private Image ballEffectImage;

    [System.Serializable]
    public class BallImagePair
    {
        public string tag;
        public Sprite sprite;
    }

    [SerializeField] private BallImagePair[] ballImages;
    private Dictionary<string, Sprite> ballImageDict;
    //void Start()
    //{
    //    stepDistance = pTR.baseSp;
    //    targetPos = transform.position;
    //    msFirst = moveSpeed;

    //    rb = GetComponent<Rigidbody>();
    //    spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer?擾
    //    rb.freezeRotation = true; // ?S??]?????
    //}


    // かご（スケールを変える対象）
    [Header("Basket (Goal)")]
    [SerializeField] private Transform basket;              // 拡大したいオブジェクトの Transform
    [SerializeField] private float basketScaleMultiplier = 1.5f; // 拡大倍率
    [SerializeField] private float basketScaleTweenTime = 0.2f;  // 拡大・縮小の補間時間

    // 効果のレイヤー（系統）分類
    private Vector3 basketBaseScale;
    private Coroutine basketScaleRoutine;

    [Header("Control Effects")]
    [SerializeField] private bool reverseControls = false; // デバフとみなすタグ

    [Header("Bom Effect")]
    [SerializeField] private GameObject bomEffectPrefab;
    [SerializeField] private Vector3 bomOffset = Vector3.zero; // 少し上に出したい場合

    [Header("SE")]
    [SerializeField] private AudioSource jumpSE;

    // バリア浜崎使ってね
    [Header("Invincible Barrier")]
    [SerializeField] private GameObject invincibleBarrier;


    public class AutoDestroy : MonoBehaviour
    {
        public float lifeTime = 0.5f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }


    private bool isGrounded = false;

    // ====== 無敵の状態管理 ======
    private enum EffectLayer
    {
        Speed,        // ???x?n?iSpeedUp/Down ???j
        Jump,        // ?W?????v?n?iJumpUp/Down ???j
        ControlLock,     // ????s??iBom?j
        ControlReverse, // ?????]
        Status,      // ???G???
        Basket,      // ?S?[???T?C?Y???
        Debafu,      // ?X?R?A?A?^?C????}?C?i?X????
        // ????K?v??????c
    }

    // ?????K?p?|???V?[
    private enum EffectPolicy
    {
        Overwrite,   // ???C???[??X???b?g???L?B??????????????
        Instant      // ???????s??I???B?X???b?g???L?????
    }
    public enum BallType
    {
        SpeedUp,
        SpeedDown,
        JumpUp,
        JumpDown,
        Bom,
        Invincible,
        BigBasket,
        MinusScore,
        MinusTime,
        Reverse,
    }

    // ????n???h???i?^?C?}?[?E??n???j
    private class EffectHandle
    {
        public Coroutine routine;
        public Action cleanup;
        public string name;
        public EffectLayer layer;
        public float endsAt; // ?f?o?b?O?p?i?I???\?莞???j
    }

    // ?A?N?e?B?u????
    private readonly Dictionary<EffectLayer, EffectHandle> activeByLayer =
        new Dictionary<EffectLayer, EffectHandle>();

    // ?????`
    private class EffectSpec
    {
        public EffectLayer layer;
        public EffectPolicy policy;
        public float duration;
        public Action apply;
        public Action cleanup;
    }

    private Coroutine bomImageRoutine;

    //private IEnumerator ShowBomImage()
    //{
    //    if (bomImage == null) yield break;

    //    bomImage.SetActive(true);
    //    yield return new WaitForSeconds(bomImageTime);
    //    bomImage.SetActive(false);
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Die();
        }
    }




    // ?f?o?t??????^?O
    private static readonly HashSet<string> DebuffTags = new HashSet<string>(StringComparer.Ordinal)
    {
      "SpeedDown", "JumpDown", "Bom", "-Score", "-Time","Reverse"
    };


    // ???G??????
    private int invincibleCharges = 0;
    private bool isInvincible = false;

    // Control ?n????????
    private int lockCount = 0;      // 1???????s??
    private int reverseCount = 0;   // ??????]?A????????

    // ?^?O????` ?????
    private Dictionary<BallType, EffectSpec> effectMap;

    [SerializeField] private float ballImageShowTime = 0.3f;
    private Coroutine ballImageRoutine;

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



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        speed = baseSpeed;
        jump = baseJump;

        BuildEffectDefinitions();
        BuildBallImageDict();
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

    // ===============================
    // ★ 画像切り替え
    // ===============================
    //private void ChangeBallImage(string tag)
    //{
    //    if (ballEffectImage == null) return;

    //    if (ballImageDict.TryGetValue(tag, out var sprite))
    //    {
    //        ballEffectImage.sprite = sprite;
    //        ballEffectImage.enabled = true;
    //    }
    //}
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

    private void ApplyBallEffect(BallType type)
    {
        if (!effectMap.TryGetValue(type, out var spec))
        {
            Debug.LogWarning($"未対応のタグ: {type}");
            return;
        }

        ChangeBallImage(type.ToString());  // ★ここで画像変更
        StartEffectByLayer(spec);
    }

    void Update()
    {
        bool locked = lockCount > 0;
        float inv = (reverseCount % 2 == 1) ? -1f : 1f;
        HandleJumpInput();
        //HandleSideMove();
        stepDistance = pTR.baseSp;
        if (!move)
            targetPos = transform.position;
        if (!locked)
        {
            if (Input.GetKey(KeyCode.D))
            {
                //rb.MovePosition(rb.position + inv * speed * transform.right * Time.deltaTime);
                transform.position += inv * speed * transform.right * Time.deltaTime;
                spriteRenderer.sprite = spriteRight; // 右画像
            }
            if (Input.GetKey(KeyCode.A))
            {
                //rb.MovePosition(rb.position + inv * speed * transform.right * Time.deltaTime);
                transform.position -= inv * speed * transform.right * Time.deltaTime;
                spriteRenderer.sprite = spriteLeft; // 左画像
            }
            //// --- ?????t ---
            //if (Input.GetKeyDown(KeyCode.D))  // ?E??
            //{
            //    move = true;
            //    targetPos += Vector3.right * stepDistance;
            //    //targetPos += new Vector3(stepDistance, 0f, 0f);
            //    spriteRenderer.sprite = spriteRight; // ?E??
            //}

            //if (Input.GetKeyDown(KeyCode.A))  // ????
            //{
            //    move = true;
            //    targetPos += Vector3.left * stepDistance;
            //    //targetPos += new Vector3(-stepDistance, 0f, 0f);
            //    spriteRenderer.sprite = spriteLeft; // ????
            //}
        }



        //---????????---
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
        Check();
    }

    void Check()
    {
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance < 0.05f)
        {
            move = false;
            moveSpeed = msFirst;
        }
    }

    void Die()
    {
        if (isDead) return;   // ???d????h?~
        isDead = true;

        Debug.Log("GAME OVER");

        // 
        //rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        SceneManager.LoadScene("ResultScene");
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


        Vector3 vel = rb.linearVelocity;
        //Vector3 vel = rb.linearVelocity;
        vel.y = 0;                   // 上方向の速度をリセット
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
    }

    //========================================
    //  ??n????i?n??????????W?????v?????Z?b?g?j
    //========================================

    // ?N??????x?[?X?X?P?[?????L???v?`??
    void Awake()
    {
        if (basket != null)
        {
            basketBaseScale = basket.localScale;
        }
    }

    // ?G?f?B?^??l???????????x?[?X???X?V?i?C??j
    void OnValidate()
    {
        if (basket != null)
        {
            basketBaseScale = basket.localScale;
        }
    }


    // ????????
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

    // ---------------- ?????`??????? ----------------
    private void BuildEffectDefinitions()
    {
        effectMap = new Dictionary<BallType, EffectSpec>();

        // ===== Move ???C???[ =====
        effectMap[BallType.SpeedUp] = new EffectSpec
        {
            // tag  から　case ball.BallTypeにしたい
            layer = EffectLayer.Speed,
            policy = EffectPolicy.Overwrite,
            duration = 5f,
            apply = () => { speed = baseSpeed + 2f; Debug.Log($"[SpeedUp] speed={speed}"); },
            cleanup = () => { speed = baseSpeed; Debug.Log($"[SpeedUp End] speed={speed}"); }
        };

        effectMap[BallType.SpeedDown] = new EffectSpec
        {
            layer = EffectLayer.Speed,
            policy = EffectPolicy.Overwrite,
            duration = 5f,
            apply = () => { speed = Mathf.Max(0f, baseSpeed - 2f); Debug.Log($"[SpeedDown] speed={speed}"); },
            cleanup = () => { speed = baseSpeed; Debug.Log($"[SpeedDown End] speed={speed}"); }
        };

        // ===== Jump ???C???[ =====
        effectMap[BallType.JumpUp] = new EffectSpec
        {
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { jump = baseJump + 2f; Debug.Log($"[JumpUp] jump={jump}"); },
            cleanup = () => { jump = baseJump; Debug.Log($"[JumpUp End] jump={jump}"); }
        };

        effectMap[BallType.JumpUp] = new EffectSpec
        {
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { jump = Mathf.Max(0f, baseJump - 2f); Debug.Log($"[JumpDown] jump={jump}"); },
            cleanup = () => { jump = baseJump; Debug.Log($"[JumpDown End] jump={jump}"); }
        };

        // ===== Control ???C???[ =====
        effectMap[BallType.Bom] = new EffectSpec
        {
            layer = EffectLayer.ControlLock,
            policy = EffectPolicy.Overwrite,
            duration = 2f,
            apply = () =>
            {
                // ▼ ステータス効果
                speed = Mathf.Max(0f, baseSpeed - 10f);
                jump = Mathf.Max(0f, baseJump - 7f);

                // ▼ ワールド演出（足元Bom）
                if (bomEffectPrefab != null)
                {
                    Instantiate(
                        bomEffectPrefab,
                        transform.position + bomOffset,
                        Quaternion.identity
                    );
                }

                Debug.Log("[Bom] 発動（UIはApplyBallEffect側）");
            },
            cleanup = () =>
            {
                speed = baseSpeed;
                jump = baseJump;
                Debug.Log("[Bom End]");
            }
        };




        // ===== Status ???C???[ =====
        effectMap[BallType.Invincible] = new EffectSpec
        {
            layer = EffectLayer.Status,
            policy = EffectPolicy.Overwrite,
            duration = Mathf.Infinity,
            apply = () =>
            {
                isInvincible = true;
                invincibleCharges = 2; // ?? 2???`???[?W
                Debug.Log($"[Invincible] ON (charges={invincibleCharges})");
            },
            cleanup = () =>
            {
                isInvincible = false;
                invincibleCharges = 0;
                Debug.Log("[Invincible] OFF");
            }
        };

        // ===== Basket ???C???[ =====
        effectMap[BallType.BigBasket] = new EffectSpec
        {
            layer = EffectLayer.Basket,
            policy = EffectPolicy.Overwrite,   // ?????????????????J?n?iRefresh/Extend???????????q?j
            duration = 3f,
            apply = () =>
            {
                if (basket == null)
                {
                    Debug.LogWarning("[BigBasket] basket ???????蓖????");
                    return;
                }

                // ???d?N?????F?O??????~???
                if (basketScaleRoutine != null)
                    StopCoroutine(basketScaleRoutine);

                // ??W?X?P?[???i?x?[?X?~?{???j
                Vector3 target = basketBaseScale * basketScaleMultiplier;

                // ????g??iTimeScale??e??????????????????3???? true?j
                basketScaleRoutine = StartCoroutine(TweenBasketScale(target, basketScaleTweenTime, false));
                Debug.Log("[BigBasket] ON");
            },
            cleanup = () =>
            {
                if (basket == null) return;

                if (basketScaleRoutine != null)
                    StopCoroutine(basketScaleRoutine);

                // ????T?C?Y????
                basketScaleRoutine = StartCoroutine(TweenBasketScale(basketBaseScale, basketScaleTweenTime, false));
                Debug.Log("[BigBasket] OFF");
            }
        };

        // ===== Control ???C???[ =====
        effectMap[BallType.Reverse] = new EffectSpec
        {
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
    }

    // ---------------- ???C???[???????????? ----------------
    private void StartEffectByLayer(EffectSpec spec)
    {
        if (spec.policy == EffectPolicy.Instant)
        {
            // ?????n?F?X???b?g???L?????A??????K?p????I???
            try { spec.apply?.Invoke(); }
            catch (Exception e) { Debug.LogWarning($"[Effect Instant] ??O: {e.Message}"); }
            return;
        }

        // Overwrite ?n?F???????C???[?????????????I?????V?K?J?n
        CancelLayer(spec.layer);

        // ?V?K????K?p
        try { spec.apply?.Invoke(); }
        catch (Exception e) { Debug.LogWarning($"[Effect Apply] ??O: {e.Message}"); }

        var handle = new EffectHandle
        {
            name = spec.layer.ToString(),
            cleanup = spec.cleanup,
            layer = spec.layer,
            endsAt = Time.time + Mathf.Max(0f, spec.duration)
        };

        handle.routine = StartCoroutine(RunEffectTimerByLayer(handle, spec.duration));
        activeByLayer[spec.layer] = handle;

        Debug.Log($"[Effect Start] {spec.layer.ToString()} layer={spec.layer} dur={spec.duration:F2}s");
    }

    private IEnumerator RunEffectTimerByLayer(EffectHandle handle, float duration)
    {
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        // ?????n???h???????????I??
        if (activeByLayer.TryGetValue(handle.layer, out var current) && current == handle)
        {
            try { handle.cleanup?.Invoke(); }
            catch (Exception e) { Debug.LogWarning($"[Effect Cleanup] ??O: {e.Message}"); }

            activeByLayer.Remove(handle.layer);
            Debug.Log($"[Effect End] {handle.name} layer={handle.layer}");
        }
    }

    private void CancelLayer(EffectLayer layer)
    {
        if (!activeByLayer.TryGetValue(layer, out var handle)) return;

        // ??n?? ?? ?R???[?`????~ ?? ??????
        try { handle.cleanup?.Invoke(); }
        catch (Exception e) { Debug.LogWarning($"[Effect Cancel Cleanup] ??O: {e.Message}"); }

        if (handle.routine != null)
            StopCoroutine(handle.routine);

        activeByLayer.Remove(layer);
        Debug.Log($"[Effect Cancel] {handle.name} layer={layer}");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

}
