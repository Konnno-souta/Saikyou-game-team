using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour

{
    [SerializeField, Header("数値")]
    int score;

    string[] rnking = { "ランキング1位", "ランキング2位", "ランキング3位", "ランキング4位", "ランキング5位" };
    int[] rankingVlue = new int[5];
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
