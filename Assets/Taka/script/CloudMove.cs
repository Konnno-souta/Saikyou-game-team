using UnityEngine;

public class CloudMove : MonoBehaviour
{
    public float speed = 0.5f;  // 移動速度
    private float width;         // 雲1枚の横幅
    private Vector3 startPos;    // 初期位置

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        //width = sr.bounds.size.x;  // 画像サイズから自動取得
        startPos = transform.position; // 中央スタート位置を記録
    }

    void Update()
    {
        // 右に移動
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 右端を超えたら、左に2枚分戻す
        if (transform.position.x >= startPos.x + width)
        {
            transform.position -= new Vector3(width * 2f, 0f, 0f);
        }
    }
}
