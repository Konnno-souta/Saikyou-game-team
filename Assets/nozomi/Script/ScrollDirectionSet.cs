using UnityEngine;

public class ScrollDirectionSet : MonoBehaviour
{
    private bool scrollLeft = false;
    private bool scrollRight = false;

    public bool scL { get { return scrollLeft; } }
    public bool scR { get { return scrollRight; } }

    float scroolTimer;

    [SerializeField] Tamaire tamaire;
    private int ballCount;
    private int scrollChangeCount = 2;

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
        if (tamaire.bC == true)
        {
            ballCount++;
        }

        if (scrollLeft && ballCount % scrollChangeCount == 0)
        {
            scrollLeft = false;
            scrollRight = true;
            ballCount = 0;
        }

        if (scrollRight && ballCount % scrollChangeCount == 0)
        {
            scrollLeft = true;
            scrollRight = false;
            ballCount = 0;
        }
 
    }
}
