namespace zpg
{
    class Level
    {
        public OpenTK.Mathematics.Vector3 CameraStartPosition { get; set; } = OpenTK.Mathematics.Vector3.Zero;
        // in init, you can choose if used dumb or grid or whatever implemented interface
        public IObjectsStore LevelObjects { get; set; }
        // just helping when loading
        public Dictionary<char, TeleportPlatform> Teleports { get; set; } = new();

        public string FilePath { get; init; } = string.Empty;
        public Shader Shader { get; init; }
        public Camera Camera { get; init; }

        // The textures are the same for every wall/floor/etc
        // Except for Teleports, that is why it's called BasePath
        public string VoidTextureDiffusePath { get; set; } = "./Textures/Void.png";
        public string VoidTextureSpecularPath { get; set; } = "./Textures/VoidSpecular.png";
        public Texture? VoidTextureDiffuse { get; set; }
        public Texture? VoidTextureSpecular { get; set; }
        public string WallTextureDiffusePath { get; set; } = "./Textures/Wall.png";
        public string WallTextureSpecularPath { get; set; } = "./Textures/WallSpecular.png";
        public Texture? WallTextureDiffuse { get; set; }
        public Texture? WallTextureSpecular { get; set; }
        public string FloorTextureDiffusePath { get; set; } = "./Textures/Floor.png";
        public string FloorTextureSpecularPath { get; set; } = "./Textures/FloorSpecular.png";
        public Texture? FloorTextureDiffuse { get; set; }
        public Texture? FloorTextureSpecular { get; set; }
        public string TeleportTextureDiffuseBasePath { get; set; } = "./Textures/Teleports/TeleportChar";
        public string TeleportTextureSpecularBasePath { get; set; } = "./Textures/Teleports/TeleportChar";

        // The dimensions of one block = wall, and how high is the platform
        // all in meters
        public float BlockX { get; set; } = 2.0f;
        public float BlockY { get; set; } = 3.0f;
        public float BlockZ { get; set; } = 2.0f;
        public float PlatformY { get; set; } = 0.01f;

        // dimensions of the map
        public int MapX { get; set; }
        public int MapY { get; set; }
        public int MapZ { get; set; }

        public Level(string path, Shader shader, Camera camera)
        {
            FilePath = path;
            Shader = shader;
            Camera = camera;
            // choose whichever interface implementation you like
            LevelObjects = new ObjectsStoreGrid(camera);
        }

        /// <summary>
        /// Load map from file, fills this level instance.
        /// Throws exceptions if file invalid, assumes file exists.
        /// </summary>
        public void LoadFile()
        {
            string[] lines = LoadMapFile();

            LoadObjectsFromStrings(lines);
        }

        /// <summary>
        /// Load the file into memory.
        /// Throws exception if bad map dimensions.
        /// </summary>
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
                // also load height
                if (xzy.Length > 2)
                    int.TryParse(xzy[2], out height);

                // this is clearly invalid
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

