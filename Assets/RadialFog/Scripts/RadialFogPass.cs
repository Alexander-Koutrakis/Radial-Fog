using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RadialFog
{
    public class RadialFogPass : ScriptableRenderPass
    {
        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("RadialFog");
        
        private Material fogMaterial;

        [NonSerialized]
        private Vector3[] frustumCorners = new Vector3[4];
        [NonSerialized]
        private Vector4[] vectorArray = new Vector4[4];
        private Settings settings;
        
        private int fogID = Shader.PropertyToID("_Fog");
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier temp;

        public RadialFogPass(Material material)
        {
            fogMaterial = material;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }


        public void SetTarget(Settings settings)
        {
            this.settings = settings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            this.source = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(fogID, desc, FilterMode.Bilinear);
            temp = new RenderTargetIdentifier(fogID);
        }


        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(fogID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            Camera camera = cameraData.camera;


            if (camera.cameraType != CameraType.Game)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {

                camera.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), camera.farClipPlane, camera.stereoActiveEye, frustumCorners);
                vectorArray[0] = frustumCorners[0];
                vectorArray[1] = frustumCorners[3];
                vectorArray[2] = frustumCorners[1];
                vectorArray[3] = frustumCorners[2];


                if (settings.FogMode == FogMode.Linear)
                {
                    fogMaterial.SetFloat("_Start", settings.StartDistance);
                    fogMaterial.SetFloat("_End", settings.EndDistance);
                    fogMaterial.EnableKeyword("FOG_LINEAR");
                    fogMaterial.DisableKeyword("FOG_EXPONENTIAL");
                    fogMaterial.DisableKeyword("FOG_EXPONENTIAL_SQUARED");
                }
                else if (settings.FogMode == FogMode.Exponential)
                {
                    fogMaterial.SetFloat("_Density", settings.Density);
                    fogMaterial.DisableKeyword("FOG_LINEAR");
                    fogMaterial.EnableKeyword("FOG_EXPONENTIAL");
                    fogMaterial.DisableKeyword("FOG_EXPONENTIAL_SQUARED");
                }
                else if (settings.FogMode == FogMode.Exponential_Squared)
                {
                    fogMaterial.SetFloat("_Density", settings.Density);
                    fogMaterial.DisableKeyword("FOG_LINEAR");
                    fogMaterial.DisableKeyword("FOG_EXPONENTIAL");
                    fogMaterial.EnableKeyword("FOG_EXPONENTIAL_SQUARED");
                }

                fogMaterial.SetColor("_Color", settings.Color);
                fogMaterial.SetVectorArray("_FrustumCorners", vectorArray);

                Blit(cmd, source, temp, fogMaterial, 0);
                Blit(cmd, temp, source);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
}