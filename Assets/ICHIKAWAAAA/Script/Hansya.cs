using UnityEngine;

public class Hansya : MonoBehaviour
{
    public float bounceForce = 10f;  // バンパーの反発力

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // 衝突点の法線方向に力を加える
                Vector3 forceDirection = collision.contacts[0].normal;
                ballRb.AddForce(forceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}