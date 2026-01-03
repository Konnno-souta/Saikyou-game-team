using UnityEngine;

public class ScrollTest2 : MonoBehaviour
{
    [SerializeField] FloorHitCheck fhc;
    [SerializeField] ScrollDirectionSet sds;
    Vector3 scrollL;
    Vector3 scrollR;
    int scrollSpeadUp = 3;
    float n;
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

        if (sds.BC % scrollSpeadUp == 0 && sds.BC != 0)
        {
            n += 0.1f;
            scrollL = new Vector3(-n, 0, 0);
            scrollR = new Vector3(n, 0, 0);
            sds.BC = 0;
        }

    }
}
