using System.Collections.Concurrent;

namespace zpg
{
    class Level
    {
        public OpenTK.Mathematics.Vector3 CameraStartPosition { get; set; } = OpenTK.Mathematics.Vector3.Zero;
        public IEnumerable<RenderObject> LevelObjects { get; set; } = new List<RenderObject>();
        // would be used when loading in parallel
        private ConcurrentBag<RenderObject>? ParallelLoadLevelObjects;
        public string FilePath { get; init; } = string.Empty;
        public Shader Shader { get; init; }
        public Camera Camera { get; init; }

        public string VoidTextureDiffusePath { get; set; } = "./Textures/void.png";
        public string VoidTextureSpecularPath { get; set; } = "./Textures/void_specular.png";
        public string WallTextureDiffusePath { get; set; } = "./Textures/container2.png";
        public string WallTextureSpecularPath { get; set; } = "./Textures/container2_specular.png";
        public string FloorTextureDiffusePath { get; set; } = "./Textures/container2.png";
        public string FloorTextureSpecularPath { get; set; } = "./Textures/container2_specular.png";

        public float BlockX { get; set; } = 2.0f;
        public float BlockY { get; set; } = 3.0f;
        public float BlockZ { get; set; } = 2.0f;

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
        public static Level LoadFile(string path, Shader shader, Camera camera)
        {
            // create new level WITH the concurentBag
            Level level = new(path, shader, camera) { ParallelLoadLevelObjects = new ConcurrentBag<RenderObject>() };

            string[] lines = level.LoadMapIntoStrings();

            level.LoadObjectsFromStrings(lines);

            // copy the objects from parallel structure and get rid of the parallel structure
            level.LevelObjects = level.ParallelLoadLevelObjects!.ToList();
            level.ParallelLoadLevelObjects = null;

            return level;
        }

        private void LoadObjectsFromStrings(string[] lines)
        {
            // for each char
            // Parallel.For(0, lines.Length, (i, state) =>
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    char ch = line[j];
                    bool addedWall = false;

                    // add all walls
                    if ('o' <= ch && ch <= 'z')
                    {
                        // Level.AddWall(Shader, BlockX, BlockY, BlockZ, Camera, j, i, LevelObjects);
                        AddWall(j, 0, i);
                        addedWall = true;
                    }
                    else
                    {
                        // Level.AddFloor(Shader, BlockX, 0.01f, BlockZ, Camera, j, 0, i, LevelObjects);
                        AddFloor(j, 0, i);
                        // Level.AddFloor(Shader, BlockX, 0.01f, BlockZ, Camera, j, BlockY, i, LevelObjects);
                        AddFloor(j, 1, i);
                    }
                    // add end-of-map walls to the edge of map
                    if (!addedWall && i == 0)
                    {
                        // Level.AddWall(Shader, BlockX, BlockY, BlockZ, Camera, j, -1, LevelObjects, false, VoidTextureDiffusePath, VoidTextureSpecularPath);
                        AddWall(j, 0, -1);
                    }
                    if (!addedWall && i == MapZ - 1)
                    {
                        // Level.AddWall(Shader, BlockX, BlockY, BlockZ, Camera, j, MapZ, LevelObjects, false, VoidTextureDiffusePath, VoidTextureSpecularPath);
                        AddWall(j, 0, MapZ);
                    }
                    if (!addedWall && j == 0)
                    {
                        // Level.AddWall(Shader, BlockX, BlockY, BlockZ, Camera, -1, i, LevelObjects, false, VoidTextureDiffusePath, VoidTextureSpecularPath);
                        AddWall(-1, 0, i);
                    }
                    if (!addedWall && j == MapX - 1)
                    {
                        // Level.AddWall(Shader, BlockX, BlockY, BlockZ, Camera, MapX, i, LevelObjects, false, VoidTextureDiffusePath, VoidTextureSpecularPath);
                        AddWall(MapX, 0, i);
                    }

                    // add doors
                    if ('A' <= ch && ch <= 'G')
                    {
                    }

                    // add lights
                    if (ch == '*' || ch == '^' || ch == '!')
                    {
                    }

                    // add solid objects
                    if ('H' <= ch && ch <= 'N')
                    {
                    }

                    // add collectables
                    if ('T' <= ch && ch <= 'Z')
                    {
                    }

                    // add camera position
                    if (ch == '@')
                    {
                        CameraStartPosition = new OpenTK.Mathematics.Vector3(j * BlockX, 1.7f, i * BlockZ);
                    }
                }
            }//);

        }


        private string[] LoadMapIntoStrings()
        {
            string[] lines;
            int width;
            int depth;

            using (StreamReader sr = new StreamReader(FilePath))
            {
                string? line = sr.ReadLine();

                // load width and depth of map
                string[] wh = line is not null ? line.Split("x") : ["0", "0"];
                int.TryParse(wh[0], out width);
                int.TryParse(wh[1], out depth);

                if (width < 1 || depth < 1)
                {
                    throw new ApplicationException($"Invalid map dimensions while trying to load {FilePath}");
                }

                lines = new string[(int)depth];

                // for each line
                for (int i = 0; i < depth; i++)
                {
                    line = sr.ReadLine();
                    // avoid possible null warning
                    lines[i] = line is not null ? line : new string(' ', width);
                }

                MapX = width;
                MapZ = depth;

                return lines;
            }
        }

        /// <summary>
        /// Add one wall of dimensions w/h/d onto position x,z (y = 1/2 h) into list.
        /// Can use non-default textures.
        /// </summary>
        // private static void AddWall(Shader shader, float w, float h, float d, Camera camera, float x, float z, IEnumerable<RenderObject> objects, bool useDefaultTextures = true, string diffuseMap = "", string specularMap = "")
        private void AddWall(int x, int y, int z)
        {
            Cube wall = new Cube(Shader, BlockX, BlockY, BlockZ, Camera, WallTextureDiffusePath, WallTextureSpecularPath);
            wall.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, BlockY / 2, z * BlockZ);
            wall.UpdateCollisionCube();

            // ensure this exists in the LoadLevel method, before this is called
            ParallelLoadLevelObjects!.Add(wall);
        }
        /// <summary>
        /// Add one floor of dimensions w/h/d onto position x,z (y = - 1/2 h) into list.
        /// Can use non-default textures.
        /// </summary>
        // private static void AddFloor(Shader shader, float w, float h, float d, Camera camera, float x, float y, float z, IEnumerable<RenderObject> objects, bool useDefaultTextures = true, string diffuseMap = "", string specularMap = "")
        private void AddFloor(int x, int y, int z)
        {
            float floorY = 0.01f;
            Cube floor = new Cube(Shader, BlockX, floorY, BlockZ, Camera, FloorTextureDiffusePath, FloorTextureSpecularPath);
            floor.Transform.Position = new OpenTK.Mathematics.Vector3(x * BlockX, -floorY / 2 + y * BlockY, z * BlockZ);
            floor.UpdateCollisionCube();

            ParallelLoadLevelObjects!.Add(floor);
        }
    }
}
