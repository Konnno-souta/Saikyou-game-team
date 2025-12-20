using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class PlayerSideSlide : MonoBehaviour
{
    [SerializeField] PlayerMovement pTR;
    [SerializeField] FloorHitCheck fhc;

    private float stepDistance;   // 1回のサイドステップ量
    public float moveSpeed;      // 移動速度（大きいほど早く滑らかに到達）
    private float msFirst;

    private Vector3 targetPos;


    [Header("ジャンプ設定")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("ステータス")]
    public float baseSpeed = 5f;
    public float speed = 5f;
    public float baseJump = 2f;
    public float jump = 2f;
    //public float airControl = 0.4f;   // 空中横移動の効き具合

    private Rigidbody rb;
    bool move;
    private float sideMoveSmooth;

    public bool Move{ get { return move; } }

    //public float baseSp { get { return baseSpeed; } }

    void Start()
    {
        stepDistance = pTR.baseSp;
        targetPos = transform.position;
        msFirst = moveSpeed;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 全回転を固定
    }

    void Update()
    {
        HandleJumpInput();
        //HandleSideMove();
        stepDistance = pTR.baseSp;
        if(!move)
            targetPos = transform.position;
        // --- 入力受付 ---
        if (Input.GetKeyDown(KeyCode.D))  // 右へ
        {
            move = true;
            targetPos += new Vector3(stepDistance, 0f, 0f);

        }

        if (Input.GetKeyDown(KeyCode.A))  // 左へ
        {
            move = true;
            targetPos += new Vector3(-stepDistance, 0f, 0f);
        }

        //---滑らかに移動---
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

    

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount < maxJumps)
            {
                Jump();
                jumpCount++;
            }
        }
    }

    // ジャンプ処理 本体
    void Jump()
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;                  // 2段目で加算が暴れないように
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * baseJump, ForceMode.Impulse);
    }

    //========================================
    //  接地判定（地面に着いたらジャンプ回数リセット）
    //========================================
    
    private void ApplyBallEffect(string tagName)
    {
        switch (tagName)
        {
            case "SpeedUp":
                StartCoroutine(SpeedUp(3f)); // スピード上がる
                break;

            case "SpeedDown":
                StartCoroutine(SpeedDown(3f)); // スピードサがる
                break;

            case "JumpUp":
                StartCoroutine(JumpUp(3f)); //　ジャンプ力アップ
                break;

            case "JumpDown":
                StartCoroutine(JumpDown(3f)); //　ジャンプ力アップ
                break;


            case "Invincible":
                // 攻撃防ぐ（デバフから）
                break;

            case "BigBasket":
                //　カゴ大きくなる
                break;

            case "MinusScore":
                //　スコアダウン
                break;

            case "MinusTime":
                //　タイム減る
                break;

            case "Bom":
                StartCoroutine(Bom(2f));//　操作不可
                break;

            default:
                Debug.LogWarning("未対応のタグ: " + tagName);
                break;

        }
    }
    private IEnumerator SpeedUp(float duration)
    {
        float originalSpeed = baseSpeed;       // 元の速度を保存
        speed = baseSpeed + 2f;            // 一時的に上げるます
        Debug.Log("Speed Up! 現在のスピード: " + speed);

        yield return new WaitForSeconds(duration); // duration秒待つ

        speed = originalSpeed;             // 元に戻す
        Debug.Log("Speed 戻った: " + speed);

    }
    //　SpeedDwonのやつ
    private IEnumerator SpeedDown(float duration)
    {
        float originalSpeed = baseSpeed;       // 元の速度を保存
        speed = baseSpeed - 2f;            // 一時的にさげるます
        Debug.Log("Speed Down! 現在のスピード: " + speed);

        yield return new WaitForSeconds(duration); // duration秒待つ

        speed = originalSpeed;             // 元に戻す
        Debug.Log("Speed 戻った: " + speed);

    }
    private IEnumerator JumpUp(float duration)
    {
        float originalJump = baseJump;       // 元の速度を保存
        jump = baseJump + 2f;            // 一時的に上げるます
        Debug.Log("Jump Up! 現在のジャンプ力: " + jump);

        yield return new WaitForSeconds(duration); // duration秒待つ

        jump = originalJump;             // 元に戻す
        Debug.Log("Jump 戻った: " + jump);

    }

    private IEnumerator JumpDown(float duration)
    {
        float originalJump = baseJump;  // 元の速度を保存
        jump = baseJump - 2f;            // 一時的にさげるます
        Debug.Log("Jump Down! 現在のジャンプ力: " + jump);

        yield return new WaitForSeconds(duration); // duration秒待つ

        jump = originalJump;             // 元に戻す
        Debug.Log("Jump 戻った: " + jump);

    }

    private IEnumerator Bom(float duration)
    {
        float originalSpeed = baseSpeed;       // 元の速度を保存
        float originalJump = baseJump;
        speed = baseSpeed - 5f;            // 一時的にさげるます
        jump = baseJump - 2f;
        Debug.Log("動けない");

        yield return new WaitForSeconds(duration); // duration秒待つ

        speed = originalSpeed;             // 元に戻す
        jump = originalJump;
        Debug.Log("動ける " + speed);
        Debug.Log("動ける " + jump);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("衝突検知: " + collision.gameObject.name + " / Tag: " + collision.gameObject.tag);



        // ボールにぶつかったら
        if (collision.gameObject.CompareTag("SpeedUp") ||
            collision.gameObject.CompareTag("SpeedDown") ||
            collision.gameObject.CompareTag("JumpUp") ||
            collision.gameObject.CompareTag("JumpDown") ||
            collision.gameObject.CompareTag("Bom") ||
            collision.gameObject.CompareTag("Invincible") ||
            collision.gameObject.CompareTag("BigBasket") ||
            collision.gameObject.CompareTag("MinusScore") ||
            collision.gameObject.CompareTag("MinusTime"))

        {
            // プレイヤーに効果を適用
            ApplyBallEffect(collision.gameObject.tag);

            // ボールを削除
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
            //isGrounded = true;
            jumpCount = 0;
    }
}
