namespace zpg;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

class WhiteScreen : RenderObject
{
    public static float Xover2 { get; set; } = 10;
    public static float Yover2 { get; set; } = 10;
    public static float Z { get; set; } = 1f;

    public WhiteScreen(Camera camera) : base(new Shader("./Shaders/fadeToWhite.vert", "./Shaders/fadeToWhite.frag"), camera,
            [
            -1, -1, 0,        0f, 0f, 1f,     0f, 0f,
            1, -1, 0,         0f, 0f, 1f,     1f, 0f,
            1, 1, 0,          0f, 0f, 1f,     1f, 1f,
            -1, 1, 0,         0f, 0f, 1f,     0f, 1f
            ],
            [
            0,1,2,
            2,3,0
            ],
            "./Textures/white.png", "./Textures/white.png")
    {
        CollisionCube.IsActive = false;
        Transform.Scale = new Vector3(Xover2, Yover2, 1);
    }

    public override void Render(DirectionalLight dirLight, List<PointLight> pointLights, SpotLight spotlight)
    {
        Shader.Use();

        Shader.SetMatrix4("projection", _camera.ProjectionMatrix);

        Shader.SetFloat("Xover2", Xover2);
        Shader.SetFloat("Yover2", Yover2);
        Shader.SetFloat("Zdist", Z);

        Shader.SetFloat("alpha", 0.5f);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        PreRender();
        Draw();
        PostRender();

        GL.Disable(EnableCap.Blend); PostRender();
    }
}
