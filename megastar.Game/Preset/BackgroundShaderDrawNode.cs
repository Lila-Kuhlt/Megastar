using System.Runtime.InteropServices;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Shapes;

namespace megastar.Game.Preset;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal record struct BackgroundShaderParameter
{
    public UniformFloat Time; // 4 Byte

    // std140 requires 16-byte alignment.
    // Padding ensures the struct aligns perfectly on the GPU.
    private readonly UniformPadding4 pad; // 4 Byte

    public UniformVector2 Resolution; // 8 Byte
}

public partial class ShaderBackground(string fsShader) : Box
{
    private IShader shader = null!;

    [BackgroundDependencyLoader]
    private void load(ShaderManager shaderManager)
    {
        shader = shaderManager.Load(VertexShaderDescriptor.TEXTURE_2, fsShader);
        RelativeSizeAxes = Axes.Both;
    }

    protected override DrawNode CreateDrawNode() => new BackgroundShaderDrawNode(this, shader);
}

internal class BackgroundShaderDrawNode(IDrawable node, IShader shader) : DrawNode(node)
{
    [CanBeNull] private IUniformBuffer<BackgroundShaderParameter> uniformBuffer;

    private Quad screenSpaceDrawQuad;

    public override void ApplyState()
    {
        base.ApplyState();

        if (Source is not ShaderBackground sourceBackground) return;
        screenSpaceDrawQuad = sourceBackground.ScreenSpaceDrawQuad;
    }

    protected override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);
        uniformBuffer ??= renderer.CreateUniformBuffer<BackgroundShaderParameter>();

        uniformBuffer.Data = uniformBuffer.Data with
        {
            Time = new UniformFloat { Value = (float)Source.Time.Current / 1000f },
            Resolution = new UniformVector2 { X = screenSpaceDrawQuad.Width, Y = screenSpaceDrawQuad.Height }
        };

        shader.BindUniformBlock("m_Parameters", uniformBuffer);
        shader.Bind();

        renderer.DrawQuad(renderer.WhitePixel, screenSpaceDrawQuad, DrawColourInfo.Colour);

        shader.Unbind();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        uniformBuffer?.Dispose();
    }
}
