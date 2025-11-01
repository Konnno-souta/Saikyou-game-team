using UnityEngine;  
using UnityEngine.UI;
using TMPro;

// ����PlayerUI2�N���X�́A�v���C���[�̈ړ��ƃX�^�~�i�Ǘ����s���܂��B
// ������UI�ŃX�^�~�i�̏�Ԃ�\�����܂��B
// ����PlayerUI2�́APlayerStaminaSystem�̉��ǔłł��B
// ��ȉ��Ǔ_�́ATextMeshPro���g�p�����X�^�~�i���l�\���̒ǉ��ƁA�X�^�~�i�o�[�̐F�ω��̎����ł��B
// �܂��A�R�[�h�̉ǐ��ƕێ琫�����コ���邽�߂ɁA�R�����g��ǉ����A�ϐ�������薾�m�ɂ��܂����B
// ����ɁA�X�^�~�i�񕜂̊Ԋu�𒲐����A�Q�[���v���C�̃o�����X�����P���܂����B
// ������PlayerUI2���g�p����ꍇ�́A�ߋ���PlayerStaminaSystem���폜�܂��͖��������Ă��������B
// ������PlayerUI2����PlayerSystemaSystem�ɖ��O��ύX���Ă��������B

public class PlayerUI2 : MonoBehaviour  //  �v���C���[�̈ړ��ƃX�^�~�i�Ǘ����s���N���X
{
    [Header("�ړ��ݒ�")]
    public float walkSpeed = 5f;    //  �ʏ�ړ����x
    public float runSpeed = 10f;    //  �_�b�V���ړ����x

    [Header("�X�^�~�i�ݒ�")] 
    public float maxStamina = 100f; //  �ő�X�^�~�i
    public float currentStamina = 100f; //  ���݂̃X�^�~�i
    public float staminaDecreasePerStep = 10f;  //  �X�^�~�i�����
    public float staminaRecoveryPerTick = 10f;  //  �X�^�~�i�񕜗�
    public float staminaConsumeInterval = 0.5f; //  �X�^�~�i����Ԋu
    public float recoveryInterval = 5.0f;   //  �X�^�~�i�񕜊Ԋu

    [Header("UI�Q��")]
    public Slider staminaSlider;    //  �X�^�~�i�o�[
    public Image staminaFill;   //  �X�^�~�i�o�[�̓h��Ԃ�����
    public TextMeshProUGUI staminaText; //  �X�^�~�i���l�\

    private bool canMove = true;    //  �ړ��\�t���O
    private bool isRunning = false; //  �_�b�V�����t���O
    private float consumeTimer = 0f;    //  �X�^�~�i����^�C�}�[
    private float recoveryTimer = 0f;   //  �X�^�~�i�񕜃^�C�}�[

    void Start()    // ������
    {   // �X�^�~�i�o�[�Ɛ��l�\���̏�����
        if (staminaSlider != null)  // �X�^�~�i�o�[�̍ő�l�Ə����l�ݒ�
        {
            staminaSlider.maxValue = maxStamina;    //  �ő�X�^�~�i��ݒ�
            staminaSlider.value = currentStamina;   //  ���݂̃X�^�~�i��ݒ�
        }
        UpdateUI(); //  UI�̏����X�V
        //  �X�^�~�i�֘A�^�C�}�[�̏�����
    }

    void Update()   //  ���t���[���X�V
    {
        HandleMovement();   //  �ړ�����
        HandleStamina();    //  �X�^�~�i����
        UpdateUI();         //  UI�X�V
    }
    void HandleMovement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isRunning = shiftHeld && moveX != 0 && currentStamina > 0;

