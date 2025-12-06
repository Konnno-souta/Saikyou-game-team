using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnManager : MonoBehaviour
{
    public GameObject RedBall;
    public GameObject BlueBall;
    public GameObject GreenBall;
    public GameObject Bom;
    public GameObject SpeedUpBall;
    public GameObject SpeedDownBall; 
    public GameObject JumpUpBall;
    public GameObject JumpDownBall;
    public GameObject InvincibleBall;
    public GameObject BigBasketBall;
    public GameObject MinusScoreBall;
    public GameObject MinusTimeBall;

    int num = 0;
    private Vector3 sphPos;

    void Start()
    {
        sphPos = transform.position;
        StartCoroutine(SpawnCoroutine());

        Debug.Log("SpawnManager Start called");

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator SpawnCoroutine()
    {
        Debug.Log("SpawnCoroutine started");
        while (true)
        {
            float randamX = Random.Range(-16f, 4f);
            GameObject[] spheres = { RedBall, BlueBall, GreenBall, Bom, SpeedUpBall, SpeedDownBall, JumpUpBall, JumpDownBall, InvincibleBall, BigBasketBall, MinusScoreBall, MinusTimeBall };
            GameObject selectedSphere = spheres[Random.Range(0, spheres.Length)];
            Instantiate(selectedSphere, new Vector3(randamX, sphPos.y, sphPos.z), Quaternion.identity);
            yield return new WaitForSeconds(5.0f);
        }

       
    }
}
