using UnityEngine;

public class ScrollTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position += new Vector3(-0.1f, 0, 0);
    }
}
