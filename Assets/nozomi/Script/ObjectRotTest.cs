using UnityEngine;

public class ObjectRotTest : MonoBehaviour
{
    [Header("回転設定")]
    public float angleRange = 180f;     // 回転範囲（例：180度）
    public float speed = 60f;           // 回転速度（度/秒）
    public Vector3 rotationAxis = Vector3.up; // 回転軸（Y軸など）

    [Header("停止設定")]
    public float stopTime = 0.5f;       // 端で停止する時間（秒）

    private float currentAngle = 0f;    // 現在の角度
    private int direction = 1;          // 回転方向（+1 or -1）
    private float stopTimer = 0f;       // 停止タイマー
    private bool isStopping = false;    // 停止中フラグ


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopping)
        {
            // 停止中ならタイマーを進める
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopTime)
            {
                // 停止終了 → 再開
                isStopping = false;
                stopTimer = 0f;
                direction *= -1; // 方向反転
            }
            return;
        }

        // 通常回転
        currentAngle += direction * speed * Time.deltaTime;

        // 範囲チェック
        if (Mathf.Abs(currentAngle) >= angleRange / 2f)
        {
            // 端に到達
            currentAngle = Mathf.Sign(currentAngle) * (angleRange / 2f);
            isStopping = true; // 停止開始
        }

        // 実際の回転を適用
        transform.localRotation = Quaternion.AngleAxis(currentAngle, rotationAxis);
    }
}

