
using System.Collections;
using UnityEngine;
using System; // Action を使うため

public class Player : MonoBehaviour
{
    [Header("ステータス")]
    public float baseSpeed = 5f;
    public float speed = 5f;
    public float baseJump = 2f;
    public float Jump = 2f;

    private Rigidbody rb;
    private bool isGrounded = false;

    // ========= 単一効果を上書き管理するためのハンドル =========
    private class EffectHandle
    {
        public Coroutine routine;   // 走っているコルーチン
        public Action cleanup;      // 値を元に戻す処理
        public string name;         // デバッグ用
    }
    private EffectHandle activeEffect; // 現在の効果（常に1つ）

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 全回転を固定

        // 現在値を基礎値で初期化（念のため）
        speed = baseSpeed;
        Jump = baseJump;
    }

    void Update()
    {
        // WASD

        // Wキー（前）
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += speed * transform.forward * Time.deltaTime;
        }

        // Sキー（後）
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= speed * transform.forward * Time.deltaTime;
        }

        // Dキー（右）
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += speed * transform.right * Time.deltaTime;
        }

        // Aキー（左）
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= speed * transform.right * Time.deltaTime;
        }

        // Spaceキー（ジャンプ）※ ← ここを Jump（現在値）で加算するよう修正
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * Jump, ForceMode.Impulse);
            isGrounded = false; // 空中にいる間はジャンプ不可
        }
    }

    // ==================== 効果の上書き管理（共通ロジック） ====================

    // いま走っている効果を「元に戻してから」停止
    private void CancelCurrentEffect()
    {
        if (activeEffect == null) return;

        // ① 先に元に戻す（StopCoroutine では後処理が走らないため）
        try
        {
            activeEffect.cleanup?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Effect] cleanup 例外: {e.Message}");
        }

        // ② タイマー停止
        if (activeEffect.routine != null)
        {
            StopCoroutine(activeEffect.routine);
        }

        Debug.Log($"[Effect] キャンセル: {activeEffect.name}");
        activeEffect = null;
    }

    // 効果開始：既存効果をキャンセルしてから新効果を適用し、タイマーを回す
    private void StartEffect_Overwrite(string effectName, Action apply, Action cleanup, float duration)
    {
        // 既存効果を確実に戻してから止める
        CancelCurrentEffect();

        // 新しい効果の適用
        apply?.Invoke();

        // タイマー開始（終了時に必ず cleanup を呼ぶ）
        var handle = new EffectHandle { name = effectName, cleanup = cleanup };
        handle.routine = StartCoroutine(RunEffectTimer(handle, duration));
        activeEffect = handle;

        Debug.Log($"[Effect] 開始: {effectName}（{duration:F1}s）");
    }

    private IEnumerator RunEffectTimer(EffectHandle handle, float duration)
    {
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }

        // 自分がまだ現役なら終了処理（上書きされていたら activeEffect は別のものになっている）
        if (activeEffect == handle)
        {
            try
            {
                handle.cleanup?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Effect] cleanup 例外: {e.Message}");
            }

            Debug.Log($"[Effect] 終了: {handle.name}");
            activeEffect = null;
        }
    }

    // ==================== ボール効果の適用（ここを書き換え） ====================
    private void ApplyBallEffect(string tagName)
    {
        switch (tagName)
        {
            case "SpeedUp":
                StartEffect_Overwrite(
                    "SpeedUp",
                    apply: () => { speed = baseSpeed + 2f; Debug.Log("Speed Up! 現在のスピード: " + speed); },
                    cleanup: () => { speed = baseSpeed; Debug.Log("Speed 戻った: " + speed); },
                    duration: 3f
                );
                break;

            case "SpeedDown":
                StartEffect_Overwrite(
                    "SpeedDown",
                    apply: () => { speed = Mathf.Max(0f, baseSpeed - 2f); Debug.Log("Speed Down! 現在のスピード: " + speed); },
                    cleanup: () => { speed = baseSpeed; Debug.Log("Speed 戻った: " + speed); },
                    duration: 3f
                );
                break;

            case "JumpUp":
                StartEffect_Overwrite(
                    "JumpUp",
                    apply: () => { Jump = baseJump + 2f; Debug.Log("Jump Up! 現在のジャンプ力: " + Jump); },
                    cleanup: () => { Jump = baseJump; Debug.Log("Jump 戻った: " + Jump); },
                    duration: 3f
                );
                break;

            case "JumpDown":
                StartEffect_Overwrite(
                    "JumpDown",
                    apply: () => { Jump = Mathf.Max(0f, baseJump - 2f); Debug.Log("Jump Down! 現在のジャンプ力: " + Jump); },
                    cleanup: () => { Jump = baseJump; Debug.Log("Jump 戻った: " + Jump); },
                    duration: 3f
                );
                break;

            case "Invincible":
                StartEffect_Overwrite(
                    "Invincible",
                    apply: () => { /* 無敵フラグ ON */ Debug.Log("Invincible!"); },
                    cleanup: () => { /* 無敵フラグ OFF */ },
                    duration: 3f
                );
                break;

            case "BigBasket":
                StartEffect_Overwrite(
                    "BigBasket",
                    apply: () => { /* かご拡大 */ Debug.Log("BigBasket!"); },
                    cleanup: () => { /* 元に戻す */ },
                    duration: 3f
                );
                break;

            case "MinusScore":
                // 即時系は duration 0 でもOK（上書きポリシーに合わせて統一）
                StartEffect_Overwrite(
                    "MinusScore",
                    apply: () => { /* スコア減算 */ Debug.Log("MinusScore!"); },
                    cleanup: () => { /* 基本なし */ },
                    duration: 0f
                );
                break;

            case "MinusTime":
                StartEffect_Overwrite(
                    "MinusTime",
                    apply: () => { /* タイム減少 */ Debug.Log("MinusTime!"); },
                    cleanup: () => { /* 基本なし */ },
                    duration: 0f
                );
                break;

            case "Bom":
                // 例：操作不可にしたい場合はフラグをここでON/OFF
                StartEffect_Overwrite(
                    "Bom",
                    apply: () => {
                        // ここでは分かりやすく速度・ジャンプを下げる挙動のまま
                        speed = Mathf.Max(0f, baseSpeed - 5f);
                        Jump = Mathf.Max(0f, baseJump - 2f);
                        Debug.Log("動けない");
                    },
                    cleanup: () => {
                        speed = baseSpeed;
                        Jump = baseJump;
                        Debug.Log("動ける " + speed);
                        Debug.Log("動ける " + Jump);
                    },
                    duration: 2f
                );
                break;

            default:
                Debug.LogWarning("未対応のタグ: " + tagName);
                break;
        }
    }

    // ==================== ここから下は衝突処理（ほぼ既存のまま） ====================
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("衝突検知: " + collision.gameObject.name + " / Tag: " + collision.gameObject.tag);

        // ★ 簡易：地面に触れたら接地を true に（Ground タグ前提）
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

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
            // プレイヤーに効果を適用（★ 上書き）
            ApplyBallEffect(collision.gameObject.tag);

            // ボールを削除
            Destroy(collision.gameObject);
        }
    }
}
