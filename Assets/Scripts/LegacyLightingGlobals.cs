using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LegacyLightingGlobals : MonoBehaviour
{
    [SerializeField] Light mainLight;

    void OnEnable()  { Push(); }
    void Update()    { Push(); }

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
