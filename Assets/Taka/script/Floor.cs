using UnityEngine;

public class Floor : MonoBehaviour
{
    private bool floorHit;
    public bool fHCheck { get { return floorHit; } }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            floorHit = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            floorHit = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorHit = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

}