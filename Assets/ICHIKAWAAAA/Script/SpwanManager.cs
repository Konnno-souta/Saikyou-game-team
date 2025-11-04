using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnManager : MonoBehaviour
{
    public GameObject sphere1;
    public GameObject sphere2;
    public GameObject sphere3;
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
            float randamZ = Random.Range(-16f, 4f);
            GameObject[] spheres = { sphere1, sphere2, sphere3 };
            GameObject selectedSphere = spheres[Random.Range(0, spheres.Length)];
            Instantiate(selectedSphere, new Vector3(sphPos.x+ Random.Range(-8f, 9f), sphPos.y, sphPos.z), Quaternion.identity);
            yield return new WaitForSeconds(1.0f);
        }

       
    }
}
