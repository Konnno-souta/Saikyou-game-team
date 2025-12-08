using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    public int maxStamina = 100;
    public int currentStamina = 100;

    // バーのサイズや位置を調整できるように
    public Vector2 barPosition = new Vector2(10, 10);
    public Vector2 barSize = new Vector2(200, 25);

    void OnGUI()
    {
        // 背景バー
        GUI.Box(new Rect(barPosition.x, barPosition.y, barSize.x, barSize.y), "");

        // 比率計算
        float ratio = currentStamina / maxStamina;
        float filledWidth = barSize.x * ratio;

        // 残量に応じて色変更（30%以下で赤）
        GUI.color = ratio < 0.3f ? Color.red : Color.green;

        // スタミナゲージ
        GUI.Box(new Rect(barPosition.x, barPosition.y, filledWidth, barSize.y), GUIContent.none);

        // 文字
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
