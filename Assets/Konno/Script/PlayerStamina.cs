using UnityEngine;
// ����PlayerStaminaSystem�N���X�́A�v���C���[�̈ړ��ƃX�^�~�i�Ǘ����s���܂��B
// ������GUI�ŃX�^�~�i�̏�Ԃ�\�����܂��B
// ����PlayerStaminaSystem�́A������PlayerUI2�ɖ��O�ύX�ō폜�����\��ł��B
public class PlayerStaminaSystem : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("�X�^�~�i�ݒ�")]
    public int maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDecreasePerStep = 25f;  //  �����
    public float staminaRecoveryPerTick = 25f;  //  �񕜗�
    public float staminaConsumeInterval = 0.4f; // ���s���̃X�^�~�i����Ԋu
    public float recoveryInterval = 0.3f; // �����������߂ɉ�

    private bool canMove = true;
    private bool isRunning = false;
    private float consumeTimer = 0f;    //  ����p�^�C�}�[
    private float recoveryTimer = 0f;   //  �񕜗p�^�C�}�[

    // UI�ݒ�
    private GUIStyle guiStyle = new GUIStyle(); //  GUI�X�^�C��
    public Vector2 barPosition = new Vector2(10, 500);   //  �o�[�̈ʒu
    public Vector2 barSize = new Vector2(500, 500);  //  �o�[�̃T�C�Y

    void Start()
    {
        guiStyle.fontSize = 100;    //  �t�H���g�T�C�Y��傫��
        guiStyle.normal.textColor = Color.white;    //  �����F�𔒂�
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

        // Shift�������Ĉړ����Ă���Ƃ��̂ݑ��s���[�h
        isRunning = shiftHeld && moveX != 0 && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);

        // ���s���̃X�^�~�i����
        if (isRunning)
        {
            consumeTimer += Time.deltaTime;
            if (consumeTimer >= staminaConsumeInterval)
            {
                consumeTimer = 0f;  //      �^�C�}�[���Z�b�g
                currentStamina -= staminaDecreasePerStep;
                if (currentStamina < 0) currentStamina = 0; // 0�����ɂ��Ȃ�

                Debug.Log($"�X�^�~�i����: -{staminaDecreasePerStep} �� {currentStamina}");

                if (currentStamina <= 0)
                {
                    canMove = false;
                    Debug.Log("�X�^�~�i��0�ɂȂ�܂����B�񕜊J�n�B");
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
        // �X�^�~�i0�ȉ��̂Ƃ��ɉ�
        if (currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0f;

                // 0�����Ŏ~�܂��Ă���ꍇ���񕜑Ώۂɂ���
                if (currentStamina < maxStamina)
                {
                    currentStamina += staminaRecoveryPerTick;
                    if (currentStamina > maxStamina) currentStamina = maxStamina;

                    Debug.Log($"�X�^�~�i��: +{staminaRecoveryPerTick} �� {currentStamina}");
                }

                // 1��ł��񕜂��n�܂�����ړ��ĊJ������
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
        // �e�L�X�g
        GUI.Label(new Rect(10, 200, 500, 200), $"Stamina: {currentStamina}/{maxStamina}", guiStyle); // �X�^�~�i�\��

        // �o�[�w�i
        GUI.color = Color.black;
        GUI.Box(new Rect(barPosition.x - 2, barPosition.y - 2, barSize.x + 4, barSize.y + 4), GUIContent.none);

        // �X�^�~�i�䗦
        float ratio = currentStamina / maxStamina;
        float fillWidth = barSize.x * ratio;

        // �c�ʂɂ���ĐF��ς���i�΁������ԁj
        if (ratio > 0.6f)
            GUI.color = Color.green;
        else if (ratio > 0.3f)
            GUI.color = Color.yellow;
        else
            GUI.color = Color.red;

        // �o�[�{��
        GUI.Box(new Rect(barPosition.x, barPosition.y, fillWidth, barSize.y), GUIContent.none);

        // �J���[���Z�b�g
        GUI.color = Color.white;
    }
}
