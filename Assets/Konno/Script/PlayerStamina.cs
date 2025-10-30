using UnityEngine;

public class PlayerStaminaSystem : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("�X�^�~�i�ݒ�")]
    public int maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDecreasePerUse = 25f;  // �����
    public float staminaRecoveryPerTick = 25f; // �񕜗�
    public float recoveryInterval = 1.0f;      // �񕜊Ԋu�������x�߂�
    private float recoveryTimer = 0f;

    private bool canMove = true;
    private bool isRunning = false;

    // UI�ݒ�
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

        // Shift�������Ă��āA���X�^�~�i���c���Ă���Ԃ͑��s���[�h
        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;

        // ���s���Ɉړ����Ă���ꍇ�̂݃X�^�~�i����
        if (isRunning && moveX != 0)
        {
            // �X�^�~�i���܂�����ꍇ�̂݌��炷
            currentStamina -= staminaDecreasePerUse * Time.deltaTime; // ���Ԍo�߂ɉ����ď���
            if (currentStamina < 0) currentStamina = 0;

            // �X�^�~�i��0�ɂȂ����瑖��Ȃ�����
            if (currentStamina <= 0)
            {
                canMove = false;
                Debug.Log("�X�^�~�i��0�ɂȂ�܂����B�񕜒��c");
            }
        }

        // ���ۂ̈ړ�����
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);
    }

    void HandleStamina()
    {
        // �X�^�~�i��0�Ȃ�񕜊J�n
        if (!canMove)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0f;
                currentStamina += staminaRecoveryPerTick;
                if (currentStamina > maxStamina) currentStamina = maxStamina;

                Debug.Log($"��: +{staminaRecoveryPerTick} (����: {currentStamina})");

                if (currentStamina >= maxStamina)
                {
                    canMove = true;
                    Debug.Log("�X�^�~�i�S�񕜁B�Ăё��s�\�B");
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
