using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace zpg
{
    public class Window : GameWindow
    {
        // main camera
        private Camera _camera;
        // all objects in the scene are stored here, it's an abstraction to easily switch between just list and grid-approach
        private IObjectsStore? _objects = null;
        // lights
        private DirectionalLight _dirLight = new(); // the sun in fact
        private SpotLight _spotLight = new(); // the flashlight of the player
        private List<PointLight> _pointLights = new(); // aren't used, but are implemented
        // stored level path for future use
        private string _levelPath;
        // stored level for future reference
        private Level? _level;
        // timer and fps for calculating fps
        private Stopwatch _sw;
        private Queue<long> _fps;
        // window Title contains also the FPS, so this stores what's before
        private string _titlePrefix;

        // At the end, point lights were not implemented.
        // This is just a little funny thingy to left here.
        private readonly Vector3[] _pointLightPositions =
        {
            // new Vector3(0.0f, 2.0f, 0.0f),
        };

        /// <summary>
        /// Create a new Window based on GameWindow, with initial WxH, it's title and path to where level is.
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
            this._sw = new();
            _sw.Start();
            this._fps = new();
            this._camera = new(Size.X / (float)Size.Y, _sw);
        }


        /// <summary>
        /// Initialize camera, shader, lights, load level and enable cullface, depthtest, debug output etc.
        /// </summary>
        protected override void OnLoad()
        {
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

            // load level and also add whitescreen for teleport purposes
            LoadLevel(_levelPath, shader);
            var whiteScreen = new WhiteScreen(_camera);
            _camera.Overlay = whiteScreen;
            if (_objects is not null)
                _objects.Add(whiteScreen);

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

        /// <summary>
        /// Clear canvas, set spotlight, draw scene, count fps.
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // update spotlight based on camera position
            // respect assignment: Y coord is 2.05 meters above ground and depression is 2 degrees
            _spotLight.Position = _camera.Transform.Position + (0f, -Camera.PlayerEyesHeight + 2.05f, 0f);
            _spotLight.Direction = _camera.Front + (0.0f, -MathHelper.DegreesToRadians(2), 0.0f);

            // render all objects
            if (_objects is not null)
                foreach (var obj in _objects.GetAll()) obj.Render(_dirLight, _pointLights, _spotLight);
            SwapBuffers();

            // count the fps
            long time = _sw.ElapsedMilliseconds;
            _fps.Enqueue(time);
            while (_fps.Count > 0 && _fps.Peek() < time - 1000)
                _fps.Dequeue();
            Title = _titlePrefix + " | FPS: " + _fps.Count;
        }

        /// <summary>
        /// De-facto process keyboard.
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            var input = KeyboardState;
            _camera.ProcessKeyboard(input, (float)args.Time, _objects is not null ? _objects.GetOnlyRelevant() : new RenderObject[0]);

            // close game - CapsLk because of my keyboard mapping sometimes fails
            if (input.IsKeyPressed(Keys.Escape) || input.IsKeyPressed(Keys.CapsLock))
            {
                Close();
            }
            // release/grab mouse - good for temporarily leaving the game
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
