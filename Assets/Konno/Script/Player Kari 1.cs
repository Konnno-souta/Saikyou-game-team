using System;
using UnityEngine;

public class PlayerKari1 : MonoBehaviour
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

    [Header("キャラ画像設定")]
    public Sprite spriteLeft;
    public Sprite spriteRight;
    private SpriteRenderer spriteRenderer;

    [Header("ジャンプ設定")]
    public float jumpForce = 7f;
    public int maxJumps = 2;
    private int jumpCount = 0;
    public float airControl = 0.4f;   // 空中横移動の効き具合

    private Rigidbody rb;
    private float currentSpeed;
    private float lastTapTime;
    private bool isGrounded = true;

    private float defaultSpeed;
    private float speedUpTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        defaultSpeed = baseSpeed;
        currentSpeed = baseSpeed;

        targetX = transform.position.x;
    }

    void Update()
    {
        HandleTapRun();
        HandleSideMove();
        HandleJumpInput();
        SmoothSideMove();
        UpdateSprite();
    }

    //========================================
    //  A / D の押し連打による速度アップ
    //========================================
    void HandleTapRun()
    {
        bool tapped = false;

        if (Input.GetKeyDown(KeyCode.A))
        {
            targetX -= laneWidth; tapped = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            targetX += laneWidth; tapped = true;
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
    // 横移動（ボタンで位置だけ更新）
    //========================================
    void HandleSideMove()
    {
        // 空中は移動弱く
        float smooth = isGrounded ? sideMoveSmooth : sideMoveSmooth * airControl;

        float newX = Mathf.Lerp(transform.position.x, targetX, smooth * Time.deltaTime);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    //========================================
    // ジャンプ入力
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

    void Jump()
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;    // ジャンプ2段目の挙動を安定
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
    }

    //========================================
    // 着地判定
    //========================================
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    //========================================
    // 画像切替
    //========================================
    void UpdateSprite()
    {
        if (Input.GetKey(KeyCode.A))
            spriteRenderer.sprite = spriteLeft;
        else if (Input.GetKey(KeyCode.D))
            spriteRenderer.sprite = spriteRight;
    }

    public void SpeedUp(float addSpeed, float duration)
    {
        baseSpeed = defaultSpeed + addSpeed;
        speedUpTimer = duration;
    }

    void SmoothSideMove() { /* 旧処理。今は HandleSideMove に統合済み */ }
}


