using UnityEngine;
// このPlayerStaminaSystemクラスは、プレイヤーの移動とスタミナ管理を行います。
// そしてGUIでスタミナの状態を表示します。
// このPlayerStaminaSystemは、いずれPlayerUI2に名前変更で削除される予定です。
public class PlayerStaminaSystem : MonoBehaviour
{
    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("スタミナ設定")]
    public int maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDecreasePerStep = 25f;  //  消費量
    public float staminaRecoveryPerTick = 25f;  //  回復量
    public float staminaConsumeInterval = 0.4f; // 走行中のスタミナ消費間隔
    public float recoveryInterval = 0.3f; // 少しゆっくりめに回復

    private bool canMove = true;
    private bool isRunning = false;
    private float consumeTimer = 0f;    //  消費用タイマー
    private float recoveryTimer = 0f;   //  回復用タイマー

    // UI設定
    private GUIStyle guiStyle = new GUIStyle(); //  GUIスタイル
    public Vector2 barPosition = new Vector2(10, 500);   //  バーの位置
    public Vector2 barSize = new Vector2(500, 500);  //  バーのサイズ

    void Start()
    {
        guiStyle.fontSize = 100;    //  フォントサイズを大きく
        guiStyle.normal.textColor = Color.white;    //  文字色を白に
    }

    void Update()
    {
        HandleMovement();
        HandleStamina();
    }

    void HandleMovement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Shiftを押して移動しているときのみ走行モード
        isRunning = shiftHeld && moveX != 0 && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);

        // 走行中のスタミナ消費
        if (isRunning)
        {
            consumeTimer += Time.deltaTime;
            if (consumeTimer >= staminaConsumeInterval)
            {
                consumeTimer = 0f;  //      タイマーリセット
                currentStamina -= staminaDecreasePerStep;
                if (currentStamina < 0) currentStamina = 0; // 0未満にしない

                Debug.Log($"スタミナ消費: -{staminaDecreasePerStep} → {currentStamina}");

                if (currentStamina <= 0)
                {
                    canMove = false;
                    Debug.Log("スタミナが0になりました。回復開始。");
                }
            }
        }
        else
        {
            consumeTimer = 0f;
        }
        void HandleStamina()
        {
            if (currentStamina < maxStamina)
            {
                recoveryTimer += Time.deltaTime;

                if (recoveryTimer >= recoveryInterval)
                {
                    recoveryTimer = 0f;

                    currentStamina += staminaRecoveryPerTick;
                    if (currentStamina > maxStamina) currentStamina = maxStamina;

                    if (currentStamina > 0)
                        canMove = true;
                }
            }
            else
            {
                recoveryTimer = 0f;
            }
        }
    }

    void HandleStamina()
    {
        // スタミナ0以下のときに回復
        if (currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0f;

                // 0未満で止まっている場合も回復対象にする
                if (currentStamina < maxStamina)
                {
                    currentStamina += staminaRecoveryPerTick;
                    if (currentStamina > maxStamina) currentStamina = maxStamina;

                    Debug.Log($"スタミナ回復: +{staminaRecoveryPerTick} → {currentStamina}");
                }

                // 1回でも回復が始まったら移動再開を許可
                if (currentStamina > 0)
                    canMove = true;
            }
        }
        else
        {
            recoveryTimer = 0f;
        }
    }
    void OnGUI()
    {
        // テキスト
        GUI.Label(new Rect(10, 200, 500, 200), $"Stamina: {currentStamina}/{maxStamina}", guiStyle); // スタミナ表示

        // バー背景
        GUI.color = Color.black;
        GUI.Box(new Rect(barPosition.x - 2, barPosition.y - 2, barSize.x + 4, barSize.y + 4), GUIContent.none);

        // スタミナ比率
        float ratio = currentStamina / maxStamina;
        float fillWidth = barSize.x * ratio;

        // 残量によって色を変える（緑→黄→赤）
        if (ratio > 0.6f)
            GUI.color = Color.green;
        else if (ratio > 0.3f)
            GUI.color = Color.yellow;
        else
            GUI.color = Color.red;

        // バー本体
        GUI.Box(new Rect(barPosition.x, barPosition.y, fillWidth, barSize.y), GUIContent.none);

        // カラーリセット
        GUI.color = Color.white;
    }
}
