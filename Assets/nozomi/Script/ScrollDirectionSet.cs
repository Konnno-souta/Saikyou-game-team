using UnityEngine;

public class ScrollDirectionSet : MonoBehaviour
{
    private bool scrollLeft = false;
    private bool scrollRight = false;

    public bool scL { get { return scrollLeft; } }
    public bool scR { get { return scrollRight; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        scrollLeft = true;
        scrollRight = false;


        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    scrollLeft = true;
        //    scrollRight = false;
        //}

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    scrollLeft = false;
        //    scrollRight = true;
        //}
    }
}
