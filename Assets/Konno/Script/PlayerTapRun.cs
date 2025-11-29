using System;
using UnityEngine;

public class PlayerTapRun : MonoBehaviour
{
    [Header("連打走行設定")]
    public float baseSpeed = 2f;
    public float maxSpeed = 5f;
    public float tapBoost = 2f;
    public float speedDecay = 3f;
    public float tapTimeLimit = 0.3f;

    [Header("横移動設定")]
    public float laneWidth = 1f;
    public float sideMoveSmooth = 10f;
    private float targetX;

    //[Header("ジャンプ設定")]
    //public float jumpForce = 5f;
    //public int maxJumps = 2;
    //private int jumpCount = 0;


    [Header("キャラ画像設定")]
    public Sprite spriteLeft;
    public Sprite spriteRight;
    private SpriteRenderer spriteRenderer;


    private Rigidbody rb;
    private float currentSpeed;
    private float lastTapTime;


    private float defaultSpeed;
    private float speedUpTimer = 0f;

    public float baseSp { get { return baseSpeed; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer取得

        defaultSpeed = baseSpeed;
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        HandleTapRun();
        //HandleSideInput();
        //HandleJumpInput();
        //SmoothSideMove();
        //float h = Input.GetAxis("Horizontal");
        //Vector3 move = new Vector3(h, 0);
        //transform.Translate(move * baseSpeed * Time.deltaTime);

        // 画像切り替え処理
        if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.sprite = spriteLeft; // 左画像
        }
        else if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.sprite = spriteRight; // 右画像
        }


        if (speedUpTimer > 0)
        {
            speedUpTimer -= Time.deltaTime;

            if (speedUpTimer <= 0)
            {
                baseSpeed = defaultSpeed;
            }

        }
    }

    ////========================================
    ////  A / D 連打で加速
    ////========================================
    void HandleTapRun()
    {
        bool tapped = false;

        if (Input.GetKeyDown(KeyCode.A))
        {
            targetX = transform.position.x;
            targetX -= laneWidth;
            tapped = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            targetX = transform.position.x;
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

        //    // 自然減速
        currentSpeed -= speedDecay * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);
    }

    public void SpeedUp(float addSpeed, float duration)
    {
        baseSpeed = defaultSpeed + addSpeed;
        speedUpTimer = duration;
    }
    ////========================================
    ////     滑らかな横移動
    ////========================================
    //void SmoothSideMove()
    //{
    //    Vector3 pos = transform.position;
    //    pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * sideMoveSmooth);
    //    transform.position = pos;
    //}

    ////========================================
    ////          ジャンプ入力
    ////========================================
    //void HandleJumpInput()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (jumpCount < maxJumps)
    //        {
    //            Jump();
    //            jumpCount++;
    //        }
    //    }
    //}

    //// ジャンプ処理
    //void Jump()
    //{
    //    Vector3 vel = rb.linearVelocity;
    //    vel.y = 0;                  // 2段目で加算が暴れないように
    //    rb.linearVelocity = vel;

    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //}

    ////========================================
    ////  接地判定（地面に着いたらジャンプ回数リセット）
    ////========================================
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //        jumpCount = 0;
    //}

    ////========================================
    //// A / D 押された瞬間だけ処理（連打判定用）
    ////========================================
    //void HandleSideInput() { /* ここは使わないが残してある */ }


}





