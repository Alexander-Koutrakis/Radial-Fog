using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RadialFog
{
    // This class defines a custom Scriptable Renderer Feature for radial fog rendering.
    public class RadialFogFeature : ScriptableRendererFeature
    {

        public Shader fogShader;
        public Settings settings;
        private Material material;

        // Render pass responsible for rendering the radial fog.
        private RadialFogPass radialFogPass;

        // Called to configure render passes for the renderer.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                radialFogPass.ConfigureInput(ScriptableRenderPassInput.Color);
                radialFogPass.ConfigureInput(ScriptableRenderPassInput.Depth);
                radialFogPass.SetTarget(settings);
                renderer.EnqueuePass(radialFogPass);
            }
        }

      
        // Called when the renderer feature is created.
        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial(fogShader);
            radialFogPass = new RadialFogPass(material);
        }

        // Called when the renderer feature is disposed.
        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }
    }
}