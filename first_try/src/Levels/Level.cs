
namespace zpg
{
    class Level
    {
        /// <summary>
        /// Load map from file, with given shader and camera.
        /// Returns new position for camera and list of objects to render in the scene.
        /// Throws exceptions if file invalid, assumes file exists.
        /// </summary>
        public static (OpenTK.Mathematics.Vector3, List<RenderObject>) LoadFile(string path, Shader shader, Camera camera)
        {
            List<RenderObject> objects = new();
            OpenTK.Mathematics.Vector3 camPos = new OpenTK.Mathematics.Vector3();
            camPos.Y = 1.7f; // hard-coded eye-level

            string voidTextureDiffusePath = "./Textures/void.png";
            string voidTextureSpecularPath = "./Textures/void_specular.png";

            float blockW = 2.0f;
            float blockH = 3.0f;
            float blockD = 2.0f;

            using (StreamReader sr = new StreamReader(path))
            {
                string? line = sr.ReadLine();

                // load width and depth of map
                string[] wh = line is not null ? line.Split("x") : ["0", "0"];
                float.TryParse(wh[0], out float width);
                float.TryParse(wh[1], out float depth);

                if (width < 1 || depth < 1)
                {
                    throw new ApplicationException($"Invalid map dimensions while trying to load {path}");
                }

                // for each line
                // Parallel.For(0, (int)depth, (i, s) =>
                for (int i = 0; i < depth; i++)
                {
                    line = sr.ReadLine();

                    if (line is null)
                    {
                        throw new ApplicationException($"Invalid line in map file while trying to load line {i} in {path}");
                    }

                    // for each char
                    for (int j = 0; j < line.Length; j++)
                    {
                        char ch = line[j];
                        bool addedWall = false;

                        // add all walls
                        if ('o' <= ch && ch <= 'z')
                        {
                            Level.AddWall(shader, blockW, blockH, blockD, camera, j, i, objects);
                            addedWall = true;
                        }
                        else
                        {
                            Level.AddFloor(shader, blockW, 0.01f, blockD, camera, j, 0, i, objects);
                            Level.AddFloor(shader, blockW, 0.01f, blockD, camera, j, blockH, i, objects);
                        }
                        // add end-of-map walls to the edge of map
                        if (!addedWall && i == 0)
                        {
                            Level.AddWall(shader, blockW, blockH, blockD, camera, j, -1, objects, false, voidTextureDiffusePath, voidTextureSpecularPath);
                        }
                        if (!addedWall && i == depth - 1)
                        {
                            Level.AddWall(shader, blockW, blockH, blockD, camera, j, depth, objects, false, voidTextureDiffusePath, voidTextureSpecularPath);
                        }
                        if (!addedWall && j == 0)
                        {
                            Level.AddWall(shader, blockW, blockH, blockD, camera, -1, i, objects, false, voidTextureDiffusePath, voidTextureSpecularPath);
                        }
                        if (!addedWall && j == width - 1)
                        {
                            Level.AddWall(shader, blockW, blockH, blockD, camera, width, i, objects, false, voidTextureDiffusePath, voidTextureSpecularPath);
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
                            camPos.X = j * blockW;
                            camPos.Z = i * blockD;
                        }
                    }
                }//);
            }

            return (camPos, objects);
        }

        /// <summary>
        /// Add one wall of dimensions w/h/d onto position x,z (y = 1/2 h) into list.
        /// Can use non-default textures.
        /// </summary>
        private static void AddWall(Shader shader, float w, float h, float d, Camera camera, float x, float z, List<RenderObject> list, bool useDefaultTextures = true, string diffuseMap = "", string specularMap = "")
        {
            Cube wall;
            if (useDefaultTextures)
                wall = new Cube(shader, w, h, d, camera);
            else
                wall = new Cube(shader, w, h, d, camera, diffuseMap, specularMap);
            wall.Transform.Position = new OpenTK.Mathematics.Vector3(x * w, h / 2, z * d);
            wall.UpdateCollisionCube();

            list.Add(wall);
        }
        /// <summary>
        /// Add one floor of dimensions w/h/d onto position x,z (y = - 1/2 h) into list.
        /// Can use non-default textures.
        /// </summary>
        private static void AddFloor(Shader shader, float w, float h, float d, Camera camera, float x, float y, float z, List<RenderObject> list, bool useDefaultTextures = true, string diffuseMap = "", string specularMap = "")
        {
            Cube floor;
            if (useDefaultTextures)
                floor = new Cube(shader, w, h, d, camera);
            else
                floor = new Cube(shader, w, h, d, camera, diffuseMap, specularMap);
            floor.Transform.Position = new OpenTK.Mathematics.Vector3(x * w, -h / 2 + y, z * d);

            floor.UpdateCollisionCube();

            // Console.WriteLine(floor.CollisionCube);
            list.Add(floor);
        }
    }
}
