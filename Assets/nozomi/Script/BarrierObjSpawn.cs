using UnityEngine;

public class BarrierObjSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] barrier=new GameObject[3] ;
    [SerializeField] GameObject barrierSpawnLeft;//ç∂
    [SerializeField] GameObject barrierSpawnRight;//âE
    [SerializeField] ScrollDirectionSet sds;
    private float popTimer;
    private int randBarrier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popTimer = 0f;
        randBarrier = Random.Range(0,3);
    }

    // Update is called once per frame
    void Update()
    {
        popTimer -= Time.deltaTime;

        if (sds.scL&& popTimer <= 0)
        {
            Instantiate(barrier[randBarrier], barrierSpawnRight.transform.position, Quaternion.identity);
            popTimer = 15f;
            randBarrier = Random.Range(0, 3);
        }
        if (sds.scR&& popTimer <= 0)
        {
            Instantiate(barrier[randBarrier], barrierSpawnLeft.transform.position, Quaternion.identity);
            popTimer = 15f;
            randBarrier = Random.Range(0, 3);
        }

    }
}
