using UnityEngine;

public class Ball2 : MonoBehaviour
{
    public enum BallType
    {
        SpeedUp,        // 一時的に速度アップ
        SpeedDown,      // 一時的に速度ダウン
        JumpUp,         // 一時的にジャンプ力アップ
        JumoDown,       // 一時的にジャンプ力ダウン
        Invincible,     // ２回だけダメージ無効
        BigBasket,      // 一時的にカゴでかくなる
        MinusScore,     // スコア減る
        MinusTime,      // タイム減る
        Bom,            // 動けなくなるやつ
     }

    [Header("ボール設定")]
    public BallType ballType = BallType.SpeedUp;

    [Tooltip("効果量（速度+2など）")]
    public float value = 2f;

    [Tooltip("一時効果の継続時間（秒）")]
    public float duration = 5f;

    [HideInInspector] public bool isCollected = false;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }
}
