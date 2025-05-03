using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace zpg
{

    class Camera : Entity
    {
        // public stats of player
        public static float PlayerSpeed { get; set; } = 1.4f;
        public static float PlayerHeight { get; set; } = 1.8f;
        public static float PlayerEyesHeight { get; set; } = 1.7f;
        public static float PlayerWeightKg { get; set; } = 80f;

        private float _pitch = 0.0f;
        private float _yaw = -90.0f;

        private float _mouseSensitivity = 0.1f;

        // to avoid jump on first frame
        private bool _firstMouse = true;
        private Vector2 _lastMousePosition;

        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public Matrix4 ViewMatrix => Matrix4.LookAt(this.Transform.Position, Transform.Position + Front, Up);
        public Matrix4 ProjectionMatrix { get; private set; }

        // on which object is player standing?
        private RenderObject? _standingOnObject = null;
        // speed of gravity - generally known number, in m * s ^ -1
        private float _gravitySpeed = 9.81f;

        public CollisionCube CollisionCube { get; private set; } = new CollisionCube()
        {
            Center = new Vector3(0, 0, 0),
            Xover2 = 0.5f,
            Yover2 = PlayerHeight / 2, // center of collisionbox in the center of player
            Zover2 = 0.5f
        };

        // just for teleportat animation
        public WhiteScreen? Overlay { get; set; }
        // Teleport to this position after animation
        private Vector3 _positionToTeleport = Vector3.Zero;
        // animation length
        private int _teleportDurationMsOver2 { get; set; } = 500;
        // if E is held, only used for teleportation though
        private bool _interacting = false;
        // run
        private float _moveSpeedMultiplier = 1.0f;

        public Camera(float aspectRatio)
        {
            // make collision cube move with camera automagically
            Transform.PropertyChanged += (s, e) =>
            {
                Vector3 newCenter = Transform.Position;
                newCenter.Y -= PlayerEyesHeight - PlayerHeight / 2; // eyes at 1.7m, center of player box at 1.8m / 2 = 0.9m => 1.7 - 0.9 = 0.8m
                CollisionCube.Center = newCenter;
            };
            Resize(aspectRatio);
        }

        public void Resize(float aspectRatio) => this.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, aspectRatio, 0.1f, 100.0f);

        /// <summary>
        /// Look around based on mouse position.
        /// </summary>
        public void OnMouseMove(Vector2 mPos)
        {
            // to avoid jump on first frame
            if (_firstMouse)
            {
                _firstMouse = false;
                _lastMousePosition = mPos;
            }

            float dX = (mPos.X - _lastMousePosition.X) * _mouseSensitivity;
            float dY = (_lastMousePosition.Y - mPos.Y) * _mouseSensitivity;
            _lastMousePosition = mPos;

            _yaw += dX;
            _pitch = Math.Clamp(_pitch + dY, -89.0f, 89.0f);

            Front = new Vector3(
                MathF.Cos(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(_pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch))
            ).Normalized();
            // More info about this math at: https://opentk.net/learn/chapter1/9-camera
        }

        /// <summary>
        /// The main logic for most of the things.
        /// Move accordingly with what was on the keyboard. Take delta time into consideration.
        /// Check for gravity fall.
        /// Check for teleports.
        /// Could be split into more smaller functions, but these would need to get mostly the same parameters, so I decided not to.
        /// Moreover, it feels like it would be less concise, so better off letting it be all together.
        /// </summary>
        public void ProcessKeyboard(KeyboardState input, float dT, IEnumerable<RenderObject> objects)
        {
            _interacting = input.IsKeyDown(Keys.E);
            if (input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.RightShift))
            {
                _moveSpeedMultiplier = 2;
            }

            // ===== TELEPORT =====
            // if teleport is active, don't do anything else
            if (Overlay!.Teleporting)
            {
                Overlay.Elapsed += (int)(dT * 1000);
                if (Overlay.Elapsed < Overlay.DurationMs) // to white
                    Overlay.Alpha = (float)Overlay.Elapsed / Overlay.DurationMs;
                else // from white to normal
                {
                    Transform.Position = _positionToTeleport;
                    Overlay.Alpha = 2 - (float)Overlay.Elapsed / Overlay.DurationMs;
                }

                // end condition
                if (Overlay.Elapsed > 2 * Overlay.DurationMs)
                {
                    Overlay.Alpha = 0;
                    Overlay.Teleporting = false;
                    Overlay.Elapsed = 0;
                }
                return;
            }

            // ===== MOVE =====

            Vector3 horizontalDirection = Vector3.Zero;
            Vector3 verticalDirection = Vector3.Zero;
            Vector3 move = Vector3.Zero;

            { // === HORIZONTAL ===
                if (input.IsKeyDown(Keys.W)) horizontalDirection += Front;
                if (input.IsKeyDown(Keys.S)) horizontalDirection -= Front;
                if (input.IsKeyDown(Keys.A)) horizontalDirection -= Vector3.Cross(Front, Up).Normalized();
                if (input.IsKeyDown(Keys.D)) horizontalDirection += Vector3.Cross(Front, Up).Normalized();

                horizontalDirection.Y = 0; // avoid flying

                // move is the combination of horizontal and vertical direction
                if (horizontalDirection.LengthSquared > 0) move += horizontalDirection.Normalized() * PlayerSpeed * _moveSpeedMultiplier * dT;
            }
            { // === GRAVITY ===
                // if not standing on object anymore, start falling
                if (_standingOnObject is not null && !CollisionCube.IsAbove(_standingOnObject.CollisionCube))
                    _standingOnObject = null;

                // gravity
                if (_standingOnObject is null)
                {
                    // falling distance: y(t) = 1/2 * g * t^2 ; [y] = m, [g] = m*s^-2, [t] = s
                    // for more info, see https://en.wikipedia.org/wiki/Free_fall#Uniform_gravitational_field_without_air_resistance
                    float y_t = 0.5f * _gravitySpeed * dT;
                    verticalDirection -= new Vector3(0.0f, y_t, 0.0f);
                }

                // only if gravity did something, update the move
                if (verticalDirection.LengthSquared > 0) move += verticalDirection;
            }
            CollisionCube.Center += move;

            // move is iteratively made smaller each time player collides with something
            foreach (var o in objects)
            {
                // but check first for teleport
                if (_interacting && o is TeleportPlatform && CollisionCube.IsOnTop(o.CollisionCube))
                {
                    float epsilon = 0.3f;
                    Vector3 position = ((TeleportPlatform)o).LinkedTeleportPlatform!.Transform.Position;
                    // without epsilon glitches through floor, big epsilon looks cool - as if dropped from the air
                    position.Y += PlayerEyesHeight + epsilon;
                    // teleport
                    Overlay.DurationMs = _teleportDurationMsOver2;
                    Overlay.Teleporting = true;
                    // store the position, but go there only when the screen is completely white
                    _positionToTeleport = position;
                    break;
                }

                // check for collision with each object
                if (CollisionCube.DoesCollide(o.CollisionCube))
                {
                    // if collide, check both horizontal and vertical axis for the maximal position that could be done
                    // in the direction the player wants to go
                    // assume that there is no collision and then subtract to avoid collisions
                    float maxXposition = CollisionCube.Center.X;
                    float maxYposition = CollisionCube.Center.Y;
                    float maxZposition = CollisionCube.Center.Z;

                    // check collision on X axis (forget Y,Z)
                    CollisionCube.Center += new Vector3(0, -move.Y, -move.Z);
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        if (CollisionCube.Center.X > o.CollisionCube.Center.X)
                            maxXposition = o.CollisionCube.Center.X + o.CollisionCube.Xover2 + CollisionCube.Xover2;
                        else
                            maxXposition = o.CollisionCube.Center.X - o.CollisionCube.Xover2 - CollisionCube.Xover2;
                    }

                    // check collision on Y axis (forget X, add Y)
                    CollisionCube.Center += new Vector3(-move.X, move.Y, 0);
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        // without epsilon, it sometimes allowed to jump through objects
                        float epsilon = 0.05f;

                        // player is higher than the object
                        if (CollisionCube.Center.Y + epsilon > o.CollisionCube.Center.Y)
                        {
                            maxYposition = o.CollisionCube.Center.Y + o.CollisionCube.Yover2 + CollisionCube.Yover2 + epsilon;
                            _standingOnObject = o;
                        }
                        else // now not used, good for jumps
                        {
                            maxYposition = o.CollisionCube.Center.Y - o.CollisionCube.Yover2 - CollisionCube.Yover2 - epsilon;
                        }
                    }

                    // check collision on Z axis (forget Y, add Z)
                    CollisionCube.Center += new Vector3(0, -move.Y, move.Z);
                    if (CollisionCube.DoesCollide(o.CollisionCube))
                    {
                        if (CollisionCube.Center.Z > o.CollisionCube.Center.Z)
                            maxZposition = o.CollisionCube.Center.Z + o.CollisionCube.Zover2 + CollisionCube.Zover2;
                        else
                            maxZposition = o.CollisionCube.Center.Z - o.CollisionCube.Zover2 - CollisionCube.Zover2;
                    }
                    // revert to no move at all
                    CollisionCube.Center += new Vector3(0, 0, -move.Z);

                    // calculate the maximum possible move
                    move = new Vector3(
                            maxXposition - CollisionCube.Center.X,
                            maxYposition - CollisionCube.Center.Y,
                            maxZposition - CollisionCube.Center.Z
                            );

                    // try the edited move in next iteration
                    CollisionCube.Center += move;
                }
            }
            // this move is final, it doesn't collide with any nearby object
            // therefore the player can be safely moved
            Transform.Position += move;
        }
    }
}
