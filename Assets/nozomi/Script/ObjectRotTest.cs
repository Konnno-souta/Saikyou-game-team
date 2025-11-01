using UnityEngine;

public class ObjectRotTest : MonoBehaviour
{
    [Header("��]�ݒ�")]
    public float angleRange = 180f;     // ��]�͈́i��F180�x�j
    public float speed = 60f;           // ��]���x�i�x/�b�j
    public Vector3 rotationAxis = Vector3.up; // ��]���iY���Ȃǁj

    [Header("��~�ݒ�")]
    public float stopTime = 0.5f;       // �[�Œ�~���鎞�ԁi�b�j

    private float currentAngle = 0f;    // ���݂̊p�x
    private int direction = 1;          // ��]�����i+1 or -1�j
    private float stopTimer = 0f;       // ��~�^�C�}�[
    private bool isStopping = false;    // ��~���t���O


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopping)
        {
            // ��~���Ȃ�^�C�}�[��i�߂�
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopTime)
            {
                // ��~�I�� �� �ĊJ
                isStopping = false;
                stopTimer = 0f;
                direction *= -1; // �������]
            }
            return;
        }

        // �ʏ��]
        currentAngle += direction * speed * Time.deltaTime;

        // �͈̓`�F�b�N
        if (Mathf.Abs(currentAngle) >= angleRange / 2f)
        {
            // �[�ɓ��B
            currentAngle = Mathf.Sign(currentAngle) * (angleRange / 2f);
            isStopping = true; // ��~�J�n
        }

        // ���ۂ̉�]��K�p
        transform.localRotation = Quaternion.AngleAxis(currentAngle, rotationAxis);
    }
}

