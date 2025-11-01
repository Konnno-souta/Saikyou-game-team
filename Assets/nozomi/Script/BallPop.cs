using UnityEngine;

public class BallPop : MonoBehaviour
{
    [SerializeField] GameObject Ball;
    private int timer = 0;//n<=x<m
    Vector3 ballPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballPos = transform.position;
        timer = Random.Range(60, 180);
}

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= 1;
        if (timer <= 0)
        {
            Instantiate(Ball, ballPos, Quaternion.identity);
            timer = Random.Range(60, 180);
        }
    }
}
