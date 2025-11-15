using UnityEngine;
 
public class UvScrollTest : MonoBehaviour
{
    [SerializeField]ScrollDirectionSet sds;
    public float scrollSpeedX = 0.1f; // 横方向のスクロール速度
    public float scrollSpeedY = 0.0f; // 縦方向のスクロール速度

    private Renderer rend;
    private Vector2 offset = Vector2.zero;

    void Start()
    {
      
        // このオブジェクトのRendererを取得
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (sds.scL)
        {
            offset.x -= scrollSpeedX * Time.deltaTime;
            offset.y += scrollSpeedY * Time.deltaTime;
        }
        if (sds.scR)
        {
            offset.x += scrollSpeedX * Time.deltaTime;
            offset.y += scrollSpeedY * Time.deltaTime;
        }
        // マテリアルのメインテクスチャのオフセットを設定
        rend.material.mainTextureOffset = offset;
    }
}
