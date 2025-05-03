using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace zpg
{
    class Cube : RenderObject
    {
        PolygonMode previousPolygonMode;

        /// <summary>
        /// Create a generic cube with defined dimensions.
        /// It has default textures, if you don't provide any.
        /// </summary>
        public Cube(Shader shader, float width, float height, float depth, Camera camera, string diffuseMap = "./Textures/container2.png", string specularMap = "./Textures/container2_specular.png")
            : this(shader, width, height, depth, Cube.MaxDimensionOver2(width, height, depth), camera, diffuseMap, specularMap)
        // The maxing is for searching the max length of all sides - useful for normalizing the cube into [-1,-1,-1:1,1,1] space.
        { }

        /// <summary>
        /// Overload for existing texures.
        /// </summary>
        public Cube(Shader shader, float width, float height, float depth, Camera camera, Texture diffuseTex, Texture specularTex) :
             this(shader, width, height, depth, Cube.MaxDimensionOver2(width, height, depth), camera, diffuseTex, specularTex)
        { }

        /// <summary>
        /// Generate vertices and indices based on what was given - it extends functionality of public constructor.
        /// </summary>
        private Cube(Shader shader, float width, float height, float depth, float maxDimOver2, Camera camera, string diffuseMap, string specularMap)
            : base(shader, camera, GenerateVertices(width, height, depth, maxDimOver2), GenerateIndices(), diffuseMap, specularMap)
        {
            Transform.Scale = Cube.DefaultScale(maxDimOver2);
            CollisionCube.Xover2 = width * 0.5f * Transform.Scale.X;
            CollisionCube.Yover2 = height * 0.5f * Transform.Scale.Y;
            CollisionCube.Zover2 = depth * 0.5f * Transform.Scale.Z;
            CollisionCube.Center = Transform.Position;
        }

        /// <summary>
        /// Overload for existing textures.
        /// </summary>
        private Cube(Shader shader, float width, float height, float depth, float maxDimOver2, Camera camera, Texture diffuseTex, Texture specularTex)
            : base(shader, camera, GenerateVertices(width, height, depth, maxDimOver2), GenerateIndices(), diffuseTex, specularTex)
        {
            Transform.Scale = Cube.DefaultScale(maxDimOver2);
            CollisionCube.Xover2 = width * 0.5f * Transform.Scale.X;
            CollisionCube.Yover2 = height * 0.5f * Transform.Scale.Y;
            CollisionCube.Zover2 = depth * 0.5f * Transform.Scale.Z;
            CollisionCube.Center = Transform.Position;
        }

        public static float MaxDimensionOver2(float x, float y, float z) => Math.Max(x, Math.Max(y, z)) / 2;
        // set the scale so it fits the dimensions given by user - these are easily changable elsewhere.
        public static Vector3 DefaultScale(float maxDimOver2) => new Vector3(maxDimOver2 / 2);

        public void UpdateCollisionCube() => CollisionCube.Center = Transform.Position;


        // this could be loaded from file, but I started with this - maybe in the other assignment part I will implement model loading...
        public static float[] GenerateVertices(float width, float height, float depth, float maxDimOver2)
        {
            float hx = width / maxDimOver2;
            float hy = height / maxDimOver2;
            float hz = depth / maxDimOver2;

            return new float[]
            {
                // VERTEX FORMAT:
                // position     normal          texture coordinates
                // Front face (normal: 0,0,1)
                -hx, -hy,  hz,   0f, 0f, 1f,   0f, 0f,  // 0
                 hx, -hy,  hz,   0f, 0f, 1f,   1f, 0f,  // 1
                 hx,  hy,  hz,   0f, 0f, 1f,   1f, 1f,  // 2
                -hx,  hy,  hz,   0f, 0f, 1f,   0f, 1f,  // 3

                // Back face (normal: 0,0,-1)
                -hx, -hy, -hz,   0f, 0f, -1f,  1f, 0f,  // 4
                 hx, -hy, -hz,   0f, 0f, -1f,  0f, 0f,  // 5
                 hx,  hy, -hz,   0f, 0f, -1f,  0f, 1f,  // 6
                -hx,  hy, -hz,   0f, 0f, -1f,  1f, 1f,  // 7

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

        public static uint[] GenerateIndices()
        {
            return new uint[]
            {
                // Front face
                0, 1, 2,
                2, 3, 0,

                // Back face
                5, 4, 7,
                5, 7, 6,

                // Left face
                11, 8, 9,
                10, 11, 9,

                // Right face
                15, 12, 13,
                14, 15, 13,

                // Top face
                17, 18, 16,
                18, 19, 16,

                // Bottom face
                23, 20, 21,
                22, 23, 21
            };
        }

        /// <summary>
        /// Save whatever polygonMode was before, to set it back after.
        /// Set the polygonmode to fill.
        /// </summary>
        protected override void PreRender()
        {
            base.PreRender();
            GL.GetInteger(GetPName.PolygonMode, out int previousMode);
            previousPolygonMode = (PolygonMode)previousMode;
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        }

        /// <summary>
        /// Reset polygonmode.
        /// </summary>
        protected override void PostRender()
        {
            base.PostRender();
            GL.PolygonMode(TriangleFace.FrontAndBack, previousPolygonMode);
        }
    }
}
