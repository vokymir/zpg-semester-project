using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace zpg
{
    class RenderObject : Entity
    {
        protected Shader Shader;
        protected int VAO, VBO, EBO;
        protected int IndexCount;

        protected Texture _diffuseMap;
        protected Texture _specularMap;

        protected Camera _camera;


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

        protected virtual void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        protected virtual void PostRender() { }

        public virtual void Render(Camera camera, DirectionalLight dirLight, List<PointLight> pointLights, SpotLight spotlight)
        {
            _diffuseMap.Use(TextureUnit.Texture0);
            _specularMap.Use(TextureUnit.Texture1);

            Shader.Use();

            Shader.SetInt("nPointLights", 4);

            Shader.SetMatrix4("model", Transform.GetMatrix());
            Shader.SetMatrix4("view", camera.ViewMatrix);
            Shader.SetMatrix4("projection", camera.ProjectionMatrix);

            Shader.SetInt("material.diffuse", 0);
            Shader.SetInt("material.specular", 1);
            /*Shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));*/
            Shader.SetFloat("material.shininess", 3200.0f);

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
