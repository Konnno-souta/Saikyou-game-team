using UnityEngine;

public class PlayerStaminaSystem : MonoBehaviour
{
    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("スタミナ設定")]
    public int maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDecreasePerMove = 25f; // 1回移動ごとに減る量
    public float staminaRecoveryPerTick = 25f; // 1回回復で増える量
    public float recoveryInterval = 1f; // 回復間隔（秒）

    private bool canMove = true;
    private bool isRunning = false;
    private float recoveryTimer = 0f;

    // UI位置設定
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
        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;

        // 走行中に移動した瞬間にスタミナを消費
        if (isRunning && moveX != 0)
        {
            currentStamina -= staminaDecreasePerMove;
            if (currentStamina < 0) currentStamina = 0;

            Debug.Log($" スタミナ消費: -{staminaDecreasePerMove} (残り: {currentStamina})");

            if (currentStamina == 0)
            {
                canMove = false;
                Debug.Log(" スタミナが0になりました。回復開始可能。");
            }
        }

        // 実際の移動処理
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);
    }

    void HandleStamina()
    {
        // スタミナが0のときだけ回復
        if (currentStamina == 0)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0f;
                currentStamina += staminaRecoveryPerTick;
                if (currentStamina > maxStamina) currentStamina = maxStamina;

                Debug.Log($" スタミナ回復: +{staminaRecoveryPerTick} (現在: {currentStamina})");

                if (currentStamina >= maxStamina)
                {
                    canMove = true;
                    Debug.Log("スタミナが全回復しました。再び走行可能。");
                }
            }
        }

        // 常にデバッグ出力
        Debug.Log($"スタミナ: {currentStamina}/{maxStamina}　走行中: {isRunning}");
    }

    void OnGUI()
    {
        // 背景バー
        GUI.Box(new Rect(barPosition.x, barPosition.y, barSize.x, barSize.y), "");

        // 比率計算
        float ratio = currentStamina / maxStamina;
        float filledWidth = barSize.x * ratio;

        // 残量に応じて色変更（30%以下で赤）
        GUI.color = ratio < 0.3f ? Color.red : Color.green;

        // スタミナゲージ
        GUI.Box(new Rect(barPosition.x, barPosition.y, filledWidth, barSize.y), GUIContent.none);

        // 文字
        GUI.color = Color.white;
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Label(new Rect(barPosition.x, barPosition.y, barSize.x, barSize.y),
            $"Stamina: {currentStamina:F0}/{maxStamina}", style);
    }
}


