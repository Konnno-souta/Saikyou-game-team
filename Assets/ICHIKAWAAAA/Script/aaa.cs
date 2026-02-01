using System;
using System.Collections;
using System.Collections.Generic;
//using NUnit;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;


public class aaa : MonoBehaviour
{

  
    private Rigidbody rb;
    bool move;
    private float sideMoveSmooth;
    public bool Move { get { return move; } }



    //public float baseSp { get { return baseSpeed; } }[Header("Ball Effect UI")]
    [SerializeField] private Image ballEffectImage;

    [System.Serializable]
    public class BallImagePair
    {
        public string tag;
        public Sprite sprite;
    }

    [SerializeField] private BallImagePair[] ballImages;
    private Dictionary<string, Sprite> ballImageDict;
    //void Start()
    //{
    //    stepDistance = pTR.baseSp;
    //    targetPos = transform.position;
    //    msFirst = moveSpeed;

    //    rb = GetComponent<Rigidbody>();
    //    spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer?擾
    //    rb.freezeRotation = true; // ?S??]?????
    //}


    // かご（スケールを変える対象）
    [Header("Basket (Goal)")]
    [SerializeField] private Transform basket;              // 拡大したいオブジェクトの Transform
    [SerializeField] private float basketScaleMultiplier = 1.5f; // 拡大倍率
    [SerializeField] private float basketScaleTweenTime = 0.2f;  // 拡大・縮小の補間時間

    // 効果のレイヤー（系統）分類
    private Vector3 basketBaseScale;
    private Coroutine basketScaleRoutine;

    [Header("Control Effects")]
    [SerializeField] private bool reverseControls = false; // デバフとみなすタグ

    [Header("Bom Effect")]
    [SerializeField] private GameObject bomEffectPrefab;
    [SerializeField] private Vector3 bomOffset = Vector3.zero; // 少し上に出したい場合

    [Header("SE")]
    [SerializeField] private AudioSource jumpSE;

    // バリア浜崎使ってね
    [Header("Invincible Barrier")]
    [SerializeField] private GameObject invincibleBarrier;


    public class AutoDestroy : MonoBehaviour
    {
        public float lifeTime = 0.5f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }

    // Control 系の合成状態
    private int lockCount = 0;      // 1以上で操作不可
    private int reverseCount = 0;   // 奇数で反転、偶数で通常

    private bool isGrounded = false;

  
    [SerializeField] private float ballImageShowTime = 0.3f;
    private Coroutine ballImageRoutine;

    private IEnumerator ShowBallImageBom(Sprite sprite)
    {
        ballEffectImage.sprite = sprite;
        ballEffectImage.enabled = true;

        RectTransform rect = ballEffectImage.rectTransform;
        CanvasGroup cg = ballEffectImage.GetComponent<CanvasGroup>();
        if (cg == null) cg = ballEffectImage.gameObject.AddComponent<CanvasGroup>();

        // 初期状態
        rect.localScale = Vector3.zero;
        cg.alpha = 1f;

        // ① ポン！と出る
        float t = 0f;
        float popTime = 0.15f;
        while (t < popTime)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1.2f, t / popTime);
            rect.localScale = Vector3.one * s;
            yield return null;
        }

        // ② 少し戻す（弾み）
        t = 0f;
        float settleTime = 0.1f;
        Vector3 start = rect.localScale;
        while (t < settleTime)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(start, Vector3.one, t / settleTime);
            yield return null;
        }

        // ③ フェードアウト
        t = 0f;
        float fadeTime = 0.4f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            rect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, t / fadeTime);
            yield return null;
        }

        ballEffectImage.enabled = false;
    }



  

    private void BuildBallImageDict()
    {
        ballImageDict = new Dictionary<string, Sprite>(StringComparer.Ordinal);

        foreach (var pair in ballImages)
        {
            if (!ballImageDict.ContainsKey(pair.tag))
            {
                ballImageDict.Add(pair.tag, pair.sprite);
            }
        }

        if (ballEffectImage != null)
            ballEffectImage.enabled = false;
    }

    private void ChangeBallImage(string tag)
    {
        if (ballEffectImage == null) return;

        if (ballImageDict.TryGetValue(tag, out var sprite))
        {
            if (ballImageRoutine != null)
                StopCoroutine(ballImageRoutine);

            if (tag == "Bom")
            {
                ballImageRoutine = StartCoroutine(ShowBallImageBom(sprite));
            }
            else
            {
                ballImageRoutine = StartCoroutine(ShowBallImageOnce(sprite));
            }
        }
    }

    private IEnumerator ShowBallImageOnce(Sprite sprite)
    {
        ballEffectImage.sprite = sprite;
        ballEffectImage.enabled = true;

        yield return new WaitForSeconds(ballImageShowTime);

        ballEffectImage.enabled = false;
    }

    

 
  
}
