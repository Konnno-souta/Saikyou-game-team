using UnityEngine;

public class ScrollDirectionSet : MonoBehaviour
{
    private bool scrollLeft = false;
    private bool scrollRight = false;

    public bool scL { get { return scrollLeft; } }
    public bool scR { get { return scrollRight; } }

    float scroolTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollLeft = true;
        scrollRight = false;

        scroolTimer = 15f;//n<=x<z
    }

    // Update is called once per frame
    void Update()
    {
        scroolTimer-=Time.deltaTime;
  
        if (scrollLeft && scroolTimer <= 0)
        {
            scrollLeft = false;
            scrollRight = true;
            scroolTimer = 15f;
        }

        if (scrollRight && scroolTimer <= 0)
        {
            scrollLeft = true;
            scrollRight = false;
            scroolTimer = 15f;
        }
 
    }
}
