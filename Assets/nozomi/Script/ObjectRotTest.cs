using UnityEngine;

public class ObjectRotTest : MonoBehaviour
{
    [Header("‰ñ“]Ý’è")]
    public Vector3 rotationAxis = Vector3.up;   // ‰ñ“]Ž²
    public float rotateAngle = 90f;             // Œ»ÝŠp“x‚©‚ç‰ñ‚·—Êi“xj
    public float speed = 60f;                   // ‰ñ“]‘¬“xi“x/•bj

    [Header("’âŽ~Ý’è")]
    public float stopTime = 0.5f;               // ’[‚Å’âŽ~‚·‚éŽžŠÔi•bj

    private Quaternion baseRotation;             // Šî€‰ñ“]
    private float currentAngle = 0f;             // Œ»Ý‚Ì·•ªŠp“x
    private float targetAngle;                   // –Ú•W·•ªŠp“x
    private float direction;                   // ‰ñ“]•ûŒü
    private float stopTimer = 0f;
    private bool isStopping = false;

    void Start()
    {
        // ‰Šú‰ñ“]‚ð•Û‘¶
        baseRotation = transform.localRotation;

        targetAngle = rotateAngle;
    }

    void Update()
    {
        if (isStopping)
        {
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopTime)
            {
                stopTimer = 0f;
                isStopping = false;

                // ”½“]‚µ‚ÄŽŸ‚Ì–Ú•W‚Ö
                direction = rotateAngle * -1;
                targetAngle = rotateAngle - targetAngle;
            }
            return;
        }

        float step = speed * Time.deltaTime;
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, step);

        transform.localRotation =
            baseRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);

        // ’âŽ~ŠJŽn
        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            isStopping = true;
        }
    }
}
