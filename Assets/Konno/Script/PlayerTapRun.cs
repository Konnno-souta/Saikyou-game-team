using UnityEngine;

public class PlayerTapRun : MonoBehaviour
{
    [Header("連打走行設定")]
    public float baseSpeed = 2f;
    public float maxSpeed = 12f;
    public float tapBoost = 2f;
    public float speedDecay = 3f;
    public float tapTimeLimit = 0.3f;

    [Header("横移動設定")]
    public float laneWidth = 1f;
    public float sideMoveSmooth = 10f;
    private float targetX;

    [Header("ジャンプ設定")]
    public float jumpForce = 5f;
    public int maxJumps = 2;
    private int jumpCount = 0;

    private Rigidbody rb;
    private float currentSpeed;
    private float lastTapTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetX = transform.position.x;
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        HandleTapRun();
        HandleSideInput();
        HandleJumpInput();
        SmoothSideMove();
    }

    //========================================
    //  A / D 連打で加速
    //========================================
    void HandleTapRun()
    {
        bool tapped = false;

        if (Input.GetKeyDown(KeyCode.A))
        {
            targetX -= laneWidth;
            tapped = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            targetX += laneWidth;
            tapped = true;
        }

        if (tapped)
        {
            float diff = Time.time - lastTapTime;
            if (diff <= tapTimeLimit)
                currentSpeed += tapBoost;
            else
                currentSpeed += tapBoost * 0.5f;

            lastTapTime = Time.time;
        }

        // 自然減速
        currentSpeed -= speedDecay * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);
    }

    //========================================
    //     滑らかな横移動
    //========================================
    void SmoothSideMove()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * sideMoveSmooth);
        transform.position = pos;
    }

    //========================================
    //          ジャンプ入力
    //========================================
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

    //========================================
    // A / D 押された瞬間だけ処理（連打判定用）
    //========================================
    void HandleSideInput() { /* ここは使わないが残してある */ }
}





