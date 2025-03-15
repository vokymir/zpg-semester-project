using OpenTK.Graphics.OpenGL;

namespace zpg
{
    class RenderObject : Entity
    {
        protected Shader Shader;
        protected int VAO, VBO, EBO;
        protected int IndexCount;

        public RenderObject(Shader shader, float[] vertices, uint[] indices)
        {
            Shader = shader;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            var position = shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(position, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(position);

            IndexCount = indices.Length;
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
            Shader.Use();
            Shader.SetMatrix4("model", Transform.GetMatrix());
            Shader.SetMatrix4("view", camera.ViewMatrix);
            Shader.SetMatrix4("projection", camera.ProjectionMatrix);

            PreRender();
            Draw();
            PostRender();
        }
    }
}
