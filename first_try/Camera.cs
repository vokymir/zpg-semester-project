using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace zpg
{

    class Camera : Entity
    {
        private float speed = 3.0f;

        private float pitch = 0.0f;
        private float yaw = -90.0f;

        private float sensitivity = 0.1f;

        private bool firstMouse = true;
        private Vector2 lastMousePosition;

        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public Matrix4 ViewMatrix => Matrix4.LookAt(this.Transform.Position, Transform.Position + Front, Up);
        public Matrix4 ProjectionMatrix { get; private set; }

        public Camera(float aspectRatio) => Resize(aspectRatio);

        public void Resize(float aspectRatio) => this.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, aspectRatio, 0.1f, 100.0f);

        public void OnMouseMove(Vector2 mPos)
        {
            if (firstMouse)
            {
                firstMouse = false;
                lastMousePosition = mPos;
            }

            float dX = (mPos.X - lastMousePosition.X) * sensitivity;
            float dY = (lastMousePosition.Y - mPos.Y) * sensitivity;
            lastMousePosition = mPos;

            yaw += dX;
            pitch = Math.Clamp(pitch + dY, -89.0f, 89.0f);

            Front = new Vector3(
                MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch))
            ).Normalized();
        }

        /// <summary>
        /// Move accordingly with what was on the keyboard. Take delta time into consideration.
        /// </summary>
        public void ProcessKeyboard(KeyboardState input, float dT)
        {
            Vector3 direction = Vector3.Zero;

            if (input.IsKeyDown(Keys.W)) direction += Front;
            if (input.IsKeyDown(Keys.S)) direction -= Front;
            if (input.IsKeyDown(Keys.A)) direction -= Vector3.Cross(Front, Up).Normalized();
            if (input.IsKeyDown(Keys.D)) direction += Vector3.Cross(Front, Up).Normalized();

            direction.Y = 0;

            if (direction.LengthSquared > 0)
            {
                Transform.Position += direction.Normalized() * speed * dT;
            }
        }

    }
}
