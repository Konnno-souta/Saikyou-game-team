using UnityEngine;
 
public class UvScrollTest : MonoBehaviour
{
    private ScrollDirectionSet sds;
    private float scrollSpeedX = 2.5f; // 横方向のスクロール速度
    private float scrollSpeedY = 0.0f; // 縦方向のスクロール速度

    private Renderer rend;
    private Vector2 offset = Vector2.zero;

    [SerializeField] Material[]mat = new Material[2];

    void Start()
    {
        sds = GameObject.Find("conveyor").GetComponent<ScrollDirectionSet>();
        // このオブジェクトのRendererを取得
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (sds.scL)
        {
            offset.x -= scrollSpeedX * Time.deltaTime;
            offset.y += scrollSpeedY * Time.deltaTime;
            this.GetComponent<MeshRenderer>().material=mat[0];
        }
        if (sds.scR)
        {
            offset.x += scrollSpeedX * Time.deltaTime;
            offset.y += scrollSpeedY * Time.deltaTime;
            this.GetComponent<MeshRenderer>().material = mat[1];
        }
        // マテリアルのメインテクスチャのオフセットを設定
        rend.material.mainTextureOffset = offset;
    }
}
