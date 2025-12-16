using UnityEngine;

public class BarrierObjSpawn : MonoBehaviour
{
    [SerializeField] GameObject barrier;
    [SerializeField] GameObject barrierSpawnLeft;//ç∂
    [SerializeField] GameObject barrierSpawnRight;//âE
    [SerializeField] ScrollDirectionSet sds;
    private float popTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        popTimer -= Time.deltaTime;

        if (sds.scL&& popTimer <= 0)
        {
            Instantiate(barrier, barrierSpawnRight.transform.position, Quaternion.identity);
            popTimer = 15f;
        }
        if (sds.scR&& popTimer <= 0)
        {
            Instantiate(barrier, barrierSpawnLeft.transform.position, Quaternion.identity);
            popTimer = 15f;
        }

    }
}
