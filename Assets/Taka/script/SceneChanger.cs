using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour

{

    public void GoTitle()

    {

        SceneManager.LoadScene("Title");

    }

    public void GoPlayer()

    {

        SceneManager.LoadScene("Player");

    }

    public void GoResult()

    {

        SceneManager.LoadScene("Result");

    }

}


