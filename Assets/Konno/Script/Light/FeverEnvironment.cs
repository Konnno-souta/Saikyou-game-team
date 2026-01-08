using UnityEngine;
using UnityEngine.Rendering;

public class FeverEnvironment : MonoBehaviour
{
    public float normalIntensity = 1f;
    public float feverIntensity = 0.2f;

    public void StartFever()
    {
        RenderSettings.ambientIntensity = feverIntensity;
    }

    public void EndFever()
    {
        RenderSettings.ambientIntensity = normalIntensity;
    }
}
