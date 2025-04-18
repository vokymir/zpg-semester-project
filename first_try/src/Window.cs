using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace zpg
{
    public class Window : GameWindow
    {
        // main camera
        private Camera _camera = new(1.0f);
        // all objects in the scene
        private IEnumerable<RenderObject> _objects = new List<RenderObject>();
        // lights
        private DirectionalLight _dirLight = new();
        private SpotLight _spotLight = new();
        private List<PointLight> _pointLights = new();
        // stored level path for future use
        private string _levelPath;
        // stored level for future reference
        private Level? _level;
        // timer and fps for calculating fps
        private System.Timers.Timer _timer = new System.Timers.Timer(1000);
        private int _fps;
        // window Title contains also the FPS, so this stores what's before
        private string _titlePrefix;

        // TEMPORARY position of lights
        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.0f, 2.0f, 0.0f),
        };

        /// <summary>
        /// Create a new Window based on GameWindow.
        /// </summary>
        public Window(int width, int height, string title, string levelPath) : base(
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
            this._levelPath = levelPath;
            this._titlePrefix = title;
            SetTimer();
        }

        /// <summary>
        /// Set timer to count FPS and display them.
        /// </summary>
        private void SetTimer()
        {
            _timer.Elapsed += (s, e) =>
            {
                Title = _titlePrefix + " | FPS: " + _fps.ToString();
                _fps = 0;
            };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Initialize camera, shader, lights, load level and enable cullface, depthtest, debug output etc.
        /// </summary>
        protected override void OnLoad()
        {
            _camera = new Camera(Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;

            Shader shader = new Shader("./Shaders/shader.vert", "./Shaders/lighting.frag");

            // LIGHTS
            {
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
                    // position and direction are changed on render frame
                    Position = _camera.Transform.Position,
                    Direction = _camera.Front,
                    Ambient = new Vector3(0.0f, 0.0f, 0.0f),
                    Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.045f,
                    Quadratic = 0.0075f,
                    CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
                    OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f))
                };
            }

            LoadLevel(_levelPath, shader);

            // don't render non-visible objects (based on triangle normal)
            GL.Enable(EnableCap.CullFace);
            // render only the nearest
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

            // update spotlight based on camera position
            // respect assignment: Y coord is 2.05 meters above ground and depression is 2 degrees
            _spotLight.Position = _camera.Transform.Position + (0f, 0.35f, 0f);
            _spotLight.Direction = _camera.Front + (0.0f, -MathHelper.DegreesToRadians(2), 0.0f);

            // render all objects
            foreach (var obj in _objects) obj.Render(_dirLight, _pointLights, _spotLight);
            SwapBuffers();

            // count the fps
            _fps++;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            var input = KeyboardState;
            _camera.ProcessKeyboard(input, (float)args.Time, this._objects);

            // close game - CapsLk because of my keyboard mapping sometimes fails
            if (input.IsKeyPressed(Keys.Escape) || input.IsKeyPressed(Keys.CapsLock))
            {
                Close();
            }
            // release/grab mouse
            if (input.IsKeyPressed(Keys.Tab))
            {
                CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            _camera.OnMouseMove(e.Position);
        }

        /// <summary>
        /// Try to load the level from path. If not successfull, close the app.
        /// </summary>
        public void LoadLevel(string path, Shader shader)
        {
            try
            {
                _level = new Level(path, shader, _camera);
                _level.LoadFile();

                _camera.Transform.Position = _level.CameraStartPosition;
                _objects = _level.LevelObjects;
            }
            catch
            {
                Console.WriteLine($"Invalid map file on path: {path}");
                Close();
            }
        }
    }
}
