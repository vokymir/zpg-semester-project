using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace zpg
{

    class Camera : Entity
    {
        private float speed = 1.4f;

        private float pitch = 0.0f;
        private float yaw = -90.0f;

        private float sensitivity = 0.1f;

        private bool firstMouse = true;
        private Vector2 lastMousePosition;

        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public Matrix4 ViewMatrix => Matrix4.LookAt(this.Transform.Position, Transform.Position + Front, Up);
        public Matrix4 ProjectionMatrix { get; private set; }

        public CollisionCube CollisionCube { get; private set; } = new CollisionCube()
        {
            Center = new Vector3(0, 0, 0),
            Xover2 = 0.5f,
            Yover2 = 2.0f,
            Zover2 = 0.5f
        };

        public Camera(float aspectRatio)
        {
            Transform.PropertyChanged += (s, e) => CollisionCube.Center = Transform.Position;
            Resize(aspectRatio);
        }

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
        public void ProcessKeyboard(KeyboardState input, float dT, List<RenderObject> objects)
        {
            Vector3 direction = Vector3.Zero;

            if (input.IsKeyDown(Keys.W)) direction += Front;
            if (input.IsKeyDown(Keys.S)) direction -= Front;
            if (input.IsKeyDown(Keys.A)) direction -= Vector3.Cross(Front, Up).Normalized();
            if (input.IsKeyDown(Keys.D)) direction += Vector3.Cross(Front, Up).Normalized();

            // avoid flying
            direction.Y = 0;

            if (direction.LengthSquared > 0)
            {
                var move = direction.Normalized() * speed * dT;
                Transform.Position += move;

                foreach (var o in objects)
                {
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        Console.WriteLine($"Does collide on {this.CollisionCube} \nwith {o.CollisionCube}");
                        float maxXposition = Transform.Position.X;
                        float maxZposition = Transform.Position.Z;
                        // check collision on X axis
                        Transform.Position += new Vector3(0, 0, -move.Z);
                        if (CollisionCube.DoesCollide(o.CollisionCube))
                        {
                            if (CollisionCube.Center.X > o.CollisionCube.Center.X)
                                maxXposition = o.CollisionCube.Center.X + o.CollisionCube.Xover2 + CollisionCube.Xover2;
                            if (CollisionCube.Center.X <= o.CollisionCube.Center.X)
                                maxXposition = o.CollisionCube.Center.X - o.CollisionCube.Xover2 - CollisionCube.Xover2;
                        }
                        // check collision on Z axis
                        Transform.Position += new Vector3(-move.X, 0, move.Z);
                        if (CollisionCube.DoesCollide(o.CollisionCube))
                        {
                            if (CollisionCube.Center.Z > o.CollisionCube.Center.Z)
                                maxZposition = o.CollisionCube.Center.Z + o.CollisionCube.Zover2 + CollisionCube.Zover2;
                            if (CollisionCube.Center.Z <= o.CollisionCube.Center.Z)
                                maxZposition = o.CollisionCube.Center.Z - o.CollisionCube.Zover2 - CollisionCube.Zover2;
                        }
                        Transform.Position = new Vector3(maxXposition, Transform.Position.Y, maxZposition);
                        Console.WriteLine($"Result: {this.Transform.Position}");
                    }
                }
            }
        }

    }
}
