using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace zpg
{
    /// <summary>
    /// Any renderable object in the scene should inherit from this class.
    /// Provides abstraction for VAO, VBO, EBO and textures.
    /// </summary>
    class RenderObject : Entity
    {
        protected Shader Shader;
        protected int VAO, VBO, EBO;
        protected int IndexCount;

        // diffuse map for basic texture
        protected Texture _diffuseMap;
        // specular map for making the specular lighting be more cool
        protected Texture _specularMap;

        protected Camera _camera;

        public CollisionCube CollisionCube { get; private set; } = new();


        public RenderObject(Shader shader, Camera camera, float[] vertices, uint[] indices, string diffuseMap, string specularMap)
        {
            Shader = shader;
            _camera = camera;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // All must be in format:
            // position normal  texture coordinates
            // X,Y,Z,   X,Y,Z,  X,Y
            var position = shader.GetAttribLocation("aPos");
            var normal = shader.GetAttribLocation("aNormal");
            var tex = shader.GetAttribLocation("aTexCoords");

            GL.VertexAttribPointer(position, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.VertexAttribPointer(normal, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(tex, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            GL.EnableVertexAttribArray(position);
            GL.EnableVertexAttribArray(normal);
            GL.EnableVertexAttribArray(tex);

            IndexCount = indices.Length;

            _diffuseMap = Texture.LoadFromFile(diffuseMap);
            _specularMap = Texture.LoadFromFile(specularMap);
        }

        protected virtual void PreRender() { }

        // simple draw of VAO, assuming triangles as primitive type
        protected virtual void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        protected virtual void PostRender() { }

        /// <summary>
        /// Render this object based on it's camera and shader, and with all lights provided.
        /// </summary>
        public virtual void Render(DirectionalLight dirLight, List<PointLight> pointLights, SpotLight spotlight)
        {
            Shader.Use();

            // set number of point lights in shader. but it cannot be more than the maximum set in there...
            Shader.SetInt("nPointLights", Math.Min(32, pointLights.Count));

            // set translations
            Shader.SetMatrix4("model", Transform.GetMatrix());
            Shader.SetMatrix4("view", _camera.ViewMatrix);
            Shader.SetMatrix4("projection", _camera.ProjectionMatrix);

            // set texture maps - and specular for light
            _diffuseMap.Use(TextureUnit.Texture0);
            _specularMap.Use(TextureUnit.Texture1);
            Shader.SetInt("material.diffuse", 0);
            Shader.SetInt("material.specular", 1);
            Shader.SetFloat("material.shininess", 3200.0f);

            // Set all lights
            dirLight.Use(Shader, "dirLight");

            for (int i = 0; i < pointLights.Count; i++)
            {
                pointLights[i].Use(Shader, $"pointLights[{i}]");
            }

            spotlight.Use(Shader, "spotLight");

            PreRender();
            Draw();
            PostRender();
        }
    }
}
