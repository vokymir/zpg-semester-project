using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

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
        /*
        float[] _vertices =
        {
            // positions        //colors
            0.5f, 0.5f, 0.0f,   1.0f, 0.0f, 0.0f, // top right
            0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f, // bottom-right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom-left
            -0.5f, 0.5f, 0.0f,  1.0f, 0.0f, 1.0f, // top left
        };*/

        /*
        float[] _vertices =
        {
            // positions        // Texture coordinates
            0.5f, 0.5f, 0.0f,   1.0f, 1.0f, // top right
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom-right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom-left
            -0.5f, 0.5f, 0.0f,  0.0f, 1.0f, // top left
        };
        */
        float[] _vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};


        uint[] indices =
        {
            0,1,3,
            1,2,3
        };
        float[] texCoords =
        {
            0.0f, 0.0f,
            1.0f, 0.0f,
            0.5f, 1.0f
        };
        int VertexBufferObject;
        int ElementBufferObject;
        int VertexArrayObject;
        Shader _shader;
        Texture _texture;
        Texture _texture2;
        Stopwatch stopwatch;


        public Game(int width, int height, string title) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Core,
                ClientSize = (width, height),
                Title = title
            }
            )
        {
            Width = width;
            Height = height;
            stopwatch = new Stopwatch();
        }

        protected float _fov;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_fov > 45.0f)
            {
                _fov = 45.0f;
            }
            else if (_fov < 1.0f)
            {
                _fov = 1.0f;
            }
            else
            {
                _fov -= e.OffsetY;
            }
        }


        bool moveRight = false;
        Vector2 mouseLastPos;
        bool mouseFirstMove = true;
        float camYaw = 0.0f;
        float camPitch = 0.0f;
        float camSensitivity = 0.5f;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            if (!IsFocused)
            {
                return;
            }

            if (mouseFirstMove)
            {
                mouseLastPos = new Vector2(MousePosition.X, MousePosition.Y);
                mouseFirstMove = false;
            }
            else
            {
                float deltaX = MousePosition.X - mouseLastPos.X;
                float deltaY = MousePosition.Y - mouseLastPos.Y;
                mouseLastPos = new Vector2(MousePosition.X, MousePosition.Y);

                camYaw += deltaX * camSensitivity;
                if (camPitch > 89.0f)
                {
                    camPitch = 89.0f;
                }
                else if (camPitch < -89.0f)
                {
                    camPitch = -89.0f;
                }
                else
                {
                    camPitch -= deltaY * camSensitivity;
                }
            }

            camFront.X = (float)Math.Cos(MathHelper.DegreesToRadians(camPitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(camYaw));
            camFront.Y = (float)Math.Sin(MathHelper.DegreesToRadians(camPitch));
            camFront.Z = (float)Math.Cos(MathHelper.DegreesToRadians(camPitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(camYaw));

            camFront = Vector3.Normalize(camFront);


            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            /*            if (KeyboardState.IsKeyDown(Keys.A))
                        {
                            moveRight = true;
                        }
                        else
                        {
                            moveRight = false;
                        }
                        */
            if (input.IsKeyDown(Keys.W))
            {
                camPos += camFront * camSpeed * (float)e.Time; //Forward 
            }

            if (input.IsKeyDown(Keys.S))
            {
                camPos -= camFront * camSpeed * (float)e.Time; //Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camPos -= Vector3.Normalize(Vector3.Cross(camFront, camUp)) * camSpeed * (float)e.Time; //Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camPos += Vector3.Normalize(Vector3.Cross(camFront, camUp)) * camSpeed * (float)e.Time; //Right
            }
            if (input.IsKeyDown(Keys.J))
            {
                camPos -= camUp * camSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.K))
            {
                camPos += camUp * camSpeed * (float)e.Time;
            }

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _fov = 45.0f;

            CursorState = CursorState.Grabbed;

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // the order matters!
            // first, define VBO, then VAO and lastly EBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);


            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            int texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            //GL.EnableVertexAttribArray(1);


            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


            GL.Enable(EnableCap.DepthTest);

            _shader.Use();


            _texture = new Texture("Textures/container.jpg");
            _texture2 = new Texture("Textures/awesomeface.png");

            _shader.SetInt("texture1", 0);
            _shader.SetInt("texture2", 1);

            stopwatch.Start();

        }
        float camSpeed = 1.5f;
        Vector3 camPos = new Vector3(0.0f, 0.0f, 3.0f);
        Vector3 camFront = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 camUp = new Vector3(0.0f, 1.0f, 0.0f);
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VertexArrayObject);


            /* Matrix4 transform = Matrix4.Identity;

             transform *= Matrix4.CreateRotationZ(MathHelper.Pi);
             transform *= Matrix4.CreateScale(0.5f, 1.5f, 1.0f);
             transform *= Matrix4.CreateTranslation(0.2f, 0.0f, 0.0f);
 */
            //Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
            Matrix4 model = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(stopwatch.ElapsedMilliseconds / 100));
            // if (moveRight) { model *= Matrix4.CreateRotationY(MathHelper.PiOver6); }
            //Matrix4 view = Matrix4.CreateTranslation(0.0f, 0.0f, -2.3f);
            Matrix4 view = Matrix4.LookAt(camPos, camPos + camFront, camUp);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), Width / Height, 0.1f, 100.0f);

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            //GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            SwapBuffers();

            Console.WriteLine(camPos);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _shader.Dispose();
        }

        public int Width;
        public int Height;

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            Width = e.Width;
            Height = e.Height;

            GL.Viewport(0, 0, e.Width, e.Height);
        }


    }
}
