using OpenTK.Graphics.OpenGL;

namespace zpg
{
    class Cube : RenderObject
    {
        public Cube(Shader shader, float width, float height, float depth)
            : this(shader, width, height, depth, Math.Max(width, Math.Max(height, depth)) / 2.0f)
        { }

        private Cube(Shader shader, float width, float height, float depth, float maxDim)
            : base(shader, new float[]
            {
                -width / maxDim, -height / maxDim, -depth / maxDim, // 0
                 width / maxDim, -height / maxDim, -depth / maxDim, // 1
                 width / maxDim, -height / maxDim,  depth / maxDim, // 2
                -width / maxDim, -height / maxDim,  depth / maxDim, // 3
                -width / maxDim,  height / maxDim, -depth / maxDim, // 4
                 width / maxDim,  height / maxDim, -depth / maxDim, // 5
                 width / maxDim,  height / maxDim,  depth / maxDim, // 6
                -width / maxDim,  height / maxDim,  depth / maxDim  // 7
            },
            new uint[]
            {
                5,1,0, // front
                4,5,0,
                6,2,1, // left
                6,1,5,
                7,3,2, // back
                7,2,6,
                4,0,7, // right
                3,7,0,
                0,1,2, // bottom
                0,2,3,
                6,5,4, // up
                7,6,4
            })
        {
            Transform.Scale = new OpenTK.Mathematics.Vector3(maxDim);
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
