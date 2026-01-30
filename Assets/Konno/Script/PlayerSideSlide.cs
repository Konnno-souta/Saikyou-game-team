using System;
using System.Collections;
using System.Collections.Generic;
using NUnit;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;


public class PlayerSideSlide : MonoBehaviour
{
    [SerializeField] PlayerMovement pTR;
    [SerializeField] FloorHitCheck fhc;

    private float stepDistance;   // 1��̃T�C�h�X�e�b�v��
    public float moveSpeed;      // �ړ����x�i�傫���قǑ������炩�ɓ��B�j
    private float msFirst;

    private Vector3 targetPos;


    [Header("ジャンプ")]
    public int maxJumps = 2;
    private int jumpCount = 0;
    //private int jumpCount = 0;


    [Header("ステータス")]
    public float baseSpeed = 10f;
    public float speed = 5f;
    public float baseJump = 10f;
    public float jump = 6f;
    //public float airControl = 0.4f;   // �󒆉��ړ��̌����

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
    public bool Move{ get { return move; } }



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
    //    spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer�擾
    //    rb.freezeRotation = true; // �S��]���Œ�
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
        Speed,        // ���x�n�iSpeedUp/Down �Ȃǁj
        Jump,        // �W�����v�n�iJumpUp/Down �Ȃǁj
        ControlLock,     // ����s�iBom�j
        ControlReverse, // ���씽�]
        Status,      // ���G�Ȃ�
        Basket,      // �S�[���T�C�Y�Ȃ�
        Debafu,      // �X�R�A�A�^�C���̃}�C�i�X����
        // �ق��K�v�Ȃ�ǉ��c
    }

    // ���ʂ̓K�p�|���V�[
    private enum EffectPolicy
    {
        Overwrite,   // ���C���[�̃X���b�g���L�B����������Ώ㏑��
        Instant      // �������s��I���B�X���b�g���L���Ȃ�
    }

    // ���ʃn���h���i�^�C�}�[�E��n���j
    private class EffectHandle
    {
        public Coroutine routine;
        public Action cleanup;
        public string name;
        public EffectLayer layer;
        public float endsAt; // �f�o�b�O�p�i�I���\�莞���j
    }

    // �A�N�e�B�u����
    private readonly Dictionary<EffectLayer, EffectHandle> activeByLayer =
        new Dictionary<EffectLayer, EffectHandle>();

    // ���ʒ�`
    private class EffectSpec
    {
        public string tag;
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


    // �f�o�t�Ƃ݂Ȃ��^�O
    private static readonly HashSet<string> DebuffTags = new HashSet<string>(StringComparer.Ordinal)
    {
      "SpeedDown", "JumpDown", "Bom", "-Score", "-Time","Reverse"
    };


    // ���G�̏�ԊǗ�
    private int invincibleCharges = 0;
    private bool isInvincible = false;

    // Control �n�̍������
    private int lockCount = 0;      // 1�ȏ�ő���s��
    private int reverseCount = 0;   // ��Ŕ��]�A�����Œʏ�

    // �^�O����` �̎���
    private Dictionary<string, EffectSpec> effectMap;


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
    private void ChangeBallImage(string tag)
    {
        if (ballEffectImage == null) return;

        if (ballImageDict.TryGetValue(tag, out var sprite))
        {
            ballEffectImage.sprite = sprite;
            ballEffectImage.enabled = true;
        }
    }
    private void ApplyBallEffect(string tagName)
    {
        if (!effectMap.TryGetValue(tagName, out var spec))
        {
            Debug.LogWarning($"未対応のタグ: {tagName}");
            return;
        }

        ChangeBallImage(tagName);   // ★ここで画像変更
        StartEffectByLayer(spec);
    }
   
    void Update()
    {
        bool locked = lockCount > 0;
        float inv = (reverseCount % 2 == 1) ? -1f : 1f;
        HandleJumpInput();
        //HandleSideMove();
        stepDistance = pTR.baseSp;
        if(!move)
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
            //// --- ���͎�t ---
            //if (Input.GetKeyDown(KeyCode.D))  // �E��
            //{
            //    move = true;
            //    targetPos += Vector3.right * stepDistance;
            //    //targetPos += new Vector3(stepDistance, 0f, 0f);
            //    spriteRenderer.sprite = spriteRight; // �E�摜
            //}

            //if (Input.GetKeyDown(KeyCode.A))  // ����
            //{
            //    move = true;
            //    targetPos += Vector3.left * stepDistance;
            //    //targetPos += new Vector3(-stepDistance, 0f, 0f);
            //    spriteRenderer.sprite = spriteLeft; // ���摜
            //}
        }



        //---���炩�Ɉړ�---
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
        if (isDead) return;   // ���d����h�~
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
    //  �ڒn����i�n�ʂɒ�������W�����v�񐔃��Z�b�g�j
    //========================================

    // �N�����Ƀx�[�X�X�P�[�����L���v�`��
    void Awake()
    {
        if (basket != null)
        {
            basketBaseScale = basket.localScale;
        }
    }

    // �G�f�B�^�Œl��ς��������x�[�X���X�V�i�C�Ӂj
    void OnValidate()
    {
        if (basket != null)
        {
            basketBaseScale = basket.localScale;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Die();
        }
    }

    // �����̂��
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

    // ---------------- ���ʒ�`���܂Ƃ߂� ----------------
    private void BuildEffectDefinitions()
    {
        effectMap = new Dictionary<string, EffectSpec>(StringComparer.Ordinal);

        // ===== Move ���C���[ =====
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

        // ===== Jump ���C���[ =====
        effectMap["JumpUp"] = new EffectSpec
        {
            tag = "JumpUp",
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { jump = baseJump + 2f; Debug.Log($"[JumpUp] jump={jump}"); },
            cleanup = () => { jump = baseJump; Debug.Log($"[JumpUp End] jump={jump}"); }
        };

        effectMap["JumpDown"] = new EffectSpec
        {
            tag = "JumpDown",
            layer = EffectLayer.Jump,
            policy = EffectPolicy.Overwrite,
            duration = 3f,
            apply = () => { jump = Mathf.Max(0f, baseJump - 2f); Debug.Log($"[JumpDown] jump={jump}"); },
            cleanup = () => { jump = baseJump; Debug.Log($"[JumpDown End] jump={jump}"); }
        };

        // ===== Control ���C���[ =====
        effectMap["Bom"] = new EffectSpec
        {
            tag = "Bom",
            layer = EffectLayer.ControlLock,
            policy = EffectPolicy.Overwrite,
            duration = 2f,
            apply = () =>
            {
                speed = Mathf.Max(0f, baseSpeed - 10f);
                jump = Mathf.Max(0f, baseJump - 7f);

                // ▼ プレイヤー位置にBom出現
                if (bomEffectPrefab != null)
                {
                    Instantiate(
                        bomEffectPrefab,
                        transform.position + bomOffset,
                        Quaternion.identity
                    );
                }

                Debug.Log("[Bom] 発動");
            },
            cleanup = () =>
            {
                speed = baseSpeed;
                jump = baseJump;
                Debug.Log("[Bom End]");
            }
        };



        // ===== Status ���C���[ =====
        effectMap["Invincible"] = new EffectSpec
        {
            tag = "Invincible",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Overwrite,
            duration = Mathf.Infinity,
            apply = () =>
            {
                isInvincible = true;
                invincibleCharges = 2; // �� 2�񕪃`���[�W
                Debug.Log($"[Invincible] ON (charges={invincibleCharges})");
            },
            cleanup = () =>
            {
                isInvincible = false;
                invincibleCharges = 0;
                Debug.Log("[Invincible] OFF");
            }
        };

        // ===== Basket ���C���[ =====
        effectMap["BigBasket"] = new EffectSpec
        {
            tag = "BigBasket",
            layer = EffectLayer.Basket,
            policy = EffectPolicy.Overwrite,   // �����ʂ�������㏑���J�n�iRefresh/Extend�ɂ������Ȃ��q�j
            duration = 3f,
            apply = () =>
            {
                if (basket == null)
                {
                    Debug.LogWarning("[BigBasket] basket �������蓖�Ăł�");
                    return;
                }

                // ���d�N���΍�F�O�̕�Ԃ��~�߂�
                if (basketScaleRoutine != null)
                    StopCoroutine(basketScaleRoutine);

                // �ڕW�X�P�[���i�x�[�X�~�{���j
                Vector3 target = basketBaseScale * basketScaleMultiplier;

                // ��ԂŊg��iTimeScale�̉e�����󂯂����Ȃ��ꍇ�͑�3���� true�j
                basketScaleRoutine = StartCoroutine(TweenBasketScale(target, basketScaleTweenTime, false));
                Debug.Log("[BigBasket] ON");
            },
            cleanup = () =>
            {
                if (basket == null) return;

                if (basketScaleRoutine != null)
                    StopCoroutine(basketScaleRoutine);

                // ���̃T�C�Y�֖߂�
                basketScaleRoutine = StartCoroutine(TweenBasketScale(basketBaseScale, basketScaleTweenTime, false));
                Debug.Log("[BigBasket] OFF");
            }
        };


        // ===== Instant�i�X���b�g���L�j =====
        effectMap["-Score"] = new EffectSpec
        {
            tag = "-Score",
            layer = EffectLayer.Debafu,  // �ǂ��ł�OK�i�Q�Ƃ��Ȃ��j
            policy = EffectPolicy.Instant,
            duration = 0f,
            apply = () => { /* �X�R�A���Z */ Debug.Log("[-Score] ��������"); },
            cleanup = null
        };

        effectMap["-Time"] = new EffectSpec
        {
            tag = "-Time",
            layer = EffectLayer.Debafu,  // �ǂ��ł�OK�i�Q�Ƃ��Ȃ��j
            policy = EffectPolicy.Instant,
            duration = 0f,
            apply = () => { /* �^�C������ */ Debug.Log("[-Time] ��������"); },
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

        // ===== Control ���C���[ =====
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


        effectMap["���������܂��Ȃ�"] = new EffectSpec
        {
            tag = "???",
            layer = EffectLayer.Status,
            policy = EffectPolicy.Instant,
            duration = 3f,
            apply = () => { Debug.Log("???"); },
            cleanup = null
        };
    }

    // ---------------- ���C���[���Ƃ̏㏑������ ----------------
    private void StartEffectByLayer(EffectSpec spec)
    {
        if (spec.policy == EffectPolicy.Instant)
        {
            // �����n�F�X���b�g���L�����A���̏�œK�p���ďI���
            try { spec.apply?.Invoke(); }
            catch (Exception e) { Debug.LogWarning($"[Effect Instant] ��O: {e.Message}"); }
            return;
        }

        // Overwrite �n�F�������C���[���̊������ʂ��I�����V�K�J�n
        CancelLayer(spec.layer);

        // �V�K���ʓK�p
        try { spec.apply?.Invoke(); }
        catch (Exception e) { Debug.LogWarning($"[Effect Apply] ��O: {e.Message}"); }

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

        // �����n���h���������Ȃ�I��
        if (activeByLayer.TryGetValue(handle.layer, out var current) && current == handle)
        {
            try { handle.cleanup?.Invoke(); }
            catch (Exception e) { Debug.LogWarning($"[Effect Cleanup] ��O: {e.Message}"); }

            activeByLayer.Remove(handle.layer);
            Debug.Log($"[Effect End] {handle.name} layer={handle.layer}");
        }
    }

    private void CancelLayer(EffectLayer layer)
    {
        if (!activeByLayer.TryGetValue(layer, out var handle)) return;

        // ��n�� �� �R���[�`����~ �� ������
        try { handle.cleanup?.Invoke(); }
        catch (Exception e) { Debug.LogWarning($"[Effect Cancel Cleanup] ��O: {e.Message}"); }

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
