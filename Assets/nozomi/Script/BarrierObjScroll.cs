using UnityEngine;

public class BarrierObjScroll : MonoBehaviour
{
    private ScrollDirectionSet sds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sds= GameObject.Find("conveyor").GetComponent<ScrollDirectionSet>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sds.scL)
        {
            transform.position += new Vector3(-0.01f, 0, 0);
        }
        if (sds.scR)
        {
            transform.position += new Vector3(0.01f, 0, 0);
        }
    }
}
