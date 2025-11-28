
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private bool isCamera1Active = true;

    void Start()
    {
        // 最初にカメラ1を有効、カメラ2を無効
        camera1.enabled = true;
        camera2.enabled = false;
    }

    void Update()
    {
        // スペースキーで切り替え
        if (Input.GetKeyDown(KeyCode.R))
        {
            isCamera1Active = !isCamera1Active;
            camera1.enabled = isCamera1Active;
            camera2.enabled = !isCamera1Active;
        }
    }
}