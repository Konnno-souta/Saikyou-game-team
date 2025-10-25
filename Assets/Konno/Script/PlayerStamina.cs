using UnityEngine;

public class PlayerStaminaSystem : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("�X�^�~�i�ݒ�")]
    public int maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDecreasePerMove = 25f; // 1��ړ����ƂɌ����
    public float staminaRecoveryPerTick = 25f; // 1��񕜂ő������
    public float recoveryInterval = 1f; // �񕜊Ԋu�i�b�j

    private bool canMove = true;
    private bool isRunning = false;
    private float recoveryTimer = 0f;

    // UI�ʒu�ݒ�
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

        // ���s���Ɉړ������u�ԂɃX�^�~�i������
        if (isRunning && moveX != 0)
        {
            currentStamina -= staminaDecreasePerMove;
            if (currentStamina < 0) currentStamina = 0;

            Debug.Log($" �X�^�~�i����: -{staminaDecreasePerMove} (�c��: {currentStamina})");

            if (currentStamina == 0)
            {
                canMove = false;
                Debug.Log(" �X�^�~�i��0�ɂȂ�܂����B�񕜊J�n�\�B");
            }
        }

        // ���ۂ̈ړ�����
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);
    }

    void HandleStamina()
    {
        // �X�^�~�i��0�̂Ƃ�������
        if (currentStamina == 0)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0f;
                currentStamina += staminaRecoveryPerTick;
                if (currentStamina > maxStamina) currentStamina = maxStamina;

                Debug.Log($" �X�^�~�i��: +{staminaRecoveryPerTick} (����: {currentStamina})");

                if (currentStamina >= maxStamina)
                {
                    canMove = true;
                    Debug.Log("�X�^�~�i���S�񕜂��܂����B�Ăё��s�\�B");
                }
            }
        }

        // ��Ƀf�o�b�O�o��
        Debug.Log($"�X�^�~�i: {currentStamina}/{maxStamina}�@���s��: {isRunning}");
    }

    void OnGUI()
    {
        // �w�i�o�[
        GUI.Box(new Rect(barPosition.x, barPosition.y, barSize.x, barSize.y), "");

        // �䗦�v�Z
        float ratio = currentStamina / maxStamina;
        float filledWidth = barSize.x * ratio;

        // �c�ʂɉ����ĐF�ύX�i30%�ȉ��Őԁj
        GUI.color = ratio < 0.3f ? Color.red : Color.green;

        // �X�^�~�i�Q�[�W
        GUI.Box(new Rect(barPosition.x, barPosition.y, filledWidth, barSize.y), GUIContent.none);

        // ����
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


