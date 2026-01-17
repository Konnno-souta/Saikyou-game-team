using UnityEngine;
 
public class UvScrollTest : MonoBehaviour
{
    private ScrollDirectionSet sds;
    private ScrollTest2 st2;
    private float scrollSpeedX; // 横方向のスクロール速度

    private Renderer rend;
    private Vector2 offset = Vector2.zero;

    [SerializeField] Material[]mat = new Material[2];
    private FeverManager fivermanager;

    void Start()
    {
        sds = GameObject.Find("conveyor").GetComponent<ScrollDirectionSet>();
        st2 = GameObject.Find("Playermain").GetComponent<ScrollTest2>();
        fivermanager = GameObject.Find("FiverManager").GetComponent<FeverManager>();
        // このオブジェクトのRendererを取得
        rend = GetComponent<Renderer>();
        scrollSpeedX = st2.stN;
    }

    void FixedUpdate()
    {
        scrollSpeedX = st2.stN;
        if (!fivermanager.IsF)
        {
            if (sds.scL)
            {
                offset.x -= (scrollSpeedX * 50) * Time.deltaTime;
                this.GetComponent<MeshRenderer>().material = mat[0];
            }
            if (sds.scR)
            {
                offset.x += (scrollSpeedX * 50) * Time.deltaTime;
                this.GetComponent<MeshRenderer>().material = mat[1];
            }
        }
        // マテリアルのメインテクスチャのオフセットを設定
        rend.material.mainTextureOffset = offset;
    }
}
