using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace zpg
{

    class Camera : Entity
    {
        private float speed = 1.4f;

        private float pitch = 0.0f;
        private float yaw = -90.0f;

        // mouse sensitivity
        private float sensitivity = 0.1f;

        // to avoid jump on first frame
        private bool firstMouse = true;
        private Vector2 lastMousePosition;

        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public Matrix4 ViewMatrix => Matrix4.LookAt(this.Transform.Position, Transform.Position + Front, Up);
        public Matrix4 ProjectionMatrix { get; private set; }

        private static float playerHeight = 1.8f;
        private static float eyesHeight = 1.7f;
        private bool gravity = false;

        public CollisionCube CollisionCube { get; private set; } = new CollisionCube()
        {
            Center = new Vector3(0, 0, 0),
            Xover2 = 0.5f,
            Yover2 = playerHeight / 2, // center of collisionbox in the center of player
            Zover2 = 0.5f
        };

        public Camera(float aspectRatio)
        {
            // make collision cube move with camera automagically
            Transform.PropertyChanged += (s, e) =>
            {
                Vector3 newCenter = Transform.Position;
                newCenter.Y -= eyesHeight - playerHeight / 2; // eyes at 1.7m, center of player box at 1.8m / 2 = 0.9m => 1.7 - 0.9 = 0.8m
                CollisionCube.Center = newCenter;
            };
            Resize(aspectRatio);
            Transform.Position += new Vector3(0.0f, 0.3f, 0.0f);
        }

        public void Resize(float aspectRatio) => this.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, aspectRatio, 0.1f, 100.0f);

        /// <summary>
        /// Look around based on mouse position.
        /// </summary>
        public void OnMouseMove(Vector2 mPos)
        {
            // to avoid jump on first frame
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
            // More info about this math at: https://opentk.net/learn/chapter1/9-camera
        }

        /// <summary>
        /// Move accordingly with what was on the keyboard. Take delta time into consideration.
        /// Should be upgraded, so it doesn't check all objects, but only the near one.
        /// </summary>
        public void ProcessKeyboard(KeyboardState input, float dT, List<RenderObject> objects)
        {
            Vector3 horizontalDirection = Vector3.Zero;
            Vector3 verticalDirection = Vector3.Zero;

            if (input.IsKeyDown(Keys.W)) horizontalDirection += Front;
            if (input.IsKeyDown(Keys.S)) horizontalDirection -= Front;
            if (input.IsKeyDown(Keys.A)) horizontalDirection -= Vector3.Cross(Front, Up).Normalized();
            if (input.IsKeyDown(Keys.D)) horizontalDirection += Vector3.Cross(Front, Up).Normalized();

            // avoid flying
            horizontalDirection.Y = 0;

            // gravity
            if (gravity)
                verticalDirection -= new Vector3(0.0f, 0.1f, 0.0f);

            // allow jumping
            if (!gravity && input.IsKeyDown(Keys.Space))
            {
                verticalDirection += new Vector3(0.0f, 10.5f, 0.0f);
                gravity = true;
            }

            var move = Vector3.Zero;
            if (horizontalDirection.LengthSquared > 0) move += horizontalDirection.Normalized() * speed * dT;
            if (verticalDirection.LengthSquared > 0) move += verticalDirection * dT * 100;

            // guard MAYBE NOT GOOD, because of continual gravity...
            if (move.LengthSquared <= 0) return;

            // try move
            Transform.Position += move;

            // check for collision with each object
            foreach (var o in objects)
            {
                if (CollisionCube.DoesCollide(o.CollisionCube))
                {
                    Console.WriteLine($"--Cam: {CollisionCube}\nObject: {o.CollisionCube}");
                    // if collide, check both horizontal and vertical axis for the maximal position that could be done
                    // in the direction the player wants to go
                    // assume that there is no collision and then subtract to avoid collisions
                    float maxXposition = CollisionCube.Center.X;
                    float maxYposition = CollisionCube.Center.Y;
                    float maxZposition = CollisionCube.Center.Z;

                    // check collision on X axis (forget Y,Z)
                    Transform.Position += new Vector3(0, -move.Y, -move.Z);
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        if (CollisionCube.Center.X > o.CollisionCube.Center.X)
                            maxXposition = o.CollisionCube.Center.X + o.CollisionCube.Xover2 + CollisionCube.Xover2;
                        if (CollisionCube.Center.X <= o.CollisionCube.Center.X)
                            maxXposition = o.CollisionCube.Center.X - o.CollisionCube.Xover2 - CollisionCube.Xover2;
                    }

                    // check collision on Y axis (forget X, add Y)
                    Transform.Position += new Vector3(-move.X, move.Y, 0);
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        // without epsilon, it sometimes allowed to jump through objects
                        float epsilon = 0.03f;
                        if (CollisionCube.Center.Y > o.CollisionCube.Center.Y)
                        {
                            maxYposition = o.CollisionCube.Center.Y + o.CollisionCube.Yover2 + CollisionCube.Yover2 + epsilon;
                            gravity = false;
                        }
                        if (CollisionCube.Center.Y <= o.CollisionCube.Center.Y)
                            maxYposition = o.CollisionCube.Center.Y - o.CollisionCube.Yover2 - CollisionCube.Yover2 - epsilon;
                    }

                    // check collision on Z axis (forget Y, add Z)
                    Transform.Position += new Vector3(0, -move.Y, move.Z);
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        if (CollisionCube.Center.Z > o.CollisionCube.Center.Z)
                            maxZposition = o.CollisionCube.Center.Z + o.CollisionCube.Zover2 + CollisionCube.Zover2;
                        if (CollisionCube.Center.Z <= o.CollisionCube.Center.Z)
                            maxZposition = o.CollisionCube.Center.Z - o.CollisionCube.Zover2 - CollisionCube.Zover2;
                    }
                    // revert to no move at all
                    Transform.Position += new Vector3(0, 0, -move.Z);

                    // calculate the maximum possible move
                    move = new Vector3(
                            maxXposition - CollisionCube.Center.X,
                            maxYposition - CollisionCube.Center.Y,
                            maxZposition - CollisionCube.Center.Z
                            );

                    // try the edited move
                    Transform.Position += move;
                    Console.WriteLine($"->Cam: {CollisionCube}");
                }
            }
        }

    }
}
