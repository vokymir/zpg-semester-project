using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace opentkLearn
{
    class Game : GameWindow
    {
        float[] vertices =
        {
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom-right
            -0.5f, -0.5f, 0.0f, // bottom-left
            -0.5f, 0.5f, 0.0f // top left
        };
        uint[] indices =
        {
            0,1,3,
            1,2,3
        };
        int VertexBufferObject;
        int ElementBufferObject;
        int VertexArrayObject;
        Shader shader;

        public Game(int width, int height, string title) : base(
            GameWindowSettings.Default, 
            new NativeWindowSettings() { 
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3,3),
                Profile = ContextProfile.Core,
                ClientSize = (width, height), 
                Title = title }
            ) { }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            GL.ClearColor(0.2f,0.3f,0.3f,1.0f);

            // the order matters!
            // first, define VBO, then VAO and lastly EBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);


            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);



            
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);



            shader.Use();
            stopwatch.Start();
        }
        Stopwatch stopwatch = new Stopwatch();
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();

            double timeValue = stopwatch.ElapsedMilliseconds / 1000f; 
            float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);

            GL.BindVertexArray(VertexArrayObject);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            shader.Dispose();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0,0, e.Width, e.Height);
        }


    }
}
