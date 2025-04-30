namespace zpg;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

class WhiteScreen : RenderObject
{
    // how big it is
    public static float Xover2 { get; set; } = 10f;
    public static float Yover2 { get; set; } = 10f;
    public static float Z { get; set; } = 1f;
    // how visible it is
    public float Alpha { get; set; } = 0.0f;

    // if teleporting is active
    public bool Teleporting { get; set; } = false;
    // how long did the animation ran for
    public int Elapsed { get; set; } = 0;
    // how long the animation will be
    public int DurationMs { get; set; } = 1000;

    // create a white rectangle
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
            "./Textures/White.png", "./Textures/White.png")
    {
        CollisionCube.IsActive = false;
        Transform.Position = new Vector3(-1000, -1000, -1000); // big numbers to avoid being with other objects in grid
        Transform.Scale = new Vector3(Xover2, Yover2, 1); // stretch the whole screen
    }

    public override void Render(DirectionalLight dirLight, List<PointLight> pointLights, SpotLight spotlight)
    {
        // don't draw when not teleporting
        if (!Teleporting)
            return;

        Shader.Use();

        Shader.SetMatrix4("projection", _camera.ProjectionMatrix);

        Shader.SetFloat("Xover2", Xover2);
        Shader.SetFloat("Yover2", Yover2);
        Shader.SetFloat("Zdist", Z);

        Shader.SetFloat("alpha", Alpha);

        GL.Enable(EnableCap.Blend); // allow alpha-blending
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        PreRender();
        Draw();
        PostRender();

        GL.Disable(EnableCap.Blend);
    }
}
