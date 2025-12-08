using UnityEngine;

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
    private Rigidbody rb;
    bool move;
    public bool Move{ get { return move; } }

    [Header("キャラ画像設定")]
    public Sprite spriteLeft;
    public Sprite spriteRight;
    private SpriteRenderer spriteRenderer;

    //public float baseSp { get { return baseSpeed; } }

    void Start()
    {
        stepDistance = pTR.baseSp;
        targetPos = transform.position;
        msFirst = moveSpeed;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleJumpInput();
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



        // //---滑らかに移動---
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
        Check();

        // 画像切り替え処理
        if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.sprite = spriteLeft; // 左画像
        }
        else if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.sprite = spriteRight; // 右画像
        }

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

    // ジャンプ処理
    void Jump()
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;                  // 2段目で加算が暴れないように
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    //========================================
    //  接地判定（地面に着いたらジャンプ回数リセット）
    //========================================
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            jumpCount = 0;
    }

}
