using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Player31 : MonoBehaviour
{
    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("スタミナ設定")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDecreasePerSecond = 25f;
    public float staminaRecoveryRate = 10f;
    public float rapidRecoveryRate = 200f;
    public float recoveryDelay = 0.5f;
    public float exhaustedMotionTime = 5f;

    [Header("UI参照")]
    public Slider staminaSlider;
    public Image staminaFill;
    public TextMeshProUGUI staminaText;

    private MeshRenderer playerMeshRenderer;
    private Color originalColor;

    private bool canMove = true;
    private bool isRunning = false;
    private bool isRecovering = false;
    private bool isExhausted = false;
    private float delayTimer = 0f;

    void Start()
    {
        //  MeshRenderer を自動取得
        playerMeshRenderer = GetComponent<MeshRenderer>();
        if (playerMeshRenderer == null)
        {
            Debug.LogError(" MeshRenderer が見つかりません。");
        }
        else
        {
            originalColor = playerMeshRenderer.material.color;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        UpdateUI();
    }

    void Update()
    {
        if (isExhausted) return;

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
            currentStamina -= staminaDecreasePerSecond * Time.deltaTime;
            isRecovering = false;
            delayTimer = 0f;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                StartCoroutine(ExhaustedMotion());
            }
        }
        else
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= recoveryDelay)
                isRecovering = true;
        }
    }

    void HandleStamina()
    {
        if (isRecovering && currentStamina < maxStamina && !isExhausted)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    IEnumerator ExhaustedMotion()
    {
        isExhausted = true;
        canMove = false;

        float timer = 0f;
        Debug.Log("Exhausted Motion 開始");

        while (timer < exhaustedMotionTime)
        {
            timer += Time.deltaTime;

            if (playerMeshRenderer != null)
            {
                // MeshRenderer のマテリアル色を赤元色に点滅
                float blink = Mathf.PingPong(Time.time * 6f, 1f); // 点滅速度
                Color blinkColor = Color.Lerp(originalColor, Color.red, blink);
                playerMeshRenderer.material.color = blinkColor;
            }

            yield return null;
        }

        // 色を元に戻す
        if (playerMeshRenderer != null)
            playerMeshRenderer.material.color = originalColor;

        yield return StartCoroutine(RapidRecovery());

        canMove = true;
        isExhausted = false;
        Debug.Log(" Exhausted Motion 終了");
    }

    IEnumerator RapidRecovery()
    {
        while (currentStamina < maxStamina)
        {
            currentStamina += rapidRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateUI();
            yield return null;
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
            staminaFill.color = Color.Lerp(Color.red, Color.green, ratio);
        }
    }
}
