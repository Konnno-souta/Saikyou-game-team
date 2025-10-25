using UnityEngine;
using UnityEngine.SceneManagement; // �V�[���������[�h���邽��

public class PlayerDeath : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // �{�[���ɂԂ�������
        if (collision.gameObject.CompareTag("Ball"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("�v���C���[���S�I");
        // �v���C���[������
        gameObject.SetActive(false);

        // 2�b��ɃV�[�������X�^�[�g
        Invoke("RestartScene", 2f);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
