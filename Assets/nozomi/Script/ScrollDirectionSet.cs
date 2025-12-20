using UnityEngine;

public class ScrollDirectionSet : MonoBehaviour
{
    private bool scrollLeft = false;
    private bool scrollRight = false;

    public bool scL { get { return scrollLeft; } }
    public bool scR { get { return scrollRight; } }
    float scroolTimer;

    [SerializeField] Tamaire tamaire;
    public static int ballCount = 0;
    private int scrollChangeCount = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollLeft = true;
        scrollRight = false;
        ballCount = 0;
        scroolTimer = 15f;//n<=x<z
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollLeft && ballCount % scrollChangeCount == 0 && ballCount != 0)
        {
            
            scrollRight = true;
            ballCount = 0;
            scrollLeft = false;
        }   
        else if(scrollRight && ballCount % scrollChangeCount == 0 && ballCount != 0)
        {
            scrollLeft = true;
            ballCount = 0;
            scrollRight = false;    
        }
 
    }
}
