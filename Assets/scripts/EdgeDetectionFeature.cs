using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetectionFeature : ScriptableRendererFeature
{
    class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material edgeDetectionMaterial;
        private RenderTargetHandle tempTexture;
        private string profilerTag;

        public EdgeDetectionPass(Material material, string tag)
        {
            edgeDetectionMaterial = material;
            profilerTag = tag;
            tempTexture.Init("_TempTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Create a temporary render texture
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor, FilterMode.Bilinear);
            ConfigureTarget(tempTexture.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (edgeDetectionMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            // Apply edge detection shader
            Blit(cmd, source, tempTexture.Identifier(), edgeDetectionMaterial);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    public Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    public override void Create()
    {
        edgeDetectionPass = new EdgeDetectionPass(edgeDetectionMaterial, "Edge Detection Pass");
        edgeDetectionPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (edgeDetectionMaterial == null) return;

        // Add the pass with proper setup
        renderer.EnqueuePass(edgeDetectionPass);
    }
}