        /// <summary>
        /// From in-memory-stored file create objects and position them correctly.
        /// </summary>
        private void LoadObjectsFromStrings(string[] lines)
        {
            // for each char
            // parallel not feasible, because OpenGL is a state machine
            // it would be possible, but it's too complicated for what benefit would it have (at least I think)
            // even though, for bigger levels it would be neccessary
            // See this: https://learnopengl.com/Advanced-OpenGL/Instancing
            // but I hadn't have the time nor will to implement this
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                Console.WriteLine($"Loading file: {FilePath}\nProcessing line: {i + 1}/{lines.Length}"); // it's usefull when loading bigger file

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

        /// <summary>
        /// Based on char, creates adequate object and positions it in the map.
        /// When on the edge of map, but no wall, add void-wall.
        /// </summary>
        private void ProcessChar(int x, int y, int z, char ch)
        {
            bool addedWall = false;
            bool addedFloor = false;
            Console.Write(ch); // output for when loading

            switch (ch)
            {
                case >= 'o' and <= 'z': // walls
                    AddWall(x, y, z);
                    addedWall = true;
                    break;
                case '-': // floor
                    AddFloor(x, y, z);
                    addedFloor = true;
                    break;
                case '+': // ceiling
                    AddFloor(x, y + 1, z);
                    break;
                case '=': // floor and ceiling
                    AddFloor(x, y, z);
                    addedFloor = true;
                    AddFloor(x, y + 1, z);
                    break;
                case >= '0' and <= '9': // teleport
                    AddTeleport(x, y, z, ch);
                    addedFloor = true;
                    break;
                case '@': // player
                    CameraStartPosition = new OpenTK.Mathematics.Vector3(x * BlockX, y * BlockY + Camera.PlayerEyesHeight + 0.4f, z * BlockZ);
                    AddFloor(x, y, z);
                    break;
                default: // ignore other characters
                    break;
            }

            if (!addedWall) // add end-of-map walls to the edge of map
            {
                if (z == 0) AddVoid(x, y, -1);
                if (z == MapZ - 1) AddVoid(x, y, MapZ);
                if (x == 0) AddVoid(-1, y, z);
                if (x == MapX - 1) AddVoid(MapX, y, z);
            }
            if (!addedFloor && y == 0) // base-level-floor
                AddFloor(x, 0, z);
            if (y == MapY - 1) AddFloor(x, MapY, z); // top-level-ceiling
        }

        /// <summary>
        /// Add one wall of BlockDimensions, position it.
        /// </summary>
        private void AddWall(int x, int y, int z)
        {
            Cube wall;
            if (WallTextureDiffuse is null || WallTextureSpecular is null)
            {
                wall = new Cube(Shader, BlockX, BlockY, BlockZ, Camera, WallTextureDiffusePath, WallTextureSpecularPath);
                WallTextureDiffuse = wall._diffuseMap;
                WallTextureSpecular = wall._specularMap;
            }
            else
            {
                wall = new Cube(Shader, BlockX, BlockY, BlockZ, Camera, WallTextureDiffuse, WallTextureSpecular);
            }
            wall.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, BlockY / 2 + y * BlockY, z * BlockZ);
            wall.UpdateCollisionCube();

            LevelObjects.Add(wall);
        }

        private void AddVoid(int x, int y, int z)
        {
            Cube @void;
            if (VoidTextureDiffuse is null || VoidTextureSpecular is null)
            {
                @void = new Cube(Shader, BlockX, BlockY, BlockZ, Camera, VoidTextureDiffusePath, VoidTextureSpecularPath);
                VoidTextureDiffuse = @void._diffuseMap;
                VoidTextureSpecular = @void._specularMap;
            }
            else
            {
                @void = new Cube(Shader, BlockX, BlockY, BlockZ, Camera, VoidTextureDiffuse, VoidTextureSpecular);
            }
            @void.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, BlockY / 2 + y * BlockY, z * BlockZ);
            @void.UpdateCollisionCube();

            LevelObjects.Add(@void);
        }

        /// <summary>
        /// Add one floor on position.
        /// </summary>
        private void AddFloor(int x, int y, int z)
        {
            Platform floor;
            if (FloorTextureDiffuse is null || FloorTextureSpecular is null)
            {
                floor = new(Shader, BlockX, PlatformY, BlockZ, Camera, FloorTextureDiffusePath, FloorTextureSpecularPath);
                FloorTextureDiffuse = floor._diffuseMap;
                FloorTextureSpecular = floor._specularMap;
            }
            else
            {
                floor = new(Shader, BlockX, PlatformY, BlockZ, Camera, FloorTextureDiffuse, FloorTextureSpecular);
            }
            floor.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, y * BlockY - PlatformY, z * BlockZ);
            floor.UpdateCollisionCube();

            LevelObjects.Add(floor);
        }

        /// <summary>
        /// Add Teleport platform, same as floor, but also chooses the right texture and bind teleports.
        /// </summary>
        private void AddTeleport(int x, int y, int z, char ch)
        {
            // choose texture based on char
            TeleportPlatform teleport = new(Shader, BlockX, PlatformY, BlockZ, Camera, TeleportTextureDiffuseBasePath + ch + ".png", TeleportTextureSpecularBasePath + ch + ".png");
            teleport.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, y * BlockY - PlatformY, z * BlockZ);
            teleport.UpdateCollisionCube();

            LevelObjects.Add(teleport);

            // bind teleports
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
