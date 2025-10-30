using UnityEngine;

public class PlayerStaminaSystem : MonoBehaviour
{
    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("スタミナ設定")]
    public int maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDecreasePerUse = 25f;  // 消費量
    public float staminaRecoveryPerTick = 25f; // 回復量
    public float recoveryInterval = 1.0f;      // 回復間隔を少し遅めに
    private float recoveryTimer = 0f;

    private bool canMove = true;
    private bool isRunning = false;

    // UI設定
    public Vector2 barPosition = new Vector2(10, 10);
    public Vector2 barSize = new Vector2(200, 25);

    void Update()
    {
        HandleMovement();
        HandleStamina();
    }

    void HandleMovement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw("Horizontal");

        // Shiftを押していて、かつスタミナが残っている間は走行モード
        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;

        // 走行中に移動している場合のみスタミナ消費
        if (isRunning && moveX != 0)
        {
            // スタミナがまだある場合のみ減らす
            currentStamina -= staminaDecreasePerUse * Time.deltaTime; // 時間経過に応じて消費
            if (currentStamina < 0) currentStamina = 0;

            // スタミナが0になったら走れなくする
            if (currentStamina <= 0)
            {
                canMove = false;
                Debug.Log("スタミナが0になりました。回復中…");
            }
        }

        // 実際の移動処理
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);
    }

    void HandleStamina()
    {
        // スタミナが0なら回復開始
        if (!canMove)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0f;
                currentStamina += staminaRecoveryPerTick;
                if (currentStamina > maxStamina) currentStamina = maxStamina;

                Debug.Log($"回復: +{staminaRecoveryPerTick} (現在: {currentStamina})");

                if (currentStamina >= maxStamina)
                {
                    canMove = true;
                    Debug.Log("スタミナ全回復。再び走行可能。");
                }
            }
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        GUI.Label(new Rect(barPosition.x, barPosition.y, 300, 50),
            $"Stamina: {currentStamina:F0}/{maxStamina}", style);
    }
}
