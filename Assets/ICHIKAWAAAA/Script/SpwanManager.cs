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

   [Header("Normal Balls")]
    public BallData[] normalBalls;

    [Header("Fever Balls")]
    public BallData[] feverBalls;


    private Vector3 spawnPos;
    private feverManager feverManager;

    internal bool isPaused;

    void Start()
    {
        spawnPos = transform.position;
        feverManager = FindAnyObjectByType<FeverManager>();

        StartCoroutine(SpawnCoroutine());
    }
    void Update()
    {
        // デバッグ用：Fキーでフィーバー発動
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (feverManager != null)
            {
                StartCoroutine(feverManager.FeverSequence());
                Debug.Log("フィーバー強制発動");
            }
            else
            {
                Debug.LogError("feverManager が見つかりません！");
            }
        }
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

    // ランダムでボールを選ぶ
    private GameObject GetRandomBall()
    {
        //フィーバー中はフィーバーボールスを使います
        BallData[] target = feverManager.IsF ? feverBalls : normalBalls;

        float total = 0;
        foreach (var b in target) total += b.probability;

        float randomPoint = Random.value * total;

        foreach (var b in target)
        {
            if (randomPoint < b.probability)
                return b.prefab;
            else
                randomPoint -= b.probability;
        }

        // 保険で最後のボールを返す
        return target[target.Length - 1].prefab;
    }
}