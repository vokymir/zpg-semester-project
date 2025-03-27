using OpenTK.Graphics.OpenGL;

namespace zpg
{
    class Floor : RenderObject
    {
        public Floor(Shader shader, Camera camera, float width, float depth) : base(shader, camera,
                new float[]{
                    0, 0, 0,
                    width, 0, 0,
                    width, 0, depth,
                    0, 0, depth

                }, new uint[]{
                    2, 1, 0,
                    3, 2, 0
                }, "./Textures/container2.png", "./Textures/container2_specular.png")
        { }

        PolygonMode previousPolygonMode;

        protected override void PreRender()
        {
            base.PreRender();
            GL.GetInteger(GetPName.PolygonMode, out int previousMode);
            previousPolygonMode = (PolygonMode)previousMode;
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        }

        protected override void PostRender()
        {
            base.PostRender();
            GL.PolygonMode(TriangleFace.FrontAndBack, previousPolygonMode);

        }
    }
}
