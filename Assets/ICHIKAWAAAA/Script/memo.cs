using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("ステータス")]
    public float baseSpeed = 5f;
    public float speed = 5f;
    public float jump = 2f;

    private Rigidbody rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 全回転を固定

    }


    void Update()
    {
        // WASD

        // Wきー（まえ）
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += speed * transform.forward * Time.deltaTime;
        }

        // Sキー（後方移動）
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= speed * transform.forward * Time.deltaTime;
        }

        // Dキー（右移動）
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += speed * transform.right * Time.deltaTime;
        }

        // Aキー（左移動）
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= speed * transform.right * Time.deltaTime;
        }

        // Spaceキー（ジャンプ）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
            isGrounded = false; // 空中にいる間はジャンプ不可
        }

    }


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
                jump += 1f;
                Debug.Log("Jump Up!!!! 現在のジャンプ力:" + jump);
                break;

            case "JumpDown":
                jump -= 1f;
                Debug.Log("Jump Down😢 現在のジャンプ力:" + jump);
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
                //　操作不可
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

    private IEnumerator SpeedDown(float duration)
    {
        float originalSpeed = baseSpeed;       // 元の速度を保存
        speed = baseSpeed -2f;            // 一時的に上げるます
        Debug.Log("Speed Up! 現在のスピード: " + speed);

        yield return new WaitForSeconds(duration); // duration秒待つ

        speed = originalSpeed;             // 元に戻す
        Debug.Log("Speed 戻った: " + speed);

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
    }


}