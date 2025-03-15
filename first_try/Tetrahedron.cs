using OpenTK.Graphics.OpenGL;

namespace zpg
{
    class Tetrahedron : RenderObject
    {

        public Tetrahedron(Shader shader) : base(shader,
            new float[]
            {
            -0.7f, 0, 0.7f,
            0.7f, 0, 0.7f,
            0, 0, -1,
            0, 1, 0
            },
            new uint[]
            {
            0, 1, 2,
            0, 3, 1,
            0, 2, 3,
            1, 3, 2
            })
        {
        }

        PolygonMode previousPolygonMode;

        protected override void PreRender()
        {
            base.PreRender();
            GL.GetInteger(GetPName.PolygonMode, out int previousMode);
            previousPolygonMode = (PolygonMode)previousMode;
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        }

        protected override void PostRender()
        {
            base.PostRender();
            GL.PolygonMode(TriangleFace.FrontAndBack, previousPolygonMode);

        }
    }
}
