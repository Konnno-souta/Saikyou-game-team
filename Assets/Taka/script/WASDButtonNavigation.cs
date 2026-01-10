using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WASDButtonNavigation : MonoBehaviour
{
    public Button[] buttons;
    private int currentIndex = 0;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Move(1);

        if (Input.GetKeyDown(KeyCode.W))
            Move(-1);
        if (Input.GetKeyDown(KeyCode.D))
            Move(1);

        if (Input.GetKeyDown(KeyCode.A))
            Move(-1);
    }

    void Move(int dir)
    {
        currentIndex += dir;

        if (currentIndex < 0)
            currentIndex = buttons.Length - 1;
        if (currentIndex >= buttons.Length)
            currentIndex = 0;

        EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
    }
}
