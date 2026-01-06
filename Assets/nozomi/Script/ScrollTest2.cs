using UnityEngine;
using UnityEngine.UI;

public class ScrollTest2 : MonoBehaviour
{
    [SerializeField] FloorHitCheck fhc;
    [SerializeField] ScrollDirectionSet sds;
    Vector3 scrollL;
    Vector3 scrollR;
    public Vector3 scSL { get { return scrollL; } }
    public Vector3 scSR { get { return scrollR; } }

    int scrollSpeedUp = 1;
    float n;
    public float stN {  get { return n; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        n = 0.05f;
        scrollL = new Vector3(-n, 0, 0);
        scrollR = new Vector3(n, 0, 0);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (fhc.fHCheck)
        {
            if (sds.scL)
            {
                transform.position += scrollL;
            }
            if (sds.scR)
            {
                transform.position += scrollR;
            }
        }

        if (sds.BC % scrollSpeedUp == 0 && sds.BC != 0)
        {
            n += 0.02f;
            scrollL = new Vector3(-n, 0, 0);
            scrollR = new Vector3(n, 0, 0);
            sds.BC = 0;
        }

    }
}
