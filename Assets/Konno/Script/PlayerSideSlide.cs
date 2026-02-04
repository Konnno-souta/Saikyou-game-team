using System;
using System.Collections;
using System.Collections.Generic;
//using NUnit;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;


public class PlayerSideSlide : MonoBehaviour
{
    //[SerializeField] PlayerMovement pTR;
    //[SerializeField] FloorHitCheck fhc;

   // private float stepDistance;   // 1��̃T�C�h�X�e�b�v��
    //public float moveSpeed;      // �ړ����x�i�傫���قǑ������炩�ɓ��B�j
    private float msFirst;

    private Vector3 targetPos;


    [Header("ジャンプ")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("ステータス")]
    public float baseSpeed = 10f;
    public float speed = 5f;
    public float baseJump = 9f;
    public float jump = 8f;

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

    // Control 系の合成状態
    private int lockCount = 0;      // 1以上で操作不可
    private int reverseCount = 0;   // 奇数で反転、偶数で通常

    private bool isGrounded = false;

    //// ====== 無敵の状態管理 ======
    //private enum EffectLayer
    //{
    //    Speed,        // ���x�n�iSpeedUp/Down �Ȃǁj
    //    Jump,        // �W�����v�n�iJumpUp/Down �Ȃǁj
    //    ControlLock,     // ����s�iBom�j
    //    ControlReverse, // ���씽�]
    //    Status,      // ���G�Ȃ�
    //    Basket,      // �S�[���T�C�Y�Ȃ�
    //    Debafu,      // �X�R�A�A�^�C���̃}�C�i�X����
    //    // �ق��K�v�Ȃ�ǉ��c
    //}

    //// ���ʂ̓K�p�|���V�[
    //private enum EffectPolicy
    //{
    //    Overwrite,   // ���C���[�̃X���b�g���L�B����������Ώ㏑��
    //    Instant      // �������s��I���B�X���b�g���L���Ȃ�
    //}
    //public enum BallType
    //{
    //    SpeedUp,
    //    SpeedDown,
    //    JumpUp,
    //    JumpDown,
    //    Bom,
    //    Invincible,
    //    BigBasket,
    //    MinusScore,
    //    MinusTime,
    //    Reverse,
    //}

    //// ���ʃn���h���i�^�C�}�[�E��n���j
    //private class EffectHandle
    //{
    //    public Coroutine routine;
    //    public Action cleanup;
    //    public string name;
    //    public EffectLayer layer;
    //    public float endsAt; // �f�o�b�O�p�i�I���\�莞���j
    //}

    //// �A�N�e�B�u����
    //private readonly Dictionary<EffectLayer, EffectHandle> activeByLayer =
    //    new Dictionary<EffectLayer, EffectHandle>();

    //// ���ʒ�`
    //private class EffectSpec
    //{
    //    public EffectLayer layer;
    //    public EffectPolicy policy;
    //    public float duration;
    //    public Action apply;
    //    public Action cleanup;
    //}

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
        // 壁
        if (other.CompareTag("Wall"))
        {
            Die();
            return;
        }

    //    // ボール取得
    //    if (ballImageDict.ContainsKey(other.tag))
    //    {
    //        Debug.Log("ボール取得 : " + other.tag);

    //        ChangeBallImage(other.tag);

    //        Destroy(other.gameObject); // ボール消す
    //    }
    }

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

        //BuildEffectDefinitions();
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

    //private void ApplyBallEffect(BallType type)
    //{
    //    if (!effectMap.TryGetValue(type, out var spec))
    //    {
    //        Debug.LogWarning($"未対応のタグ: {type}");
    //        return;
    //    }

    //    ChangeBallImage(type.ToString());  // ★ここで画像変更
    //    //StartEffectByLayer(spec);
    //}
   
    void Update()
    {
        bool locked = lockCount > 0;
        float inv = (reverseCount % 2 == 1) ? -1f : 1f;
        HandleJumpInput();
        //HandleSideMove();
        //stepDistance = pTR.baseSp;
        //if(!move)
        //    targetPos = transform.position;
        Vector3 move = Vector3.zero;

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
        }
        //rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        //transform.position = Vector3.MoveTowards(
        //    transform.position,
        //    targetPos,
        //    moveSpeed * Time.deltaTime
        //);
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

}
