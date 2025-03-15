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
        private Camera camera;
        private List<RenderObject> objects = new();


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
            camera = new Camera(Size.X / (float)Size.Y);
            camera.Transform.Position = new OpenTK.Mathematics.Vector3(0, 1.7f, 0);
            CursorState = CursorState.Grabbed;

            // create two objects
            Shader shader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
            var obj = new Tetrahedron(shader);
            obj.Transform.Position = new Vector3(1, 0, 0);
            /*objects.Add(obj);*/
            obj = new Tetrahedron(shader);
            obj.Transform.Position = new Vector3(-1, 0, 0);
            /*objects.Add(obj);*/

            var floor = new Floor(shader, 10, 10);
            /*objects.Add(floor);*/
            var cube = new Cube(shader, 2, 3, 2);
            cube.Transform.Position.Z = -9;
            objects.Add(cube);

            // don't render non-visible objects (based on triangle normal)
            GL.Enable(EnableCap.CullFace);

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
            camera.Resize(Size.X / (float)Size.Y);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (var obj in objects) obj.Render(camera);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            var input = KeyboardState;
            camera.ProcessKeyboard(input, (float)args.Time);

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
            camera.OnMouseMove(e.Position);
        }
    }
}
