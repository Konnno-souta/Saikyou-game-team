using UnityEngine;  
using UnityEngine.UI;
using TMPro;

// このPlayerUI2クラスは、プレイヤーの移動とスタミナ管理を行います。
// そしてUIでスタミナの状態を表示します。
// このPlayerUI2は、PlayerStaminaSystemの改良版です。
// 主な改良点は、TextMeshProを使用したスタミナ数値表示の追加と、スタミナバーの色変化の実装です。
// また、コードの可読性と保守性を向上させるために、コメントを追加し、変数名をより明確にしました。
// さらに、スタミナ回復の間隔を調整し、ゲームプレイのバランスを改善しました。
// ※このPlayerUI2を使用する場合は、過去のPlayerStaminaSystemを削除または無効化してください。
// そしてPlayerUI2からPlayerSystemaSystemに名前を変更してください。

public class PlayerUI2 : MonoBehaviour  //  プレイヤーの移動とスタミナ管理を行うクラス
{
    [Header("移動設定")]
    public float walkSpeed = 5f;    //  通常移動速度
    public float runSpeed = 10f;    //  ダッシュ移動速度

    [Header("スタミナ設定")] 
    public float maxStamina = 100f; //  最大スタミナ
    public float currentStamina = 100f; //  現在のスタミナ
    public float staminaDecreasePerStep = 10f;  //  スタミナ消費量
    public float staminaRecoveryPerTick = 10f;  //  スタミナ回復量
    public float staminaConsumeInterval = 0.5f; //  スタミナ消費間隔
    public float recoveryInterval = 5.0f;   //  スタミナ回復間隔

    [Header("UI参照")]
    public Slider staminaSlider;    //  スタミナバー
    public Image staminaFill;   //  スタミナバーの塗りつぶし部分
    public TextMeshProUGUI staminaText; //  スタミナ数値表

    private bool canMove = true;    //  移動可能フラグ
    private bool isRunning = false; //  ダッシュ中フラグ
    private float consumeTimer = 0f;    //  スタミナ消費タイマー
    private float recoveryTimer = 0f;   //  スタミナ回復タイマー

    void Start()    // 初期化
    {   // スタミナバーと数値表示の初期化
        if (staminaSlider != null)  // スタミナバーの最大値と初期値設定
        {
            staminaSlider.maxValue = maxStamina;    //  最大スタミナを設定
            staminaSlider.value = currentStamina;   //  現在のスタミナを設定
        }
        UpdateUI(); //  UIの初期更新
        //  スタミナ関連タイマーの初期化
    }

    void Update()   //  毎フレーム更新
    {
        HandleMovement();   //  移動処理
        HandleStamina();    //  スタミナ処理
        UpdateUI();         //  UI更新
    }
    void HandleMovement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isRunning = shiftHeld && moveX != 0 && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);

        // ▼ スタミナ消費処理を「徐々に減少」に変更 ▼
        if (isRunning)
        {
            // 1秒間にstaminaDecreasePerStepずつ減少するように
            currentStamina -= staminaDecreasePerStep * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canMove = false;
            }
        }
        else
        {
            consumeTimer = 0f; // ダッシュしていないときはタイマーリセット
        }
    }

    //void HandleMovement()   //  移動処理
    //{
    //    if (!canMove) return;   //  移動不可なら処理終了

    //    float moveX = Input.GetAxisRaw("Horizontal");   //  水平方向の入力取得  // -1:左, 0:停止, 1:右
    //    bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);   //  シフトキーが押されているか
    //    isRunning = shiftHeld && moveX != 0 && currentStamina > 0;  //  ダッシュ中フラグ設定//  シフトキーが押されていて、移動中で、スタミナがある場合のみダッシュ

    //    float speed = isRunning ? runSpeed : walkSpeed; //  移動速度設定
    //    transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);    //  移動処理

    //    // スタミナ消費
    //    if (isRunning)  //  ダッシュ中のみスタミナ消費
    //    {
    //        consumeTimer += Time.deltaTime; //  タイマー加算
    //        if (consumeTimer >= staminaConsumeInterval) //  消費間隔到達時
    //        {
    //            consumeTimer = 0f;  //  タイマーリセット
    //            currentStamina -= staminaDecreasePerStep;   //  スタミナ減少
    //            if (currentStamina < 0) currentStamina = 0; //  0未満にしない

    //            if (currentStamina <= 0)    //  スタミナが0以下になった場合
    //                canMove = false;    //  移動不可にする
    //        }
    //    }
    //    else
    //    {
    //        consumeTimer = 0f;  //  ダッシュしていない場合はタイマーリセット
    //    }
    //}

    void HandleStamina()    //  スタミナ回復処理
    {
        if (currentStamina < maxStamina)    //  スタミナが最大未満の場合
        {
            recoveryTimer += Time.deltaTime;    //  タイマー加算
            if (recoveryTimer >= recoveryInterval)  //  回復間隔到達時
            {
                recoveryTimer = 0f; //  タイマーリセット
                currentStamina += staminaRecoveryPerTick;   //  スタミナ回復
                if (currentStamina > maxStamina) currentStamina = maxStamina;   //  最大値を超えないように

                if (currentStamina > 0) //  スタミナが回復して0より大きくなった場合
                    canMove = true;     //  移動可能にする
            }
        }
        else
        {
            recoveryTimer = 0f; //  スタミナが最大の場合はタイマーリセット
        }
    }

    void UpdateUI() //  UI更新
    {
        if (staminaSlider != null)  //  スタミナバーの値更新
            staminaSlider.value = currentStamina;   //  現在のスタミナを設定

        if (staminaText != null)    //  スタミナ数値表示更新
            staminaText.text = $"Stamina: {currentStamina}/{maxStamina}";   //  スタミナ数値表示更新

        if (staminaFill != null)    //  スタミナバーの色更新
        {
            float ratio = currentStamina / maxStamina;  //  スタミナ比率計算//  0.0～1.0
            // 色変化（緑→黄→赤）//  赤(0,1,0) → 黄(1,1,0) → 緑(0,1,0)//  緑から赤への変化に修正//  緑(1,0,0) → 黄(1,1,0) → 赤(0,1,0)
            Color barColor = Color.Lerp(Color.red, Color.green, ratio); //  赤から緑への線形補間
            staminaFill.color = barColor;   //  スタミナバーの色設定
        }
    }
}
