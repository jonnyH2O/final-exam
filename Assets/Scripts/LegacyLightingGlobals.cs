using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LegacyLightingGlobals : MonoBehaviour
{
    [SerializeField] Light mainLight;
    [SerializeField] Camera waterCamera;

    void OnEnable()  { EnsureDepthTexture(); Push(); }
    void Update()    { EnsureDepthTexture(); Push(); }

    // The LowPoly water shader's edge-blend samples _CameraDepthTexture. On WebGL the
    // built-in pipeline doesn't render that texture automatically (no screen-space
    // directional shadows), so the shader reads empty depth and blows the surface out
    // to white. Forcing the camera to render a depth texture keeps the water blue.
    void EnsureDepthTexture()
    {
        if (waterCamera == null) waterCamera = Camera.main;
        if (waterCamera == null) return;

        if ((waterCamera.depthTextureMode & DepthTextureMode.Depth) == 0)
            waterCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    void Push()
    {
        if (mainLight == null) mainLight = RenderSettings.sun;
        if (mainLight == null) return;

        Shader.SetGlobalColor("_LightColor0", mainLight.color * mainLight.intensity);
        Shader.SetGlobalVector("_WorldSpaceLightPos0",
            new Vector4(-mainLight.transform.forward.x,
                        -mainLight.transform.forward.y,
                        -mainLight.transform.forward.z, 0f));

        Color ambient = RenderSettings.ambientLight;
        if (RenderSettings.ambientMode != UnityEngine.Rendering.AmbientMode.Flat)
            ambient = RenderSettings.ambientSkyColor;
        Shader.SetGlobalColor("glstate_lightmodel_ambient", ambient * 0.5f);
        Shader.SetGlobalColor("unity_AmbientSky", RenderSettings.ambientSkyColor);
        Shader.SetGlobalColor("unity_AmbientEquator", RenderSettings.ambientEquatorColor);
        Shader.SetGlobalColor("unity_AmbientGround", RenderSettings.ambientGroundColor);
    }
}
