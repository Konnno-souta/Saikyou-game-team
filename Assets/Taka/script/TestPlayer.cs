using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public ScoreManager scoreManager;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            scoreManager.AddScore(10);
        }
    }
}
