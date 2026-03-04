using UnityEngine;

public class CrystalBallRenderer : MonoBehaviour
{
    public Camera cubeCamera;
    public RenderTexture cubemapTexture;

    void LateUpdate()
    {
        if (cubeCamera != null && cubemapTexture != null)
        {
            cubeCamera.RenderToCubemap(cubemapTexture);
        }
    }
}