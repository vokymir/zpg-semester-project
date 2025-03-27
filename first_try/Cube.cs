using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace zpg
{
    class Cube : RenderObject
    {

        public Cube(Shader shader, float width, float height, float depth, Camera camera, string diffuseMap = "./Textures/container2.png", string specularMap = "./Textures/container2_specular.png")
            : this(shader, width, height, depth, Math.Max(width, Math.Max(height, depth)) / 2, camera, diffuseMap, specularMap)
        { }

        private Cube(Shader shader, float width, float height, float depth, float maxDimTimes2, Camera camera, string diffuseMap, string specularMap)
            : base(shader, camera, GenerateVertices(width, height, depth, maxDimTimes2), GenerateIndices(), diffuseMap, specularMap)
        {
            Transform.Scale = new Vector3(maxDimTimes2 / 2);
        }

        private static float[] GenerateVertices(float width, float height, float depth, float maxDimTimes2)
        {
            float hx = width / maxDimTimes2;
            float hy = height / maxDimTimes2;
            float hz = depth / maxDimTimes2;

            return new float[]
            {
                // Front face (normal: 0,0,1)
                -hx, -hy,  hz,   0f, 0f, 1f,   0f, 0f,  // 0
                 hx, -hy,  hz,   0f, 0f, 1f,   1f, 0f,  // 1
                 hx,  hy,  hz,   0f, 0f, 1f,   1f, 1f,  // 2
                -hx,  hy,  hz,   0f, 0f, 1f,   0f, 1f,  // 3

                // Back face (normal: 0,0,-1)
                -hx, -hy, -hz,   0f, 0f, -1f,  0f, 0f,  // 4
                 hx, -hy, -hz,   0f, 0f, -1f,  1f, 0f,  // 5
                 hx,  hy, -hz,   0f, 0f, -1f,  1f, 1f,  // 6
                -hx,  hy, -hz,   0f, 0f, -1f,  0f, 1f,  // 7

                // Left face (normal: -1,0,0)
                -hx, -hy, -hz,  -1f, 0f, 0f,   0f, 0f,  // 8
                -hx, -hy,  hz,  -1f, 0f, 0f,   1f, 0f,  // 9
                -hx,  hy,  hz,  -1f, 0f, 0f,   1f, 1f,  // 10
                -hx,  hy, -hz,  -1f, 0f, 0f,   0f, 1f,  // 11

                // Right face (normal: 1,0,0)
                 hx, -hy,  hz,   1f, 0f, 0f,   0f, 0f,  // 12
                 hx, -hy, -hz,   1f, 0f, 0f,   1f, 0f,  // 13
                 hx,  hy, -hz,   1f, 0f, 0f,   1f, 1f,  // 14
                 hx,  hy,  hz,   1f, 0f, 0f,   0f, 1f,  // 15

                // Top face (normal: 0,1,0)
                -hx,  hy,  hz,   0f, 1f, 0f,   0f, 0f,  // 16
                 hx,  hy,  hz,   0f, 1f, 0f,   1f, 0f,  // 17
                 hx,  hy, -hz,   0f, 1f, 0f,   1f, 1f,  // 18
                -hx,  hy, -hz,   0f, 1f, 0f,   0f, 1f,  // 19

                // Bottom face (normal: 0,-1,0)
                -hx, -hy, -hz,   0f, -1f, 0f,  0f, 0f,  // 20
                 hx, -hy, -hz,   0f, -1f, 0f,  1f, 0f,  // 21
                 hx, -hy,  hz,   0f, -1f, 0f,  1f, 1f,  // 22
                -hx, -hy,  hz,   0f, -1f, 0f,  0f, 1f   // 23
            };
        }

        private static uint[] GenerateIndices()
        {
            return new uint[]
            {
                // Front face (CCW when viewed from +Z)
                0, 1, 2,
                2, 3, 0,

                // Back face (CCW when viewed from -Z)
                5, 4, 7,
                5, 7, 6,

                // Left face (CCW when viewed from -X)
                11, 8, 9,
                10, 11, 9,

                // Right face (CCW when viewed from +X)
                15, 12, 13,
                14, 15, 13,

                // Top face (CCW when viewed from +Y)
                17, 18, 16,
                18, 19, 16,

                // Bottom face (CCW when viewed from -Y)
                23, 20, 21,
                22, 23, 21
            };
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
