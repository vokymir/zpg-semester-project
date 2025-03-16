using OpenTK.Graphics.OpenGL;

namespace zpg
{
    class Cube : RenderObject
    {
        public Cube(Shader shader, float width, float height, float depth)
            : this(shader, width, height, depth, Math.Max(width, Math.Max(height, depth)) / 2)
        { }

        /// its param name="maxDimTimes2" because I need to divide all parameters by 2, to achieve the w/h/d and centered
        private Cube(Shader shader, float width, float height, float depth, float maxDimTimes2)
            : base(shader, new float[]
            {
                -width / maxDimTimes2, -height / maxDimTimes2, -depth / maxDimTimes2, // 0
                 width / maxDimTimes2, -height / maxDimTimes2, -depth / maxDimTimes2, // 1
                 width / maxDimTimes2, -height / maxDimTimes2,  depth / maxDimTimes2, // 2
                -width / maxDimTimes2, -height / maxDimTimes2,  depth / maxDimTimes2, // 3
                -width / maxDimTimes2,  height / maxDimTimes2, -depth / maxDimTimes2, // 4
                 width / maxDimTimes2,  height / maxDimTimes2, -depth / maxDimTimes2, // 5
                 width / maxDimTimes2,  height / maxDimTimes2,  depth / maxDimTimes2, // 6
                -width / maxDimTimes2,  height / maxDimTimes2,  depth / maxDimTimes2  // 7
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
            Transform.Scale = new OpenTK.Mathematics.Vector3(maxDimTimes2 / 2);
        }

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
