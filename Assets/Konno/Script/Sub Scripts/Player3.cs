using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player3 : MonoBehaviour
{
    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("スタミナ設定")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDecreasePerSecond = 25f; // 走るときの毎秒消費量
    public float staminaRecoveryRate = 40f;      // 1秒あたりの回復量（徐々に回復）
    public float recoveryDelay = 10f;           // 0になってから回復が始まるまでの遅延時間

    [Header("UI参照")]
    public Slider staminaSlider;
    public Image staminaFill;
    public TextMeshProUGUI staminaText;

    private bool canMove = true;
    private bool isRunning = false;
    private bool isRecovering = false;
    private float delayTimer = 0f;

    void Start()
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        UpdateUI();
    }

    void Update()
    {
        HandleMovement();
        HandleStamina();
        UpdateUI();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        isRunning = shiftHeld && moveX != 0 && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;

        if (canMove)
            transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);

        if (isRunning)
        {
            // スタミナを毎秒徐々に減らす
            currentStamina -= staminaDecreasePerSecond * Time.deltaTime;
            isRecovering = false;
            delayTimer = 0f;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canMove = false;
            }
        }
        else if (currentStamina <= 0)
        {
            // 0になったら遅延タイマー開始
            delayTimer += Time.deltaTime;
            if (delayTimer >= recoveryDelay)
                isRecovering = true;
        }
        else
        {
            // 通常時も自然回復ON
            isRecovering = true;
        }
    }

    void HandleStamina()
    {
        // スタミナが徐々に回復
        if (isRecovering && currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            if (currentStamina > 0)
                canMove = true;
        }
    }

    void UpdateUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;

        if (staminaText != null)
            staminaText.text = $"Stamina: {Mathf.FloorToInt(currentStamina)}/{maxStamina}";

        if (staminaFill != null)
        {
            float ratio = currentStamina / maxStamina;
            Color barColor = Color.Lerp(Color.red, Color.green, ratio);
            staminaFill.color = barColor;
        }
    }
}


