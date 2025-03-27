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

        protected readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };

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

        public virtual void Render(Camera camera)
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

            Shader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            Shader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            Shader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            Shader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                Shader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
                Shader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                Shader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                Shader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                Shader.SetFloat($"pointLights[{i}].constant", 1.0f);
                Shader.SetFloat($"pointLights[{i}].linear", 0.09f);
                Shader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }

            Shader.SetVector3("spotLight.position", _camera.Transform.Position);
            Shader.SetVector3("spotLight.direction", _camera.Front);
            Shader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            Shader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            Shader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            Shader.SetFloat("spotLight.constant", 1.0f);
            Shader.SetFloat("spotLight.linear", 0.09f);
            Shader.SetFloat("spotLight.quadratic", 0.032f);
            Shader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            Shader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

            PreRender();
            Draw();
            PostRender();
        }
    }
}
