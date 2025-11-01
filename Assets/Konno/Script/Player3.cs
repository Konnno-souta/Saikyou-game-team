using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player3 : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("�X�^�~�i�ݒ�")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDecreasePerSecond = 25f; // ����Ƃ��̖��b�����
    public float staminaRecoveryRate = 40f;      // 1�b������̉񕜗ʁi���X�ɉ񕜁j
    public float recoveryDelay = 10f;           // 0�ɂȂ��Ă���񕜂��n�܂�܂ł̒x������

    [Header("UI�Q��")]
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
            // �X�^�~�i�𖈕b���X�Ɍ��炷
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
            // 0�ɂȂ�����x���^�C�}�[�J�n
            delayTimer += Time.deltaTime;
            if (delayTimer >= recoveryDelay)
                isRecovering = true;
        }
        else
        {
            // �ʏ펞�����R��ON
            isRecovering = true;
        }
    }

    void HandleStamina()
    {
        // �X�^�~�i�����X�ɉ�
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


