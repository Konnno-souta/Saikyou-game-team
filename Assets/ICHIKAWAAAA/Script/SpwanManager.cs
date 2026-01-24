
using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class BallData
    {
        public GameObject prefab;
        [Range(0, 100)]
        public float probability; // Fever時はこれをそのまま使う
    }

    // ★ 時間帯ステージ（通常用のみ）
    [System.Serializable]
    public class ProbabilityStage
    {
        [Tooltip("この時間(秒)以降にこのステージを有効にする（昇順で並べる）")]
        public float startTimeSec = 0f;

        [Tooltip("normalBalls の並びと同じ長さに。値は重み（合計1.0でなくてOK）")]
        public float[] weights;
    }

    [Header("Normal Balls")]
    public BallData[] normalBalls;

    [Header("Fever Balls")]
    public BallData[] feverBalls;

    [Header("Spawn Settings")]
    [Tooltip("通常時のスポーン間隔（秒）")]
    [Min(0.05f)] public float normalInterval = 1.5f;
    [Tooltip("フィーバー中のスポーン間隔（秒）")]
    [Min(0.02f)] public float feverInterval = 0.5f;
    [Tooltip("Time.timeScaleの影響を受けない")]
    public bool useUnscaledTime = false;

    [Header("Spawn Area")]
    public Vector2 xRange = new Vector2(-16f, 4f);

    // ★ 通常時のみ：時間帯テーブル（ここに4つ作成）
    [Header("Time-based Tables (Normal only)")]
    [Tooltip("通常時のみ使用。フィーバー中は無視（feverBallsの確率で抽選）。")]
    public ProbabilityStage[] normalStages;

    private Vector3 spawnPos;
    private FeverManager feverManager;

    internal bool isPaused;

    // ★ 経過時間（useUnscaledTimeに追従）
    private float elapsed = 0f;

    void Start()
    {
        spawnPos = transform.position;
        feverManager = FindAnyObjectByType<FeverManager>();

        // 念のため開始時に昇順ソート（OnValidateでも実施）
        //SortStages();
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        //// デバッグ用：Fキーでフィーバー発動
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    if (feverManager != null)
        //    {
        //        StartCoroutine(feverManager.FeverSequence());
        //        Debug.Log("フィーバー強制発動");
        //    }
        //    else
        //    {
        //        Debug.LogError("feverManager が見つかりません！");
        //    }
        //}
    }

    private IEnumerator SpawnLoop()
    {
        float timer = 0f;

        while (true)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            timer += dt;
            elapsed += dt; // ★ 経過時間を進める

            float currentInterval = (feverManager != null && feverManager.IsF)
                ? Mathf.Max(0.02f, feverInterval)
                : Mathf.Max(0.05f, normalInterval);

            if (timer >= currentInterval)
            {
                timer -= currentInterval; // 可変間隔でも破綻しにくいように減算

                float randomX = Random.Range(xRange.x, xRange.y);

                GameObject selectedBall = GetRandomBall();
                GameObject spawned = Instantiate(
                    selectedBall,
                    new Vector3(randomX, spawnPos.y, spawnPos.z),
                    Quaternion.identity
                );

                Debug.Log($"Spawned Ball: {spawned.name} (Tag: {spawned.tag})");
            }

            yield return null; // 毎フレーム継続
        }
    }

    // ★ 現在のステージを取得（elapsed に最も近い startTimeSec の最後のもの）
    private ProbabilityStage GetActiveStage()
    {
        if (normalStages == null || normalStages.Length == 0) return null;
        ProbabilityStage current = null;
        for (int i = 0; i < normalStages.Length; i++)
        {
            var s = normalStages[i];
            if (s == null) continue;
            if (elapsed >= s.startTimeSec) current = s;
            else break;
        }
        return current;
    }

    // ★ 正規化不要の重み抽選（合計0ならfalse）
    private bool TryPickByWeights(BallData[] candidates, float[] weights, out GameObject prefab)
    {
        prefab = null;
        if (candidates == null || weights == null) return false;
        if (weights.Length != candidates.Length) return false;

        float total = 0f;
        for (int i = 0; i < weights.Length; i++)
            total += Mathf.Max(0f, weights[i]);

        if (total <= 0f) return false;

        float r = Random.value * total;
        for (int i = 0; i < candidates.Length; i++)
        {
            float w = Mathf.Max(0f, weights[i]);
            if (r < w) { prefab = candidates[i].prefab; return true; }
            r -= w;
        }
        prefab = candidates[candidates.Length - 1].prefab;
        return true;
    }

    // ★ 通常時はステージの重み、フィーバー時は従来のprobability
    private GameObject GetRandomBall()
    {
        bool isFever = (feverManager != null && feverManager.IsF);

        if (isFever)
        {
            // --- Fever: 従来のprobabilityで抽選 ---
            BallData[] target = feverBalls;
            float total = 0;
            foreach (var b in target) total += b.probability;

            if (total <= 0f)
            {
                Debug.LogWarning("Fever: 確率の合計が0です。最後のボールを返します。");
                return target[target.Length - 1].prefab;
            }

            float randomPoint = Random.value * total;
            foreach (var b in target)
            {
                if (randomPoint < b.probability) return b.prefab;
                randomPoint -= b.probability;
            }
            return target[target.Length - 1].prefab;
        }
        else
        {
            // --- Normal: ステージがあればステージ重み、なければ従来probability ---
            var stage = GetActiveStage();
            if (stage != null && stage.weights != null && stage.weights.Length == normalBalls.Length)
            {
                if (TryPickByWeights(normalBalls, stage.weights, out var pf))
                    return pf;

                Debug.LogWarning("Normal: ステージ重み合計が0、もしくは不正。従来のprobabilityで抽選します。");
            }

            // フォールバック：従来のprobability
            float total = 0;
            foreach (var b in normalBalls) total += b.probability;

            if (total <= 0f)
            {
                Debug.LogWarning("Normal: 確率の合計が0です。最後のボールを返します。");
                return normalBalls[normalBalls.Length - 1].prefab;
            }

            float randomPoint = Random.value * total;
            foreach (var b in normalBalls)
            {
                if (randomPoint < b.probability) return b.prefab;
                randomPoint -= b.probability;
            }
            return normalBalls[normalBalls.Length - 1].prefab;
        }
    }

#if UNITY_EDITOR
    // インスペクター調整時に軽く整える（ステージ昇順・配列長合わせ）
    private void OnValidate()
    {
        SortStages();

        // 各ステージのweights長を normalBalls 長に合わせる（足りない分を0で拡張）
        if (normalStages != null && normalBalls != null)
        {
            int n = normalBalls.Length;
            for (int i = 0; i < normalStages.Length; i++)
            {
                if (normalStages[i] == null) continue;
                if (normalStages[i].weights == null)
                {
                    normalStages[i].weights = new float[n];
                }
                else if (normalStages[i].weights.Length != n)
                {
                    var arr = new float[n];
                    int copy = Mathf.Min(n, normalStages[i].weights.Length);
                    for (int k = 0; k < copy; k++) arr[k] = normalStages[i].weights[k];
                    // 足りない部分は0（＝抽選されない）
                    normalStages[i].weights = arr;
                }
            }
        }
    }

    private void SortStages()
    {
        if (normalStages == null || normalStages.Length <= 1) return;
        System.Array.Sort(normalStages, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            return a.startTimeSec.CompareTo(b.startTimeSec);
        });
    }
#endif
}
