using UnityEngine;

public class ScrollTest2 : MonoBehaviour
{
    [SerializeField] FloorHitCheck fhc;
    [SerializeField] ScrollDirectionSet sds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (fhc.fHCheck)
        {
            if (sds.scL)
            {
                transform.position += new Vector3(-0.05f, 0, 0);
            }
            if (sds.scR)
            {
                transform.position += new Vector3(0.05f, 0, 0);
            }
        }

    }
}