        float speed = isRunning ? runSpeed : walkSpeed;
        transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);

        // �� �X�^�~�i��������u���X�Ɍ����v�ɕύX ��
        if (isRunning)
        {
            // 1�b�Ԃ�staminaDecreasePerStep����������悤��
            currentStamina -= staminaDecreasePerStep * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canMove = false;
            }
        }
        else
        {
            consumeTimer = 0f; // �_�b�V�����Ă��Ȃ��Ƃ��̓^�C�}�[���Z�b�g
        }
    }

    //void HandleMovement()   //  �ړ�����
    //{
    //    if (!canMove) return;   //  �ړ��s�Ȃ珈���I��

    //    float moveX = Input.GetAxisRaw("Horizontal");   //  ���������̓��͎擾  // -1:��, 0:��~, 1:�E
    //    bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);   //  �V�t�g�L�[��������Ă��邩
    //    isRunning = shiftHeld && moveX != 0 && currentStamina > 0;  //  �_�b�V�����t���O�ݒ�//  �V�t�g�L�[��������Ă��āA�ړ����ŁA�X�^�~�i������ꍇ�̂݃_�b�V��

    //    float speed = isRunning ? runSpeed : walkSpeed; //  �ړ����x�ݒ�
    //    transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);    //  �ړ�����

    //    // �X�^�~�i����
    //    if (isRunning)  //  �_�b�V�����̂݃X�^�~�i����
    //    {
    //        consumeTimer += Time.deltaTime; //  �^�C�}�[���Z
    //        if (consumeTimer >= staminaConsumeInterval) //  ����Ԋu���B��
    //        {
    //            consumeTimer = 0f;  //  �^�C�}�[���Z�b�g
    //            currentStamina -= staminaDecreasePerStep;   //  �X�^�~�i����
    //            if (currentStamina < 0) currentStamina = 0; //  0�����ɂ��Ȃ�

    //            if (currentStamina <= 0)    //  �X�^�~�i��0�ȉ��ɂȂ����ꍇ
    //                canMove = false;    //  �ړ��s�ɂ���
    //        }
    //    }
    //    else
    //    {
    //        consumeTimer = 0f;  //  �_�b�V�����Ă��Ȃ��ꍇ�̓^�C�}�[���Z�b�g
    //    }
    //}

    void HandleStamina()    //  �X�^�~�i�񕜏���
    {
        if (currentStamina < maxStamina)    //  �X�^�~�i���ő喢���̏ꍇ
        {
            recoveryTimer += Time.deltaTime;    //  �^�C�}�[���Z
            if (recoveryTimer >= recoveryInterval)  //  �񕜊Ԋu���B��
            {
                recoveryTimer = 0f; //  �^�C�}�[���Z�b�g
                currentStamina += staminaRecoveryPerTick;   //  �X�^�~�i��
                if (currentStamina > maxStamina) currentStamina = maxStamina;   //  �ő�l�𒴂��Ȃ��悤��

                if (currentStamina > 0) //  �X�^�~�i���񕜂���0���傫���Ȃ����ꍇ
                    canMove = true;     //  �ړ��\�ɂ���
            }
        }
        else
        {
            recoveryTimer = 0f; //  �X�^�~�i���ő�̏ꍇ�̓^�C�}�[���Z�b�g
        }
    }

    void UpdateUI() //  UI�X�V
    {
        if (staminaSlider != null)  //  �X�^�~�i�o�[�̒l�X�V
            staminaSlider.value = currentStamina;   //  ���݂̃X�^�~�i��ݒ�

        if (staminaText != null)    //  �X�^�~�i���l�\���X�V
            staminaText.text = $"Stamina: {currentStamina}/{maxStamina}";   //  �X�^�~�i���l�\���X�V

        if (staminaFill != null)    //  �X�^�~�i�o�[�̐F�X�V
        {
            float ratio = currentStamina / maxStamina;  //  �X�^�~�i�䗦�v�Z//  0.0�`1.0
            // �F�ω��i�΁������ԁj//  ��(0,1,0) �� ��(1,1,0) �� ��(0,1,0)//  �΂���Ԃւ̕ω��ɏC��//  ��(1,0,0) �� ��(1,1,0) �� ��(0,1,0)
            Color barColor = Color.Lerp(Color.red, Color.green, ratio); //  �Ԃ���΂ւ̐��`���
            staminaFill.color = barColor;   //  �X�^�~�i�o�[�̐F�ݒ�
        }
    }
}
