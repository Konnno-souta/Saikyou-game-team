using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    // Inspectorで管理するためのクラス
    [System.Serializable]
    public class BallData
    {
        public GameObject prefab;   // 出すボールのPrefab
        [Range(0, 100)]
        public float probability;   // 出現確率（％）
    }

    // Inspectorに表示される配列
    public BallData[] balls;

    private Vector3 spawnPos;
    internal bool isPaused;

    void Start()
    {
        spawnPos = transform.position;
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            float randamX = Random.Range(-16f, 4f);

            // 確率に基づいてボールを選択
            GameObject selectedBall = GetRandomBall();

            // 生成したオブジェクトを変数に入れる
            GameObject spawned = Instantiate(
                selectedBall,
                new Vector3(randamX, spawnPos.y, spawnPos.z),
                Quaternion.identity
            );

            // ここでログを出す
            Debug.Log("Spawned Ball: " + spawned.name + " (Tag: " + spawned.tag + ")");

            yield return new WaitForSeconds(1.5f);
        }
    }

    // 重み付きランダムでボールを選ぶ
    private GameObject GetRandomBall()
    {
        float total = 0;
        foreach (var b in balls) total += b.probability;

        float randomPoint = Random.value * total;

        foreach (var b in balls)
        {
            if (randomPoint < b.probability)
                return b.prefab;
            else
                randomPoint -= b.probability;
        }

        // 保険で最後のボールを返す
        return balls[balls.Length - 1].prefab;
    }
}