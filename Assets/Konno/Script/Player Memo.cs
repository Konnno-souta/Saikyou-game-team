using UnityEngine;
using System.Collections;

public class Playermemo : MonoBehaviour
{
    [SerializeField] PlayerMovement pTR;
    [SerializeField] FloorHitCheck fhc;

    private float stepDistance;
    public float moveSpeed = 5f;
    private float msFirst;

    private Vector3 targetPos;

    [Header("ジャンプ設定")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("ステータス")]
    public float baseSpeed = 5f;   // 初期横移動速度
    public float baseJump = 2f;    // 初期ジャンプ力

    private Rigidbody rb;
    bool move;

    public bool Move { get { return move; } }

    void Start()
    {
        stepDistance = pTR.baseSp;
        targetPos = transform.position;

        moveSpeed = baseSpeed;
        msFirst = baseSpeed;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleJumpInput();

        stepDistance = pTR.baseSp;

        if (!move)
            targetPos = transform.position;

        if (Input.GetKeyDown(KeyCode.D))
        {
            move = true;
            targetPos += new Vector3(stepDistance, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            move = true;
            targetPos += new Vector3(-stepDistance, 0f, 0f);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        Check();
    }

    void Check()
    {
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            move = false;
        }
    }

    //================================
    // ジャンプ
    //================================
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
        vel.y = 0;
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * baseJump, ForceMode.Impulse);
    }

    //================================
    // アイテム効果
    //================================
    void ApplyBallEffect(string tagName)
    {
        switch (tagName)
        {
            case "SpeedUp":
                StartCoroutine(SpeedUp(3f));
                break;

            case "SpeedDown":
                StartCoroutine(SpeedDown(3f));
                break;

            case "JumpUp":
                StartCoroutine(JumpUp(3f));
                break;

            case "JumpDown":
                StartCoroutine(JumpDown(3f));
                break;

            case "Bom":
                StartCoroutine(Bom(2f));
                break;
        }
    }

    IEnumerator SpeedUp(float duration)
    {
        moveSpeed = baseSpeed + 2f;
        yield return new WaitForSeconds(duration);
        moveSpeed = baseSpeed;
    }

    IEnumerator SpeedDown(float duration)
    {
        moveSpeed = baseSpeed - 2f;
        yield return new WaitForSeconds(duration);
        moveSpeed = baseSpeed;
    }

    IEnumerator JumpUp(float duration)
    {
        baseJump += 2f;
        yield return new WaitForSeconds(duration);
        baseJump -= 2f;
    }

    IEnumerator JumpDown(float duration)
    {
        baseJump -= 2f;
        yield return new WaitForSeconds(duration);
        baseJump += 2f;
    }

    // ボム：完全に行動不能
    IEnumerator Bom(float duration)
    {
        float originalSpeed = baseSpeed;
        float originalJump = baseJump;

        moveSpeed = 0f;
        baseJump = 0f;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;
        baseJump = originalJump;
    }

    //================================
    // 当たり判定
    //================================
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            jumpCount = 0;

        if (collision.gameObject.CompareTag("SpeedUp") ||
            collision.gameObject.CompareTag("SpeedDown") ||
            collision.gameObject.CompareTag("JumpUp") ||
            collision.gameObject.CompareTag("JumpDown") ||
            collision.gameObject.CompareTag("Bom"))
        {
            ApplyBallEffect(collision.gameObject.tag);
            Destroy(collision.gameObject);
        }
    }
}
