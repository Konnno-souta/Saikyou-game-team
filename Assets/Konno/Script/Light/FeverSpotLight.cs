using UnityEngine;

public class FeverSpotLight : MonoBehaviour
{
    public Transform target;
    public float height = 10f;
    public float followSpeed = 5f;

    void Update()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + Vector3.up * height;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        transform.LookAt(target);
    }
}
