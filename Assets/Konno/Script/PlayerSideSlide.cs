using UnityEngine;
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
    public float jumpForce = 5f;
    public int maxJumps = 2;
    private int jumpCount = 0;
    public float airControl = 0.4f;   // 空中横移動の効き具合

    private Rigidbody rb;
    bool move;
    private float sideMoveSmooth;
    private float targetX;
    private bool isGrounded;

    public bool Move{ get { return move; } }

    //[Header("キャラ画像設定")]
    //public Sprite spriteLeft;
    //public Sprite spriteRight;
    //private SpriteRenderer spriteRenderer;

    //public float baseSp { get { return baseSpeed; } }

    void Start()
    {
        stepDistance = pTR.baseSp;
        targetPos = transform.position;
        msFirst = moveSpeed;

        rb = GetComponent<Rigidbody>();
        targetX = transform.position.x;
    }

    void Update()
    {
        HandleJumpInput();
        HandleSideMove();
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

        //// 画像切り替え処理
        //if (Input.GetKey(KeyCode.A))
        //{
        //    spriteRenderer.sprite = spriteLeft; // 左画像
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    spriteRenderer.sprite = spriteRight; // 右画像
        //}

        //if (speedUpTimer > 0)
        //{
        //    speedUpTimer -= Time.deltaTime;

        //    if (speedUpTimer <= 0)
        //    {
        //        baseSpeed = defaultSpeed;
        //    }
        //}
    }

    void Check()
    {
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance < 0.05f)
        {
            Debug.Log("False");
            move = false;
            moveSpeed = msFirst;
        }
    }

    void HandleSideMove()
    {
        // 空中は移動弱く
        float smooth = isGrounded ? sideMoveSmooth : sideMoveSmooth * airControl;

        float newX = Mathf.Lerp(transform.position.x, targetX, smooth * Time.deltaTime);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
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
    //void Jump()
    //{
    //    Vector3 vel = rb.linearVelocity;
    //    vel.y = 0;                  // 2段目で加算が暴れないように
    //    rb.linearVelocity = vel;

    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //}
    void Jump() // 実験
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;    // ジャンプ2段目の挙動を安定
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    //========================================
    //  接地判定（地面に着いたらジャンプ回数リセット）
    //========================================
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
            jumpCount = 0;
    }

}
