using UnityEngine;

public class BarrierObjScroll : MonoBehaviour
{
    private ScrollDirectionSet sds;
    private ScrollTest2 st2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sds= GameObject.Find("conveyor").GetComponent<ScrollDirectionSet>();
        st2=GameObject.Find("Playermain").GetComponent<ScrollTest2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sds.scL)
        {
            transform.position += st2.scSL;
        }
        if (sds.scR)
        {
            transform.position +=st2.scSR;
        }
    }
}
