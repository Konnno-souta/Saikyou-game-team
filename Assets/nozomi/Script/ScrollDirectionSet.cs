using UnityEngine;

public class ScrollDirectionSet : MonoBehaviour
{
    private bool scrollLeft = false;
    private bool scrollRight = false;

    public bool scL { get { return scrollLeft; } }
    public bool scR { get { return scrollRight; } }
    float scroolTimer;

    [SerializeField] Tamaire tamaire;
    [SerializeField]fiverManager fivermanager;
    public static int ballCount;
    public static int ballCount2;
    public int BC { get { return ballCount2; } set { ballCount2 = value; } }
    private int scrollChangeCount = 5;
    internal bool isPaused;

    //internal static bool isPaused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollRight = true;
        scrollLeft = false;
        ballCount = 0;
        ballCount2 = 0;
        scroolTimer = 15f;//n<=x<z
    }

    // Update is called once per frame
    void Update()
    {
        if (!fivermanager.IsF)
        {
            if (scrollLeft && ballCount % scrollChangeCount == 0 && ballCount != 0)
            {
                scrollRight = true;
                scrollLeft = false;
                ballCount = 0;
            }
            else if (scrollRight && ballCount % scrollChangeCount == 0 && ballCount != 0)
            {
                scrollLeft = true;
                scrollRight = false;
                ballCount = 0;
            }
        }

        if (fivermanager.IsF)
        {
            ballCount = 0;
            ballCount2 = 0;
        }


    }
}
