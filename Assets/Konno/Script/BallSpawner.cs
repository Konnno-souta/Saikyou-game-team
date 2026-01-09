using System.Collections;
using UnityEngine;
using UnityEngine.WSA;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public bool isPaused = true;

    public float minInterval = 0.3f;
    public float maxInterval = 0.5f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!isPaused)
            {
                SpawnBall();
            }

            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnBall()
    {
        Vector3 pos = new Vector3(Random.Range(-3f, 3f), transform.position.y, 0);
        Instantiate(ballPrefab, pos, Quaternion.identity);
    }
}
