using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using OpenTK.Mathematics;


namespace zpg
{
    public class Window : GameWindow
    {
        private Camera _camera;
        private List<RenderObject> _objects = new();
        private DirectionalLight _dirLight;
        private SpotLight _spotLight;
        private List<PointLight> _pointLights = new();

        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };

        public Window(int width, int height, string title) : base(
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
        { }

        protected override void OnLoad()
        {
            _camera = new Camera(Size.X / (float)Size.Y);
            _camera.Transform.Position = new OpenTK.Mathematics.Vector3(0, 1.7f, 0);

            CursorState = CursorState.Grabbed;

            // create two objects
            Shader shader = new Shader("./Shaders/shader.vert", "./Shaders/lighting.frag");

            // LIGHTS
            _dirLight = new DirectionalLight()
            {
                Direction = new Vector3(-0.2f, -1.0f, -0.3f),
                Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                Diffuse = new Vector3(0.4f, 0.4f, 0.4f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f)
            };

            foreach (var position in _pointLightPositions)
            {
                PointLight light = new PointLight()
                {
                    Position = position,
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                };

                _pointLights.Add(light);
            }

            _spotLight = new SpotLight()
            {
                Position = _camera.Transform.Position,
                Direction = _camera.Front,
                Ambient = new Vector3(0.0f, 0.0f, 0.0f),
                Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Constant = 1.0f,
                Linear = 0.09f,
                Quadratic = 0.032f,
                CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
                OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f))
            };

            (_camera.Transform.Position, _objects) = Level.LoadFile("./Levels/lvl01.txt", shader, _camera);
            if (_objects.Count == 0)
            {
                return;
            }

            // don't render non-visible objects (based on triangle normal)
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        }

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"OpenGL Debug Message: {msg}\nSource: {source}, Type: {type}, Severity: {severity}, ID: {id}");
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            _camera.Resize(Size.X / (float)Size.Y);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _spotLight.Position = _camera.Transform.Position;
            _spotLight.Direction = _camera.Front;

            foreach (var obj in _objects) obj.Render(_camera, _dirLight, _pointLights, _spotLight);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            var input = KeyboardState;
            _camera.ProcessKeyboard(input, (float)args.Time);

            if (input.IsKeyPressed(Keys.Escape) || input.IsKeyPressed(Keys.CapsLock))
            {
                Close();
            }
            if (input.IsKeyPressed(Keys.Tab))
            {
                CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            _camera.OnMouseMove(e.Position);
        }
    }
}
