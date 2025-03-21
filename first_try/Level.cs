
namespace zpg
{
    class Level
    {
        public static (OpenTK.Mathematics.Vector3, List<RenderObject>) LoadFile(string path, Shader shader)
        {
            List<RenderObject> objects = new();
            OpenTK.Mathematics.Vector3 camPos = new OpenTK.Mathematics.Vector3();

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

                // return if map invalid
                if (width < 1 || depth < 1)
                {
                    return (camPos, new List<RenderObject>());
                }

                for (int i = 0; i < depth; i++)
                {
                    line = sr.ReadLine();

                    // if invalid input
                    if (line is null)
                    {
                        return (camPos, new List<RenderObject>());
                    }

                    for (int j = 0; j < line.Length; j++)
                    {
                        char ch = line[j];

                        // add all walls
                        if ('o' <= ch && ch <= 'z')
                        {
                            Cube wall = new Cube(shader, blockW, blockH, blockD);
                            wall.Transform.Position = new OpenTK.Mathematics.Vector3(j * blockW, blockH / 2, i * blockD);

                            objects.Add(wall);
                        }

                        // add doors
                        if ('A' <= ch && ch <= 'G')
                        {
                            Cube wall = new Cube(shader, blockW, blockH, blockD);
                            wall.Transform.Position = new OpenTK.Mathematics.Vector3(j * blockW, blockH / 2, i * blockD);
                            wall.Transform.Scale *= 0.5f;

                            objects.Add(wall);
                        }

                        // add lights
                        if (ch == '*' || ch == '^' || ch == '!')
                        {
                            Cube wall = new Cube(shader, blockW, blockH, blockD);
                            wall.Transform.Position = new OpenTK.Mathematics.Vector3(j * blockW, 4.0f, i * blockD);
                            wall.Transform.Scale *= 0.5f;

                            objects.Add(wall);
                        }

                        // add solid objects
                        if ('H' <= ch && ch <= 'N')
                        {
                            Cube wall = new Cube(shader, blockW, blockH, blockD);
                            wall.Transform.Position = new OpenTK.Mathematics.Vector3(j * blockW, 0.0f, i * blockD);
                            wall.Transform.Scale *= 0.5f;

                            objects.Add(wall);
                        }

                        // add collectables
                        if ('T' <= ch && ch <= 'Z')
                        {
                            Cube wall = new Cube(shader, blockW, blockH, blockD);
                            wall.Transform.Position = new OpenTK.Mathematics.Vector3(j * blockW, 0.0f, i * blockD);
                            wall.Transform.Scale *= 0.5f;
                            wall.Transform.Rotation.Z = 3;

                            objects.Add(wall);
                        }

                        // add camera position
                        if (ch == '@')
                        {
                            camPos.X = i * blockW;
                            camPos.Y = 1.7f; // hard-coded eye-level
                            camPos.Z = j * blockD;
                        }
                    }
                }
            }

            return (camPos, objects);
        }
    }
}
