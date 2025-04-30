
namespace zpg
{
    class Level
    {
        public OpenTK.Mathematics.Vector3 CameraStartPosition { get; set; } = OpenTK.Mathematics.Vector3.Zero;
        public IObjectsStore LevelObjects { get; set; } = new ObjectsStoreDumb();
        public Dictionary<char, TeleportPlatform> Teleports { get; set; } = new();

        public string FilePath { get; init; } = string.Empty;
        public Shader Shader { get; init; }
        public Camera Camera { get; init; }

        public string VoidTextureDiffusePath { get; set; } = "./Textures/void.png";
        public string VoidTextureSpecularPath { get; set; } = "./Textures/void_specular.png";
        public string WallTextureDiffusePath { get; set; } = "./Textures/container2.png";
        public string WallTextureSpecularPath { get; set; } = "./Textures/container2_specular.png";
        public string FloorTextureDiffusePath { get; set; } = "./Textures/container2.png";
        public string FloorTextureSpecularPath { get; set; } = "./Textures/container2_specular.png";
        public string TeleportTextureDiffusePath { get; set; } = "./Textures/scaryTeleport.png";
        public string TeleportTextureSpecularPath { get; set; } = "./Textures/scaryTeleport.png";

        public float BlockX { get; set; } = 2.0f;
        public float BlockY { get; set; } = 3.0f;
        public float BlockZ { get; set; } = 2.0f;
        public float PlatformY { get; set; } = 0.01f;

        public int MapX { get; set; }
        public int MapY { get; set; }
        public int MapZ { get; set; }

        public Level(string path, Shader shader, Camera camera)
        {
            FilePath = path;
            Shader = shader;
            Camera = camera;
        }

        /// <summary>
        /// Load map from file, with given shader and camera.
        /// Returns Level instance with position for camera and IEnumerable of objects to render in the scene.
        /// Throws exceptions if file invalid, assumes file exists.
        /// </summary>
        public void LoadFile()
        {
            string[] lines = LoadMapFile();

            LoadObjectsFromStrings(lines);
        }

        private string[] LoadMapFile()
        {
            string[] lines;
            int width = 0;
            int height = 1;
            int depth = 0;

            using (StreamReader sr = new StreamReader(FilePath))
            {
                string? line = sr.ReadLine();

                // load width and depth of map
                string[] xzy = line is not null ? line.Split("x") : ["0", "0", "0"];
                int.TryParse(xzy[0], out width);
                int.TryParse(xzy[1], out depth);
                if (xzy.Length > 2)
                    int.TryParse(xzy[2], out height);

                if (width < 1 || depth < 1)
                {
                    throw new ApplicationException($"Invalid 2D map dimensions while trying to load {FilePath}");
                }

                int lineNumbers = depth * height;

                lines = new string[lineNumbers];

                // for each line
                for (int i = 0; i < lineNumbers; i++)
                {
                    line = sr.ReadLine();
                    // avoid possible null warning
                    lines[i] = line is not null ? line : new string(' ', width);
                }

                MapX = width;
                MapZ = depth;
                MapY = height;

                return lines;
            }
        }

        private void LoadObjectsFromStrings(string[] lines)
        {
            // for each char
            // parallel not feasible, because OpenGL is a state machine
            // it would be possible, but it's too complicated for what would it add.
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Console.WriteLine($"Processing line: {i + 1}/{lines.Length}");
                ProcessLine(i / MapZ, i % MapZ, line);
                Console.Clear();
                Console.CursorTop = 0;
                Console.CursorLeft = 0;
            }
        }

        private void ProcessLine(int y, int z, string line)
        {
            for (int x = 0; x < line.Length; x++)
            {
                char ch = line[x];
                ProcessChar(x, y, z, ch);
            }
        }

        private void ProcessChar(int x, int y, int z, char ch)
        {
            bool addedWall = false;
            Console.Write(ch);

            switch (ch)
            {
                case >= 'o' and <= 'z': // walls
                    AddWall(x, y, z);
                    addedWall = true;
                    break;
                case '-': // floor
                    AddFloor(x, y, z);
                    break;
                case '+': // ceiling
                    AddFloor(x, y + 1, z);
                    break;
                case '=': // floor and ceiling
                    AddFloor(x, y, z);
                    AddFloor(x, y + 1, z);
                    break;
                case >= '0' and <= '9': // teleport
                    AddTeleport(x, y, z, ch);
                    break;
                case '@': // player
                    CameraStartPosition = new OpenTK.Mathematics.Vector3(x * BlockX, y * BlockY + Camera.eyesHeight + 0.4f, z * BlockZ);
                    AddFloor(x, y, z);
                    break;
                default: // ignore
                    break;
            }

            // add end-of-map walls to the edge of map
            if (!addedWall)
            {
                if (z == 0) AddWall(x, y, -1, true);
                if (z == MapZ - 1) AddWall(x, y, MapZ, true);
                if (x == 0) AddWall(-1, y, z, true);
                if (x == MapX - 1) AddWall(MapX, y, z, true);
            }
        }

        /// <summary>
        /// Add one wall of dimensions w/h/d onto position x,z (y = 1/2 h) into list.
        /// </summary>
        private void AddWall(int x, int y, int z, bool isVoid = false)
        {
            Cube wall = new Cube(Shader, BlockX, BlockY, BlockZ, Camera, isVoid ? VoidTextureDiffusePath : WallTextureDiffusePath, isVoid ? VoidTextureSpecularPath : WallTextureSpecularPath);
            wall.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, BlockY / 2 + y * BlockY, z * BlockZ);
            wall.UpdateCollisionCube();

            LevelObjects.Add(wall);
        }

        /// <summary>
        /// Add one floor of dimensions w/h/d onto position x,z (y = - 1/2 h) into list.
        /// Can use non-default textures.
        /// </summary>
        // private static void AddFloor(Shader shader, float w, float h, float d, Camera camera, float x, float y, float z, IEnumerable<RenderObject> objects, bool useDefaultTextures = true, string diffuseMap = "", string specularMap = "")
        private void AddFloor(int x, int y, int z)
        {
            Platform floor = new(Shader, BlockX, PlatformY, BlockZ, Camera, FloorTextureDiffusePath, FloorTextureSpecularPath);
            floor.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, -PlatformY / 2 + y * BlockY, z * BlockZ);
            floor.UpdateCollisionCube();

            LevelObjects.Add(floor);
        }

        private void AddTeleport(int x, int y, int z, char ch)
        {
            TeleportPlatform teleport = new(Shader, BlockX, PlatformY, BlockZ, Camera, TeleportTextureDiffusePath, TeleportTextureSpecularPath);
            teleport.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, -PlatformY / 2 + y * BlockY, z * BlockZ);
            teleport.UpdateCollisionCube();

            LevelObjects.Add(teleport);

            TeleportPlatform? otherTeleport = Teleports.GetValueOrDefault(ch);
            if (otherTeleport != null)
            {
                otherTeleport.LinkedTeleportPlatform = teleport;
                teleport.LinkedTeleportPlatform = otherTeleport;
            }
            else
            {
                Teleports.Add(ch, teleport);
            }
        }
    }
}
