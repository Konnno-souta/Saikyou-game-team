using UnityEngine;

public class fiverManager : MonoBehaviour
{
    public GameObject feverTextPrefab;
    public Canvas canvas;

    bool fiverOn;

    public void StartFever()
    {
        if (fiverOn) return;

        fiverOn = true;

        Instantiate(feverTextPrefab, canvas.transform);
    }

    public void EndFever()
    {
        fiverOn = false;
    }
}
