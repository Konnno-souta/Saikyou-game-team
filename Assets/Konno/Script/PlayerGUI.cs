using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    public int maxStamina = 100;
    public int currentStamina = 100;

    // �o�[�̃T�C�Y��ʒu�𒲐��ł���悤��
    public Vector2 barPosition = new Vector2(10, 10);
    public Vector2 barSize = new Vector2(200, 25);

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
