using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnManager : MonoBehaviour
{
    public GameObject sphere1;
    public GameObject sphere2;
    public GameObject sphere3;
    int num = 0;

    void Start()
    {
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
            Instantiate(selectedSphere, new Vector3(-3, 8, randamZ), Quaternion.identity);
            yield return new WaitForSeconds(1.0f);
        }

       
    }
}
