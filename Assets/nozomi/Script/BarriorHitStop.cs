using UnityEngine;

public class BarriorHitStop : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb == null) return;

        // 押し返す方向（障害物 → プレイヤー）
        Vector3 normal = collision.contacts[0].normal;
        normal.y = 0f;

        // 物理的に押し返す
        playerRb.AddForce(normal * 30f, ForceMode.VelocityChange);
    }
}
