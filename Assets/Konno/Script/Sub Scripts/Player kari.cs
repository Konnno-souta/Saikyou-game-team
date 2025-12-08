using UnityEngine;

public class Playerkari : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float defaultSpeed;
    private float speedUpTimer = 0f;

    

    public void AddSpeed(float amount)
    {
        moveSpeed += amount;
    }
    //[Header("ステップ移動")]
   // public float stepDistance = 1f;        // 左右への1マス移動距離
    //public float stepCooldown = 0.15f;     // 連続移動防止クールタイム

    [Header("ジャンプ設定")]
    public float jumpForce = 7f;           // ジャンプ力
    public int maxJumpCount = 2;           // 2段ジャンプ
    private int jumpCount = 0;             // 現在のジャンプ回数

    private float lastStepTime = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody が必要です！");
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(h, 0);
        transform.Translate(move * moveSpeed * Time.deltaTime);

        if (speedUpTimer > 0)
        {
            speedUpTimer -= Time.deltaTime;

            if (speedUpTimer <= 0)
            {
                moveSpeed = defaultSpeed;
            }

        }
        //{
        //    HandleStepMove();
        //    HandleJump();
        //}

        // ===============================
        //  左右のステップ移動（マス移動）
        // ===============================
        //void HandleStepMove()
        //{
        //    if (Time.time - lastStepTime < stepCooldown) return;

        //    if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        transform.position += Vector3.left * stepDistance;
        //        lastStepTime = Time.time;
        //    }
        //    if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        transform.position += Vector3.right * stepDistance;
        //        lastStepTime = Time.time;
        //    }
        //}

        // ===============================
        //            ジャンプ
        // ===============================
        void HandleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (jumpCount < maxJumpCount)
                {
                    Vector3 velocity = rb.linearVelocity;
                    velocity.y = 0;
                    rb.linearVelocity = velocity;             // 落下中の速さをリセットしてクセなくする

                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    jumpCount++;
                }
            }
        }
    }
    public void SpeedUp(float addSpeed, float duration)
    {
        moveSpeed = defaultSpeed + addSpeed;
        speedUpTimer = duration;
    }

    // ===============================
    //   地面に触れたらジャンプ回数リセット
    // ===============================
    private void OnCollisionEnter(Collision collision)
    {
        // 地面タグを Ground として扱う例
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }
}



