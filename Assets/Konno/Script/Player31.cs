using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Player31 : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("�X�^�~�i�ݒ�")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDecreasePerSecond = 25f;
    public float staminaRecoveryRate = 10f;
    public float rapidRecoveryRate = 200f;
    public float recoveryDelay = 0.5f;
    public float exhaustedMotionTime = 5f;

    [Header("UI�Q��")]
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
        //  MeshRenderer �������擾
        playerMeshRenderer = GetComponent<MeshRenderer>();
        if (playerMeshRenderer == null)
        {
            Debug.LogError(" MeshRenderer ��������܂���B");
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
        Debug.Log("Exhausted Motion �J�n");

        while (timer < exhaustedMotionTime)
        {
            timer += Time.deltaTime;

            if (playerMeshRenderer != null)
            {
                // MeshRenderer �̃}�e���A���F��Ԍ��F�ɓ_��
                float blink = Mathf.PingPong(Time.time * 6f, 1f); // �_�ő��x
                Color blinkColor = Color.Lerp(originalColor, Color.red, blink);
                playerMeshRenderer.material.color = blinkColor;
            }

            yield return null;
        }

        // �F�����ɖ߂�
        if (playerMeshRenderer != null)
            playerMeshRenderer.material.color = originalColor;

        yield return StartCoroutine(RapidRecovery());

        canMove = true;
        isExhausted = false;
        Debug.Log(" Exhausted Motion �I��");
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
