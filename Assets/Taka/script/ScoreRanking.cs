using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour

{
    [SerializeField, Header("���l")]
    int score;

    string[] rnking = { "�����L���O1��", "�����L���O2��", "�����L���O3��", "�����L���O4��", "�����L���O5��" };
    int[] rankingVlue = new int[5];
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
